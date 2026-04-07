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
        flow.LuggageScan = Luggage_;
        var scanTime = Simulation.LuggageScanGen.GenerateNext();
        EEndLuggageScan endLugScan = new(ExecuteTime + scanTime, Simulation, Luggage_);
        Simulation.PlanEvent(endLugScan);

        if (flow.PassengersLuggageQueue.Count > 0
            && flow.LuggagesBeforeQueue.Count < Simulation.MaxLuggageBefore)
        {
            var passenger = flow.PassengersLuggageQueue.Peek();
            EStartLuggageDelivery startLugDel = new(ExecuteTime, Simulation, passenger);
            Simulation.PlanEvent(startLugDel);
        }
    }
}
