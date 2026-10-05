using VegetableLine.Models;
using VegetableLine.Models.Enums;
using VegetableLine.Models;

namespace VegetableLine.Simulation;

// Evalua en cada tick las condiciones de alarma de cada estacion.
// Crea una alarma cuando la condicion se cumple y la normaliza cuando deja de cumplirse
public class AlarmMonitor
{
    private readonly SimulationContext _context;
    private readonly ILogger<AlarmMonitor> _logger;

    public AlarmMonitor(SimulationContext context, ILogger<AlarmMonitor> logger)
    {
        _context = context;
        _logger = logger;
    }

    //Punto de entrada: lo llama el simulador al final de cada tick.
    public void Evaluate()
    {
        var order = _context.CurrentOrder;
        if (order is null) return;

        // Las alarmas por tasa solo se evalúan con la ventana completa (un minuto de datos).
        // Si no, al iniciar una orden habría muy pocos datos y saltarían falsas alarmas.
        int windowTicks = _context.SimulationSettings.AlarmWindowSeconds / _context.SimulationSettings.TickIntervalSeconds;
        bool windowFull = _context.RecentTicks.Count >= windowTicks;

        CheckCleaning(windowFull);
        CheckInspection(windowFull);
        CheckWeighing(order, windowFull);
        CheckClosing(order);
    }

    // Limpieza: porcentaje de tierra sobre lo ingresado en el lutimo minuto
    private void CheckCleaning(bool windowFull)
    {
        var s = _context.SimulationSettings.Cleaning;
        double kgIn = _context.RecentTicks.Sum(t => t.KgIn);
        double soil = _context.RecentTicks.Sum(t => t.SoilKg);
        double soilRate = kgIn > 0 ? soil / kgIn : 0;

        UpdateAlarm<Cleaner>(
            AlarmType.HighSoil, AlarmSeverity.Warning,
            windowFull && soilRate > s.SoilAlarmThreshold,
            $"Tierra elevada: {soilRate:P1} en el último minuto (límite {s.SoilAlarmThreshold:P0})");
    }

    // Inspeccion: porcentaje de descarte sobre lo que llegp a la mesa (ya sin tierra).
    private void CheckInspection(bool windowFull)
    {
        var s = _context.SimulationSettings.Inspection;
        double kgToInspection = _context.RecentTicks.Sum(t => t.KgIn - t.SoilKg);
        double rejected = _context.RecentTicks.Sum(t => t.RejectedKg);
        double rejectRate = kgToInspection > 0 ? rejected / kgToInspection : 0;

        UpdateAlarm<InspectionTable>(
            AlarmType.HighReject, AlarmSeverity.Warning,
            windowFull && rejectRate > s.RejectAlarmThreshold,
            $"Descarte elevado: {rejectRate:P1} en el último minuto (límite {s.RejectAlarmThreshold:P0})");
    }

    // Pesadora: bolsas fuera de tolerancia entre las ultimas N y bolsas por minuto
    private void CheckWeighing(ProductionOrder order, bool windowFull)
    {
        var s = _context.SimulationSettings.Weighing;

        // ultimas N bolsas de la orden en curso
        var recentBags = _context.Bags
            .Where(b => b.ProductionOrderId == order.Id)
            .TakeLast(s.RecentBagsWindow)
            .ToList();
        int outOfTolerance = recentBags.Count(b => b.Status == BagStatus.Rejected);

        UpdateAlarm<Weigher>(
            AlarmType.OutOfTolerance, AlarmSeverity.Warning,
            outOfTolerance > s.OutOfToleranceAlarmCount,
            $"{outOfTolerance} de las últimas {recentBags.Count} bolsas fuera de tolerancia");

        // Bolsas producidas en la ventana. Como la ventana es de 60 segundos, equivale a bolsas por minuto.
        int bagsPerMinute = _context.RecentTicks.Sum(t => t.BagsProduced);

        UpdateAlarm<Weigher>(
            AlarmType.LowThroughput, AlarmSeverity.Critical,
            windowFull && bagsPerMinute < s.MinBagsPerMinute,
            $"Producción baja: {bagsPerMinute} bolsas/min (mínimo {s.MinBagsPerMinute})");
    }

    //Cosedora: fallas de cosido entre las ultimas N bolsas 
    private void CheckClosing(ProductionOrder order)
    {
        var s = _context.SimulationSettings.Closing;

        // Solo cuentan las bolsas que llegaron a la cosedora (cosidas o con falla)
        var recentClosed = _context.Bags
            .Where(b => b.ProductionOrderId == order.Id &&
                        (b.Status == BagStatus.Stitched || b.Status == BagStatus.StitchedFailure))
            .TakeLast(s.RecentBagsWindow)
            .ToList();
        int failed = recentClosed.Count(b => b.Status == BagStatus.StitchedFailure);

        UpdateAlarm<BagCloser>(
            AlarmType.SewingFailures, AlarmSeverity.Warning,
            failed > s.SewingFailureAlarmCount,
            $"{failed} de las últimas {recentClosed.Count} bolsas con falla de cosido");
    }

    /// Logica comun a todas las alarmas:
    /// - Si la condicion se cumple y no hay una alarma activa de ese tipo -> la crea
    /// - Si la condicion dejo de cumplirse y habia una activa -> la normaliza
    /// Asi no se repite la misma alarma en cada tick.
    private void UpdateAlarm<TStation>(AlarmType type, AlarmSeverity severity, bool conditionMet, string message)
        where TStation : Station
    {
        var station = _context.Stations.OfType<TStation>().First();
        var active = _context.Alarms.FirstOrDefault(a =>
            a.StationId == station.Id && a.Type == type && a.IsActive);

        if (conditionMet && active is null)
        {
            _context.Alarms.Add(new Alarm
            {
                Id = _context.Alarms.Count + 1,
                StationId = station.Id,
                Type = type,
                Severity = severity,
                Message = message,
                RaisedAt = DateTime.Now
            });
            _logger.LogWarning("ALARMA [{Severity}] {Station}: {Message}", severity, station.Name, message);
        }
        else if (!conditionMet && active is not null)
        {
            active.ResolvedAt = DateTime.Now;
            _logger.LogInformation("Alarma normalizada: {Station} - {Type}", station.Name, type);
        }
    }
}