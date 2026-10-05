using Microsoft.AspNetCore.Mvc;
using VegetableLine.Dtos;
using VegetableLine.Models.Enums;
using VegetableLine.Simulation;

namespace VegetableLine.Controllers;

[ApiController]
[Route("api/kpis")]
public class KpisController : ControllerBase
{
    private readonly SimulationContext _context;

    public KpisController(SimulationContext context)
    {
        _context = context;
    }

    [HttpGet]
    public ActionResult<KpisDto> Get()
    {
        lock (_context.SyncLock)
        {
            var order = _context.CurrentOrder;
            if (order is null) return NoContent();

            var settings = _context.SimulationSettings;
            var orderBags = LineMetrics.OrderBags(_context, order);

            double processedKg = order.Batch.WeightKg - _context.RemainingBatchKg;
            double baggedKg = orderBags.Sum(b => b.ActualWeight);
            int outOfTolerance = orderBags.Count(b => b.Status == BagStatus.Rejected);

            // La alarma salta con mas de N bolsas fuera de tolerancia entre las ultimas M: se expresa como porcentaje
            var limits = new KpiLimitsDto(
                settings.Weighing.MinBagsPerMinute,
                LineMetrics.Percent(settings.Weighing.OutOfToleranceAlarmCount, settings.Weighing.RecentBagsWindow),
                Math.Round(settings.Cleaning.SoilAlarmThreshold * 100, 2),
                Math.Round(settings.Inspection.RejectAlarmThreshold * 100, 2));

            return new KpisDto(
                order.Id,
                LineMetrics.RecentBagsPerMinute(_context),
                LineMetrics.Percent(outOfTolerance, orderBags.Count),
                LineMetrics.Percent(_context.SoilRemovedKg, processedKg),
                // El descarte se mide sobre lo que llego a la mesa de inspeccion (ya sin tierra)
                LineMetrics.Percent(_context.RejectedKg, processedKg - _context.SoilRemovedKg),
                LineMetrics.Percent(baggedKg, processedKg),
                limits);
        }
    }
}
