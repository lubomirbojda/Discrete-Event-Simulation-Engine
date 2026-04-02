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
            Simulation.AverageLuggageWaitingQueueLength.Add(flow.PassengersWaitingLuggageQueue.Count, ExecuteTime);
            if (flow.PassengersWaitingLuggageQueue.Peek() == Passenger_)
            {
                Passenger_.IsReadyToLeave = true;
                EPassengerLeave passLeave = new(ExecuteTime, Simulation, Passenger_);
                Simulation.PlanEvent(passLeave);
            }
        }
    }
}
