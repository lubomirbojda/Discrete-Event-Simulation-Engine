namespace DS_SEM_2.Generators.Empiric.Continuous;

public class IntervalContinuous(double min, double max, double probability, double upperBound, int seedForGenerator) 
    : BaseInterval<double>(min, max, probability, upperBound, seedForGenerator)
{
    public override double GenerateNext()
    {
        return (_generator.NextDouble() * _generationRange) + Min;
    }
}
