namespace DS_SEM_2.EventSimulation.TestEvenSim;

public class ServiceStart : TestEvent
{
    public ServiceStart(double executeTime, TestSimulation simulation) : base(executeTime, simulation)
    {
    }

    public override void Execute()
    {
        var u = Simulation.ServiceTimeGen.GenerateNext();
        var serviceEndTime = Simulation.SimTime + u;
        ServiceEnd serviceEnd = new(serviceEndTime, Simulation)
        {
            Customer = Customer
        };
        Simulation.PlanEvent(serviceEnd);

        Simulation.AverageCustomerTimeInQueue.Add(ExecuteTime - Customer.ArrivalTime);
    }
}
