namespace DS_SEM_2.EventSimulation.AirportSimulation.Events;

public abstract class PassengerEvent : BaseEvent<AirportSim>
{
    public Passenger Passenger_ { get; private set; }

    public PassengerEvent(double executeTime, AirportSim simulation, Passenger passenger) : base(executeTime, simulation)
    {
        Passenger_ = passenger;
    }
}
