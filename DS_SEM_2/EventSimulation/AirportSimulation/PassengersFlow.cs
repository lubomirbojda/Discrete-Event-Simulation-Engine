namespace DS_SEM_2.EventSimulation.AirportSimulation;

public class PassengersFlow
{
    public Queue<Passenger> PassengersLuggageQueue { get; private set; } = new();
    public Queue<Passenger> PassengersRTGQueue { get; private set; } = new();
    public Queue<Luggage> LuggagesBefore { get; private set; } = new();
    public Queue<Luggage> LuggagesAfter { get; private set; } = new();

    public bool IsLuggageScanAvail { get; set; } = true;
    public bool IsRTGAvail { get; set; } = true;
    public bool IsWorkerAvail { get; set; } = true;
}
