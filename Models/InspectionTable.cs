using VegetableLine.Simulation;

namespace VegetableLine.Models;

public class InspectionTable : Station
{
    public int OperatorCount { get; set; }

    public override double Process(double kgIn, SimulationContext context)
    {
        var s = context.SimulationSettings.Inspection;

        double rate = RandomHelper.Between(s.MinRejectRate, s.MaxRejectRate) * context.CurrentOrder!.Batch.DefectFactor;
        double rejected = kgIn * rate;

        context.RejectedKg += rejected;
        return kgIn - rejected;
    }
}