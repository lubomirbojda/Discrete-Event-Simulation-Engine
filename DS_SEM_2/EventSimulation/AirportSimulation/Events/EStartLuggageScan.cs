namespace DS_SEM_2.EventSimulation.AirportSimulation.Events;

public class EStartLuggageScan : LuggageEvent
{
    public EStartLuggageScan(double executeTime, AirportSim simulation, Luggage luggage) : base(executeTime, simulation, luggage)
    {
    }

    public override void Execute()
    {
        //Set scanner availability to false and plan end of luggage scan
        Luggage_.Passenger.PassengerFlow.IsLuggageScanAvail = false;
        var scanTime = Simulation.LuggageScanGen.GenerateNext();
        EEndLuggageScan endLugScan = new(ExecuteTime + scanTime, Simulation, Luggage_);
        Simulation.PlanEvent(endLugScan);

        //Plan start luggage delivery if there is passenger with luggage in the queue and luggage queue before is full
        if (Luggage_.Passenger.PassengerFlow.PassengersLuggageQueue.Count > 0
            && Luggage_.Passenger.PassengerFlow.LuggagesBefore.Count == Simulation.MaxLuggageBefore)
        {
            var passenger = Luggage_.Passenger.PassengerFlow.PassengersLuggageQueue.Peek();
            EStartLuggageDelivery startLugDel = new(ExecuteTime, Simulation, passenger);
            Simulation.PlanEvent(startLugDel);
        }
    }
}
