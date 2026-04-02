namespace DS_SEM_2.EventSimulation.AirportSimulation;

public class Passenger
{
    public int Id { get; private set; }
    public double ArrivalTime { get; set; }
    public List<Luggage> Luggages { get; private set; } = new();
    public PassengersFlow PassengerFlow { get; set; } = new();

    public int LuggageTotal { get; private set; }
    public int LuggageCurrent { get; set; }
    public bool IsReadyToLeave { get; set; } = false;

    public bool AlreadyLeft { get; set; } = false;

    public Passenger(int id, int[] luggageIds)
    {
        Id = id;
        LuggageTotal = luggageIds.Length;
        LuggageCurrent = LuggageTotal;
        foreach (var luggageId in luggageIds)
        {
            Luggages.Add(new Luggage(this, luggageId));
        }
    }

    public static int[] CreateLuggage(AirportSim simulation)
    {
        int count = simulation.LuggageCountGen.GenerateNext();
        var luggageIds = new int[count];
        for (int i = 0; i < count; i++)
        {
            luggageIds[i] = simulation.GetNextLuggageID();
        }

        return luggageIds;
    }
}
