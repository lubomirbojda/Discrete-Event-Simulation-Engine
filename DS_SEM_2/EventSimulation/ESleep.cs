namespace DS_SEM_2.EventSimulation;

public class ESleep : BaseEvent<BaseEventSimulation>
{
    private const double _simTimeStep = 0.363;

    public ESleep(double executeTime, BaseEventSimulation simulation) : base(executeTime, simulation)
    {
    }

    public override void Execute()
    {
        if (Simulation.IsTurboActive)
            return;

        ExecuteTime += _simTimeStep * Simulation.Speed;
        Simulation.PlanEvent(this);
        var sleepTime = (int)(_simTimeStep * 1000 / Simulation.Speed);
        Simulation.RefreshGUI();
        if (sleepTime > 1)
            Thread.Sleep(sleepTime);
    }
}
