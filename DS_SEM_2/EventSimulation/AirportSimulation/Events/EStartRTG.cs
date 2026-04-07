namespace DS_SEM_2.EventSimulation.AirportSimulation.Events;

public class EStartRTG : PassengerEvent
{
    public EStartRTG(double executeTime, AirportSim simulation, Passenger passenger) : base(executeTime, simulation, passenger)
    {
    }

    public override void Execute()
    {
        Passenger_.PassengerFlow.IsRTGAvail = false;
        Passenger_.PassengerFlow.PassengerRTG = Passenger_;
        var scanTime = Simulation.RTGScanGen.GenerateNext();
        EEndRTG endRTG = new(ExecuteTime + scanTime, Simulation, Passenger_);
        Simulation.PlanEvent(endRTG);
    }
}
