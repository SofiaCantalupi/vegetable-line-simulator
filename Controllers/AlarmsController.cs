using Microsoft.AspNetCore.Mvc;
using VegetableLine.Dtos;
using VegetableLine.Models;
using VegetableLine.Simulation;

namespace VegetableLine.Controllers;

[ApiController]
[Route("api/alarms")]
public class AlarmsController : ControllerBase
{
    private readonly SimulationContext _context;

    public AlarmsController(SimulationContext context)
    {
        _context = context;
    }

    [HttpGet]
    public ActionResult<List<AlarmDto>> GetAll([FromQuery] bool? active)
    {
        lock (_context.SyncLock)
        {
            return _context.Alarms
                .Where(a => active is null || a.IsActive == active)
                .OrderByDescending(a => a.RaisedAt)
                .ThenByDescending(a => a.Id)
                .Select(ToDto)
                .ToList();
        }
    }

    [HttpPost("{id:int}/acknowledge")]
    public ActionResult<AlarmDto> Acknowledge(int id)
    {
        lock (_context.SyncLock)
        {
            var alarm = _context.Alarms.FirstOrDefault(a => a.Id == id);
            if (alarm is null) return NotFound();

            alarm.IsAcknowledged = true;
            return ToDto(alarm);
        }
    }

    private AlarmDto ToDto(Alarm alarm)
    {
        // AlarmMonitor solo carga StationId, asi que el nombre se busca en la lista de estaciones
        var station = _context.Stations.FirstOrDefault(s => s.Id == alarm.StationId);

        return new AlarmDto(
            alarm.Id,
            new AlarmStationDto(alarm.StationId, station?.Name ?? string.Empty),
            alarm.Type,
            alarm.Severity,
            alarm.Message,
            alarm.RaisedAt,
            alarm.ResolvedAt,
            alarm.IsAcknowledged,
            alarm.IsActive);
    }
}
