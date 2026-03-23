using DS_SEM_2.Generators.Exponential;
using DS_SEM_2.Generators.Triangular;

namespace DS_SEM_2.EventSimulation.TestEvenSim;

public class MainSimulation : BaseEventSimulation
{
    public Queue<Customer> CustomersQueue { get; private set; } = new();
    public bool IsSellerAvailable { get; set; } = true;

    public ExponentialDist CustommerArrivalGen { get; private set; }
    public TriangularDist ServiceTimeGen { get; private set; }

    public MainSimulation(int seed) : base(seed)
    {
        CustommerArrivalGen = new ExponentialDist(_seedGenerator, 0.025);
        ServiceTimeGen = new TriangularDist(_seedGenerator, 12.0, 48.0, 30.0);
    }

    protected override void BeforeSimulation()
    {
    }

    protected override void BeforeReplication()
    {
        var firstArrivalTime = CustommerArrivalGen.GenerateNext();
        CustomerArrival firstArrival = new(firstArrivalTime, this);
        PlanEvent(firstArrival);
    }

    protected override void AfterReplication()
    {
    }

    protected override void AfterSimulation()
    {
    }
}
