using VegetableLine.Simulation;
using VegetableLine.SimulationSettings;
using VegetableLine.Models.Enums;

namespace VegetableLine.Models;

//Acumula kilos y arma bolsas cuando puede 
public class Weigher : Station
{
    public double AccumulatedKg { get; set; }

    public override double Process(double kgIn, SimulationContext context)
    {
        var order = context.CurrentOrder!;
        var s = context.SimulationSettings.Weighing;

        // Suma lo que llego en ese tick a lo que ya acumulado
        AccumulatedKg += kgIn;

        // Mientras haya kilos suficientes para una bolsa, sigue armando bolsa
        while (AccumulatedKg >= order.TargetWeight)
        {
            // Peso real de esta bolsa: casi siempre cerca del objetivo
            double weight = GenerateWeight(order, s);
            // Descuenta el peso real, no el objetivo
            AccumulatedKg -= weight;

            //La bolsa esta bien si esta dentro de la tolerancia
            bool withinTolerance = Math.Abs(weight - order.TargetWeight) <= order.Tolerance;

            var bag = new Bag
            {
                Id = context.Bags.Count + 1,
                ProductionOrderId = order.Id,
                ActualWeight = weight,
                WeighedAt = DateTime.Now,
                // Operador ternario: si esta en tolerancia queda pesada, si no, rechazada
                Status = withinTolerance ? BagStatus.Weighed : BagStatus.Rejected
            };

            context.Bags.Add(bag);
            context.NewBagsThisTick.Add(bag);
        }

        return 0; // no pasa kilos a la siguiente estacion, todo queda en la balanza
    }

    // Genera un peso aleatorio para una bolsa. Segun la probabilidad configurada, el peso queda dentro o fuera de tolerancia
    private static double GenerateWeight(ProductionOrder order, WeighingSettings s)
    {
        // Caso poco frecuente (por defecto 10%): bolsa fuera de tolerancia
        if (RandomHelper.Chance(s.OutOfToleranceProbability))
        {
            // Desvio entre 1 y 4 veces la tolerancia
            double deviation = RandomHelper.Between(order.Tolerance, order.Tolerance * 4);

            //// 50% de probabilidad de que sobre peso y 50% de que falte
            return RandomHelper.Chance(0.5)
                ? order.TargetWeight + deviation
                : order.TargetWeight - deviation;
        }

        // Caso mas frecuente: bolsa dentro de tolerancia
        return order.TargetWeight + RandomHelper.Between(-order.Tolerance, order.Tolerance);
    }
}