namespace VegetableLine.SimulationSettings;

public class SimulationSettings
{
    public int TickIntervalSeconds { get; set;} = 1;
    public int KgPerTick { get; set;} = 50;
    public double FailureProbabilityPerTick { get; set;} = 0.003;
    public int MinStoppageSeconds { get; set;} = 10;
    public int MaxStoppageSeconds { get; set;} = 40;
    public int AlarmWindowSeconds { get; set; } = 60;
    // Pausa entre el fin de la ultima orden y el reinicio del ciclo de demo
    public int RestartDelaySeconds { get; set; } = 30;

    public CleaningSettings Cleaning { get; set; } = new();
    public GradingSettings Grading { get; set; } = new();
    public InspectionSettings Inspection { get; set; } = new();
    public WeighingSettings Weighing { get; set; } = new();
    public ClosingSettings Closing { get; set; } = new();

 }