namespace VegetableLine.SimulationSettings;

public class WeighingSettings
{
    public double TargetWeightKg { get; set; } = 25;
    public double ToleranceKg { get; set; } = 0.2;
    public double OutOfToleranceProbability { get; set; } = 0.10;
    public int RecentBagsWindow { get; set; } = 20;
    public int OutOfToleranceAlarmCount { get; set; } = 3;
    public int MinBagsPerMinute { get; set; } = 80;
}