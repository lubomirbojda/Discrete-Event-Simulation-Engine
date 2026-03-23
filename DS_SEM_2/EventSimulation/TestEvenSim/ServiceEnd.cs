namespace DS_SEM_2.EventSimulation.TestEvenSim;

public class ServiceEnd : TestEvent
{
    public ServiceEnd(double executeTime, TestSimulation simulation) : base(executeTime, simulation)
    {
    }

    public override void Execute()
    {
        if (_simulation.CustomersQueue.Count > 0)
        {
            var nextCustomer = _simulation.CustomersQueue.Dequeue();
            ServiceStart serviceStart = new(_simulation.SimTime, _simulation)
            {
                Customer = nextCustomer
            };
            _simulation.PlanEvent(serviceStart);

            _simulation.AverageLenghtOfQueue.Add(_simulation.CustomersQueue.Count, ExecuteTime);
        }
        else
        {
            _simulation.IsSellerAvailable = true;
        }

        _simulation.OneCustomerServed();
    }
}
