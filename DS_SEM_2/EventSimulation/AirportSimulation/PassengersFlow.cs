namespace DS_SEM_2.EventSimulation.AirportSimulation;

public class PassengersFlow
{
    public Queue<Passenger> PassengersLuggageQueue { get; private set; } = new();
    public Queue<Passenger> PassengersRTGQueue { get; private set; } = new();
    public Queue<Luggage> LuggagesBeforeQueue { get; private set; } = new();
    public Queue<Luggage> LuggagesAfterQueue { get; private set; } = new();
    public Queue<Passenger> PassengersWaitingLuggageQueue { get; private set; } = new();

    public bool IsLuggageScanAvail { get; set; } = true;
    public bool IsRTGAvail { get; set; } = true;
    public bool IsWorkerAvail { get; set; } = true;

    public void Reset()
    {
        PassengersLuggageQueue.Clear();
        PassengersRTGQueue.Clear();
        LuggagesBeforeQueue.Clear();
        LuggagesAfterQueue.Clear();
        PassengersWaitingLuggageQueue.Clear();
        IsLuggageScanAvail = true;
        IsWorkerAvail = true;
        IsRTGAvail = true;
    }
}
