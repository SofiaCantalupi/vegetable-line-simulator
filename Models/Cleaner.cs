using VegetableLine.Simulation;
using VegetableLine.SimulationSettings;

namespace VegetableLine.Models;

public class Cleaner : Station
{
    // Saca un porcentaje de tierra aleatorio
    public override double Process(double kgIn, SimulationContext context)
    {
        var s = context.SimulationSettings.Cleaning;
        double soil = kgIn * RandomHelper.Between(s.MinSoilRate, s.MaxSoilRate);
        context.SoilRemovedKg += soil;
        return kgIn - soil;
    }
}