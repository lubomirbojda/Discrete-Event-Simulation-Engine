namespace DS_SEM_2.EventSimulation.AirportSimulation;

public class EStartLuggageScan : LuggageEvent
{
    public EStartLuggageScan(double executeTime, AirportSim simulation, Luggage luggage) : base(executeTime, simulation, luggage)
    {
    }

    public override void Execute()
    {
        Luggage_.Passenger.PassengerFlow.IsLuggageScanAvail = false;

    }
}
