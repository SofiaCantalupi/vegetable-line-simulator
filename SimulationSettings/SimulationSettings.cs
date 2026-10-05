namespace VegetableLine.SimulationSettings;

public class SimulationSettings
{
    public int TickIntervalSeconds { get; set;} = 2;
    public int KgPerTick { get; set;} = 50;
    public double FailureProbabilityPerTick { get; set;} = 0.003;
    public int MinStoppageSeconds { get; set;} = 10;
    public int MaxStoppageSeconds { get; set;} = 40;
    public int AlarmWindowSeconds { get; set; } = 60;

    public CleaningSettings Cleaning { get; set; } = new();
    public GradingSettings Grading { get; set; } = new();
    public InspectionSettings Inspection { get; set; } = new();
    public WeighingSettings Weighing { get; set; } = new();
    public ClosingSettings Closing { get; set; } = new();

 }