using GaitoBotEditorCore;
using MultiLang;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace GaitoBotEditor
{
	public class frmAbout : Form
	{
		private IContainer components = null;

		private Label labelUeberschrift;

		private Button buttonOk;

		private TextBox textBoxInhalt;

		public frmAbout()
		{
			this.InitializeComponent();
			base.Load += this.frmAbout_Load;
		}

		private void frmAbout_Load(object sender, EventArgs e)
		{
			this.Text = string.Format(global::MultiLang.ml.ml_string(61, "über {0}"), LizenzManager.ProgrammName);
			this.labelUeberschrift.Text = string.Format("{0} {1} V{2}", LizenzManager.ProgrammName, LizenzManager.ProgrammLizenzName, LizenzManager.ProgrammVersionNummerAnzeige);
			StringBuilder stringBuilder = new StringBuilder();
			LizenzManager.LizenzArten lizenz = LizenzManager.Lizenz;
			if (lizenz != 0 && lizenz != LizenzManager.LizenzArten.Standard)
			{
				stringBuilder.Append("\r\n");
				stringBuilder.AppendFormat(global::MultiLang.ml.ml_string(52, "Lizenziert für: {0}"), LizenzManager.LizenziertAuf);
				stringBuilder.Append("\r\n");
				stringBuilder.Append("\r\n");
			}
			stringBuilder.Append(ResReader.Reader.GetString("about"));
			this.textBoxInhalt.Text = stringBuilder.ToString();
		}

		private void buttonOk_Click(object sender, EventArgs e)
		{
			base.Close();
			base.Dispose();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(frmAbout));
			this.labelUeberschrift = new Label();
			this.buttonOk = new Button();
			this.textBoxInhalt = new TextBox();
			base.SuspendLayout();
			componentResourceManager.ApplyResources(this.labelUeberschrift, "labelUeberschrift");
			this.labelUeberschrift.Name = "labelUeberschrift";
			componentResourceManager.ApplyResources(this.buttonOk, "buttonOk");
			this.buttonOk.BackColor = SystemColors.Control;
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.UseVisualStyleBackColor = false;
			this.buttonOk.Click += this.buttonOk_Click;
			componentResourceManager.ApplyResources(this.textBoxInhalt, "textBoxInhalt");
			this.textBoxInhalt.BackColor = Color.White;
			this.textBoxInhalt.BorderStyle = BorderStyle.None;
			this.textBoxInhalt.Name = "textBoxInhalt";
			this.textBoxInhalt.ReadOnly = true;
			componentResourceManager.ApplyResources(this, "$this");
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = Color.White;
			base.Controls.Add(this.textBoxInhalt);
			base.Controls.Add(this.buttonOk);
			base.Controls.Add(this.labelUeberschrift);
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "frmAbout";
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
