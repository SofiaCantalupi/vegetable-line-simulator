using Microsoft.AspNetCore.Mvc;
using VegetableLine.Dtos;
using VegetableLine.Models;
using VegetableLine.Simulation;

namespace VegetableLine.Controllers;

[ApiController]
[Route("api/line")]
public class LineController : ControllerBase
{
    private const string LineName = "Línea de empaque 1";

    private readonly SimulationContext _context;

    public LineController(SimulationContext context)
    {
        _context = context;
    }

    [HttpGet("status")]
    public ActionResult<LineStatusDto> GetStatus()
    {
        lock (_context.SyncLock)
        {
            var stations = _context.Stations
                .OrderBy(s => s.Position)
                .Select(s => new StationDto(
                    s.Id,
                    s.Name,
                    s.GetType().Name,
                    s.Position,
                    _context.GetStationStatus(s),
                    GetKeyMetric(s)))
                .ToList();

            return new LineStatusDto(LineName, _context.Alarms.Count(a => a.IsActive), stations);
        }
    }

    private KeyMetricDto? GetKeyMetric(Station station)
    {
        // Sin orden en curso no hay dato: la ventana reciente conserva los ticks de la orden anterior
        var order = _context.CurrentOrder;
        if (order is null) return null;

        switch (station)
        {
            case Cleaner:
                return new KeyMetricDto("Tierra (ventana reciente)", LineMetrics.RecentSoilPercent(_context), "%");

            case Grader:
                double gradedKg = _context.SmallKg + _context.MediumKg + _context.LargeKg;
                return new KeyMetricDto("Papa mediana (orden en curso)", LineMetrics.Percent(_context.MediumKg, gradedKg), "%");

            case InspectionTable:
                return new KeyMetricDto("Descarte (ventana reciente)", LineMetrics.RecentRejectPercent(_context), "%");

            case Weigher:
                return new KeyMetricDto("Producción (ventana reciente)", LineMetrics.RecentBagsPerMinute(_context), "bolsas/min");

            case BagCloser:
                var orderBags = LineMetrics.OrderBags(_context, order);
                return new KeyMetricDto("Bolsas bien cosidas (orden en curso)", LineMetrics.StitchedOkPercent(orderBags), "%");

            default:
                return null;
        }
    }
}
