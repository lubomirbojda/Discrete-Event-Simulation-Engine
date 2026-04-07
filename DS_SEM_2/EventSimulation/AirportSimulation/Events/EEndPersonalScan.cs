namespace DS_SEM_2.EventSimulation.AirportSimulation.Events;

public class EEndPersonalScan : PassengerEvent
{
    public EEndPersonalScan(double executeTime, AirportSim simulation, Passenger passenger) : base(executeTime, simulation, passenger)
    {
    }

    public override void Execute()
    {
        var flow = Passenger_.PassengerFlow;
        flow.IsRTGAvail = true;
        Passenger_.PassengerFlow.PassengerRTG = null;
        Passenger_.PassengerFlow.PassengerPersonalScan = null;
        if (flow.PassengersRTGQueue.Count > 0)
        {
            var nextPass = flow.PassengersRTGQueue.Dequeue();
            EStartRTG startRTG = new(ExecuteTime, Simulation, nextPass);
            Simulation.PlanEvent(startRTG);
        }

        if (Passenger_.Luggages.Count == 0)
        {
            EPassengerLeave passLeave = new(ExecuteTime, Simulation, Passenger_);
            Simulation.PlanEvent(passLeave);
        }
        else
        {
            flow.PassengersWaitingLuggageQueue.Enqueue(Passenger_);
            var luggageWaitQueueCountSum = Simulation.PassengersFlow1.PassengersWaitingLuggageQueue.Count
                    + Simulation.PassengersFlow2.PassengersWaitingLuggageQueue.Count;
            Simulation.AverageLuggageWaitingQueueLength.Add(luggageWaitQueueCountSum, ExecuteTime);
            if (flow.PassengersWaitingLuggageQueue.Peek() == Passenger_)
            {
                Passenger_.IsReadyToLeave = true;
                EPassengerLeave passLeave = new(ExecuteTime, Simulation, Passenger_);
                Simulation.PlanEvent(passLeave);
            }
        }
    }
}
