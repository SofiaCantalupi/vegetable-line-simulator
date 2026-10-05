namespace VegetableLine.Models;
using VegetableLine.Models.Enums;
public class Stoppage
{
    public int Id { get; set;}
    public int StationId { get; set; }
    public Station Station { get; set; } = null!;
    public int? ProductionOrderId { get; set; }
    public ProductionOrder? ProductionOrder { get; set; }
    public StoppageReason Reason { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
}