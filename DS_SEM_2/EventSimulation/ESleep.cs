namespace DS_SEM_2.EventSimulation;

public class ESleep : BaseEvent<BaseEventSimulation>
{
    private double _simTimeStep = 0.1;

    public ESleep(double executeTime, BaseEventSimulation simulation) : base(executeTime, simulation)
    {
    }

    public override void Execute()
    {
        
    }
}
