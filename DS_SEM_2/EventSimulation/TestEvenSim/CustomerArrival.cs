namespace DS_SEM_2.EventSimulation.TestEvenSim;

public class CustomerArrival : TestEvent
{
    public CustomerArrival(double executeTime, TestSimulation simulation) : base(executeTime, simulation)
    {
    }

    public override void Execute()
    {
        Customer.ArrivalTime = ExecuteTime;
        if (Simulation.IsSellerAvailable)
        {
            ServiceStart serviceStart = new(Simulation.SimTime, Simulation)
            {
                Customer = Customer
            };
            Simulation.PlanEvent(serviceStart);
            Simulation.IsSellerAvailable = false;
        }
        else
        {
            Simulation.CustomersQueue.Enqueue(Customer);
            Simulation.AverageLenghtOfQueue.Add(Simulation.CustomersQueue.Count, ExecuteTime);
        }

        var u = Simulation.CustommerArrivalGen.GenerateNext();
        var nextArrivalTime = Simulation.SimTime + u;
        CustomerArrival nextCustArr = new(nextArrivalTime, Simulation);
        Simulation.PlanEvent(nextCustArr);
    }
}
