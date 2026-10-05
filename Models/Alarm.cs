namespace VegetableLine.Models;

using VegetableLine.Models.Enums;

public class Alarm
{
    public int Id { get; set; }
    public int StationId { get; set; }
    public Station Station { get; set; } = null!;
    public AlarmType Type { get; set; }
    public AlarmSeverity Severity { get; set; }
    public string? Message { get; set; }
    public bool IsAcknowledged { get; set; }
    public DateTime RaisedAt { get; set; }

    public DateTime? ResolvedAt { get; set; }

    // Una alarma esta activa mientras su condicion no se normalizo
    public bool IsActive => ResolvedAt is null;
}