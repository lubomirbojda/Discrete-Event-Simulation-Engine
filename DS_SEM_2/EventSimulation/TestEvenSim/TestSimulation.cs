using DS_SEM_2.EventSimulation.Statistics;
using DS_SEM_2.Generators.Exponential;
using DS_SEM_2.Generators.Triangular;

namespace DS_SEM_2.EventSimulation.TestEvenSim;

public class TestSimulation : BaseEventSimulation
{
    public Queue<Customer> CustomersQueue { get; private set; } = new();
    public bool IsSellerAvailable { get; set; } = true;

    public ExponentialDist CustommerArrivalGen { get; private set; }
    public TriangularDist ServiceTimeGen { get; private set; }

    public int MaxServedCustomers { get; private set; }

    public int ServedCustomers { get; private set; } = 0;

    public SimpleStatistics AverageCustomerTimeInQueue { get; private set; } = new();
    public TimeStatistics AverageLenghtOfQueue { get; private set; } = new();

    private SimpleStatistics _averageCustomerTimeInQueue = new();
    private SimpleStatistics _averageLengthOfQueue = new();

    public TestSimulation(int seed, int maxServedCustomers) : base(seed)
    {
        CustommerArrivalGen = new ExponentialDist(_seedGenerator, 0.025);
        ServiceTimeGen = new TriangularDist(_seedGenerator, 12.0, 48.0, 30.0);
        MaxServedCustomers = maxServedCustomers;
    }

    public void OneCustomerServed()
    {
        ServedCustomers++;
        if (ServedCustomers >= MaxServedCustomers)
            StopReplication();
    }

    protected override void BeforeSimulation()
    {
    }

    protected override void BeforeReplication()
    {
        base.BeforeReplication();
        CustomersQueue.Clear();
        IsSellerAvailable = true;
        ServedCustomers = 0;

        var firstArrivalTime = CustommerArrivalGen.GenerateNext();
        CustomerArrival firstArrival = new(firstArrivalTime, this);
        PlanEvent(firstArrival);
    }

    protected override void AfterReplication()
    {
        _averageCustomerTimeInQueue.Add(AverageCustomerTimeInQueue.GetAverage());
        _averageLengthOfQueue.Add(AverageLenghtOfQueue.GetAverage(SimTime));

        Console.WriteLine("One rep done " + _currentReplication);

        AverageCustomerTimeInQueue.Reset();
        AverageLenghtOfQueue.Reset();
    }

    protected override void AfterSimulation()
    {
        Console.WriteLine("Priemerny cas cakania v rade: " + _averageCustomerTimeInQueue.GetAverage() + " sekund");
        var (lower, higher) = _averageCustomerTimeInQueue.GetConfidenceInterval90();
        Console.WriteLine("90% Interval spolahlivosti: <" + lower + ", " + higher + ">");
        
        Console.WriteLine("Priemerna dlzka radu: " + _averageLengthOfQueue.GetAverage());
        (lower, higher) = _averageLengthOfQueue.GetConfidenceInterval90();
        Console.WriteLine("90% Interval spolahlivosti: <" + lower + ", " + higher + ">");
    }
}
