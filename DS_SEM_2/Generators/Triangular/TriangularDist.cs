namespace DS_SEM_2.Generators.Triangular;

public class TriangularDist
{
    private Random _rand;
    private double _min;
    private double _max;
    private double _mod;
    public TriangularDist(Random seedGenerator, double min, double max, double mod)
    {
        _rand = new Random(seedGenerator.Next());
        if (min >= max)
            throw new ArgumentException("Min musi byt mensi nez max.");
        if (mod < min || mod > max)
            throw new ArgumentException("Mod musi byt mezi min a max.");

        _min = min;
        _max = max;
        _mod = mod;
    }

    //metóda vytvorená pomocou AI - zdokumentované v kapitole 1
    public double GenerateNext()
    {
        double u = _rand.NextDouble();
        double F_mod = (_mod - _min) / (_max - _min);

        if (u < F_mod)
        {
            return _min + Math.Sqrt(u * (_max - _min) * (_mod - _min));
        }
        else
        {
            return _max - Math.Sqrt((1 - u) * (_max - _min) * (_max - _mod));
        }
    }
}
