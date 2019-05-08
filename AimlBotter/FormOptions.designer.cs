namespace AimlBotter {
  partial class FormOptions {
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
      this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.button1 = new System.Windows.Forms.Button();
      this.button2 = new System.Windows.Forms.Button();
      this.tbResponseVariable = new System.Windows.Forms.TextBox();
      this.label2 = new System.Windows.Forms.Label();
      this.ucScriptResponse = new EZ_Builder.UCForms.UC.UCScriptEditInput();
      this.label4 = new System.Windows.Forms.Label();
      this.button3 = new System.Windows.Forms.Button();
      this.textBox1 = new System.Windows.Forms.TextBox();
      this.button5 = new System.Windows.Forms.Button();
      this.button4 = new System.Windows.Forms.Button();
      this.btnChangeConfigFolder = new System.Windows.Forms.Button();
      this.lblConfigFolder = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // dataGridViewTextBoxColumn1
      // 
      this.dataGridViewTextBoxColumn1.HeaderText = "Phrase";
      this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
      this.dataGridViewTextBoxColumn1.Width = 120;
      // 
      // button1
      // 
      this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.button1.Location = new System.Drawing.Point(77, 250);
      this.button1.Margin = new System.Windows.Forms.Padding(4);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(100, 28);
      this.button1.TabIndex = 6;
      this.button1.Text = "&Save";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new System.EventHandler(this.button1_Click);
      // 
      // button2
      // 
      this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.button2.Location = new System.Drawing.Point(215, 250);
      this.button2.Margin = new System.Windows.Forms.Padding(4);
      this.button2.Name = "button2";
      this.button2.Size = new System.Drawing.Size(100, 28);
      this.button2.TabIndex = 7;
      this.button2.Text = "&Cancel";
      this.button2.UseVisualStyleBackColor = true;
      this.button2.Click += new System.EventHandler(this.button2_Click);
      // 
      // tbResponseVariable
      // 
      this.tbResponseVariable.Location = new System.Drawing.Point(159, 15);
      this.tbResponseVariable.Margin = new System.Windows.Forms.Padding(4);
      this.tbResponseVariable.Name = "tbResponseVariable";
      this.tbResponseVariable.Size = new System.Drawing.Size(220, 20);
      this.tbResponseVariable.TabIndex = 64;
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(19, 19);
      this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(99, 13);
      this.label2.TabIndex = 63;
      this.label2.Text = "Response Variable:";
      // 
      // ucScriptResponse
      // 
      this.ucScriptResponse.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.ucScriptResponse.Location = new System.Drawing.Point(159, 46);
      this.ucScriptResponse.Margin = new System.Windows.Forms.Padding(0);
      this.ucScriptResponse.Name = "ucScriptResponse";
      this.ucScriptResponse.Size = new System.Drawing.Size(221, 25);
      this.ucScriptResponse.TabIndex = 66;
      this.ucScriptResponse.Value = "";
      this.ucScriptResponse.XML = "";
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(33, 49);
      this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(88, 13);
      this.label4.TabIndex = 67;
      this.label4.Text = "Response Script:";
      // 
      // button3
      // 
      this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.button3.Location = new System.Drawing.Point(159, 113);
      this.button3.Margin = new System.Windows.Forms.Padding(4);
      this.button3.Name = "button3";
      this.button3.Size = new System.Drawing.Size(190, 28);
      this.button3.TabIndex = 68;
      this.button3.Text = "&Open Config Folder";
      this.button3.UseVisualStyleBackColor = true;
      this.button3.Click += new System.EventHandler(this.button3_Click);
      // 
      // textBox1
      // 
      this.textBox1.Location = new System.Drawing.Point(2, 188);
      this.textBox1.Name = "textBox1";
      this.textBox1.Size = new System.Drawing.Size(265, 20);
      this.textBox1.TabIndex = 70;
      // 
      // button5
      // 
      this.button5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.button5.Location = new System.Drawing.Point(274, 188);
      this.button5.Margin = new System.Windows.Forms.Padding(4);
      this.button5.Name = "button5";
      this.button5.Size = new System.Drawing.Size(102, 28);
      this.button5.TabIndex = 71;
      this.button5.Text = "&Search";
      this.button5.UseVisualStyleBackColor = true;
      this.button5.Click += new System.EventHandler(this.button5_Click);
      // 
      // button4
      // 
      this.button4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.button4.Location = new System.Drawing.Point(159, 149);
      this.button4.Margin = new System.Windows.Forms.Padding(4);
      this.button4.Name = "button4";
      this.button4.Size = new System.Drawing.Size(190, 28);
      this.button4.TabIndex = 72;
      this.button4.Text = "Open AIML &Editor";
      this.button4.UseVisualStyleBackColor = true;
      this.button4.Click += new System.EventHandler(this.button4_Click_1);
      // 
      // btnChangeConfigFolder
      // 
      this.btnChangeConfigFolder.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnChangeConfigFolder.Location = new System.Drawing.Point(12, 75);
      this.btnChangeConfigFolder.Name = "btnChangeConfigFolder";
      this.btnChangeConfigFolder.Size = new System.Drawing.Size(106, 31);
      this.btnChangeConfigFolder.TabIndex = 73;
      this.btnChangeConfigFolder.Text = "Config Folder";
      this.btnChangeConfigFolder.UseVisualStyleBackColor = true;
      this.btnChangeConfigFolder.Click += new System.EventHandler(this.btnChangeConfigFolder_Click);
      // 
      // lblConfigFolder
      // 
      this.lblConfigFolder.AutoSize = true;
      this.lblConfigFolder.Location = new System.Drawing.Point(124, 84);
      this.lblConfigFolder.Name = "lblConfigFolder";
      this.lblConfigFolder.Size = new System.Drawing.Size(35, 13);
      this.lblConfigFolder.TabIndex = 74;
      this.lblConfigFolder.Text = "label1";
      // 
      // FormOptions
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
      this.BackColor = System.Drawing.Color.White;
      this.ClientSize = new System.Drawing.Size(793, 297);
      this.Controls.Add(this.lblConfigFolder);
      this.Controls.Add(this.btnChangeConfigFolder);
      this.Controls.Add(this.button4);
      this.Controls.Add(this.button5);
      this.Controls.Add(this.textBox1);
      this.Controls.Add(this.button3);
      this.Controls.Add(this.label4);
      this.Controls.Add(this.ucScriptResponse);
      this.Controls.Add(this.tbResponseVariable);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.button2);
      this.Controls.Add(this.button1);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.Margin = new System.Windows.Forms.Padding(4);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "FormOptions";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Settings";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.Button button2;
    private System.Windows.Forms.TextBox tbResponseVariable;
    private System.Windows.Forms.Label label2;
    private EZ_Builder.UCForms.UC.UCScriptEditInput ucScriptResponse;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Button button3;
    private System.Windows.Forms.TextBox textBox1;
    private System.Windows.Forms.Button button5;
    private System.Windows.Forms.Button button4;
    private System.Windows.Forms.Button btnChangeConfigFolder;
    private System.Windows.Forms.Label lblConfigFolder;
  }
}