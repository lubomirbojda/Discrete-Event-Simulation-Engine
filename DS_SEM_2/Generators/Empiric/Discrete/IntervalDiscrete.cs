namespace DS_SEM_2.Generators.Empiric.Discrete;

public class IntervalDiscrete(int min, int max, double probability, double upperBound, int seedForGenerator) 
    : BaseInterval<int>(min, max, probability, upperBound, seedForGenerator)
{
    public override int GenerateNext()
    {
        return _generator.Next(_generationRange) + Min;
    }
}
