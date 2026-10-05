namespace VegetableLine.Models;

using VegetableLine.Models.Enums;

public class Batch
{
    public int Id { get; set; }
    public string Producer { get; set; } = string.Empty;
    public string Origin { get; set; } = string.Empty;
    public PotatoVariety Variety { get; set; }
    public double WeightKg { get; set; }
    public DateTime ArrivedAt { get; set; }
    // Factor de calidad del lote.
    public double SoilFactor { get; set; } = 1.0;
    public double DefectFactor { get; set; } = 1.0;
}

