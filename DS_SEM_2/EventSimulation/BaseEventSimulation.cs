using DS_SEM_2.MonteCarlo;

namespace DS_SEM_2.EventSimulation;

public abstract class BaseEventSimulation : BaseMonteCarlo
{
    private PriorityQueue<BaseEvent, double> _eventsQueue = new();

    private double _simTime = 0;

    private double _maxSimTime = double.MaxValue;

    protected BaseEventSimulation(int seed) : base(seed)
    {
    }

    public void SetMaxSimulationTime(double maxSimTime)
    {
        _maxSimTime = maxSimTime;
    }

    protected override void DoReplication()
    {
        while (_eventsQueue.Count > 0)
        {
            var actualEvent = _eventsQueue.Dequeue();

            if (actualEvent.ExecuteTime > _maxSimTime)
                break;

            _simTime = actualEvent.ExecuteTime;
            actualEvent.Execute();
        }
    }
}
