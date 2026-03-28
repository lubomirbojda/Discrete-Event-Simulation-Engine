using DS_SEM_2.MonteCarlo;

namespace DS_SEM_2.EventSimulation;

public abstract class BaseEventSimulation : BaseMonteCarlo
{
    private PriorityQueue<EventCore, double> _eventsQueue = new();

    public double SimTime { get; private set; } = 0;

    private double _maxSimTime = double.MaxValue;

    private bool _isReplicationRunning = true;

    private bool _isStopped = false;

    private bool _firstSleepPlanned = false;
    public bool IsTurboActive { get; private set; } = true;

    public double Speed { get; set; } = 1;

    protected BaseEventSimulation(int seed) : base(seed)
    {
    }

    public bool PlanEvent(EventCore newEvent)
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

    public void EnableTurboMode()
    {
        IsTurboActive = true;
    }

    public void DisableTurboMode()
    {
        IsTurboActive = false;
        _firstSleepPlanned = false;
    }

    public void StopReplication()
    {
        _isReplicationRunning = false;
    }

    protected override void BeforeReplication()
    {
        SimTime = 0;
        _isReplicationRunning = true;
        _eventsQueue.Clear();
    }

    protected override void DoReplication()
    {
        while (_eventsQueue.Count > 0 && _isReplicationRunning)
        {
            while (_isStopped)
            {
                Thread.Sleep(100);
            }

            if (!IsTurboActive && !_firstSleepPlanned)
            {
                PlanEvent(new ESleep(SimTime, this));
                _firstSleepPlanned = true;
            }

            var actualEvent = _eventsQueue.Dequeue();

            if (actualEvent.ExecuteTime > _maxSimTime)
                break;

            SimTime = actualEvent.ExecuteTime;
            actualEvent.Execute();
        }
    }
}
