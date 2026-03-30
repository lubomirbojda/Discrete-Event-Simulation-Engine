namespace DS_SEM_2.EventSimulation.AirportSimulation;

public class Luggage
{
    public List<int> IDs { get; private set; } = new();
    public Passenger Passenger { get; private set; }

    public Luggage(Passenger passenger)
    {
        Passenger = passenger;
    }

    public void AddLuggage(int id)
    {
        IDs.Add(id);
    }
}
