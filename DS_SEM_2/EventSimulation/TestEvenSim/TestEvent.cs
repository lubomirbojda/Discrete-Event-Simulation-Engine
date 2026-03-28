namespace DS_SEM_2.EventSimulation.TestEvenSim;

public abstract class TestEvent : BaseEvent<TestSimulation>
{
    protected TestEvent(double executeTime, TestSimulation simulation) : base(executeTime, simulation)
    {
    }

    public Customer Customer { get; set; } = new();

}
