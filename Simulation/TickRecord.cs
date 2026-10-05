namespace VegetableLine.Simulation;

// Lo que paso en un tick. Se guardan los ultimos para calcular tasas por ventana de tiempo.
public record TickRecord(DateTime Timestamp, double KgIn, double SoilKg, double RejectedKg, int BagsProduced);