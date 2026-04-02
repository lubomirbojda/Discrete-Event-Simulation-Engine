namespace DS_SEM_2.EventSimulation.AirportSimulation.Events;

public class EEndLuggageScan : LuggageEvent
{
    public EEndLuggageScan(double executeTime, AirportSim simulation, Luggage luggage) : base(executeTime, simulation, luggage)
    {
    }

    public override void Execute()
    {
        Luggage_.Passenger.PassengerFlow.LuggagesAfterQueue.Enqueue(Luggage_);
        Luggage_.Passenger.PassengerFlow.IsLuggageScanAvail = true;

        //Plan passenger leave if all luggage is scanned - else set ready to leave on true and put to leave queue
        //not implemented yet
        if (Luggage_.Passenger.IsReadyToLeave)
        {
            EPassengerLeave passLeave = new(ExecuteTime, Simulation, Luggage_.Passenger);
            Simulation.PlanEvent(passLeave);
        }
        
        //Plan start luggage scan for next luggage if there is any
        if (Luggage_.Passenger.PassengerFlow.LuggagesBeforeQueue.Count > 0
            && Luggage_.Passenger.PassengerFlow.LuggagesAfterQueue.Count < Simulation.MaxLuggageAfter)
        {
            var luggage = Luggage_.Passenger.PassengerFlow.LuggagesBeforeQueue.Dequeue();
            EStartLuggageScan startLugScan = new(ExecuteTime, Simulation, luggage);
            Simulation.PlanEvent(startLugScan);
        }
    }
}
