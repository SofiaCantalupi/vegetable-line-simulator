using Microsoft.AspNetCore.Mvc;
using VegetableLine.Dtos;
using VegetableLine.Models.Enums;
using VegetableLine.Simulation;

namespace VegetableLine.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly SimulationContext _context;

    public OrdersController(SimulationContext context)
    {
        _context = context;
    }

    [HttpGet("current")]
    public ActionResult<CurrentOrderDto> GetCurrent()
    {
        lock (_context.SyncLock)
        {
            var order = _context.CurrentOrder;
            if (order is null) return NoContent();

            var batch = order.Batch;
            double processedKg = batch.WeightKg - _context.RemainingBatchKg;

            return new CurrentOrderDto(
                order.Id,
                new BatchDto(batch.Producer, batch.Origin, batch.Variety, batch.WeightKg),
                Math.Round(processedKg, 2),
                LineMetrics.Percent(processedKg, batch.WeightKg),
                _context.Bags.Count(b => b.ProductionOrderId == order.Id),
                (int)order.PlannedQuantity,
                order.StartedAt);
        }
    }

    [HttpGet]
    public ActionResult<List<OrderSummaryDto>> GetAll()
    {
        lock (_context.SyncLock)
        {
            return _context.Orders
                .Select(o => new OrderSummaryDto(
                    o.Id,
                    o.Batch.Producer,
                    o.Batch.Origin,
                    o.Batch.Variety,
                    o.Status,
                    // StartedAt no es nullable en la entidad: una orden pendiente todavia no tiene inicio
                    o.Status == OrderStatus.Pending ? null : o.StartedAt,
                    o.EndedAt,
                    _context.Bags.Count(b => b.ProductionOrderId == o.Id)))
                .ToList();
        }
    }
}
