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

    private const int MAX_SIM_TIME = 24 * 60 * 60;

    public int ServedPassengers { get; set; } = 0;
    public int CamePass { get; set; } = 0;

    private int PassengerIDSeq = 0;
    private int LuggageIDSeq = 0;

    public int MaxLuggageBefore { get; private set; }
    public int MaxLuggageAfter { get; private set; }

    //Statistics
    public SimpleStatistics AverageTimeInSystem { get; private set; } = new();
    public SimpleStatistics AverageTimeInSystemFinal { get; private set; } = new();

    public TimeStatistics AverageLuggageQueueLength { get; private set; } = new();
    public SimpleStatistics AverageLuggageQueueLengthFinal { get; private set; } = new();

    public TimeStatistics AverageLuggageWaitingQueueLength { get; private set; } = new();
    public SimpleStatistics AverageLuggageWaitingQueueLengthFinal { get; private set; } = new();


    //Warm-up
    public int WarmUpSamplesCount { get; } = 100;
    public double WarpUpTolerance { get; } = 0.005;

    public bool IsWarmedUp { get; set; } = false;
    public double LastWarmUpAverage { get; set; } = 0;

    public int WarpUpStabilityCount { get; } = 50;
    public int WarpUpSuccessStabilityCounter { get; set; } = 0;

    public Queue<double> WarmUpSamples { get; private set; } = new();

    public bool IsSimulationFinished { get; private set; } = false; 

    public AirportSim(int seed, int capacity, int maxLuggageBefore, int maxLuggageAfter) : base(seed)
    {
        FlowChooseRand = new Random(_seedGenerator.Next());
        LuggageCountGen = new DiscreteEmpiric(_seedGenerator, [
            (0, 0, 0.15),
            (1, 1, 0.68),
            (2, 2, 0.17)
            ]);
        LuggageScanGen = new ContinuousUniform(_seedGenerator, 9, 46);
        RTGScanGen = new ContinuousUniform(_seedGenerator, 6, 27);
        PersonalScanRand = new Random(_seedGenerator.Next());
        PersonalScanGen = new TriangularDist(_seedGenerator, 10, 120, 35);

        MaxLuggageBefore = maxLuggageBefore;
        MaxLuggageAfter = maxLuggageAfter;
        
        if (capacity <= 0)
            capacity = 1;

        double passArr = MAX_SIM_TIME / (double)capacity;
        PassengerArrivalGen = new ExponentialDist(_seedGenerator, 1.0 / passArr);
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
        IsWarmedUp = false;
        LastWarmUpAverage = 0;
        WarmUpSamples.Clear();


        var u = PassengerArrivalGen.GenerateNext();
        var nextArrivalTime = SimTime + u;
        Passenger nextPass = new(GetNextPassengerID(), Passenger.CreateLuggage(this));
        EPassengerArrival nextArrival = new(nextArrivalTime, this, nextPass);
        PlanEvent(nextArrival);
    }

    protected override void AfterReplication()
    {
        if (!IsRunning)
            return;

        AverageTimeInSystemFinal.Add(AverageTimeInSystem.GetAverage());
        AverageLuggageQueueLengthFinal.Add(AverageLuggageQueueLength.GetAverage(SimTime));
        AverageLuggageWaitingQueueLengthFinal.Add(AverageLuggageWaitingQueueLength.GetAverage(SimTime));

        ResetStatisticsAfterRep();
    }

    protected override void AfterSimulation()
    {
        IsSimulationFinished = true;
        RefreshGUI();
    }

    protected override void BeforeSimulation() 
    {
        SetMaxSimulationTime(MAX_SIM_TIME);
        IsSimulationFinished = false;
    }

    public void ResetStatistics(double time)
    {
        AverageTimeInSystem.Reset();
        AverageLuggageQueueLength.ResetAtTime(time);
        AverageLuggageWaitingQueueLength.ResetAtTime(time);
    }

    private void ResetStatisticsAfterRep()
    {
        AverageTimeInSystem.Reset();
        AverageLuggageQueueLength.Reset();
        AverageLuggageWaitingQueueLength.Reset();
    }
}
