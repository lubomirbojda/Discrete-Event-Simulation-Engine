using DS_SEM_2.EventSimulation;
using DS_SEM_2.EventSimulation.AirportSimulation;
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

    private DataGridView dgvQueuePassLuggage_1;
    private DataGridView dgvQueueRTG_1;
    private DataGridView dgvLuggageBefore_1;
    private DataGridView dgvLuggageAfter_1;

    private DataGridView dgvQueuePassLuggage_2;
    private DataGridView dgvQueueRTG_2;
    private DataGridView dgvLuggageBefore_2;
    private DataGridView dgvLuggageAfter_2;

    private FlowLayoutPanel gridPanel;

    private readonly double[] _speedSteps = { 0.05, 0.1, 0.25, 0.5, 0.75, 1, 2.5, 5, 10, 25, 50, 100, 1000, 5000, 10000, 100_000, 1_000_000, double.MaxValue };
    private int _currentSpeedIndex = 5;

    private AirportSim _simulation = new(100);

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

        gridPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoScroll = true, WrapContents = true };

        dgvQueuePassLuggage_1 = CreateGrid("id cestujuceho", "id batozin");
        dgvQueueRTG_1 = CreateGrid("id cestujuceho", "id batozin");
        dgvLuggageBefore_1 = CreateGrid("id batoziny", "id osoby");
        dgvLuggageAfter_1 = CreateGrid("id batoziny", "id osoby");

        dgvQueuePassLuggage_2 = CreateGrid("id cestujuceho", "id batozin");
        dgvQueueRTG_2 = CreateGrid("id cestujuceho", "id batozin");
        dgvLuggageBefore_2 = CreateGrid("id batoziny", "id osoby");
        dgvLuggageAfter_2 = CreateGrid("id batoziny", "id osoby");

        gridPanel.Controls.Add(CreateGridContainer("Front(1) odovzdávanie batožiny", dgvQueuePassLuggage_1));
        gridPanel.Controls.Add(CreateGridContainer("Front(1) pred RTG", dgvQueueRTG_1));
        gridPanel.Controls.Add(CreateGridContainer("Front(1) batožín pred skenerom", dgvLuggageBefore_1));
        gridPanel.Controls.Add(CreateGridContainer("Front(1) batožín po skeneri", dgvLuggageAfter_1));

        gridPanel.Controls.Add(CreateGridContainer("Front(2) odovzdávanie batožiny", dgvQueuePassLuggage_2));
        gridPanel.Controls.Add(CreateGridContainer("Front(2) pred RTG", dgvQueueRTG_2));
        gridPanel.Controls.Add(CreateGridContainer("Front(2) batožín pred skenerom", dgvLuggageBefore_2));
        gridPanel.Controls.Add(CreateGridContainer("Front(2) batožín po skeneri", dgvLuggageAfter_2));

        this.Controls.Add(gridPanel);
        this.Controls.Add(topPanel);
    }

    private DataGridView CreateGrid(string col1, string col2)
    {
        var dgv = new DataGridView
        {
            ColumnCount = 2,
            ReadOnly = true,
            RowHeadersVisible = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AllowUserToResizeRows = false,
            AllowUserToOrderColumns = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            Height = 300,
            Width = 250
        };

        dgv.Columns[0].Name = col1;
        dgv.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;

        dgv.Columns[1].Name = col2;
        dgv.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;

        return dgv;
    }

    private Panel CreateGridContainer(string title, DataGridView dgv)
    {
        var panel = new FlowLayoutPanel { FlowDirection = FlowDirection.TopDown, AutoSize = true, Margin = new Padding(10) };
        panel.Controls.Add(new Label { Text = title, AutoSize = true, Font = new Font(this.Font, FontStyle.Bold) });
        panel.Controls.Add(dgv);
        return panel;
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
        btnPauseResume.Text = "Pozastav";
        _simulation = new(100);
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
            _simulation.PauseSimulation();
        }
        else
        {
            btnPauseResume.Text = "Pozastav";
            _simulation.ResumeSimulation();
        }
    }

    public void RefreshGUI()
    {
        if (InvokeRequired)
        {
            Invoke(new Action(RefreshGUI));
            return;
        }

        lblSimTime.Text = $"Simulačný čas: {_simulation.SimTime:F2}" + " replikacia: " + _simulation.CurrentReplication;
        //Queues luggage delivery
        dgvQueuePassLuggage_1.SuspendLayout();
        dgvQueuePassLuggage_1.Rows.Clear();
        dgvQueuePassLuggage_2.SuspendLayout();
        dgvQueuePassLuggage_2.Rows.Clear();
        foreach (var passenger in _simulation.PassengersFlow1.PassengersLuggageQueue)
        {
            dgvQueuePassLuggage_1.Rows.Add(passenger.Id, string.Join(",", passenger.Luggages.Select(l => l.ID)));
        }
        foreach (var passenger in _simulation.PassengersFlow2.PassengersLuggageQueue)
        {
            dgvQueuePassLuggage_2.Rows.Add(passenger.Id, string.Join(",", passenger.Luggages.Select(l => l.ID)));
        }
        dgvQueuePassLuggage_1.ResumeLayout();
        dgvQueuePassLuggage_2.ResumeLayout();

        //Queues RTG
        dgvQueueRTG_1.SuspendLayout();
        dgvQueueRTG_1.Rows.Clear();
        dgvQueueRTG_2.SuspendLayout();
        dgvQueueRTG_2.Rows.Clear();
        foreach (var passenger in _simulation.PassengersFlow1.PassengersRTGQueue)
        {
            dgvQueueRTG_1.Rows.Add(passenger.Id, string.Join(",", passenger.Luggages.Select(l => l.ID)));
        }
        foreach (var passenger in _simulation.PassengersFlow2.PassengersRTGQueue)
        {
            dgvQueueRTG_2.Rows.Add(passenger.Id, string.Join(",", passenger.Luggages.Select(l => l.ID)));
        }
        dgvQueueRTG_1.ResumeLayout();
        dgvQueueRTG_2.ResumeLayout();

        //Queues luggage before scanner
        dgvLuggageBefore_1.SuspendLayout();
        dgvLuggageBefore_1.Rows.Clear();
        dgvLuggageBefore_2.SuspendLayout();
        dgvLuggageBefore_2.Rows.Clear();
        foreach (var luggage in _simulation.PassengersFlow1.LuggagesBeforeQueue)
        {
            dgvLuggageBefore_1.Rows.Add(luggage.ID, luggage.Passenger.Id);
        }
        foreach (var luggage in _simulation.PassengersFlow2.LuggagesBeforeQueue)
        {
            dgvLuggageBefore_2.Rows.Add(luggage.ID, luggage.Passenger.Id);
        }
        dgvLuggageBefore_1.ResumeLayout();
        dgvLuggageBefore_2.ResumeLayout();

        //Queues luggage after scanner
        dgvLuggageAfter_1.SuspendLayout();
        dgvLuggageAfter_1.Rows.Clear();
        dgvLuggageAfter_2.SuspendLayout();
        dgvLuggageAfter_2.Rows.Clear();
        foreach (var luggage in _simulation.PassengersFlow1.LuggagesAfterQueue)
        {
            dgvLuggageAfter_1.Rows.Add(luggage.ID, luggage.Passenger.Id);
        }
        foreach (var luggage in _simulation.PassengersFlow2.LuggagesAfterQueue)
        {
            dgvLuggageAfter_2.Rows.Add(luggage.ID, luggage.Passenger.Id);
        }
        dgvLuggageAfter_1.ResumeLayout();
        dgvLuggageAfter_2.ResumeLayout();
    }
}