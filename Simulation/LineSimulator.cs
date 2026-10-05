namespace VegetableLine.Simulation;
using VegetableLine.SimulationSettings;

public class LineSimulator : BackgroundService
{
    private readonly SimulationSettings _settings;
    private readonly ILogger<LineSimulator> _logger;
    private int _tickCount = 0;

    public LineSimulator(SimulationSettings settings, ILogger<LineSimulator> logger)
    {
        _settings = settings;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Comenzando Simulación de la línea de producción...");

        while (!stoppingToken.IsCancellationRequested)
        {
            _tickCount++;
            _logger.LogInformation($"Tick {_tickCount}");

            await Task.Delay(TimeSpan.FromSeconds(_settings.TickIntervalSeconds), stoppingToken);
        }
    }
}