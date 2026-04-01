using DS_SEM_2.EventSimulation.AirportSimulation.Events;
using DS_SEM_2.Generators.Exponential;
using DS_SEM_2.Generators.Uniform;

namespace DS_SEM_2.EventSimulation.AirportSimulation;

public class AirportSim : BaseEventSimulation
{
    public PassengersFlow PassengersFlow1 { get; private set; } = new();
    public PassengersFlow PassengersFlow2 { get; private set; } = new();

    public Random FlowChooser { get; private set; }
    public Random LuggageCountRand { get; private set; }

    public ExponentialDist PassengerArrivalGen { get; private set; }
    public ContinuousUniform LuggageScanGen { get; private set; }
    public ContinuousUniform RTGScanGen { get; private set; }

    private int PassengerIDSeq = 0;
    private int LuggageIDSeq = 0;

    public int MaxLuggageBefore { get; private set; } = 4;
    public int MaxLuggageAfter { get; private set; } = 5;

    public AirportSim(int seed) : base(seed)
    {
        FlowChooser = new Random(_seedGenerator.Next());
        LuggageCountRand = new Random(_seedGenerator.Next());
        PassengerArrivalGen = new ExponentialDist(_seedGenerator, 0.025);
        LuggageScanGen = new ContinuousUniform(_seedGenerator, 9, 46);
        RTGScanGen = new ContinuousUniform(_seedGenerator, 6, 27);
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

        var u = PassengerArrivalGen.GenerateNext();
        var nextArrivalTime = SimTime + u;
        Passenger nextPass = new(GetNextPassengerID(), Passenger.CreateLuggage(this));
        EPassengerArrival nextArrival = new(nextArrivalTime, this, nextPass);
        PlanEvent(nextArrival);
    }

    protected override void AfterReplication()
    {
    }

    protected override void AfterSimulation()
    {
    }

    protected override void BeforeSimulation()
    {
    }
}
