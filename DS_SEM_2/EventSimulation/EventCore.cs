namespace DS_SEM_2.EventSimulation;

public abstract class EventCore
{
    public double ExecuteTime { get; protected set; }

    public EventCore(double executeTime)
    {
        ExecuteTime = executeTime;
    }

    public abstract void Execute();
}
