namespace DS_SEM_2.EventSimulation.AirportSimulation.Events;

public class EEndLuggageDelivery : PassengerEvent
{
    public EEndLuggageDelivery(double executeTime, AirportSim simulation, Passenger passenger) : base(executeTime, simulation, passenger)
    {
    }

    public override void Execute()
    {
        var flow = Passenger_.PassengerFlow;
        //Delivered all luggages
        if (Passenger_.LuggageDelivered == Passenger_.Luggages.Count)
        {
            flow.PassengersLuggageQueue.Dequeue();
            var luggageQueueCountSum = Simulation.PassengersFlow1.PassengersLuggageQueue.Count + Simulation.PassengersFlow2.PassengersLuggageQueue.Count;
            Simulation.AverageLuggageQueueLength.Add(luggageQueueCountSum, ExecuteTime);
            flow.PassengersRTGQueue.Enqueue(Passenger_);
            if (flow.PassengersRTGQueue.Peek() == Passenger_
                && flow.IsRTGAvail)
            {
                flow.IsRTGAvail = false;
                var passenger = flow.PassengersRTGQueue.Dequeue();
                EStartRTG startRTG = new(ExecuteTime, Simulation, passenger);
                Simulation.PlanEvent(startRTG);
            }

            if (flow.PassengersLuggageQueue.Count > 0)
            {
                var nextPass = flow.PassengersLuggageQueue.Peek();
                EStartLuggageDelivery startLugDel = new(ExecuteTime, Simulation, nextPass);
                Simulation.PlanEvent(startLugDel);
            }
        }

        //Plan luggage scan if possible
        if (flow.LuggagesBeforeQueue.Count > 0
            && flow.LuggagesAfterQueue.Count < Simulation.MaxLuggageAfter
            && !flow.IsScannerBusy)
        {
            flow.IsScannerBusy = true;
            var luggage = flow.LuggagesBeforeQueue.Dequeue();
            EStartLuggageScan startLugScan = new(ExecuteTime, Simulation, luggage);
            Simulation.PlanEvent(startLugScan);
        }
    }
}
