namespace DS_SEM_2.EventSimulation.AirportSimulation.Events;

public class EWarmUp : BaseEvent<AirportSim>
{
    private double _timeInSystem;
    public EWarmUp(double executeTime, AirportSim simulation, double timeInSystem) : base(executeTime, simulation)
    {
        _timeInSystem = timeInSystem;
    }

    public override void Execute()
    {
        if (!Simulation.IsWarmedUp)
        {
            Simulation.WarmUpSamples.Enqueue(_timeInSystem);
            Simulation.PassToWarmUp++;
            if (Simulation.WarmUpSamples.Count > Simulation.WarmUpSamplesCount)
            {
                Simulation.WarmUpSamples.Dequeue();
                WarmUpCheck();
            }
        }
    }

    private void WarmUpCheck()
    {
        var avg = Simulation.WarmUpSamples.Average();
        var tolerance = Simulation.WarpUpTolerance * avg;

        if (Simulation.LastWarmUpAverage != 0 && Math.Abs(avg - Simulation.LastWarmUpAverage) < tolerance)
            Simulation.WarpUpSuccessStabilityCounter++;
        else
            Simulation.WarpUpSuccessStabilityCounter = 0;


        if (Simulation.WarpUpSuccessStabilityCounter >= Simulation.WarpUpStabilityCount)
        {
            Simulation.IsWarmedUp = true;
            Simulation.ResetStatistics(ExecuteTime);
        }

        Simulation.LastWarmUpAverage = avg;
    }
}
