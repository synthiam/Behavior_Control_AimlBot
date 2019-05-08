using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace GaitoBotEditor
{
	public class EULA : Form
	{
		private bool _bestaetigt;

		private bool _darfSchliessen;

		private IContainer components = null;

		private WebBrowser webBrowserEULA;

		private Button buttonDrucken;

		private Button buttonAkzeptieren;

		private Button buttonAblehnen;

		public bool DarfSchliessen
		{
			set
			{
				this._darfSchliessen = value;
			}
		}

		public bool Bestaetigt
		{
			get
			{
				return this._bestaetigt;
			}
		}

		public EULA()
		{
			this.InitializeComponent();
		}

		private void EULA_Load(object sender, EventArgs e)
		{
			this.webBrowserEULA.DocumentText = ResReader.Reader.GetString("EULA");
			this.webBrowserEULA.AllowNavigation = false;
			this.webBrowserEULA.IsWebBrowserContextMenuEnabled = false;
			base.FormClosing += this.EULA_FormClosing;
		}

		private void buttonAblehnen_Click(object sender, EventArgs e)
		{
			base.Hide();
		}

		private void buttonAkzeptieren_Click(object sender, EventArgs e)
		{
			this._bestaetigt = true;
			base.Hide();
		}

		private void buttonDrucken_Click(object sender, EventArgs e)
		{
			this.webBrowserEULA.Print();
		}

		private void EULA_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (!this._darfSchliessen)
			{
				base.Hide();
				e.Cancel = true;
			}
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(EULA));
			this.webBrowserEULA = new WebBrowser();
			this.buttonDrucken = new Button();
			this.buttonAkzeptieren = new Button();
			this.buttonAblehnen = new Button();
			base.SuspendLayout();
			componentResourceManager.ApplyResources(this.webBrowserEULA, "webBrowserEULA");
			this.webBrowserEULA.MinimumSize = new Size(20, 20);
			this.webBrowserEULA.Name = "webBrowserEULA";
			componentResourceManager.ApplyResources(this.buttonDrucken, "buttonDrucken");
			this.buttonDrucken.Name = "buttonDrucken";
			this.buttonDrucken.UseVisualStyleBackColor = true;
			this.buttonDrucken.Click += this.buttonDrucken_Click;
			componentResourceManager.ApplyResources(this.buttonAkzeptieren, "buttonAkzeptieren");
			this.buttonAkzeptieren.Name = "buttonAkzeptieren";
			this.buttonAkzeptieren.UseVisualStyleBackColor = true;
			this.buttonAkzeptieren.Click += this.buttonAkzeptieren_Click;
			componentResourceManager.ApplyResources(this.buttonAblehnen, "buttonAblehnen");
			this.buttonAblehnen.DialogResult = DialogResult.Cancel;
			this.buttonAblehnen.Name = "buttonAblehnen";
			this.buttonAblehnen.UseVisualStyleBackColor = true;
			this.buttonAblehnen.Click += this.buttonAblehnen_Click;
			base.AcceptButton = this.buttonAblehnen;
			componentResourceManager.ApplyResources(this, "$this");
			base.AutoScaleMode = AutoScaleMode.Font;
			base.CancelButton = this.buttonAblehnen;
			base.Controls.Add(this.buttonAblehnen);
			base.Controls.Add(this.buttonAkzeptieren);
			base.Controls.Add(this.buttonDrucken);
			base.Controls.Add(this.webBrowserEULA);
			base.Name = "EULA";
			base.Load += this.EULA_Load;
			base.ResumeLayout(false);
		}
	}
}
