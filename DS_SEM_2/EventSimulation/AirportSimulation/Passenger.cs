namespace DS_SEM_2.EventSimulation.AirportSimulation;

public class Passenger
{
    public int Id { get; private set; }
    public double ArrivalTime { get; set; }
    public List<Luggage> Luggages { get; private set; } = new();
    public PassengersFlow PassengerFlow { get; set; } = new();

    public int LuggageTotal { get; private set; }

    public Passenger(int id, int[] luggageIds)
    {
        Id = id;
        LuggageTotal = luggageIds.Length;
        foreach (var luggageId in luggageIds)
        {
            Luggages.Add(new Luggage(this, luggageId));
        }
    }

    public static int[] CreateLuggage(AirportSim simulation)
    {
        int count;
        var r = simulation.LuggageCountRand.NextDouble();
        if (r < 0.15)
            count = 0;
        else if (r < 0.83)
            count = 1;
        else
            count = 2;

        var luggageIds = new int[count];
        for (int i = 0; i < count; i++)
        {
            luggageIds[i] = simulation.GetNextLuggageID();
        }

        return luggageIds;
    }
}
