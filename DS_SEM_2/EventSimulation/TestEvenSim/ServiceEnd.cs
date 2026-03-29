namespace DS_SEM_2.EventSimulation.TestEvenSim;

public class ServiceEnd : TestEvent
{
    public ServiceEnd(double executeTime, TestSimulation simulation) : base(executeTime, simulation)
    {
    }

    public override void Execute()
    {
        if (Simulation.CustomersQueue.Count > 0)
        {
            var nextCustomer = Simulation.CustomersQueue.Dequeue();
            ServiceStart serviceStart = new(Simulation.SimTime, Simulation)
            {
                Customer = nextCustomer
            };
            Simulation.PlanEvent(serviceStart);

            Simulation.AverageLenghtOfQueue.Add(Simulation.CustomersQueue.Count, ExecuteTime);
        }
        else
        {
            Simulation.IsSellerAvailable = true;
        }

        Simulation.OneCustomerServed();
    }
}
