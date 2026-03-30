using DS_SEM_2.Generators.Exponential;

namespace DS_SEM_2.EventSimulation.AirportSimulation;

public class AirportSim : BaseEventSimulation
{
    public PassengersFlow PassengersFlow1 { get; private set; } = new();
    public PassengersFlow PassengersFlow2 { get; private set; } = new();

    public Random FlowChooser { get; private set; }
    public Random LuggageCountRand { get; private set; }

    public ExponentialDist PassengerArrivalGen { get; private set; }

    private int PassengerIDSeq = 0;
    private int LuggageIDSeq = 0;

    public int MaxLuggageBefore { get; private set; } = 4;
    public int MaxLuggageAfter { get; private set; } = 5;

    public AirportSim(int seed) : base(seed)
    {
        FlowChooser = new Random(_seedGenerator.Next());
        LuggageCountRand = new Random(_seedGenerator.Next());
        PassengerArrivalGen = new ExponentialDist(_seedGenerator, 0.025);
    }

    public int GetNextPassengerID()
    {
        return PassengerIDSeq++;
    }

    public int GetNextLuggageID()
    {
        return LuggageIDSeq++;
    }

    protected override void AfterReplication()
    {
        throw new NotImplementedException();
    }

    protected override void AfterSimulation()
    {
        throw new NotImplementedException();
    }

    protected override void BeforeSimulation()
    {
        throw new NotImplementedException();
    }
}
