using Microsoft.AspNetCore.Mvc;
using VegetableLine.Dtos;
using VegetableLine.Simulation;

namespace VegetableLine.Controllers;

[ApiController]
[Route("api/charts")]
public class ChartsController : ControllerBase
{
    private const int ChartMinutes = 30;
    // Intervalos de 0,1 kg
    private const int BinsPerKg = 10;

    private readonly SimulationContext _context;

    public ChartsController(SimulationContext context)
    {
        _context = context;
    }

    [HttpGet("bags-per-minute")]
    public ActionResult<List<BagsPerMinutePointDto>> GetBagsPerMinute()
    {
        // El ultimo punto es el minuto en curso, que todavia esta incompleto
        var firstMinute = TruncateToMinute(DateTime.Now).AddMinutes(-(ChartMinutes - 1));

        Dictionary<DateTime, int> bagsByMinute;
        lock (_context.SyncLock)
        {
            bagsByMinute = _context.Bags
                .Where(b => b.WeighedAt >= firstMinute)
                .GroupBy(b => TruncateToMinute(b.WeighedAt))
                .ToDictionary(g => g.Key, g => g.Count());
        }

        // Se devuelven los 30 minutos, tambien los que no tuvieron bolsas
        return Enumerable.Range(0, ChartMinutes)
            .Select(i => firstMinute.AddMinutes(i))
            .Select(minute => new BagsPerMinutePointDto(minute, bagsByMinute.GetValueOrDefault(minute)))
            .ToList();
    }

    [HttpGet("weight-distribution")]
    public ActionResult<WeightDistributionDto> GetWeightDistribution()
    {
        lock (_context.SyncLock)
        {
            var order = _context.CurrentOrder;
            if (order is null) return NoContent();

            // Indice del intervalo: una bolsa de 24,87 kg cae en el 248 (de 24,8 a 24,9)
            var countByBin = LineMetrics.OrderBags(_context, order)
                .GroupBy(b => (int)Math.Floor(b.ActualWeight * BinsPerKg))
                .ToDictionary(g => g.Key, g => g.Count());

            var bins = new List<WeightBinDto>();
            if (countByBin.Count > 0)
            {
                // Incluye los intervalos vacios del medio para que el histograma quede continuo
                for (int i = countByBin.Keys.Min(); i <= countByBin.Keys.Max(); i++)
                {
                    bins.Add(new WeightBinDto(
                        (double)i / BinsPerKg,
                        (double)(i + 1) / BinsPerKg,
                        countByBin.GetValueOrDefault(i)));
                }
            }

            return new WeightDistributionDto(order.TargetWeight, order.Tolerance, 1.0 / BinsPerKg, bins);
        }
    }

    [HttpGet("grading")]
    public ActionResult<GradingDto> GetGrading()
    {
        lock (_context.SyncLock)
        {
            if (_context.CurrentOrder is null) return NoContent();

            double gradedKg = _context.SmallKg + _context.MediumKg + _context.LargeKg;

            return new GradingDto(
                LineMetrics.Percent(_context.SmallKg, gradedKg),
                LineMetrics.Percent(_context.MediumKg, gradedKg),
                LineMetrics.Percent(_context.LargeKg, gradedKg));
        }
    }

    private static DateTime TruncateToMinute(DateTime value)
        => value.AddTicks(-(value.Ticks % TimeSpan.TicksPerMinute));
}
