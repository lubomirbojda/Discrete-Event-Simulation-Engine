namespace DS_SEM_2.EventSimulation.TestEvenSim;

public class ServiceEnd : MainEvent
{
    public ServiceEnd(double executeTime, MainSimulation simulation) : base(executeTime, simulation)
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
        }
        else
        {
            _simulation.IsSellerAvailable = true;
        }
    }
}
