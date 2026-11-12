using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

public partial class MainForm : Form
{
    private CancellationTokenSource? _cts;
    private Task? _botTask;

    private List<(AdzunaJob job, int score)> _rankedJobs;
    private string _resumePath;
    private List<string> _skills;

    public MainForm(
        List<(AdzunaJob job, int score)> rankedJobs,
        string resumePath,
        List<string> skills)
    {
        InitializeComponent();

        _rankedJobs = rankedJobs;
        _resumePath = resumePath;
        _skills = skills;
    }

    private void btnStart_Click(object sender, EventArgs e)
    {
        if (_botTask == null || _botTask.IsCompleted)
        {
            lblStatus.Text = "Status: Running...";
            _cts = new CancellationTokenSource();

            var runner = new BotRunner();

            _botTask = Task.Run(() =>
                runner.RunAsync(_rankedJobs, _resumePath, _skills, _cts.Token)
            );
        }
        else
        {
            lblStatus.Text = "Status: Already running";
        }
    }

    private void btnStop_Click(object sender, EventArgs e)
    {
        if (_cts != null)
        {
            lblStatus.Text = "Status: Stopping...";
            _cts.Cancel();
        }
        else
        {
            lblStatus.Text = "Status: Not running";
        }
    }
}

