namespace DS_SEM_2.EventSimulation.AirportSimulation.Events;

public class EEndLuggageDelivery : PassengerEvent
{
    public EEndLuggageDelivery(double executeTime, AirportSim simulation, Passenger passenger) : base(executeTime, simulation, passenger)
    {
    }

    public override void Execute()
    {
        //If gave all luggage - dequeu, put to RTGQueue, possibly plan RTGStart event
        if (Passenger_.LuggageCurrent == 0)
        {
            Passenger_.PassengerFlow.PassengersLuggageQueue.Dequeue();
            Passenger_.PassengerFlow.PassengersRTGQueue.Enqueue(Passenger_);
            if (Passenger_.PassengerFlow.PassengersRTGQueue.Peek() == Passenger_)
            {
                EStartRTG startRTG = new(ExecuteTime, Simulation, Passenger_);
                Simulation.PlanEvent(startRTG);
            }

            //Plan start luggage delivery for next passenger
            if (Passenger_.PassengerFlow.PassengersLuggageQueue.Count > 0)
            {
                var nextPass = Passenger_.PassengerFlow.PassengersLuggageQueue.Dequeue();
                EStartLuggageDelivery startLugDel = new(ExecuteTime, Simulation, nextPass);
                Simulation.PlanEvent(startLugDel);
            }
        }

        //If no luggage is scanning now - plan scanning from queue
        if (Passenger_.PassengerFlow.LuggagesBefore.Count > 0 
            && Passenger_.PassengerFlow.LuggagesAfter.Count < Simulation.MaxLuggageAfter
            && Passenger_.PassengerFlow.IsLuggageScanAvail)
        {
            var luggage = Passenger_.PassengerFlow.LuggagesBefore.Dequeue();
            EStartLuggageScan startLugScan = new(ExecuteTime, Simulation, luggage);
            Simulation.PlanEvent(startLugScan);
        }

        
    }
}
