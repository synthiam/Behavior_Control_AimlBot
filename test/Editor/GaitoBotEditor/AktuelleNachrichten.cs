using GaitoBotEditorCore;
using MultiLang;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace GaitoBotEditor
{
	public class AktuelleNachrichten : UserControl
	{
		private IContainer components = null;

		private WebBrowser webBrowserAktInfo;

		private Label labelTitelAktuelleNachrichten;

		private string NewsFehlerSeiteTitel
		{
			get
			{
				return global::MultiLang.ml.ml_string(57, "Kann aktuelle News nicht laden");
			}
		}

		private string NewsFehlerSeiteInhalt
		{
			get
			{
				return string.Format("<p><b>{0}</b></p><p>{1}</p>", global::MultiLang.ml.ml_string(57, "Kann aktuelle News nicht laden"), global::MultiLang.ml.ml_string(48, "Bitte prüfen Sie Ihre Internetverbindung und stellen Sie sicher, dass das Programm nicht durch eine Firewall blockiert wird."));
			}
		}

		public AktuelleNachrichten()
		{
			this.InitializeComponent();
			this.webBrowserAktInfo.DocumentCompleted += this.webBrowserAktInfo_DocumentCompleted;
			this.webBrowserAktInfo.WebBrowserShortcutsEnabled = false;
			this.webBrowserAktInfo.IsWebBrowserContextMenuEnabled = false;
		}

		public void Anzeigen()
		{
			this.webBrowserAktInfo.DocumentText = global::MultiLang.ml.ml_string(56, "News werden geladen...");
			string uRLAktuelleInformationen = Config.GlobalConfig.URLAktuelleInformationen;
			this.webBrowserAktInfo.Navigate(uRLAktuelleInformationen);
		}

		private void webBrowserAktInfo_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
			if (!(this.webBrowserAktInfo.Url.AbsoluteUri == "about:blank") && !(this.webBrowserAktInfo.DocumentTitle == this.NewsFehlerSeiteTitel) && !this.webBrowserAktInfo.DocumentTitle.ToLower().Contains("gaito"))
			{
				this.webBrowserAktInfo.DocumentText = this.NewsFehlerSeiteInhalt;
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(AktuelleNachrichten));
			this.webBrowserAktInfo = new WebBrowser();
			this.labelTitelAktuelleNachrichten = new Label();
			base.SuspendLayout();
			componentResourceManager.ApplyResources(this.webBrowserAktInfo, "webBrowserAktInfo");
			this.webBrowserAktInfo.MinimumSize = new Size(20, 20);
			this.webBrowserAktInfo.Name = "webBrowserAktInfo";
			componentResourceManager.ApplyResources(this.labelTitelAktuelleNachrichten, "labelTitelAktuelleNachrichten");
			this.labelTitelAktuelleNachrichten.BackColor = Color.WhiteSmoke;
			this.labelTitelAktuelleNachrichten.Name = "labelTitelAktuelleNachrichten";
			componentResourceManager.ApplyResources(this, "$this");
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = Color.LightSteelBlue;
			base.Controls.Add(this.labelTitelAktuelleNachrichten);
			base.Controls.Add(this.webBrowserAktInfo);
			base.Name = "AktuelleNachrichten";
			base.ResumeLayout(false);
		}
	}
}
