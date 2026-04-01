namespace DS_SEM_2.EventSimulation.AirportSimulation;

public class EStartLuggageDelivery : PassengerEvent
{
    public EStartLuggageDelivery(double executeTime, AirportSim simulation, Passenger passenger) : base(executeTime, simulation, passenger)
    {
    }

    public override void Execute()
    {
        //Put as many luggages as can
        for (int i = Passenger_.Luggages.Count; i >= 0; i--)
        {
            if (Passenger_.PassengerFlow.LuggagesBefore.Count < Simulation.MaxLuggageBefore)
            {
                Passenger_.PassengerFlow.LuggagesBefore.Enqueue(Passenger_.Luggages[i]);
                Passenger_.Luggages.RemoveAt(i);
            }
        }

        EEndLuggageDelivery endLugDel = new(ExecuteTime, Simulation, Passenger_);
        Simulation.PlanEvent(endLugDel);
    }
}
