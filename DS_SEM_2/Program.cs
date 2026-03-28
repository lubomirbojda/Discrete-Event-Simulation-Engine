using DS_SEM_2.EventSimulation.TestEvenSim;
using DS_SEM_2.Generators;

namespace DS_SEM_2;

public class Program
{
    static void Main(string[] args)
    {
        //GeneratorsTester.TriangularDistributionTest(0, 100, 50, 10000);
        //GeneratorsTester.ExponentialDistributionTest(0.1, 10000);

        Random r = new();
        TestSimulation test = new(r.Next(), 100000);
        test.RunSimulation(100);
    }
}
