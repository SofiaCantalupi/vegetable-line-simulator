namespace VegetableLine.Models;

using VegetableLine.Simulation;

public class Grader : Station
{
    // Registra cuantos kilos de cada tamaño se procesan
    public override double Process(double kgIn, SimulationContext context)
    {
        var s = context.SimulationSettings.Grading;
        double small = s.SmallRate + RandomHelper.Between(-s.RateVariation, s.RateVariation);
        double large = s.LargeRate + RandomHelper.Between(-s.RateVariation, s.RateVariation);
        double medium = 1 - small - large;

        context.SmallKg += kgIn * small;
        context.MediumKg += kgIn * medium;
        context.LargeKg += kgIn * large;
        return kgIn;
    }

}