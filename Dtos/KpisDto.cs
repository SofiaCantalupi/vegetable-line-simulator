namespace VegetableLine.Dtos;

public record KpisDto(
    int OrderId,
    double BagsPerMinute,
    double OutOfTolerancePercent,
    double SoilPercent,
    double RejectPercent,
    double YieldPercent,
    KpiLimitsDto Limits);

// Limites configurados, en las mismas unidades que los KPIs
public record KpiLimitsDto(
    int MinBagsPerMinute,
    double MaxOutOfTolerancePercent,
    double MaxSoilPercent,
    double MaxRejectPercent);
