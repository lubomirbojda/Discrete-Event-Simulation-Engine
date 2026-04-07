namespace DS_SEM_2.EventSimulation.Statistics;

public class TimeStatistics
{
    private double _sum = 0;
    private double _lastValue = 0;
    private double _lastTime = 0;
    private double _startTime = 0;

    public void Add(double value, double time)
    {
        double timeDifference = time - _lastTime;
        _sum += _lastValue * timeDifference;
        _lastValue = value;
        _lastTime = time;
    }

    public double GetAverage(double time)
    {
        double elapsedTime = time - _startTime;
        if (elapsedTime <= 0)
            return 0;

        double timeDifference = time - _lastTime;
        double finalSum = _sum + (_lastValue * timeDifference);

        return finalSum / elapsedTime;
    }

    public void Reset()
    {
        _sum = 0;
        _lastValue = 0;
        _lastTime = 0;
        _startTime = 0;
    }

    public void ResetAtTime(double time)
    {
        _sum = 0;
        _lastTime = time;
        _startTime = time;
    }
}
