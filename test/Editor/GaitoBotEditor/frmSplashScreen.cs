using GaitoBotEditor.Properties;
using GaitoBotEditorCore;
using MultiLang;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace GaitoBotEditor
{
	public class frmSplashScreen : Form
	{
		private int _warteAusblenden = 200;

		private int _opacity = 100;

		private IContainer components = null;

		private Label labelOben;

		private Label labelLizenzArt;

		private Timer timerAusblenden;

		private Label labelLizenziertFuer;

		public frmSplashScreen()
		{
			this.InitializeComponent();
			base.Load += this.frmSplashScreen_Load;
		}

		private void Anzeigen()
		{
			this.labelLizenzArt.Text = LizenzManager.ProgrammLizenzName;
			LizenzManager.LizenzArten lizenz = LizenzManager.Lizenz;
			if (lizenz == LizenzManager.LizenzArten.Betaversion || lizenz == LizenzManager.LizenzArten.Standard)
			{
				this.labelLizenziertFuer.Visible = false;
			}
			else
			{
				this.labelLizenziertFuer.Text = string.Format(global::MultiLang.ml.ml_string(52, "Lizenziert für: {0}"), LizenzManager.LizenziertAuf);
			}
			this.labelOben.Text = string.Format(global::MultiLang.ml.ml_string(53, "version {0}"), LizenzManager.ProgrammVersionNummerAnzeige);
		}

		private void timerAusblenden_Tick(object sender, EventArgs e)
		{
			if (this._warteAusblenden > 0)
			{
				this._warteAusblenden--;
			}
			else
			{
				this._opacity -= 4;
				if (this._opacity <= 0)
				{
					base.Close();
					base.Dispose();
				}
				else
				{
					base.Opacity = (double)this._opacity / 100.0;
				}
			}
		}

		private void frmSplashScreen_Load(object sender, EventArgs e)
		{
			this.Anzeigen();
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
			this.components = new Container();
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(frmSplashScreen));
			this.labelOben = new Label();
			this.labelLizenzArt = new Label();
			this.timerAusblenden = new Timer(this.components);
			this.labelLizenziertFuer = new Label();
			base.SuspendLayout();
			componentResourceManager.ApplyResources(this.labelOben, "labelOben");
			this.labelOben.Name = "labelOben";
			componentResourceManager.ApplyResources(this.labelLizenzArt, "labelLizenzArt");
			this.labelLizenzArt.Name = "labelLizenzArt";
			this.timerAusblenden.Enabled = true;
			this.timerAusblenden.Interval = 10;
			this.timerAusblenden.Tick += this.timerAusblenden_Tick;
			componentResourceManager.ApplyResources(this.labelLizenziertFuer, "labelLizenziertFuer");
			this.labelLizenziertFuer.Name = "labelLizenziertFuer";
			componentResourceManager.ApplyResources(this, "$this");
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = Color.White;
			this.BackgroundImage = Resources.splashScreen;
			base.ControlBox = false;
			base.Controls.Add(this.labelLizenziertFuer);
			base.Controls.Add(this.labelLizenzArt);
			base.Controls.Add(this.labelOben);
			base.FormBorderStyle = FormBorderStyle.None;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "frmSplashScreen";
			base.ShowInTaskbar = false;
			base.TopMost = true;
			base.ResumeLayout(false);
		}
	}
}
