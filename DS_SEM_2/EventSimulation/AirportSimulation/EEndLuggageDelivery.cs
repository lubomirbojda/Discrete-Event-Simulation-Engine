namespace DS_SEM_2.EventSimulation.AirportSimulation;

public class EEndLuggageDelivery : PassengerEvent
{
    public EEndLuggageDelivery(double executeTime, AirportSim simulation, Passenger passenger) : base(executeTime, simulation, passenger)
    {
    }

    public override void Execute()
    {
        throw new NotImplementedException();
    }
}
