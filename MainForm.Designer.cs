partial class MainForm
{
    private System.ComponentModel.IContainer components = null;
    private Button btnStart;
    private Button btnStop;
    private Label lblStatus;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
            components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        this.btnStart = new Button();
        this.btnStop = new Button();
        this.lblStatus = new Label();

        // 
        // btnStart
        // 
        this.btnStart.Font = new System.Drawing.Font("Segoe UI", 14F);
        this.btnStart.Location = new System.Drawing.Point(40, 40);
        this.btnStart.Size = new System.Drawing.Size(200, 60);
        this.btnStart.Text = "Start Bot";
        this.btnStart.Click += new System.EventHandler(this.btnStart_Click);

        // 
        // btnStop
        // 
        this.btnStop.Font = new System.Drawing.Font("Segoe UI", 14F);
        this.btnStop.Location = new System.Drawing.Point(260, 40);
        this.btnStop.Size = new System.Drawing.Size(200, 60);
        this.btnStop.Text = "Stop Bot";
        this.btnStop.Click += new System.EventHandler(this.btnStop_Click);

        // 
        // lblStatus
        // 
        this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 12F);
        this.lblStatus.Location = new System.Drawing.Point(40, 120);
        this.lblStatus.Size = new System.Drawing.Size(420, 40);
        this.lblStatus.Text = "Status: Idle";

        // 
        // MainForm
        // 
        this.ClientSize = new System.Drawing.Size(500, 200);
        this.Controls.Add(this.btnStart);
        this.Controls.Add(this.btnStop);
        this.Controls.Add(this.lblStatus);
        this.Text = "Job Application Bot";
    }
}
