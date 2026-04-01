namespace DS_SEM_2.EventSimulation.AirportSimulation;

public abstract class LuggageEvent : BaseEvent<AirportSim>
{
    public Luggage Luggage_ {  get; private set; }

    public LuggageEvent(double executeTime, AirportSim simulation, Luggage luggage) : base(executeTime, simulation)
    {
        Luggage_ = luggage;
    }
}
