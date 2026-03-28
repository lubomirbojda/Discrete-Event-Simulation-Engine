namespace DS_SEM_2.EventSimulation;

public abstract class BaseEvent<TSimulation> : EventCore
{
    protected TSimulation _simulation;
    protected BaseEvent(double executeTime, TSimulation simulation) : base(executeTime)
    {
        _simulation = simulation;
    }
}
