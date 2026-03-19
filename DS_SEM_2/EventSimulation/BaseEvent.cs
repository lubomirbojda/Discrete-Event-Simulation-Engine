namespace DS_SEM_2.EventSimulation;

public abstract class BaseEvent
{
    public double ExecuteTime { get; private set; }

    public abstract void Execute();
}
