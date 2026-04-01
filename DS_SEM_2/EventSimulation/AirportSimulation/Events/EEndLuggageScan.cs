namespace DS_SEM_2.EventSimulation.AirportSimulation.Events;

public class EEndLuggageScan : LuggageEvent
{
    public EEndLuggageScan(double executeTime, AirportSim simulation, Luggage luggage) : base(executeTime, simulation, luggage)
    {
    }

    public override void Execute()
    {
        Luggage_.Passenger.PassengerFlow.LuggagesAfter.Enqueue(Luggage_);
        Luggage_.Passenger.PassengerFlow.IsLuggageScanAvail = true;

        //Plan passenger leave if all luggage is scanned - else set ready to leave on true and put to leave queue
        //not implemented yet

        //Plan start luggage scan for next luggage if there is any
        if (Luggage_.Passenger.PassengerFlow.LuggagesBefore.Count > 0
            && Luggage_.Passenger.PassengerFlow.LuggagesAfter.Count < Simulation.MaxLuggageAfter)
        {
            var luggage = Luggage_.Passenger.PassengerFlow.LuggagesBefore.Dequeue();
            EStartLuggageScan startLugScan = new(ExecuteTime, Simulation, luggage);
            Simulation.PlanEvent(startLugScan);
        }
    }
}
