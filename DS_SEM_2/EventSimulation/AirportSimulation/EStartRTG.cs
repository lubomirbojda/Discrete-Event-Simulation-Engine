namespace DS_SEM_2.EventSimulation.AirportSimulation;

public class EStartRTG : PassengerEvent
{
    public EStartRTG(double executeTime, AirportSim simulation, Passenger passenger) : base(executeTime, simulation, passenger)
    {
    }

    public override void Execute()
    {
        
    }
}
