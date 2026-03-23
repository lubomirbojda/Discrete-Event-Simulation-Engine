namespace DS_SEM_2.EventSimulation.Statistics;

public class SimpleStatistics
{
    private double _sumOfValues = 0;
    private int _count = 0;

    public void Add(double value)
    {
        _sumOfValues += value;
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
}
