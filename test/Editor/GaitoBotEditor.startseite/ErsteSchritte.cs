using GaitoBotEditorCore;
using MultiLang;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace GaitoBotEditor.startseite
{
	public class ErsteSchritte : UserControl
	{
		private IContainer components = null;

		private WebBrowser webBrowserErsteSchritte;

		private Label labelTitelErsteSchritte;

		private string ErsteSchritteFehlerSeiteTitel
		{
			get
			{
				return global::MultiLang.ml.ml_string(49, "Kann erste Schritte nicht laden");
			}
		}

		private string ErsteSchritteFehlerSeiteInhalt
		{
			get
			{
				return string.Format("<p><b>{0}</b></p><p>{1}</p>", global::MultiLang.ml.ml_string(49, "Kann erste Schritte nicht laden"), global::MultiLang.ml.ml_string(48, "Bitte prüfen Sie Ihre Internetverbindung und stellen Sie sicher, dass das Programm nicht durch eine Firewall blockiert wird."));
			}
		}

		public ErsteSchritte()
		{
			this.InitializeComponent();
			this.webBrowserErsteSchritte.DocumentCompleted += this.webBrowserErsteSchritte_DocumentCompleted;
			this.webBrowserErsteSchritte.WebBrowserShortcutsEnabled = false;
			this.webBrowserErsteSchritte.IsWebBrowserContextMenuEnabled = false;
		}

		public void Anzeigen()
		{
			this.webBrowserErsteSchritte.DocumentText = global::MultiLang.ml.ml_string(50, "Erste Schritte werden geladen");
			this.webBrowserErsteSchritte.Navigate(Config.GlobalConfig.URLErsteSchritte);
		}

		private void webBrowserErsteSchritte_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
			if (!(this.webBrowserErsteSchritte.Url.AbsoluteUri == "about:blank") && !(this.webBrowserErsteSchritte.DocumentTitle == this.ErsteSchritteFehlerSeiteTitel) && !this.webBrowserErsteSchritte.DocumentTitle.Contains("GAITO"))
			{
				this.webBrowserErsteSchritte.DocumentText = this.ErsteSchritteFehlerSeiteInhalt;
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ErsteSchritte));
			this.webBrowserErsteSchritte = new WebBrowser();
			this.labelTitelErsteSchritte = new Label();
			base.SuspendLayout();
			componentResourceManager.ApplyResources(this.webBrowserErsteSchritte, "webBrowserErsteSchritte");
			this.webBrowserErsteSchritte.MinimumSize = new Size(20, 20);
			this.webBrowserErsteSchritte.Name = "webBrowserErsteSchritte";
			componentResourceManager.ApplyResources(this.labelTitelErsteSchritte, "labelTitelErsteSchritte");
			this.labelTitelErsteSchritte.BackColor = Color.WhiteSmoke;
			this.labelTitelErsteSchritte.Name = "labelTitelErsteSchritte";
			componentResourceManager.ApplyResources(this, "$this");
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = Color.LightSteelBlue;
			base.Controls.Add(this.labelTitelErsteSchritte);
			base.Controls.Add(this.webBrowserErsteSchritte);
			base.Name = "ErsteSchritte";
			base.ResumeLayout(false);
		}
	}
}
