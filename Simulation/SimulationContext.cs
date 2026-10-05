namespace VegetableLine.Simulation;
using VegetableLine.SimulationSettings;
using VegetableLine.Models;

public class SimulationContext
{
    public SimulationSettings SimulationSettings { get; }

    public List<Batch> Batches { get;} = new();
    public List<ProductionOrder> Orders { get; } = new();
    public List<Station> Stations {get;} = new();
    public List<Bag> Bags { get;} = new();
    public List<Bag> NewBagsThisTick { get;} = new();

    public ProductionOrder? CurrentOrder {get; set;}
    public double RemainingBatchKg {get; set;}

    public double SoilRemovedKg { get; set; }
    public double RejectedKg { get; set; }
    public double SmallKg { get; set; }
    public double MediumKg { get; set; }
    public double LargeKg { get; set; }

    public SimulationContext(SimulationSettings settings)
    {
        SimulationSettings = settings;
        SeedData.Populate(this);
    }
}