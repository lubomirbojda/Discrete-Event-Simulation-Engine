namespace DS_SEM_2.EventSimulation.TestEvenSim;

public class CustomerArrival : MainEvent
{
    public CustomerArrival(double executeTime, MainSimulation simulation) : base(executeTime, simulation)
    {
    }

    public override void Execute()
    {
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
        }

        var u = _simulation.CustommerArrivalGen.GenerateNext();
        var nextArrivalTime = _simulation.SimTime + u;
        CustomerArrival nextCustArr = new(nextArrivalTime, _simulation);
        _simulation.PlanEvent(nextCustArr);
    }
}
