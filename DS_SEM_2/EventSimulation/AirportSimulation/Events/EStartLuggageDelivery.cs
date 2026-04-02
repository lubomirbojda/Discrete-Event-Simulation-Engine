namespace DS_SEM_2.EventSimulation.AirportSimulation.Events;

public class EStartLuggageDelivery : PassengerEvent
{
    public EStartLuggageDelivery(double executeTime, AirportSim simulation, Passenger passenger) : base(executeTime, simulation, passenger)
    {
    }

    public override void Execute()
    {
        //Put as many luggages as can
        for (int i = 0; i < Passenger_.Luggages.Count; i++)
        {
            if (Passenger_.PassengerFlow.LuggagesBeforeQueue.Count < Simulation.MaxLuggageBefore
                && !Passenger_.Luggages[i].IsDelivered)
            {
                Passenger_.PassengerFlow.LuggagesBeforeQueue.Enqueue(Passenger_.Luggages[i]);
                Passenger_.Luggages[i].IsDelivered = true;
                Passenger_.LuggageDelivered++;
            }
        }

        EEndLuggageDelivery endLugDel = new(ExecuteTime, Simulation, Passenger_);
        Simulation.PlanEvent(endLugDel);
    }
}
