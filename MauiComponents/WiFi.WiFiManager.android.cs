namespace MauiComponents;

using System.Runtime.Versioning;

using Android.Content;
using Android.Content.PM;
using Android.Net;
using Android.Net.Wifi;

using AndroidWifiManager = Android.Net.Wifi.WifiManager;
using Application = Android.App.Application;

#pragma warning disable CA1822
public sealed partial class WiFiManager
{
    // API 30 未満の信号レベルの段階数 (0〜4)
    private const int LegacySignalLevels = 5;

    private ConnectivityManager? connectivityManager;

    private AndroidWifiManager? wifiManager;

#pragma warning disable CA2213
    // Stop で解除・破棄する
    private NetworkCallback? callback;

    private WifiStateReceiver? receiver;

    private ScanResultsCallback? scanCallback;
#pragma warning restore CA2213

    // NetworkCallback は能力 (WifiInfo) とリンク情報 (アドレス) を別々に通知するため、直近の値を合成する
    private WifiInfo? wifiInfo;

    private LinkProperties? linkProperties;

    public partial bool IsSupported => Application.Context.PackageManager?.HasSystemFeature(PackageManager.FeatureWifi) ?? false;

    public partial bool IsRadioOn => ResolveWifiManager()?.IsWifiEnabled ?? false;

    private AndroidWifiManager? ResolveWifiManager() =>
        wifiManager ??= (AndroidWifiManager?)Application.Context.GetSystemService(Context.WifiService);

    // Android 9 以降はスキャン回数が制限される (前面アプリで 2 分に 4 回)。制限中は false
#pragma warning disable CA1422
    public partial bool StartScan() => ResolveWifiManager()?.StartScan() ?? false;
#pragma warning restore CA1422

    public partial void OpenSettings()
    {
        using var intent = new Intent(Android.Provider.Settings.ActionWifiSettings);
        intent.AddFlags(ActivityFlags.NewTask);
        Application.Context.StartActivity(intent);
    }

    private partial void Start()
    {
        connectivityManager ??= (ConnectivityManager?)Application.Context.GetSystemService(Context.ConnectivityService);
        if (connectivityManager is null)
        {
            return;
        }

        using var builder = new NetworkRequest.Builder();
        builder.AddTransportType(TransportType.Wifi);
        using var request = builder.Build();
        if (request is null)
        {
            return;
        }

        // SSID / BSSID を受け取るには位置情報の権限に加えて IncludeLocationInfo が必要 (Android 12 以降)
        callback = OperatingSystem.IsAndroidVersionAtLeast(31) ? new NetworkCallback(this, (int)NetworkCallbackFlags.IncludeLocationInfo) : new NetworkCallback(this);
        connectivityManager.RegisterNetworkCallback(request, callback);

        // 無線のオン / オフは接続の有無と別に通知される。スキャン結果は API 30 未満ではブロードキャストで受ける
        receiver = new WifiStateReceiver(this);
        using var filter = new IntentFilter(AndroidWifiManager.WifiStateChangedAction);
        if (!OperatingSystem.IsAndroidVersionAtLeast(30))
        {
            filter.AddAction(AndroidWifiManager.ScanResultsAvailableAction);
        }

        if (OperatingSystem.IsAndroidVersionAtLeast(33))
        {
            Application.Context.RegisterReceiver(receiver, filter, ReceiverFlags.NotExported);
        }
        else
        {
            Application.Context.RegisterReceiver(receiver, filter);
        }

        // スキャン結果の更新通知 (API 30 以降。自アプリ以外が要求したスキャンも含む)。キャッシュ済みの結果は先に読む
        if (OperatingSystem.IsAndroidVersionAtLeast(30) && (ResolveWifiManager() is { } manager) && (Application.Context.MainExecutor is { } executor))
        {
            scanCallback = new ScanResultsCallback(this);
            manager.RegisterScanResultsCallback(executor, scanCallback);
        }

        UpdateAccessPoints(ReadScanResults());
    }

