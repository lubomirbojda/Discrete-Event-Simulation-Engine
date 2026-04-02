namespace DS_SEM_2.EventSimulation.AirportSimulation.Events;

public class EPassengerLeave : PassengerEvent
{
    public EPassengerLeave(double executeTime, AirportSim simulation, Passenger passenger) : base(executeTime, simulation, passenger)
    {
    }

    public override void Execute()
    {
        if (Passenger_.AlreadyLeft)
            return;

        var flow = Passenger_.PassengerFlow;
        if (Passenger_.IsReadyToLeave && Passenger_.Luggages.Count > 0)
        {
            var luggageAfter = flow.LuggagesAfterQueue;
            var luggageBefore = flow.LuggagesBeforeQueue;
            while (luggageAfter.Count > 0 && luggageAfter.Peek().Passenger == Passenger_)
            {
                luggageAfter.Dequeue();
                Passenger_.LuggagePickedFromScanner++;
                if (luggageAfter.Count < Simulation.MaxLuggageAfter
                    && !flow.IsScannerBusy
                    && luggageBefore.Count > 0)
                {
                    flow.IsScannerBusy = true;
                    var nextLuggForScan = luggageBefore.Dequeue();
                    EStartLuggageScan startLugScan = new(ExecuteTime, Simulation, nextLuggForScan);
                    Simulation.PlanEvent(startLugScan);
                }
            }
        }

        if (Passenger_.LuggagePickedFromScanner == Passenger_.Luggages.Count)
        {
            if (Passenger_.Luggages.Count > 0)
            {
                if (flow.PassengersWaitingLuggageQueue.Count > 0
                    && flow.PassengersWaitingLuggageQueue.Peek() == Passenger_)
                {
                    flow.PassengersWaitingLuggageQueue.Dequeue();
                    Simulation.AverageLuggageWaitingQueueLength.Add(Passenger_.PassengerFlow.PassengersWaitingLuggageQueue.Count, ExecuteTime);
                }

                if (flow.PassengersWaitingLuggageQueue.Count > 0)
                {
                    var nextPass = flow.PassengersWaitingLuggageQueue.Peek();
                    nextPass.IsReadyToLeave = true;
                    EPassengerLeave passLeave = new(ExecuteTime, Simulation, nextPass);
                    Simulation.PlanEvent(passLeave);
                }
            }

            if (!Passenger_.AlreadyLeft)
            {
                Simulation.AverageTimeInSystem.Add(ExecuteTime - Passenger_.ArrivalTime);
                Simulation.ServedPassengers++;
                Passenger_.AlreadyLeft = true;
            }
        }
    }
}
