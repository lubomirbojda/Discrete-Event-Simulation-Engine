namespace DS_SEM_2.EventSimulation.Statistics;

public class SimpleStatistics
{
    private double _sumOfValues = 0;
    private double _sumOfSquares = 0;
    private int _count = 0;

    public void Add(double value)
    {
        _sumOfValues += value;
        _sumOfSquares += value * value;
        _count++;
    }

    public double GetAverage()
    {
        if (_count == 0)
            return 0;
        return _sumOfValues / _count;
    }

    public void Reset()
    {
        _sumOfValues = 0;
        _count = 0;
    }

    //metóda vytvorená pomocou AI - zdokumentované v kapitole 2
    private double GetStandardDeviation()
    {
        if (_count < 2) return 0;
        double variance = (_sumOfSquares - (_sumOfValues * _sumOfValues) / _count) / (_count - 1);
        return Math.Sqrt(Math.Max(0, variance));
    }

    //metóda vytvorená pomocou AI - zdokumentované v kapitole 2
    public (double Lower, double Upper) GetConfidenceInterval95()
    {
        return CalculateInterval(1.96);
    }

    //metóda vytvorená pomocou AI - zdokumentované v kapitole 2
    public (double Lower, double Upper) GetConfidenceInterval90()
    {
        return CalculateInterval(1.645);
    }

    //metóda vytvorená pomocou AI - zdokumentované v kapitole 2
    private (double Lower, double Upper) CalculateInterval(double z)
    {
        if (_count < 2) return (0, 0);

        double avg = GetAverage();
        double standardError = GetStandardDeviation() / Math.Sqrt(_count);
        double halfWidth = z * standardError;

        return (avg - halfWidth, avg + halfWidth);
    }
}
