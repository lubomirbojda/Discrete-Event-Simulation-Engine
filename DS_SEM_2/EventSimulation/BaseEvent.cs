namespace DS_SEM_2.EventSimulation;

public abstract class BaseEvent<TSimulation> : EventCore
{
    protected TSimulation Simulation { get; }
    protected BaseEvent(double executeTime, TSimulation simulation) : base(executeTime)
    {
        Simulation = simulation;
    }
}
