namespace VegetableLine.Simulation;

using VegetableLine.Models;
using VegetableLine.Models.Enums;

// Servicio en segundo plano que ejecuta la simulacion de la linea.
// Arranca junto con la aplicacion y avanza un tick por intervalo configurado.
public class LineSimulator : BackgroundService
{
    private readonly SimulationContext _context;
    private readonly ILogger<LineSimulator> _logger;

    private readonly AlarmMonitor _alarmMonitor;
    //Contador de ticks desde que inicia la app
    private int _tickCount = 0;

    public LineSimulator(SimulationContext context, ILogger<LineSimulator> logger, AlarmMonitor alarmMonitor)
    {
        _context = context;
        _logger = logger;
        _alarmMonitor = alarmMonitor;
    }

    // Se ejecuta al inicia la app y corre hasta que se cierra.
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Comenzando Simulación de la línea de producción...");

        while (!stoppingToken.IsCancellationRequested)
        {
            _tickCount++;
            Tick();

            // Espera el intervalo configurado sin bloquear el hilo.
            // Si la app se detiene durante la espera, el token la interrumpe.
            await Task.Delay(TimeSpan.FromSeconds(_context.SimulationSettings.TickIntervalSeconds), stoppingToken);
        }
    }

    // ingresa una porcion del lote a la linea y hace que cada estacion lo procese
    private void Tick()
    {
        // Limpia la lista de bolsas del tick anterior
        _context.NewBagsThisTick.Clear();

        // Si no hay orden en curso, busca una nueva. Si no hay órdenes pendientes, termina el tick sin hacer nada
        if (_context.CurrentOrder is null && !StartNextOrder())
        {
            _logger.LogInformation("Tick {Tick}: no hay órdenes pendientes", _tickCount);
            return;
        }

        // Kilos que entran a la línea en este tick: lo configurado (50 kg),
        // salvo que quede menos en el lote.
        double kgIn = Math.Min(_context.SimulationSettings.KgPerTick, _context.RemainingBatchKg);
        _context.RemainingBatchKg -= kgIn;

        // Acumulados antes de procesar, para calcular cuánto se sumó en este tick
        double soilBefore = _context.SoilRemovedKg;
        double rejectedBefore = _context.RejectedKg;

        // Recorre las estaciones en el orden de la línea.
        // Cada una recibe los kilos que dejó la anterior y devuelve los que pasa a la siguiente
        double kg = kgIn;
        foreach (var station in _context.Stations.OrderBy(s => s.Position))
        {
            kg = station.Process(kg, _context);
        }

        // Registra lo que pasó en este tick: kilos que entraron, tierra y descarte
        // de este tick (la diferencia con los acumulados de antes) y bolsas producidas
        _context.RecentTicks.Add(new TickRecord(
            DateTime.Now,
            kgIn,
            _context.SoilRemovedKg - soilBefore,
            _context.RejectedKg - rejectedBefore,
            _context.NewBagsThisTick.Count));

        // Mantiene solo los ticks de la ventana (60 con la configuración actual):
        // si se pasó, saca el más viejo
        int windowTicks = _context.SimulationSettings.AlarmWindowSeconds / _context.SimulationSettings.TickIntervalSeconds;
        if (_context.RecentTicks.Count > windowTicks)
        {
            _context.RecentTicks.RemoveAt(0);
        }

        // Evalúa las alarmas con los datos actualizados.
        // Va antes de CompleteCurrentOrder porque ese método deja CurrentOrder en null
        _alarmMonitor.Evaluate();

        _logger.LogInformation(
            "Tick {Tick} | Orden {Order} | Restan {Remaining:F0} kg | Bolsas nuevas: {NewBags}",
            _tickCount, _context.CurrentOrder!.Id, _context.RemainingBatchKg, _context.NewBagsThisTick.Count);

        // Si no quedan kilos, termina la orden
        if (_context.RemainingBatchKg <= 0)
        {
            CompleteCurrentOrder();
        }
    }

    //Busca la primera orden pendiente y la pone en curso.
    // Devuelve true si encontro una, false si no quedan pendientes.
    private bool StartNextOrder()
    {
        // Primera orden con estado Pending, o null si no hay ninguna
        var next = _context.Orders.FirstOrDefault(o => o.Status == OrderStatus.Pending);
        if (next is null) return false;

        // Marca la orden como en curso
        next.Status = OrderStatus.InProgress;
        next.StartedAt = DateTime.Now;

        // La deja como orden actual y carga los kilos de su lote para procesar
        _context.CurrentOrder = next;
        _context.RemainingBatchKg = next.Batch.WeightKg;

        // Reinicia los acumulados, asi cada orden tiene sus propios totales
        _context.SoilRemovedKg = 0;
        _context.RejectedKg = 0;
        _context.SmallKg = 0;
        _context.MediumKg = 0;
        _context.LargeKg = 0;
        _context.RecentTicks.Clear();

        _logger.LogInformation("Orden {Order} iniciada: lote de {Kg} kg de {Variety}",
            next.Id, next.Batch.WeightKg, next.Batch.Variety);
        return true;
    }

    // Cierra la orden en curso, muestra su resumen y deja la linea lista para la siguiente.
    private void CompleteCurrentOrder()
    {
        // Marca la orden como finalizada
        var order = _context.CurrentOrder!;
        order.Status = OrderStatus.Completed;
        order.EndedAt = DateTime.Now;

        // Todas las bolsas que pertenecen a esta orden.
        var orderBags = _context.Bags.Where(b => b.ProductionOrderId == order.Id).ToList();

        // Resumen de la orden: bolsas reales contra planificadas, rechazos y perdidas
        _logger.LogInformation(
            "Orden {Order} finalizada | Bolsas: {Total} (planificadas {Planned}) | Fuera de tolerancia: {Rejected} | Mal cosidas: {Failed} | Tierra: {Soil:F0} kg | Descarte: {Reject:F0} kg",
            order.Id, orderBags.Count, order.PlannedQuantity,
            orderBags.Count(b => b.Status == BagStatus.Rejected),
            orderBags.Count(b => b.Status == BagStatus.StitchedFailure),
            _context.SoilRemovedKg, _context.RejectedKg);

        // Vacia la pesadora: los kilos sobrantes no alcanzan para una bolsa completa.
        // OfType<Weigher>() filtra la lista y devuelve solo las pesadoras.
        _context.Stations.OfType<Weigher>().First().AccumulatedKg = 0;

        // Sin orden actual
        _context.CurrentOrder = null;
    }
}