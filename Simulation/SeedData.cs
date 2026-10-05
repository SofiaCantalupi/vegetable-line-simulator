namespace VegetableLine.Simulation;

using VegetableLine.Models;
using VegetableLine.Models.Enums;


public static class SeedData
{
    public static void Populate(SimulationContext context)
    {
        var weighing = context.SimulationSettings.Weighing;

        //El lote 2 llega con mucha tierra (factor 1.8) y el 3 con papas defectuosas (factor 1.5)
        var batches = new List<Batch>
        {
            new() { Id = 1, Producer = "Agro Los Teros", Origin = "Balcarce",
                    Variety = PotatoVariety.Spunta, WeightKg = 6000, ArrivedAt = DateTime.Now.AddHours(-5) },
            new() { Id = 2, Producer = "Hnos. Ferreyra", Origin = "Villa Dolores",
                    Variety = PotatoVariety.Asterix, WeightKg = 9000, ArrivedAt = DateTime.Now.AddHours(-3), SoilFactor = 1.8 },
            new() { Id = 3, Producer = "La Cosecha SRL", Origin = "Tafí del Valle",
                    Variety = PotatoVariety.Donata, WeightKg = 12000, ArrivedAt = DateTime.Now.AddHours(-1), DefectFactor = 1.5 },
        };

        context.Batches.AddRange(batches); // agrega todos los elementos de una vez

        foreach (var batch in batches)
        {
            context.Orders.Add(new ProductionOrder
            {
                Id = batch.Id,
                BatchId = batch.Id,
                Batch = batch,
                Status = OrderStatus.Pending,
                TargetWeight = weighing.TargetWeightKg,
                Tolerance = weighing.ToleranceKg,
                PlannedQuantity = (int)(batch.WeightKg * 0.93 / weighing.TargetWeightKg) // Es una estimacion de cuantas bolsas deberian salir. El int redondea
            });
        }

        context.Stations.AddRange(new Station[]
        {
            new ConveyorBelt        { Id = 1,  Name = "Feed conveyor",               Position = 1,  IsRunning = true, Speed = 1.2 },
            new Cleaner         { Id = 2,  Name = "Cleaner",                     Position = 2,  IsRunning = true },
            new ConveyorBelt        { Id = 3,  Name = "Conveyor cleaner-grader",     Position = 3,  IsRunning = true, Speed = 1.2 },
            new Grader          { Id = 4,  Name = "Grader",                      Position = 4,  IsRunning = true },
            new ConveyorBelt        { Id = 5,  Name = "Conveyor grader-inspection",  Position = 5,  IsRunning = true, Speed = 1.0 },
            new InspectionTable { Id = 6,  Name = "Inspection table",            Position = 6,  IsRunning = true, OperatorCount = 4 },
            new ConveyorBelt        { Id = 7,  Name = "Conveyor inspection-weigher", Position = 7,  IsRunning = true, Speed = 1.2 },
            new Weigher         { Id = 8,  Name = "Weigher",                     Position = 8,  IsRunning = true },
            new ConveyorBelt        { Id = 9,  Name = "Conveyor weigher-closer",     Position = 9,  IsRunning = true, Speed = 0.8 },
            new BagCloser       { Id = 10, Name = "Bag closer",                  Position = 10, IsRunning = true },
        });
    }
}