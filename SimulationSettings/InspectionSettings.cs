namespace VegetableLine.SimulationSettings;

public class InspectionSettings
{
    public double MinRejectRate { get; set; } = 0.01;
    public double MaxRejectRate { get; set; } = 0.04;
    public double RejectAlarmThreshold { get; set; } = 0.05;
}

// Reject: descarte