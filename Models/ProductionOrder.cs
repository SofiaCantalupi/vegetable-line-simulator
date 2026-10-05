namespace VegetableLine.Models;
using VegetableLine.Models.Enums;

public class ProductionOrder
{
    public int Id { get; set;}
    public int BatchId { get; set;}
    public Batch Batch { get; set;} = null!;
    public OrderStatus Status { get; set;}
    public double PlannedQuantity { get; set;}
    public double TargetWeight { get; set;}
    public double Tolerance { get; set;}
    public DateTime StartedAt { get; set;}
    public DateTime? EndedAt { get; set;}
}

