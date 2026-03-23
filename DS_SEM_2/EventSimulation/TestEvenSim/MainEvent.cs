namespace DS_SEM_2.EventSimulation.TestEvenSim;

public abstract class MainEvent : BaseEvent
{
    protected MainSimulation _simulation;
    public Customer Customer { get; set; } = new();

    protected MainEvent(double executeTime, MainSimulation simulation) : base(executeTime)
    {
        _simulation = simulation;
    }
}
