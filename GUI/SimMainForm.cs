using System.Data;
using System.Reflection;
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
    private FlowLayoutPanel mainGridContainer;

    // Prvky pre Flow 1
    private DataGridView dgvQueuePassLuggage_1;
    private DataGridView dgvQueueRTG_1;
    private DataGridView dgvLuggageBefore_1;
    private DataGridView dgvLuggageAfter_1;
    private DataGridView dgvBelt_1;
    private Label lblScanning_1;
    private Label lblAtRTG_1;
    private Label lblBodyCheck_1;

    // Prvky pre Flow 2
    private DataGridView dgvQueuePassLuggage_2;
    private DataGridView dgvQueueRTG_2;
    private DataGridView dgvLuggageBefore_2;
    private DataGridView dgvLuggageAfter_2;
    private DataGridView dgvBelt_2;
    private Label lblScanning_2;
    private Label lblAtRTG_2;
    private Label lblBodyCheck_2;

    private readonly double[] _speedSteps = { 0.05, 0.1, 0.25, 0.5, 0.75, 1, 2.5, 5, 10, 25, 50};
    private int _currentSpeedIndex = 5;

    private AirportSim _simulation;

    public SimMainForm()
    {
        InitializeComponent();
        this.Text = "Airport Simulation Monitor";
        this.WindowState = FormWindowState.Maximized;
        _simulation = new AirportSim(100);
        InitCustomComponents();
    }

    private void InitCustomComponents()
    {
        // --- HORNÝ PANEL ---
        topPanel = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 60, AutoSize = true, Padding = new Padding(10) };

        btnStart = new Button { Text = "Spustiť", AutoSize = true };
        btnStart.Click += (s, e) => StartSimulation();

        btnPauseResume = new Button { Text = "Pozastaviť", Enabled = false, AutoSize = true };
        btnPauseResume.Click += (s, e) => TogglePause();

        btnSpeedDown = new Button { Text = "-", Width = 30 };
        btnSpeedDown.Click += (s, e) => ChangeSpeed(-1);

        lblSpeedValue = new Label { Text = "1", AutoSize = true, Margin = new Padding(5, 7, 5, 0) };

        btnSpeedUp = new Button { Text = "+", Width = 30 };
        btnSpeedUp.Click += (s, e) => ChangeSpeed(1);

        cbTurbo = new CheckBox { Text = "Turbo", Margin = new Padding(10, 5, 0, 0) };

        lblSimTime = new Label { Text = "Simulačný čas: 0.00", AutoSize = true, Font = new Font(this.Font, FontStyle.Bold), Margin = new Padding(20, 7, 0, 0) };

        topPanel.Controls.AddRange(new Control[] {
            btnStart, btnPauseResume, new Label { Text = "Rýchlosť:", Margin = new Padding(20, 7, 0, 0) },
            btnSpeedDown, lblSpeedValue, btnSpeedUp, cbTurbo, lblSimTime
        });

        // --- HLAVNÝ KONTAJNER ---
        mainGridContainer = new FlowLayoutPanel {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            Padding = new Padding(10)
        };

        mainGridContainer.Controls.Add(CreateFlowRow(1));
        mainGridContainer.Controls.Add(new Label { Text = " ", Height = 20 });
        mainGridContainer.Controls.Add(CreateFlowRow(2));

        this.Controls.Add(mainGridContainer);
        this.Controls.Add(topPanel);
    }

    private Panel CreateFlowRow(int flowNum)
    {
        var rowPanel = new FlowLayoutPanel {
            FlowDirection = FlowDirection.LeftToRight,
            AutoSize = true,
            WrapContents = false,
            BackColor = flowNum == 1 ? Color.GhostWhite : Color.SeaShell,
            BorderStyle = BorderStyle.FixedSingle,
            Padding = new Padding(5)
        };

        var dgvPass = CreateGrid("ID Pasažier", "Batožiny");
        var dgvRTG = CreateGrid("ID Pasažier", "Batožiny");
        var dgvBefore = CreateGrid("ID Batožina", "Vlastník");
        var dgvAfter = CreateGrid("ID Batožina", "Vlastník");
        var dgvBelt = CreateGrid("ID Pasažier", "Batožiny");

        var lblScan = new Label { Text = "Skenuje sa: ---", AutoSize = true, ForeColor = Color.DarkBlue, Margin = new Padding(5, 60, 5, 0) };
        var lblRtgP = new Label { Text = "Pasažier RTG: ---", AutoSize = true, ForeColor = Color.DarkGreen };
        var lblBody = new Label { Text = "Osobná prehliadka: ---", AutoSize = true, ForeColor = Color.DarkRed };

        if (flowNum == 1) {
            dgvQueuePassLuggage_1 = dgvPass; dgvQueueRTG_1 = dgvRTG; dgvLuggageBefore_1 = dgvBefore;
            dgvLuggageAfter_1 = dgvAfter; dgvBelt_1 = dgvBelt;
            lblScanning_1 = lblScan; lblAtRTG_1 = lblRtgP; lblBodyCheck_1 = lblBody;
        } else {
            dgvQueuePassLuggage_2 = dgvPass; dgvQueueRTG_2 = dgvRTG; dgvLuggageBefore_2 = dgvBefore;
            dgvLuggageAfter_2 = dgvAfter; dgvBelt_2 = dgvBelt;
            lblScanning_2 = lblScan; lblAtRTG_2 = lblRtgP; lblBodyCheck_2 = lblBody;
        }

        rowPanel.Controls.Add(CreateGridContainer($"({flowNum}) Odovzdávanie", dgvPass));
        var rtgSection = new FlowLayoutPanel { FlowDirection = FlowDirection.TopDown, AutoSize = true };
        rtgSection.Controls.Add(CreateGridContainer($"({flowNum}) Pred RTG", dgvRTG));
        rtgSection.Controls.Add(lblRtgP); rtgSection.Controls.Add(lblBody);
        rowPanel.Controls.Add(rtgSection);
        rowPanel.Controls.Add(CreateGridContainer($"({flowNum}) Pred skenerom", dgvBefore));
        rowPanel.Controls.Add(lblScan);
        rowPanel.Controls.Add(CreateGridContainer($"({flowNum}) Po skeneri", dgvAfter));
        rowPanel.Controls.Add(CreateGridContainer($"({flowNum}) Posuvný pás", dgvBelt));

        return rowPanel;
    }

    private DataGridView CreateGrid(string col1, string col2)
    {
        var dgv = new DataGridView {
            ColumnCount = 2, ReadOnly = true, RowHeadersVisible = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            AllowUserToAddRows = false, Height = 150, Width = 180, BackgroundColor = Color.White
        };
        dgv.Columns[0].Name = col1; dgv.Columns[1].Name = col2;

        typeof(Control).GetProperty("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.SetValue(dgv, true, null);
        
        return dgv;
    }

    private Panel CreateGridContainer(string title, DataGridView dgv)
    {
        var panel = new FlowLayoutPanel { FlowDirection = FlowDirection.TopDown, AutoSize = true, Margin = new Padding(5) };
        panel.Controls.Add(new Label { Text = title, AutoSize = true, Font = new Font(this.Font, FontStyle.Bold) });
        panel.Controls.Add(dgv);
        return panel;
    }

    private void ChangeSpeed(int delta)
    {
        int newIndex = Math.Clamp(_currentSpeedIndex + delta, 0, _speedSteps.Length - 1);
        _currentSpeedIndex = newIndex;
        lblSpeedValue.Text = _speedSteps[_currentSpeedIndex].ToString();
        _simulation.SetSpeed(_speedSteps[_currentSpeedIndex]);
    }

    private void StartSimulation()
    {
        btnPauseResume.Enabled = true;
        btnPauseResume.Text = "Pozastaviť";
        
        _simulation = new AirportSim(100);
        _simulation.SetSpeed(_speedSteps[_currentSpeedIndex]);
        if (cbTurbo.Checked) _simulation.EnableTurboMode(); else _simulation.DisableTurboMode();
        
        _simulation.RegisterGUIDelegate(this);
        Task.Run(() => _simulation.RunSimulation(1000));
    }

    private void TogglePause()
    {
        if (btnPauseResume.Text == "Pozastaviť") {
            btnPauseResume.Text = "Pokračovať";
            _simulation.PauseSimulation();
        } else {
            btnPauseResume.Text = "Pozastaviť";
            _simulation.ResumeSimulation();
        }
    }

    public void RefreshGUI()
    {
        if (this.IsDisposed || !this.IsHandleCreated) return;

        if (InvokeRequired)
        {
            this.BeginInvoke(new Action(RefreshGUI));
            return;
        }

        lblSimTime.Text = $"Simulačný čas: {_simulation.SimTime:F2}" + " replikacia: " + _simulation.CurrentReplication;

        if (_simulation.Speed > 50)
            return;

        //Queues luggage delivery
        dgvQueuePassLuggage_1.SuspendLayout();
        dgvQueuePassLuggage_1.Rows.Clear();
        dgvQueuePassLuggage_2.SuspendLayout();
        dgvQueuePassLuggage_2.Rows.Clear();
        foreach (var passenger in _simulation.PassengersFlow1.PassengersLuggageQueue.ToList())
        {
            dgvQueuePassLuggage_1.Rows.Add(passenger.Id, string.Join(",", passenger.Luggages.Select(l => l.ID)));
        }
        foreach (var passenger in _simulation.PassengersFlow2.PassengersLuggageQueue.ToList())
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
        foreach (var passenger in _simulation.PassengersFlow1.PassengersRTGQueue.ToList())
        {
            dgvQueueRTG_1.Rows.Add(passenger.Id, string.Join(",", passenger.Luggages.Select(l => l.ID)));
        }
        foreach (var passenger in _simulation.PassengersFlow2.PassengersRTGQueue.ToList())
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
        foreach (var luggage in _simulation.PassengersFlow1.LuggagesBeforeQueue.ToList())
        {
            dgvLuggageBefore_1.Rows.Add(luggage.ID, luggage.Passenger.Id);
        }
        foreach (var luggage in _simulation.PassengersFlow2.LuggagesBeforeQueue.ToList())
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
        foreach (var luggage in _simulation.PassengersFlow1.LuggagesAfterQueue.ToList())
        {
            dgvLuggageAfter_1.Rows.Add(luggage.ID, luggage.Passenger.Id);
        }
        foreach (var luggage in _simulation.PassengersFlow2.LuggagesAfterQueue.ToList())
        {
            dgvLuggageAfter_2.Rows.Add(luggage.ID, luggage.Passenger.Id);
        }
        dgvLuggageAfter_1.ResumeLayout();
        dgvLuggageAfter_2.ResumeLayout();

        //Queues waiting for luggage
        dgvBelt_1.SuspendLayout();
        dgvBelt_1.Rows.Clear();
        dgvBelt_2.SuspendLayout();
        dgvBelt_2.Rows.Clear();
        foreach (var passenger in _simulation.PassengersFlow1.PassengersWaitingLuggageQueue.ToList())
        {
            dgvBelt_1.Rows.Add(passenger.Id, string.Join(",", passenger.Luggages.Select(l => l.ID)));
        }
        foreach (var passenger in _simulation.PassengersFlow2.PassengersWaitingLuggageQueue.ToList())
        {
            dgvBelt_2.Rows.Add(passenger.Id, string.Join(",", passenger.Luggages.Select(l => l.ID)));
        }
        dgvBelt_1.ResumeLayout();
        dgvBelt_2.ResumeLayout();


        //Labels update
        lblScanning_1.Text = "Skenuje sa: " + _simulation.PassengersFlow1.LuggageScan?.ID;
        lblScanning_2.Text = "Skenuje sa: " + _simulation.PassengersFlow2.LuggageScan?.ID;
        lblAtRTG_1.Text = "Pasažier RTG: " + _simulation.PassengersFlow1.PassengerRTG?.Id;
        lblAtRTG_2.Text = "Pasažier RTG: " + _simulation.PassengersFlow2.PassengerRTG?.Id;
        lblBodyCheck_1.Text = "Osobná prehliadka: " + _simulation.PassengersFlow1.PassengerPersonalScan?.Id;
        lblBodyCheck_2.Text = "Osobná prehliadka: " + _simulation.PassengersFlow2.PassengerPersonalScan?.Id;
    }
}