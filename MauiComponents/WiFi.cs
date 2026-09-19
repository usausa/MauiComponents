namespace MauiComponents;

public enum WiFiSecurity
{
    Open,
    Wep,
    Wpa,
    Wpa2,
    Wpa3,
    Enterprise
}

// Connection information
public sealed record WiFiConnection(
    string Ssid,
    string Bssid,
    int Rssi,
    int SignalLevel,
    int MaxSignalLevel,
    int LinkSpeed,
    int RxLinkSpeed,
    int TxLinkSpeed,
    int Frequency,
    string Standard,
    string IpAddress,
    string Gateway,
    string Dns);

// Access point information
public sealed record WiFiAccessPoint(
    string Ssid,
    string Bssid,
    int Rssi,
    int SignalLevel,
    int Frequency,
    int Channel,
    int ChannelWidth,
    WiFiSecurity Security,
    string Standard,
    DateTime SeenAt);

public interface IWiFiManager : IDisposable
{
    event EventHandler<EventArgs>? StateChanged;

    bool IsSupported { get; }

    bool IsRadioOn { get; }

    WiFiConnection? Connection { get; }

    IReadOnlyList<WiFiAccessPoint> AccessPoints { get; }

    bool Enabled { get; set; }

    bool StartScan();

    void OpenSettings();
}
