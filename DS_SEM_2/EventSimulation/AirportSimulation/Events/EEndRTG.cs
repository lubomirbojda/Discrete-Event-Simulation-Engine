namespace DS_SEM_2.EventSimulation.AirportSimulation.Events;

public class EEndRTG : PassengerEvent
{
    public EEndRTG(double executeTime, AirportSim simulation, Passenger passenger) : base(executeTime, simulation, passenger)
    {
    }

    public override void Execute()
    {
        var flow = Passenger_.PassengerFlow;
        var r = Simulation.PersonalScanRand.NextDouble();
        if (r < 0.19)
        {
            EStartPersonalScan startPersonalScan = new(ExecuteTime, Simulation, Passenger_);
            Simulation.PlanEvent(startPersonalScan);
        }
        else
        {
            flow.IsRTGAvail = true;
            Passenger_.PassengerFlow.PassengerRTG = null;
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
}
