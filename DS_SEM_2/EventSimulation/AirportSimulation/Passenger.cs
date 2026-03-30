namespace DS_SEM_2.EventSimulation.AirportSimulation;

public class Passenger
{
    public int Id { get; private set; }
    public double ArrivalTime { get; set; }
    public Luggage Luggage { get; private set; }
    public PassengersFlow PassengerFlow { get; set; } = new();

    public Passenger(int id, int[] luggageIds)
    {
        Id = id;
        Luggage = new Luggage(this);
        foreach (var luggageId in luggageIds)
        {
            Luggage.AddLuggage(luggageId);
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
