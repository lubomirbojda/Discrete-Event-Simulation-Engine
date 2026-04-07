using DS_SEM_2.MonteCarlo;

namespace DS_SEM_2.EventSimulation;

public abstract class BaseEventSimulation : BaseMonteCarlo
{
    private PriorityQueue<EventCore, double> _eventsQueue = new();

    public double SimTime { get; private set; } = 0;

    private double _maxSimTime = double.MaxValue;

    private bool _isReplicationRunning = true;

    private bool _isPaused = false;

    private List<ISimGuiDelegate> _simGUIDelegates = new();

    private bool _firstSleepPlanned = false;

    public bool IsTurboActive { get; private set; } = true;

    public double Speed { get; private set; } = 1;

    protected BaseEventSimulation(int seed) : base(seed)
    {
    }

    public void SetSpeed(double speed)
    {
        if (speed > 0)
            Speed = speed;
    }

    public void PauseSimulation()
    {
        _isPaused = true;
    }

    public void ResumeSimulation()
    {
        _isPaused = false;
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

    public void RegisterGUIDelegate(ISimGuiDelegate simGuiDelegate)
    {
        _simGUIDelegates.Add(simGuiDelegate);
    }

    public void StopReplication()
    {
        _isReplicationRunning = false;
    }

    public void RefreshGUI()
    {
        foreach (var gui in _simGUIDelegates)
        {
            gui.RefreshGUI();
        }
    }

    protected override void BeforeReplication()
    {
        SimTime = 0;
        _isReplicationRunning = true;
        _eventsQueue.Clear();
        _firstSleepPlanned = false;
    }

    protected override void DoReplication()
    {
        while (_eventsQueue.Count > 0 && _isReplicationRunning)
        {
            while (_isPaused)
            {
                Thread.Sleep(100);
            }

            if (!IsTurboActive && !_firstSleepPlanned)
            {
                PlanEvent(new ESleep(SimTime, this));
                _firstSleepPlanned = true;
            }

            if (!IsRunning)
                return;

            var actualEvent = _eventsQueue.Dequeue();

            if (actualEvent.ExecuteTime > _maxSimTime)
                break;

            SimTime = actualEvent.ExecuteTime;
            actualEvent.Execute();
        }
    }
}
