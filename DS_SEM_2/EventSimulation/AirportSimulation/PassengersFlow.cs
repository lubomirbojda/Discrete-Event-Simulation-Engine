namespace DS_SEM_2.EventSimulation.AirportSimulation;

public class PassengersFlow
{
    public Queue<Passenger> PassengersLuggageQueue { get; private set; } = new();
    public Queue<Passenger> PassengersRTGQueue { get; private set; } = new();
    public Queue<int> LuggagesBefore { get; private set; } = new();
    public Queue<int> LuggagesAfter { get; private set; } = new();
}
