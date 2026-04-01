namespace DS_SEM_2.EventSimulation.AirportSimulation;

public class Luggage
{
    public int ID { get; private set; }
    public Passenger Passenger { get; private set; }

    public Luggage(Passenger passenger, int iD)
    {
        Passenger = passenger;
        ID = iD;
    }
}
