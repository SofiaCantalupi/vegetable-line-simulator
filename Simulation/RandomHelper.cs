namespace VegetableLine.Simulation;

public class RandomHelper
{
    public static double Between(double min, double max)
    => min + Random.Shared.NextDouble() * (max - min);

    public static bool Chance(double probability)
    => Random.Shared.NextDouble() < probability;
}