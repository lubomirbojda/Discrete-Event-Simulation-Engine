namespace DS_SEM_2.MonteCarlo;

public abstract class BaseMonteCarlo
{
    public Random _seedGenerator {  get; private set; }
    private bool _isRunning = true;
    public int CurrentReplication { get; protected set; }
    protected int _replicationsCount;

    protected BaseMonteCarlo(int seed)
    {
        _seedGenerator = new(seed);
    }

    public void StopSimulation()
    {
        _isRunning = false;
    }

    protected abstract void BeforeSimulation();
    protected abstract void BeforeReplication();
    protected abstract void DoReplication();
    protected abstract void AfterReplication();
    protected abstract void AfterSimulation();

    public void RunSimulation(int replicationsCount)
    {
        _replicationsCount = replicationsCount;
        BeforeSimulation();

        CurrentReplication = 0;
        while (CurrentReplication < replicationsCount && _isRunning)
        {
            BeforeReplication();

            DoReplication();

            CurrentReplication++;
            AfterReplication();
        }

        AfterSimulation();
    }

}
