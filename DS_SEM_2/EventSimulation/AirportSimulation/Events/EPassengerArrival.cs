namespace DS_SEM_2.EventSimulation.AirportSimulation.Events;

public class EPassengerArrival : PassengerEvent
{
    public EPassengerArrival(double executeTime, AirportSim simulation, Passenger passenger) : base(executeTime, simulation, passenger)
    {
    }

    public override void Execute()
    {
        //Set passenger arrival time
        Passenger_.ArrivalTime = Simulation.SimTime;
        Simulation.CamePass++;
        if (Simulation.CamePass == 5000)
        {
            Console.WriteLine();
        }

        //Choose passenger flow
        var luggageQueueCount1 = Simulation.PassengersFlow1.PassengersLuggageQueue.Count;
        var luggageQueueCount2 = Simulation.PassengersFlow2.PassengersLuggageQueue.Count;
        if (luggageQueueCount1 < luggageQueueCount2)
        {
            Passenger_.PassengerFlow = Simulation.PassengersFlow1;
        }
        else if (luggageQueueCount1 > luggageQueueCount2)
        {
            Passenger_.PassengerFlow = Simulation.PassengersFlow2;
        }
        else
        {
            var r = Simulation.FlowChooseRand.NextDouble();
            if (r < 0.5)
                Passenger_.PassengerFlow = Simulation.PassengersFlow1;
            else
                Passenger_.PassengerFlow = Simulation.PassengersFlow2;
        }
        Passenger_.PassengerFlow.PassengersLuggageQueue.Enqueue(Passenger_);

        //Can plan luggage delivery
        if (Passenger_.PassengerFlow.PassengersLuggageQueue.Peek() == Passenger_)
        {
            EStartLuggageDelivery startLugDel = new(ExecuteTime, Simulation, Passenger_);
            Simulation.PlanEvent(startLugDel);
        }

        //Plan next passenger arrival
        var u = Simulation.PassengerArrivalGen.GenerateNext();
        var nextArrivalTime = Simulation.SimTime + u;
        Passenger nextPass = new(Simulation.GetNextPassengerID(), Passenger.CreateLuggage(Simulation));
        EPassengerArrival nextArrival = new(nextArrivalTime, Simulation, nextPass);
        Simulation.PlanEvent(nextArrival);
    }
}
