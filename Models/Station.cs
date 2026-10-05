namespace VegetableLine.Models;
using VegetableLine.Models.Enums;
using VegetableLine.Simulation;

public abstract class Station
{
    public int Id { get; set;}
    public string Name { get; set;} = string.Empty;
    public int Position { get; set;}
    public bool IsRunning { get; set;}
    public StationCondition Condition { get; set;}
    public virtual double Process(double kgIn, SimulationContext context) => kgIn;
    // cada estacion recibe los kilos que le llegan y devuelve los kilos que pasan a la otra estacion.
}