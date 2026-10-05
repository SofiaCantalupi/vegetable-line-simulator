using VegetableLine.Simulation;

namespace VegetableLine.Models;

public class InspectionTable : Station
{
    public int OperatorCount { get; set; }

    public override double Process(double kgIn, SimulationContext context)
    {
        var s = context.SimulationSettings.Inspection;
        double rejected = kgIn * RandomHelper.Between(s.MinRejectRate, s.MaxRejectRate);
        context.RejectedKg += rejected;
        return kgIn - rejected;
    }
}