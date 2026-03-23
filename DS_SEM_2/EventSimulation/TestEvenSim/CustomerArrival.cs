namespace DS_SEM_2.EventSimulation.TestEvenSim;

public class CustomerArrival : TestEvent
{
    public CustomerArrival(double executeTime, TestSimulation simulation) : base(executeTime, simulation)
    {
    }

    public override void Execute()
    {
        Customer.ArrivalTime = ExecuteTime;
        if (_simulation.IsSellerAvailable)
        {
            ServiceStart serviceStart = new(_simulation.SimTime, _simulation)
            {
                Customer = Customer
            };
            _simulation.PlanEvent(serviceStart);
            _simulation.IsSellerAvailable = false;
        }
        else
        {
            _simulation.CustomersQueue.Enqueue(Customer);
            _simulation.AverageLenghtOfQueue.Add(_simulation.CustomersQueue.Count, ExecuteTime);
        }

        var u = _simulation.CustommerArrivalGen.GenerateNext();
        var nextArrivalTime = _simulation.SimTime + u;
        CustomerArrival nextCustArr = new(nextArrivalTime, _simulation);
        _simulation.PlanEvent(nextCustArr);
    }
}
