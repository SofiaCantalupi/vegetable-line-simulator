namespace VegetableLine.Models.Enums;
public enum AlarmType
{
    HighSoil,        // Tierra elevada en limpieza
    HighReject,      // Descarte elevado en inspección
    OutOfTolerance,  // Muchas bolsas fuera de tolerancia
    LowThroughput,   // Pocas bolsas por minuto
    SewingFailures   // Muchas fallas de cosido
}