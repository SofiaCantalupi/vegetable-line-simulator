namespace VegetableLine.Dtos;

using VegetableLine.Models;
using VegetableLine.Models.Enums;

public record AlarmDto(
    int Id,
    AlarmStationDto Station,
    AlarmType Type,
    AlarmSeverity Severity,
    string? Message,
    DateTime RaisedAt,
    DateTime? ResolvedAt,
    bool IsAcknowledged,
    bool IsActive);

public record AlarmStationDto(int Id, string Name);
