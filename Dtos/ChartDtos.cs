namespace VegetableLine.Dtos;

public record BagsPerMinutePointDto(DateTime Minute, int Bags);

public record WeightDistributionDto(double TargetWeight, double Tolerance, double BinSizeKg, List<WeightBinDto> Bins);

// Intervalo [From, To) en kg
public record WeightBinDto(double From, double To, int Count);

public record GradingDto(double SmallPercent, double MediumPercent, double LargePercent);
