namespace DS_SEM_2.EventSimulation.TestEvenSim;

public abstract class TestEvent : BaseEvent
{
    protected TestSimulation _simulation;
    public Customer Customer { get; set; } = new();

    protected TestEvent(double executeTime, TestSimulation simulation) : base(executeTime)
    {
        _simulation = simulation;
    }
}
