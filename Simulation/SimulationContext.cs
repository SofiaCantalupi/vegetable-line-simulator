namespace VegetableLine.Simulation;

using VegetableLine.SimulationSettings;
using VegetableLine.Models;
using VegetableLine.Models.Enums;

public class SimulationContext
{
    public SimulationSettings SimulationSettings { get; }

    // El simulador lo toma durante cada tick y la API al leer datos o modificar alarmas,
    // asi nadie ve el estado a medio actualizar.
    public Lock SyncLock { get; } = new();

    public List<Batch> Batches { get; } = new();
    public List<ProductionOrder> Orders { get; } = new();
    public List<Station> Stations { get; } = new();
    public List<Bag> Bags { get; } = new();
    public List<Bag> NewBagsThisTick { get; } = new();

    public ProductionOrder? CurrentOrder { get; set; }
    public double RemainingBatchKg { get; set; }

    public double SoilRemovedKg { get; set; }
    public double RejectedKg { get; set; }
    public double SmallKg { get; set; }
    public double MediumKg { get; set; }
    public double LargeKg { get; set; }

    public List<Alarm> Alarms { get; } = new();
    public List<TickRecord> RecentTicks { get; } = new();

    public SimulationContext(SimulationSettings settings)
    {
        SimulationSettings = settings;
        SeedData.Populate(this);
    }

    // Calcula el estado que se muestra en el dashboard para una estacion
    // La condicion mas grave tiene prioridad.
    public StationStatus GetStationStatus(Station station)
    {
        if (!station.IsRunning) return StationStatus.Stopped;
        if (Alarms.Any(a => a.StationId == station.Id && a.IsActive)) return StationStatus.Alarm;
        return StationStatus.Running;
    }
}