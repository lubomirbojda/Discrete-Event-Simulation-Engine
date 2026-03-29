using DS_SEM_2.EventSimulation;
using DS_SEM_2.EventSimulation.TestEvenSim;

namespace GUI;

public partial class SimMainForm : Form, ISimGuiDelegate
{
    private Button btnStart;
    private Button btnPauseResume;
    private Button btnSpeedUp;
    private Button btnSpeedDown;
    private Label lblSpeedValue;
    private CheckBox cbTurbo;
    private Label lblSimTime;
    private FlowLayoutPanel topPanel;

    private readonly double[] _speedSteps = { 0.05, 0.1, 0.25, 0.5, 0.75, 1, 2.5, 5, 10, 25, 50, 100, 1000, 5000, 10000, 100_000, 1_000_000, double.MaxValue};
    private int _currentSpeedIndex = 5;

    private TestSimulation _simulation = new(100, 100000);

    public SimMainForm()
    {
        InitializeComponent();
        InitCustomComponents();
    }

    private void InitCustomComponents()
    {
        topPanel = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 50, AutoSize = true };

        btnStart = new Button { Text = "Spusti", AutoSize = true };
        btnStart.Click += (s, e) => StartSimulation();

        btnPauseResume = new Button { Text = "Pozastav", Enabled = false, AutoSize = true };
        btnPauseResume.Click += (s, e) => TogglePause();

        btnSpeedDown = new Button { Text = "-", Width = 30 };
        btnSpeedDown.Click += (s, e) => ChangeSpeed(-1);

        lblSpeedValue = new Label { Text = "1", AutoSize = true, Margin = new Padding(5, 7, 5, 0) };

        btnSpeedUp = new Button { Text = "+", Width = 30 };
        btnSpeedUp.Click += (s, e) => ChangeSpeed(1);

        cbTurbo = new CheckBox { Text = "Turbo", Margin = new Padding(10, 5, 0, 0) };

        lblSimTime = new Label { Text = "Simulačný čas: 0.00", AutoSize = true, Margin = new Padding(15, 7, 0, 0) };

        topPanel.Controls.Add(btnStart);
        topPanel.Controls.Add(btnPauseResume);
        topPanel.Controls.Add(new Label { Text = "Rýchlosť:", Margin = new Padding(10, 7, 0, 0) });
        topPanel.Controls.Add(btnSpeedDown);
        topPanel.Controls.Add(lblSpeedValue);
        topPanel.Controls.Add(btnSpeedUp);
        topPanel.Controls.Add(cbTurbo);
        topPanel.Controls.Add(lblSimTime);

        this.Controls.Add(topPanel);
    }

    private void ChangeSpeed(int delta)
    {
        int newIndex = _currentSpeedIndex + delta;
        if (newIndex >= 0 && newIndex < _speedSteps.Length)
        {
            _currentSpeedIndex = newIndex;
            if (_currentSpeedIndex == _speedSteps.Length - 1)
                lblSpeedValue.Text = "inf";
            else
                lblSpeedValue.Text = _speedSteps[_currentSpeedIndex].ToString();

            _simulation.SetSpeed(_speedSteps[_currentSpeedIndex]);
        }
    }

    private void StartSimulation()
    {
        btnPauseResume.Enabled = true;
        _simulation.SetSpeed(_speedSteps[_currentSpeedIndex]);
        _simulation.DisableTurboMode();
        _simulation.RegisterGUIDelegate(this);

        Task.Run(() => _simulation.RunSimulation(1000));
    }

    private void TogglePause()
    {
        if (btnPauseResume.Text == "Pozastav")
        {
            btnPauseResume.Text = "Pokračuj";
        }
        else
        {
            btnPauseResume.Text = "Pozastav";
        }
    }

    public void RefreshGUI()
    {
        if (InvokeRequired)
        {
            Invoke(new Action(RefreshGUI));
            return;
        }

        lblSimTime.Text = $"Simulačný čas: {_simulation.SimTime:F2}" + " replikacia: " + _simulation.CurrentReplication 
            + " served in repl: " + _simulation.ServedCustomers;
    }
}