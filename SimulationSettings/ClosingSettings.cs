namespace VegetableLine.SimulationSettings;

public class ClosingSettings
{
    public double SewingFailureProbability { get; set; } = 0.03;
    public int RecentBagsWindow { get; set; } = 20;
    public int SewingFailureAlarmCount { get; set; } = 2;
}