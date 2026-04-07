using DS_SEM_2.EventSimulation.AirportSimulation;

namespace DS_SEM_2;

public class Program
{
    static void Main(string[] args)
    {
        int startCapacity = 5710;
        int endCapacity = 5730;
        int replications = 10000;

        int maxBefore = 4;
        int maxAfter = 5;

        string fileName = "SimulationResults.csv";

        using (StreamWriter writer = new StreamWriter(fileName))
        {
            writer.WriteLine("Capacity;AvgTime;CITime;AvgLugQueue;CILugQueue;AvgWaitQueue;CIWaitQueue");

            Console.WriteLine($"Spúšťam experimenty od {startCapacity} po {endCapacity}");

            for (int cap = startCapacity; cap <= endCapacity; cap++)
            {
                var sim = new AirportSim(new Random().Next(), cap, maxBefore, maxAfter);
                sim.RunSimulation(replications);

                var ciTime = sim.AverageTimeInSystemFinal.GetConfidenceInterval90();
                var ciLug = sim.AverageLuggageQueueLengthFinal.GetConfidenceInterval90();
                var ciWait = sim.AverageLuggageWaitingQueueLengthFinal.GetConfidenceInterval90();

                double avgTime = sim.AverageTimeInSystemFinal.GetAverage();
                double avgLug = sim.AverageLuggageQueueLengthFinal.GetAverage();
                double avgWait = sim.AverageLuggageWaitingQueueLengthFinal.GetAverage();

                string line = $"{cap};" +
                    $"{avgTime:F5};[{ciTime.Lower:F5}  -  {ciTime.Upper:F5}];" +
                    $"{avgLug:F5};[{ciLug.Lower:F5}  -  {ciLug.Upper:F5}];" +
                    $"{avgWait:F5};[{ciWait.Lower:F5}  -  {ciWait.Upper:F5}]";

                writer.WriteLine(line);
                Console.WriteLine($"Kapacita {cap} zapísaná.");
            }
        }

        Console.WriteLine($"\nHotovo - súbor: {Path.GetFullPath(fileName)}");
    }
}