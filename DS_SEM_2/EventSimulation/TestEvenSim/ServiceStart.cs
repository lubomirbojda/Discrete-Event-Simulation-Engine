namespace DS_SEM_2.EventSimulation.TestEvenSim;

public class ServiceStart : MainEvent
{
    public ServiceStart(double executeTime, MainSimulation simulation) : base(executeTime, simulation)
    {
    }

    public override void Execute()
    {
        var u = _simulation.ServiceTimeGen.GenerateNext();
        var serviceEndTime = _simulation.SimTime + u;
        ServiceEnd serviceEnd = new(serviceEndTime, _simulation)
        {
            Customer = Customer
        };
        _simulation.PlanEvent(serviceEnd);
    }
}
