using DS_SEM_2.MonteCarlo;

namespace DS_SEM_2.EventSimulation;

public abstract class BaseEventSimulation : BaseMonteCarlo
{
    private PriorityQueue<BaseEvent, double> _eventsQueue = new();

    public double SimTime { get; private set; } = 0;

    private double _maxSimTime = double.MaxValue;

    private bool _isRunning = true;

    protected BaseEventSimulation(int seed) : base(seed)
    {
    }

    public bool PlanEvent(BaseEvent newEvent)
    {
        if (newEvent.ExecuteTime < SimTime)
            return false;
        _eventsQueue.Enqueue(newEvent, newEvent.ExecuteTime);
        return true;
    }

    public void SetMaxSimulationTime(double maxSimTime)
    {
        _maxSimTime = maxSimTime;
    }

    public void StopReplication()
    {
        _isRunning = false;
    }

    protected override void BeforeReplication()
    {
        SimTime = 0;
        _isRunning = true;
        _eventsQueue.Clear();
    }

    protected override void DoReplication()
    {
        while (_eventsQueue.Count > 0 && _isRunning)
        {
            var actualEvent = _eventsQueue.Dequeue();

            if (actualEvent.ExecuteTime > _maxSimTime)
                break;

            SimTime = actualEvent.ExecuteTime;
            actualEvent.Execute();
        }
    }
}
