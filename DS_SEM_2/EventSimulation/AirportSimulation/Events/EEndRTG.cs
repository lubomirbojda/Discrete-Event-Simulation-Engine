namespace DS_SEM_2.EventSimulation.AirportSimulation.Events;

public class EEndRTG : PassengerEvent
{
    public EEndRTG(double executeTime, AirportSim simulation, Passenger passenger) : base(executeTime, simulation, passenger)
    {
    }

    public override void Execute()
    {
        var r = Simulation.PersonalScanRand.NextDouble();
        if (r < 0.19)
        {
            EStartPersonalScan startPersonalScan = new(ExecuteTime, Simulation, Passenger_);
            Simulation.PlanEvent(startPersonalScan);
        }
        else
        {
            Passenger_.PassengerFlow.IsRTGAvail = true;

            if (Passenger_.LuggageTotal == 0)
            {
                EPassengerLeave passLeave = new(ExecuteTime, Simulation, Passenger_);
                Simulation.PlanEvent(passLeave);
            }
            else
            {
                Passenger_.PassengerFlow.PassengersWaitingLuggageQueue.Enqueue(Passenger_);
                if (Passenger_.PassengerFlow.PassengersWaitingLuggageQueue.Peek() == Passenger_)
                {
                    Passenger_.IsReadyToLeave = true;
                    EPassengerLeave passLeave = new(ExecuteTime, Simulation, Passenger_);
                    Simulation.PlanEvent(passLeave);
                }
            }
            
            if (Passenger_.PassengerFlow.PassengersRTGQueue.Count > 0)
            {
                var nextPass = Passenger_.PassengerFlow.PassengersRTGQueue.Dequeue();
                EStartRTG startRTG = new(ExecuteTime, Simulation, nextPass);
                Simulation.PlanEvent(startRTG);
            }
        }
    }
}