    private partial void Stop()
    {
        if (OperatingSystem.IsAndroidVersionAtLeast(30) && (scanCallback is not null))
        {
            ResolveWifiManager()?.UnregisterScanResultsCallback(scanCallback);
            scanCallback.Dispose();
            scanCallback = null;
        }

        if (receiver is not null)
        {
            Application.Context.UnregisterReceiver(receiver);
            receiver.Dispose();
            receiver = null;
        }

        if (callback is not null)
        {
            connectivityManager?.UnregisterNetworkCallback(callback);
            callback.Dispose();
            callback = null;
        }

        wifiInfo = null;
        linkProperties = null;
        AccessPoints = [];
        Update(null);
    }

    private void OnCapabilitiesChanged(NetworkCapabilities capabilities)
    {
        // API 29 未満は TransportInfo が無いため接続情報を直接読む
#pragma warning disable CA1422
        wifiInfo = OperatingSystem.IsAndroidVersionAtLeast(29) ? capabilities.TransportInfo as WifiInfo : ResolveWifiManager()?.ConnectionInfo;
#pragma warning restore CA1422
        Update(Compose());
    }

    private void OnLinkPropertiesChanged(LinkProperties properties)
    {
        linkProperties = properties;
        Update(Compose());
    }

    private void OnLost()
    {
        wifiInfo = null;
        linkProperties = null;
        Update(null);
    }

    private WiFiConnection? Compose()
    {
        var info = wifiInfo;
        if (info is null)
        {
            return null;
        }

        var ipAddress = string.Empty;
        var gateway = string.Empty;
        var dns = string.Empty;
        if (linkProperties is { } link)
        {
            ipAddress = link.LinkAddresses.Select(static x => x.Address).OfType<Java.Net.Inet4Address>().FirstOrDefault()?.HostAddress ?? string.Empty;
            gateway = link.Routes.Where(static x => x.IsDefaultRoute).Select(static x => x.Gateway).OfType<Java.Net.Inet4Address>().FirstOrDefault()?.HostAddress ?? string.Empty;
            dns = String.Join(", ", link.DnsServers.OfType<Java.Net.Inet4Address>().Select(static x => x.HostAddress));
        }

        return new WiFiConnection(
            (info.SSID ?? string.Empty).Trim('"'),
            info.BSSID ?? string.Empty,
            info.Rssi,
            CalculateSignalLevel(info.Rssi),
            OperatingSystem.IsAndroidVersionAtLeast(30) ? ResolveWifiManager()?.MaxSignalLevel ?? 0 : LegacySignalLevels - 1,
            info.LinkSpeed,
            OperatingSystem.IsAndroidVersionAtLeast(29) ? info.RxLinkSpeedMbps : 0,
            OperatingSystem.IsAndroidVersionAtLeast(29) ? info.TxLinkSpeedMbps : 0,
            info.Frequency,
            OperatingSystem.IsAndroidVersionAtLeast(30) ? ToStandard(info.WifiStandard) : string.Empty,
            ipAddress,
            gateway,
            dns);
    }

    // 権限が無い場合は空 (SecurityException)
    private List<WiFiAccessPoint> ReadScanResults()
    {
        var manager = ResolveWifiManager();
        if (manager is null)
        {
            return [];
        }

        try
        {
#pragma warning disable CA1422
            var results = manager.ScanResults;
#pragma warning restore CA1422
            if (results is null)
            {
                return [];
            }

            var now = DateTime.Now;
            var uptime = Android.OS.SystemClock.ElapsedRealtime();
            return results
                .Select(x => new WiFiAccessPoint(
                    ToSsid(x),
                    x.Bssid ?? string.Empty,
                    x.Level,
                    CalculateSignalLevel(x.Level),
                    x.Frequency,
                    ToChannel(x.Frequency),
                    ToChannelWidth(x.ChannelWidth),
                    ToSecurity(x.Capabilities),
                    OperatingSystem.IsAndroidVersionAtLeast(30) ? ToStandard(x.WifiStandard) : string.Empty,
                    now.AddMilliseconds((x.Timestamp / 1000d) - uptime)))
                .OrderByDescending(static x => x.Rssi)
                .ToList();
        }
        catch (Java.Lang.SecurityException)
        {
            return [];
        }
    }

    // API 30 以降は端末の段階数で、それ未満は 5 段階で計算する
#pragma warning disable CA1422
    private int CalculateSignalLevel(int rssi) =>
        OperatingSystem.IsAndroidVersionAtLeast(30)
            ? ResolveWifiManager()?.CalculateSignalLevel(rssi) ?? 0
            : AndroidWifiManager.CalculateSignalLevel(rssi, LegacySignalLevels);
#pragma warning restore CA1422

#pragma warning disable CA1422
    private static string ToSsid(ScanResult result) =>
        OperatingSystem.IsAndroidVersionAtLeast(33) ? result.WifiSsid?.ToString().Trim('"') ?? string.Empty : result.Ssid ?? string.Empty;
#pragma warning restore CA1422

