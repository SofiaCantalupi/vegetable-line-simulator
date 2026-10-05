namespace VegetableLine.Dtos;

using VegetableLine.Models.Enums;

public record LineStatusDto(string Name, int ActiveAlarms, List<StationDto> Stations);

// KeyMetric es null en las cintas y cuando no hay orden en curso
public record StationDto(int Id, string Name, string Type, int Position, StationStatus Status, KeyMetricDto? KeyMetric);

public record KeyMetricDto(string Label, double Value, string Unit);
