namespace VegetableLine.Simulation;

using VegetableLine.Models;
using VegetableLine.Models.Enums;

// Calculos que comparten varios endpoints.
// Quien los llama tiene que tener tomado el lock del contexto.
public static class LineMetrics
{
    // Devuelve 0 si el total es 0 (por ejemplo, al inicio de una orden)
    public static double Percent(double part, double total)
        => total > 0 ? Math.Round(part / total * 100, 2) : 0;

    public static List<Bag> OrderBags(SimulationContext context, ProductionOrder order)
        => context.Bags.Where(b => b.ProductionOrderId == order.Id).ToList();

    public static double RecentSoilPercent(SimulationContext context)
    {
        double kgIn = context.RecentTicks.Sum(t => t.KgIn);
        double soil = context.RecentTicks.Sum(t => t.SoilKg);
        return Percent(soil, kgIn);
    }

    // Igual que en AlarmMonitor: el descarte se mide sobre lo que llego a la mesa (ya sin tierra)
    public static double RecentRejectPercent(SimulationContext context)
    {
        double kgToInspection = context.RecentTicks.Sum(t => t.KgIn - t.SoilKg);
        double rejected = context.RecentTicks.Sum(t => t.RejectedKg);
        return Percent(rejected, kgToInspection);
    }

    // Se proyecta a un minuto segun los segundos que cubre la ventana,
    // asi el valor no arranca en cero mientras la ventana se llena.
    public static double RecentBagsPerMinute(SimulationContext context)
    {
        int bags = context.RecentTicks.Sum(t => t.BagsProduced);
        double seconds = context.RecentTicks.Count * context.SimulationSettings.TickIntervalSeconds;
        return seconds > 0 ? Math.Round(bags / seconds * 60, 1) : 0;
    }

    // Solo cuentan las bolsas que llegaron a la cosedora (cosidas o con falla)
    public static double StitchedOkPercent(List<Bag> orderBags)
    {
        int stitched = orderBags.Count(b => b.Status == BagStatus.Stitched);
        int failed = orderBags.Count(b => b.Status == BagStatus.StitchedFailure);
        return Percent(stitched, stitched + failed);
    }
}
