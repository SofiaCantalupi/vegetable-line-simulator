namespace VegetableLine.Dtos;

using VegetableLine.Models.Enums;

public record BatchDto(string Producer, string Origin, PotatoVariety Variety, double WeightKg);

public record CurrentOrderDto(
    int Id,
    BatchDto Batch,
    double ProcessedKg,
    double ProgressPercent,
    int BagsProduced,
    int PlannedBags,
    DateTime StartedAt);

public record OrderSummaryDto(
    int Id,
    string Producer,
    string Origin,
    PotatoVariety Variety,
    OrderStatus Status,
    DateTime? StartedAt,
    DateTime? EndedAt,
    int BagCount);
