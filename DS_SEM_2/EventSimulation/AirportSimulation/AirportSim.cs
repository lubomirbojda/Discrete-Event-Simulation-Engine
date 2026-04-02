using DS_SEM_2.EventSimulation.AirportSimulation.Events;
using DS_SEM_2.EventSimulation.Statistics;
using DS_SEM_2.Generators.Empiric.Discrete;
using DS_SEM_2.Generators.Exponential;
using DS_SEM_2.Generators.Triangular;
using DS_SEM_2.Generators.Uniform;

namespace DS_SEM_2.EventSimulation.AirportSimulation;

public class AirportSim : BaseEventSimulation
{
    public PassengersFlow PassengersFlow1 { get; private set; } = new();
    public PassengersFlow PassengersFlow2 { get; private set; } = new();

    public Random FlowChooseRand { get; private set; }
    public Random PersonalScanRand { get; private set; }
    public DiscreteEmpiric LuggageCountGen { get; private set; }

    public ExponentialDist PassengerArrivalGen { get; private set; }
    public ContinuousUniform LuggageScanGen { get; private set; }
    public ContinuousUniform RTGScanGen { get; private set; }
    public TriangularDist PersonalScanGen { get; private set; }

    public int ServedPassengers { get; set; } = 0;
    public int CamePass { get; set; } = 0;

    private int PassengerIDSeq = 0;
    private int LuggageIDSeq = 0;

    public int MaxLuggageBefore { get; private set; } = 4;
    public int MaxLuggageAfter { get; private set; } = 5;

    //Statistics
    public SimpleStatistics AverageTimeInSystem { get; private set; } = new();
    private SimpleStatistics _averageTimeInSystem = new();

    public TimeStatistics AverageLuggageQueueLength { get; private set; } = new();
    private SimpleStatistics _averageLuggageQueueLength = new();

    public SimpleStatistics AverageServedPassengers { get; private set; } = new();
    private SimpleStatistics _averageServedPassengers = new();

    public TimeStatistics AverageLuggageWaitingQueueLength { get; private set; } = new();
    private SimpleStatistics _averageLuggageWaitingQueueLength = new();

    public AirportSim(int seed) : base(seed)
    {
        FlowChooseRand = new Random(_seedGenerator.Next());
        LuggageCountGen = new DiscreteEmpiric(_seedGenerator, [
            (0, 0, 0.15),
            (1, 1, 0.68),
            (2, 2, 0.17)
            ]);
        PassengerArrivalGen = new ExponentialDist(_seedGenerator, 1.0/15.0);
        LuggageScanGen = new ContinuousUniform(_seedGenerator, 9, 46);
        RTGScanGen = new ContinuousUniform(_seedGenerator, 6, 27);
        PersonalScanRand = new Random(_seedGenerator.Next());
        PersonalScanGen = new TriangularDist(_seedGenerator, 10, 120, 35);
    }

    public int GetNextPassengerID()
    {
        return PassengerIDSeq++;
    }

    public int GetNextLuggageID()
    {
        return LuggageIDSeq++;
    }

    protected override void BeforeReplication()
    {
        base.BeforeReplication();
        LuggageIDSeq = 0;
        PassengerIDSeq = 0;
        CamePass = 0;
        ServedPassengers = 0;
        PassengersFlow1.Reset();
        PassengersFlow2.Reset();


        var u = PassengerArrivalGen.GenerateNext();
        var nextArrivalTime = SimTime + u;
        Passenger nextPass = new(GetNextPassengerID(), Passenger.CreateLuggage(this));
        EPassengerArrival nextArrival = new(nextArrivalTime, this, nextPass);
        PlanEvent(nextArrival);
    }

    protected override void AfterReplication()
    {
        _averageTimeInSystem.Add(AverageTimeInSystem.GetAverage());
        _averageLuggageQueueLength.Add(AverageLuggageQueueLength.GetAverage(SimTime));
        _averageServedPassengers.Add(ServedPassengers);
        _averageLuggageWaitingQueueLength.Add(AverageLuggageWaitingQueueLength.GetAverage(SimTime));

        Console.WriteLine("Replikacia c. " + CurrentReplication + " hotova");
        Console.WriteLine("Prislo: " + CamePass);
        Console.WriteLine("Obsluzenyvh bolo " + ServedPassengers);
        Console.WriteLine("End sim time: " + SimTime);

        AverageTimeInSystem.Reset();
        AverageLuggageQueueLength.Reset();
        AverageServedPassengers.Reset();
        AverageLuggageWaitingQueueLength.Reset();
    }

    protected override void AfterSimulation()
    {
        Console.WriteLine("Priemerny cas zakaznika v systeme: " + _averageTimeInSystem.GetAverage() / 60 + " minut");
        Console.WriteLine("Primerna dlzka radu pred batozinami: " + _averageLuggageQueueLength.GetAverage());
        Console.WriteLine("Priemerny pocet obsluzenych zakaznikov: " + _averageServedPassengers.GetAverage());
        Console.WriteLine("Priemerna dlzka radu cakajucich na batoziny: " + _averageLuggageWaitingQueueLength.GetAverage());
    }

    protected override void BeforeSimulation() 
    {
        SetMaxSimulationTime(24*60*60);
    }
}
