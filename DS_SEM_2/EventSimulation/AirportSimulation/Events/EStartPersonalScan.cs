namespace DS_SEM_2.EventSimulation.AirportSimulation.Events;

public class EStartPersonalScan : PassengerEvent
{
    public EStartPersonalScan(double executeTime, AirportSim simulation, Passenger passenger) : base(executeTime, simulation, passenger)
    {
    }

    public override void Execute()
    {
        Passenger_.PassengerFlow.PassengerPersonalScan = Passenger_;
        var scanTime = Simulation.PersonalScanGen.GenerateNext();
        EEndPersonalScan endPersonalScan = new(ExecuteTime + scanTime, Simulation, Passenger_);
        Simulation.PlanEvent(endPersonalScan);
    }
}
