namespace DS_SEM_2.EventSimulation.AirportSimulation.Events;

public class EEndLuggageScan : LuggageEvent
{
    public EEndLuggageScan(double executeTime, AirportSim simulation, Luggage luggage) : base(executeTime, simulation, luggage)
    {
    }

    public override void Execute()
    {
        var flow = Luggage_.Passenger.PassengerFlow;
        flow.LuggagesAfterQueue.Enqueue(Luggage_);
        flow.IsScannerBusy = false;
        flow.LuggageScan = null;

        //Plan passenger leave if all luggage is scanned - else set ready to leave on true and put to leave queue
        if (Luggage_.Passenger.IsReadyToLeave)
        {
            EPassengerLeave passLeave = new(ExecuteTime, Simulation, Luggage_.Passenger);
            Simulation.PlanEvent(passLeave);
        }


        if (flow.LuggagesAfterQueue.Count < Simulation.MaxLuggageAfter)
        {
            if (flow.LuggagesBeforeQueue.Count > 0)
            {
                flow.IsScannerBusy = true;
                var luggage = flow.LuggagesBeforeQueue.Dequeue();
                EStartLuggageScan startLugScan = new(ExecuteTime, Simulation, luggage);
                Simulation.PlanEvent(startLugScan);
            }
        }   
    }
}
