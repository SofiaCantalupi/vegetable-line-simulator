using VegetableLine.Simulation;
using VegetableLine.Models.Enums;

namespace VegetableLine.Models;
public class BagCloser : Station
{
    public override double Process(double kgIn, SimulationContext context)
    {
        var s = context.SimulationSettings.Closing;

        foreach (var bag in context.NewBagsThisTick.Where(b => b.Status == BagStatus.Weighed))
    {
        bag.Status = RandomHelper.Chance(s.SewingFailureProbability)
            ? BagStatus.StitchedFailure
            : BagStatus.Stitched;
    }

    return kgIn;
    }
}