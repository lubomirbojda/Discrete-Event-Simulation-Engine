namespace DS_SEM_2.EventSimulation.AirportSimulation;

public class EStartLuggageDelivery : PassengerEvent
{
    public EStartLuggageDelivery(double executeTime, AirportSim simulation, Passenger passenger) : base(executeTime, simulation, passenger)
    {
    }

    public override void Execute()
    {
        for (int i = Passenger_.Luggage.IDs.Count; i >= 0; i--)
        {
            if (Passenger_.PassengerFlow.LuggagesBefore.Count )
            {
                
            }
        }
    }
}
