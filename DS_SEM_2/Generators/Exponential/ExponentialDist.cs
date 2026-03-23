namespace DS_SEM_2.Generators.Exponential;

public class ExponentialDist
{
    private Random _rand;
    private double _lambda;

    public ExponentialDist(Random seedGenerator, double lambda)
    {
        _rand = new Random(seedGenerator.Next());

        if (lambda <= 0)
            throw new ArgumentException("Lambda musi byt vacsia ako 0.");

        _lambda = lambda;
    }
    public double GenerateNext()
    {
        var r = _rand.NextDouble();
        while (r == 0)
            r = _rand.NextDouble();

        return -Math.Log(r) / _lambda;
    }
}
