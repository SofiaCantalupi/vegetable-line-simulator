namespace VegetableLine.Models;
using VegetableLine.Models.Enums;

public class Bag
{
    public int Id { get; set;}
    public int? ProductionOrderId { get; set;}
    public ProductionOrder? ProductionOrder { get; set; }
    public int? PalletId { get; set;}
    public Pallet? Pallet { get; set; }
    public double ActualWeight { get; set;}
    public DateTime WeighedAt { get; set;}
    public BagStatus Status { get; set;}

}