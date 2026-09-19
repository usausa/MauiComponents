namespace MauiComponents;

public sealed partial class WiFiManager : IWiFiManager
{
    public event EventHandler<EventArgs>? StateChanged;

    public partial bool IsSupported { get; }

    public partial bool IsRadioOn { get; }

    public WiFiConnection? Connection { get; private set; }

    public IReadOnlyList<WiFiAccessPoint> AccessPoints { get; private set; } = [];

    public bool Enabled
    {
        get;
        set
        {
            if (value)
            {
                if (!field)
                {
                    Start();
                    field = true;
                }
            }
            else
            {
                if (field)
                {
                    Stop();
                    field = false;
                }
            }
        }
    }

    public void Dispose()
    {
        Enabled = false;
    }

    public partial bool StartScan();

    public partial void OpenSettings();

    private partial void Start();

    private partial void Stop();

    private void Update(WiFiConnection? connection)
    {
        Connection = connection;
        StateChanged?.Invoke(this, EventArgs.Empty);
    }

    private void UpdateAccessPoints(IReadOnlyList<WiFiAccessPoint> accessPoints)
    {
        AccessPoints = accessPoints;
        StateChanged?.Invoke(this, EventArgs.Empty);
    }
}
