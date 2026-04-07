using System.Data;
using System.Reflection;
using DS_SEM_2.EventSimulation;
using DS_SEM_2.EventSimulation.AirportSimulation;
using ScottPlot.WinForms;

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

    private NumericUpDown numReplications;
    private NumericUpDown numCapacity;
    private NumericUpDown numMaxBeforeScanner;
    private NumericUpDown numMaxAfterScanner;

    private CheckBox cbCustomSeed;
    private NumericUpDown numSeed;

    private FlowLayoutPanel topPanel;
    private TabControl tabControl;
    private TabPage tabLive;
    private TabPage tabGraphs;

    private FlowLayoutPanel mainGridContainer;
    private DataGridView dgvQueuePassLuggage_1, dgvQueueRTG_1, dgvLuggageBefore_1, dgvLuggageAfter_1, dgvBelt_1;
    private Label lblScanning_1, lblAtRTG_1, lblBodyCheck_1;
    private DataGridView dgvQueuePassLuggage_2, dgvQueueRTG_2, dgvLuggageBefore_2, dgvLuggageAfter_2, dgvBelt_2;
    private Label lblScanning_2, lblAtRTG_2, lblBodyCheck_2;

    private Label lblStat1, lblStat2, lblStat3;

    private FormsPlot plot1, plot2, plot3;
    private ScottPlot.Plottables.Scatter s1, s2, s3;
    private List<ScottPlot.Coordinates> plot1Coord = new();
    private List<ScottPlot.Coordinates> plot2Coord = new();
    private List<ScottPlot.Coordinates> plot3Coord = new();
    private int _actualCapacityGraphs;

    private readonly double[] _speedSteps = { 0.05, 0.1, 0.25, 0.5, 0.75, 1, 2.5, 5, 10, 25, 50 };
    private int _currentSpeedIndex = 5;

    private AirportSim _simulation;

    public SimMainForm()
    {
        InitializeComponent();
        this.Text = "Airport Simulation Monitor";
        this.WindowState = FormWindowState.Maximized;

        _simulation = new AirportSim(100, 0, 0, 0);
        InitCustomComponents();
    }

    private void InitCustomComponents()
    {
        topPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 120,
            AutoSize = true,
            Padding = new Padding(10),
            BackColor = Color.FromArgb(235, 235, 235)
        };

        btnStart = new Button { Text = "Spustiť simuláciu", AutoSize = true, BackColor = Color.LightGreen, Font = new Font(this.Font, FontStyle.Bold) };
        btnStart.Click += (s, e) => StartSimulation();

        btnPauseResume = new Button { Text = "Pozastaviť", Enabled = false, AutoSize = true };
        btnPauseResume.Click += (s, e) => TogglePause();

        var speedPanel = new FlowLayoutPanel { AutoSize = true, Margin = new Padding(10, 0, 0, 0) };
        btnSpeedDown = new Button { Text = "-", Width = 30 };
        btnSpeedDown.Click += (s, e) => ChangeSpeed(-1);
        lblSpeedValue = new Label { Text = "1", AutoSize = true, Margin = new Padding(5, 7, 5, 0), Font = new Font(this.Font, FontStyle.Bold) };
        btnSpeedUp = new Button { Text = "+", Width = 30 };
        btnSpeedUp.Click += (s, e) => ChangeSpeed(1);
        speedPanel.Controls.AddRange(new Control[] { new Label { Text = "Rýchlosť:", Margin = new Padding(0, 7, 0, 0) }, btnSpeedDown, lblSpeedValue, btnSpeedUp });

        cbTurbo = new CheckBox { Text = "Turbo", Margin = new Padding(10, 5, 0, 0) };
        cbTurbo.CheckedChanged += (s, e) =>
        {
            if (cbTurbo.Checked) _simulation.EnableTurboMode(); else _simulation.DisableTurboMode();
        };

        cbCustomSeed = new CheckBox { Text = "Vlastný Seed", Margin = new Padding(10, 5, 0, 0), AutoSize = true };
        numSeed = new NumericUpDown { Minimum = 0, Maximum = int.MaxValue, Width = 100, Enabled = false, Value = 123 };
        cbCustomSeed.CheckedChanged += (s, e) => numSeed.Enabled = cbCustomSeed.Checked;

        lblSimTime = new Label { Text = "Čas: 0.00", AutoSize = true, Font = new Font(this.Font, FontStyle.Bold), Margin = new Padding(20, 7, 0, 0), ForeColor = Color.Blue };

        numReplications = CreateNumberInput(1000, 1, 1000000);
        numCapacity = CreateNumberInput(5717, 1, 1000000);
        numMaxBeforeScanner = CreateNumberInput(4, 1, 100);
        numMaxAfterScanner = CreateNumberInput(5, 1, 100);

        topPanel.Controls.AddRange(new Control[] {
            btnStart, btnPauseResume, speedPanel, cbTurbo,
            new Label { Text = "Replikácie:", AutoSize = true, Margin = new Padding(15, 7, 0, 0) }, numReplications,
            new Label { Text = "Kapacita:", AutoSize = true, Margin = new Padding(15, 7, 0, 0) }, numCapacity,
            new Label { Text = "Max pred sken:", AutoSize = true, Margin = new Padding(15, 7, 0, 0) }, numMaxBeforeScanner,
            new Label { Text = "Max po sken:", AutoSize = true, Margin = new Padding(15, 7, 0, 0) }, numMaxAfterScanner,
            cbCustomSeed, numSeed,
            lblSimTime
        });

        tabControl = new TabControl { Dock = DockStyle.Fill };
        tabLive = new TabPage { Text = "Sledovanie simulácie" };
        tabGraphs = new TabPage { Text = "Grafy a Štatistiky" };

        tabControl.SelectedIndexChanged += (s, e) =>
        {
            if (tabControl.SelectedTab == tabGraphs)
            {
                cbTurbo.Checked = true;
                cbTurbo.Enabled = false;
                if (_simulation != null) _simulation.EnableTurboMode();
            }
            else
            {
                cbTurbo.Enabled = true;
            }
        };

        tabControl.Selecting += (s, e) =>
        {
            if (btnStart.Text == "Zastaviť simuláciu")
            {
                e.Cancel = true;
            }
        };

        mainGridContainer = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            Padding = new Padding(10)
        };

        mainGridContainer.Controls.Add(CreateFlowRow(1));
        mainGridContainer.Controls.Add(new Label { Height = 10 });
        mainGridContainer.Controls.Add(CreateFlowRow(2));
        mainGridContainer.Controls.Add(new Label { Height = 15 });
        mainGridContainer.Controls.Add(CreateStatsRow());
        tabLive.Controls.Add(mainGridContainer);

        var graphsContainer = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 3,
            RowCount = 1
        };
        graphsContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));
        graphsContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));
        graphsContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));

        plot1 = new FormsPlot { Dock = DockStyle.Fill};
        plot1.Plot.Title("Priemerná doba bezpečnostnej kontroly");
        plot1.Plot.XLabel("Kapacita");
        plot1.Plot.YLabel("Čas");

        plot2 = new FormsPlot { Dock = DockStyle.Fill };
        plot2.Plot.Title("Priemerný počet ľudí čakajúcich na prevzatie batožiny");
        plot2.Plot.XLabel("Kapacita");
        plot2.Plot.YLabel("Počet ľudí");

        plot3 = new FormsPlot { Dock = DockStyle.Fill };
        plot3.Plot.Title("Priemerný počet ľudí čakajúcich na začiatok prehliadky");
        plot3.Plot.XLabel("Kapacita");
        plot3.Plot.YLabel("Počet ľudí");

        s1 = plot1.Plot.Add.Scatter(plot1Coord);
        s2 = plot2.Plot.Add.Scatter(plot2Coord);
        s3 = plot3.Plot.Add.Scatter(plot3Coord);

        graphsContainer.Controls.Add(plot1, 0, 0);
        graphsContainer.Controls.Add(plot2, 1, 0);
        graphsContainer.Controls.Add(plot3, 2, 0);
        tabGraphs.Controls.Add(graphsContainer);

        tabControl.TabPages.Add(tabLive);
        tabControl.TabPages.Add(tabGraphs);

        this.Controls.Add(tabControl);
        this.Controls.Add(topPanel);
    }

    private Panel CreateStatsRow()
    {
        var statsPanel = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.TopDown,
            AutoSize = true,
            BackColor = Color.LightSteelBlue,
            BorderStyle = BorderStyle.FixedSingle,
            Padding = new Padding(15),
            Width = 1100
        };

        lblStat1 = new Label { Text = "Priemerný čas na bezpečnostnej kontrole: ---", AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold), Margin = new Padding(0, 5, 0, 5) };
        lblStat2 = new Label { Text = "Priemerný počet ľudí čakajúcich na prevzatie batožiny: ---", AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold), Margin = new Padding(0, 5, 0, 5) };
        lblStat3 = new Label { Text = "Priemerný počet ľudí čakajúcich na začiatok prehliadky: ---", AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold), Margin = new Padding(0, 5, 0, 5) };

        statsPanel.Controls.AddRange(new Control[] {
            new Label { Text = "ŠTATISTIKY", Font = new Font(this.Font, FontStyle.Underline | FontStyle.Bold), Margin = new Padding(0, 0, 0, 10), Width = 800 },
            lblStat1, lblStat2, lblStat3
        });

        return statsPanel;
    }

    private NumericUpDown CreateNumberInput(int defaultValue, int min, int max)
    {
        return new NumericUpDown { Minimum = min, Maximum = max, Width = 70, Value = defaultValue };
    }

    private Panel CreateFlowRow(int flowNum)
    {
        var rowPanel = new FlowLayoutPanel
        {
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

        var lblScan = new Label { Text = "Skenuje sa: ---", AutoSize = true, ForeColor = Color.DarkBlue, Margin = new Padding(5, 60, 5, 0), Font = new Font(this.Font, FontStyle.Italic) };
        var lblRtgP = new Label { Text = "Pasažier RTG: ---", AutoSize = true, ForeColor = Color.DarkGreen, Margin = new Padding(0, 5, 0, 0) };
        var lblBody = new Label { Text = "Osobná prehliadka: ---", AutoSize = true, ForeColor = Color.DarkRed, Margin = new Padding(0, 5, 0, 0) };

        if (flowNum == 1)
        {
            dgvQueuePassLuggage_1 = dgvPass; dgvQueueRTG_1 = dgvRTG; dgvLuggageBefore_1 = dgvBefore; dgvLuggageAfter_1 = dgvAfter; dgvBelt_1 = dgvBelt;
            lblScanning_1 = lblScan; lblAtRTG_1 = lblRtgP; lblBodyCheck_1 = lblBody;
        }
        else
        {
            dgvQueuePassLuggage_2 = dgvPass; dgvQueueRTG_2 = dgvRTG; dgvLuggageBefore_2 = dgvBefore; dgvLuggageAfter_2 = dgvAfter; dgvBelt_2 = dgvBelt;
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
        var dgv = new DataGridView
        {
            ColumnCount = 2,
            ReadOnly = true,
            RowHeadersVisible = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            AllowUserToAddRows = false,
            Height = 140,
            Width = 175,
            BackgroundColor = Color.White
        };
        dgv.Columns[0].Name = col1; dgv.Columns[1].Name = col2;
        typeof(Control).GetProperty("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(dgv, true, null);
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
        if (_simulation != null) _simulation.SetSpeed(_speedSteps[_currentSpeedIndex]);
    }

    private void StartSimulation()
    {
        if (btnStart.Text == "Spustiť simuláciu")
        {
            StartButtonChangeOnStart();
            int reps = (int)numReplications.Value;
            int capacity = (int)numCapacity.Value;
            int maxBefore = (int)numMaxBeforeScanner.Value;
            int maxAfter = (int)numMaxAfterScanner.Value;
            Random r = new();

            int seed = cbCustomSeed.Checked ? (int)numSeed.Value : r.Next();
            _simulation = new AirportSim(seed, capacity, maxBefore, maxAfter);

            if (tabControl.SelectedTab == tabGraphs)
            {
                plot1Coord.Clear(); plot2Coord.Clear(); plot3Coord.Clear();
                cbTurbo.Checked = true;

                int points = 10;
                int capacity90 = (int)(capacity * 0.9);
                int capacity110 = (int)(capacity * 1.1);
                int step = (capacity110 - capacity90) / points;

                Task.Run(() =>
                {
                    int currentCap = capacity90;
                    Random r = new();

                    for (int i = 0; i < points; i++)
                    {
                        int seed = cbCustomSeed.Checked ? (int)numSeed.Value : r.Next();
                        var sim = new AirportSim(seed, currentCap, (int)numMaxBeforeScanner.Value, (int)numMaxAfterScanner.Value);
                        sim.EnableTurboMode();

                        sim.RunSimulation(reps);

                        this.Invoke(() =>
                        {
                            plot1Coord.Add(new ScottPlot.Coordinates(currentCap, sim.AverageTimeInSystemFinal.GetAverage()));
                            plot2Coord.Add(new ScottPlot.Coordinates(currentCap, sim.AverageLuggageWaitingQueueLengthFinal.GetAverage()));
                            plot3Coord.Add(new ScottPlot.Coordinates(currentCap, sim.AverageLuggageQueueLengthFinal.GetAverage()));

                            RefreshGUI();
                        });

                        currentCap += step;
                    }

                    this.Invoke(() => StartButtonChangeOnStop());
                });

                return;
            }
            else
            {
                if (cbTurbo.Checked) _simulation.EnableTurboMode(); else _simulation.DisableTurboMode();
                _simulation.SetSpeed(_speedSteps[_currentSpeedIndex]);
            }

            _simulation.RegisterGUIDelegate(this);
            Task.Run(() =>
            {
                _simulation.RunSimulation(reps);
                this.Invoke(new Action(StartButtonChangeOnStop));
            });

        }
        else
        {
            StartButtonChangeOnStop();
            _simulation.StopSimulation();
        }
    }

    private void StartButtonChangeOnStart()
    {
        btnPauseResume.Enabled = true;
        btnPauseResume.Text = "Pozastaviť";
        btnStart.Text = "Zastaviť simuláciu";
        btnStart.BackColor = Color.IndianRed;
    }

    private void StartButtonChangeOnStop()
    {
        btnPauseResume.Enabled = false;
        btnStart.Text = "Spustiť simuláciu";
        btnStart.BackColor = Color.LightGreen;
    }

    private void TogglePause()
    {
        if (btnPauseResume.Text == "Pozastaviť")
        {
            btnPauseResume.Text = "Pokračovať";
            _simulation.PauseSimulation();
        }
        else
        {
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

        if (tabControl.SelectedTab == tabGraphs)
        {
            plot1.Plot.Axes.AutoScale();
            plot2.Plot.Axes.AutoScale();
            plot3.Plot.Axes.AutoScale();
            plot1.Refresh();
            plot2.Refresh();
            plot3.Refresh();
            return;
        }




        if (_simulation.IsSimulationFinished)
            StartButtonChangeOnStop();

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

        var stat1Conf = _simulation.AverageTimeInSystemFinal.GetConfidenceInterval90();
        var stat2Conf = _simulation.AverageLuggageWaitingQueueLengthFinal.GetConfidenceInterval90();
        var stat3Conf = _simulation.AverageLuggageQueueLengthFinal.GetConfidenceInterval90();
        lblStat1.Text = $"Priemerný čas na bezpečnostnej kontrole: " +
            TimeSpan.FromSeconds(_simulation.AverageTimeInSystemFinal.GetAverage()) + "   " +
            MakeConfidenceIntervalString(TimeSpan.FromSeconds(stat1Conf.Lower).ToString(), TimeSpan.FromSeconds(stat1Conf.Upper).ToString());
        lblStat2.Text = $"Priemerný počet ľudí čakajúcich na prevzatie batožiny: {_simulation.AverageLuggageWaitingQueueLengthFinal.GetAverage():F5}   " +
            MakeConfidenceIntervalString(stat2Conf.Lower.ToString("F5"), stat2Conf.Upper.ToString("F5"));
        lblStat3.Text = $"Priemerný počet ľudí čakajúcich na začiatok prehliadky: {_simulation.AverageLuggageQueueLengthFinal.GetAverage():F5}   " +
            MakeConfidenceIntervalString(stat3Conf.Lower.ToString("F5"), stat3Conf.Upper.ToString("F5"));
    }

    private string MakeConfidenceIntervalString(string lower, string upper)
    {
        return "[" + lower + " ; " + upper + "]";
    }
}