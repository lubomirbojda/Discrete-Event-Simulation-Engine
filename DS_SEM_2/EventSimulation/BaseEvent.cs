namespace DS_SEM_2.EventSimulation;

public abstract class BaseEvent
{
    public double ExecuteTime { get; private set; }

    public BaseEvent(double executeTime)
    {
        ExecuteTime = executeTime;
    }

    public abstract void Execute();
}
