namespace VegetableLine.Models;

public class Alarm
{
    public int Id { get; set;}
    public int StationId { get; set;}
    public Station Station {get; set;} = null!;

    public string? Type { get; set;}

    public AlarmSeverity Severity { get; set; }
    public string? Message { get; set;}
    public bool IsAcknowledged { get; set;}
    public DateTime RaisedAt { get; set;}
}