    private static int ToChannel(int frequency) => frequency switch
    {
        2484 => 14,
        >= 2412 and <= 2472 => (frequency - 2407) / 5,
        >= 5935 and <= 7115 => (frequency - 5950) / 5,
        >= 5180 and <= 5885 => (frequency - 5000) / 5,
        _ => 0
    };

    // ScanResult.CHANNEL_WIDTH_* の値 (320MHz は Android 13 以降)
    private static int ToChannelWidth(int width) => width switch
    {
        0 => 20,
        1 => 40,
        2 => 80,
        3 => 160,
        4 => 160,
        5 => 320,
        _ => 0
    };

    // ScanResult.Capabilities の例: [WPA2-PSK-CCMP][RSN-PSK-CCMP][ESS]
    private static WiFiSecurity ToSecurity(string? capabilities)
    {
        if (String.IsNullOrEmpty(capabilities))
        {
            return WiFiSecurity.Open;
        }

        if (capabilities.Contains("EAP", StringComparison.Ordinal))
        {
            return WiFiSecurity.Enterprise;
        }

        if (capabilities.Contains("SAE", StringComparison.Ordinal))
        {
            return WiFiSecurity.Wpa3;
        }

        if (capabilities.Contains("WPA2", StringComparison.Ordinal) || capabilities.Contains("RSN", StringComparison.Ordinal))
        {
            return WiFiSecurity.Wpa2;
        }

        if (capabilities.Contains("WPA", StringComparison.Ordinal))
        {
            return WiFiSecurity.Wpa;
        }

        return capabilities.Contains("WEP", StringComparison.Ordinal) ? WiFiSecurity.Wep : WiFiSecurity.Open;
    }

    // ScanResult.WifiStandard* の値
    private static string ToStandard(int standard) => standard switch
    {
        1 => "802.11a/b/g",
        4 => "802.11n (Wi-Fi 4)",
        5 => "802.11ac (Wi-Fi 5)",
        6 => "802.11ax (Wi-Fi 6)",
        7 => "802.11ad",
        8 => "802.11be (Wi-Fi 7)",
        _ => string.Empty
    };

    // 無線の状態変化と、API 30 未満のスキャン結果通知
    private sealed class WifiStateReceiver(WiFiManager owner) : BroadcastReceiver
    {
        public override void OnReceive(Context? context, Intent? intent)
        {
            owner.Update(owner.Compose());
            owner.UpdateAccessPoints(owner.ReadScanResults());
        }
    }

    // SCAN_RESULTS_AVAILABLE ブロードキャストの後継 (API 30 以降)。誰かのスキャンが完了するたびに呼ばれる
    [SupportedOSPlatform("android30.0")]
    private sealed class ScanResultsCallback(WiFiManager owner) : AndroidWifiManager.ScanResultsCallback
    {
        public override void OnScanResultsAvailable() => owner.UpdateAccessPoints(owner.ReadScanResults());
    }

    private sealed class NetworkCallback : ConnectivityManager.NetworkCallback
    {
        private readonly WiFiManager owner;

        public NetworkCallback(WiFiManager owner)
        {
            this.owner = owner;
        }

        [SupportedOSPlatform("android31.0")]
        public NetworkCallback(WiFiManager owner, int flags)
            : base(flags)
        {
            this.owner = owner;
        }

        public override void OnCapabilitiesChanged(Network network, NetworkCapabilities networkCapabilities)
        {
            base.OnCapabilitiesChanged(network, networkCapabilities);
            owner.OnCapabilitiesChanged(networkCapabilities);
        }

        public override void OnLinkPropertiesChanged(Network network, LinkProperties linkProperties)
        {
            base.OnLinkPropertiesChanged(network, linkProperties);
            owner.OnLinkPropertiesChanged(linkProperties);
        }

        public override void OnLost(Network network)
        {
            base.OnLost(network);
            owner.OnLost();
        }
    }
}
#pragma warning restore CA1822
