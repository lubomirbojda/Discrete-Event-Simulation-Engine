namespace DS_SEM_2.EventSimulation.AirportSimulation.Events;

public class EStartLuggageScan : LuggageEvent
{
    public EStartLuggageScan(double executeTime, AirportSim simulation, Luggage luggage) : base(executeTime, simulation, luggage)
    {
    }

    public override void Execute()
    {
        var flow = Luggage_.Passenger.PassengerFlow;
        //Set scanner availability to false and plan end of luggage scan
        flow.IsScannerBusy = true;
        var scanTime = Simulation.LuggageScanGen.GenerateNext();
        EEndLuggageScan endLugScan = new(ExecuteTime + scanTime, Simulation, Luggage_);
        Simulation.PlanEvent(endLugScan);

        if (flow.PassengersLuggageQueue.Count > 0
            && flow.LuggagesAfterQueue.Count < Simulation.MaxLuggageAfter)
        {
            var passenger = flow.PassengersLuggageQueue.Peek();
            EStartLuggageDelivery startLugDel = new(ExecuteTime, Simulation, passenger);
            Simulation.PlanEvent(startLugDel);
        }
    }
}
