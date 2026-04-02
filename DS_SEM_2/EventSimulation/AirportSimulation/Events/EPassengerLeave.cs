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

        if (Passenger_.IsReadyToLeave && Passenger_.LuggageTotal > 0)
        {
            var luggageAfter = Passenger_.PassengerFlow.LuggagesAfterQueue;
            var luggageBefore = Passenger_.PassengerFlow.LuggagesBeforeQueue;
            while (luggageAfter.Count > 0 && luggageAfter.Peek().Passenger == Passenger_)
            {
                if (luggageAfter.Count == Simulation.MaxLuggageAfter
                    && luggageBefore.Count > 0)
                {
                    var nextLuggForScan = luggageBefore.Dequeue();
                    EStartLuggageScan startLugScan = new(ExecuteTime, Simulation, nextLuggForScan);
                    Simulation.PlanEvent(startLugScan);
                }
                luggageAfter.Dequeue();
                Passenger_.LuggageCurrent++;
            }
        }

        if (Passenger_.LuggageTotal == Passenger_.LuggageCurrent)
        {
            //Passenger leaves system
            if (Passenger_.LuggageTotal != 0)
            {
                Passenger_.PassengerFlow.PassengersWaitingLuggageQueue.Dequeue();
                if (Passenger_.PassengerFlow.PassengersWaitingLuggageQueue.Count > 0)
                {
                    var nextPass = Passenger_.PassengerFlow.PassengersWaitingLuggageQueue.Peek();
                    nextPass.IsReadyToLeave = true;
                    EPassengerLeave passLeave = new(ExecuteTime, Simulation, nextPass);
                    Simulation.PlanEvent(passLeave);
                }
            }
            Simulation.AverageTimeInSystem.Add(ExecuteTime - Passenger_.ArrivalTime);
            Simulation.ServedPassengers++;
            Passenger_.AlreadyLeft = true;
        }
    }
}
