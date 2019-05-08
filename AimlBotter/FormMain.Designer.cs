namespace AimlBotter {
  partial class FormMain {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (disposing && (components != null)) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
      this.textBox1 = new System.Windows.Forms.TextBox();
      this.tbLog = new System.Windows.Forms.TextBox();
      this.panel1 = new System.Windows.Forms.Panel();
      this.ucConfigurationButton1 = new EZ_Builder.UCForms.UC.UCConfigurationButton();
      this.button1 = new System.Windows.Forms.Button();
      this.panel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // textBox1
      // 
      this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.textBox1.Location = new System.Drawing.Point(38, 0);
      this.textBox1.Name = "textBox1";
      this.textBox1.Size = new System.Drawing.Size(488, 23);
      this.textBox1.TabIndex = 1;
      // 
      // tbLog
      // 
      this.tbLog.BackColor = System.Drawing.Color.Black;
      this.tbLog.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tbLog.Font = new System.Drawing.Font("Consolas", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.tbLog.ForeColor = System.Drawing.Color.Lime;
      this.tbLog.Location = new System.Drawing.Point(0, 32);
      this.tbLog.Multiline = true;
      this.tbLog.Name = "tbLog";
      this.tbLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.tbLog.Size = new System.Drawing.Size(601, 449);
      this.tbLog.TabIndex = 2;
      // 
      // panel1
      // 
      this.panel1.Controls.Add(this.textBox1);
      this.panel1.Controls.Add(this.ucConfigurationButton1);
      this.panel1.Controls.Add(this.button1);
      this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
      this.panel1.Location = new System.Drawing.Point(0, 0);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(601, 32);
      this.panel1.TabIndex = 3;
      // 
      // ucConfigurationButton1
      // 
      this.ucConfigurationButton1.Dock = System.Windows.Forms.DockStyle.Left;
      this.ucConfigurationButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.ucConfigurationButton1.Image = ((System.Drawing.Image)(resources.GetObject("ucConfigurationButton1.Image")));
      this.ucConfigurationButton1.Location = new System.Drawing.Point(0, 0);
      this.ucConfigurationButton1.Name = "ucConfigurationButton1";
      this.ucConfigurationButton1.Size = new System.Drawing.Size(38, 32);
      this.ucConfigurationButton1.TabIndex = 3;
      this.ucConfigurationButton1.UseVisualStyleBackColor = true;
      this.ucConfigurationButton1.Click += new System.EventHandler(this.ucConfigurationButton1_Click);
      // 
      // button1
      // 
      this.button1.Dock = System.Windows.Forms.DockStyle.Right;
      this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.button1.Location = new System.Drawing.Point(526, 0);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(75, 32);
      this.button1.TabIndex = 2;
      this.button1.Text = "&Send";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new System.EventHandler(this.button1_Click_1);
      // 
      // FormMain
      // 
      this.AcceptButton = this.button1;
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
      this.ClientSize = new System.Drawing.Size(601, 481);
      this.Controls.Add(this.tbLog);
      this.Controls.Add(this.panel1);
      this.Name = "FormMain";
      this.Text = "FormMain";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
      this.panel1.ResumeLayout(false);
      this.panel1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion
    private System.Windows.Forms.TextBox textBox1;
    private System.Windows.Forms.TextBox tbLog;
    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.Button button1;
    private EZ_Builder.UCForms.UC.UCConfigurationButton ucConfigurationButton1;
  }
}