using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace de.springwald.toolbox
{
	public class frmFehlerReport : Form
	{
		private IContainer components = null;

		private WebBrowser webBrowserSenden;

		public string ProgrammVersionNummerAnzeige
		{
			get
			{
				Version version = new Version(Application.ProductVersion);
				return string.Format("{0}.{1} B{2}", version.Major, version.Minor, version.Build);
			}
		}

		public frmFehlerReport()
		{
			this.InitializeComponent();
		}

		public void ZeigeFehler(Exception e, string additiveMeldung, string supportEmailAdresse, string supportWebScript)
		{
			string value = "-----------------------------------------------\r\n";
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(value);
			stringBuilder.Append("Exception:\r\n");
			stringBuilder.AppendFormat(">Message:  {0}\r\n", e.Message);
			stringBuilder.Append(value);
			stringBuilder.AppendFormat(">Source:  {0}\r\n", e.Source);
			stringBuilder.Append(value);
			stringBuilder.AppendFormat(">StackTrace:  {0}\r\n", e.StackTrace);
			stringBuilder.Append(value);
			if (e.InnerException != null)
			{
				stringBuilder.Append("InnerException:\r\n");
				stringBuilder.AppendFormat(">Message:  {0}\r\n", e.InnerException.Message);
				stringBuilder.Append(value);
				stringBuilder.AppendFormat(">Source:  {0}\r\n", e.InnerException.Source);
				stringBuilder.Append(value);
				stringBuilder.AppendFormat(">StackTrace:  {0}\r\n", e.InnerException.StackTrace);
				stringBuilder.Append(value);
			}
			stringBuilder.Append(value);
			stringBuilder.Append("Program:\r\n");
			stringBuilder.AppendFormat(">Version:  {0}\r\n", this.ProgrammVersionNummerAnzeige);
			stringBuilder.AppendFormat(">VersionInternal:  {0}\r\n", Application.ProductVersion);
			stringBuilder.AppendFormat(">Name:  {0}\r\n", Application.ProductName);
			stringBuilder.AppendFormat(">CommonAppDataPath:  {0}\r\n", Application.CommonAppDataPath);
			try
			{
				stringBuilder.AppendFormat(">CommonAppDataRegistry:  {0}\r\n", Application.CommonAppDataRegistry);
			}
			catch (Exception)
			{
			}
			stringBuilder.AppendFormat(">CurrentCulture:  {0}\r\n", Application.CurrentCulture);
			stringBuilder.AppendFormat(">CurrentInputLanguage:  {0}\r\n", Application.CurrentInputLanguage);
			stringBuilder.AppendFormat(">ExecutablePath:  {0}\r\n", Application.ExecutablePath);
			stringBuilder.AppendFormat(">LocalUserAppDataPath:  {0}\r\n", Application.LocalUserAppDataPath);
			stringBuilder.AppendFormat(">StartupPath:  {0}\r\n", Application.StartupPath);
			stringBuilder.AppendFormat(">UserAppDataPath:  {0}\r\n", Application.UserAppDataPath);
			stringBuilder.AppendFormat(">UserAppDataRegistry:  {0}\r\n", Application.UserAppDataRegistry);
			stringBuilder.Append(value);
			stringBuilder.Append("Debug-Log:\r\n");
			string value2 = "";
			try
			{
				if (Debugger.GlobalDebugger.ProtokollDateiname != null)
				{
					Debugger.GlobalDebugger.ProtokollDateiSchliessen();
					value2 = ToolboxStrings.File2String(Debugger.GlobalDebugger.ProtokollDateiname);
				}
			}
			catch (Exception)
			{
			}
			stringBuilder.Append(value2);
			stringBuilder.Append(value);
			if (additiveMeldung != null && additiveMeldung != "")
			{
				stringBuilder.Append(value);
				stringBuilder.Append(additiveMeldung);
				stringBuilder.Append(value);
			}
			this.UeberBrowserAnzeigen(stringBuilder.ToString(), supportEmailAdresse, supportWebScript);
			base.Show();
		}

		private void UeberBrowserAnzeigen(string meldung, string supportEmailAdresse, string supportWebScript)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("<html><head><title>error report</title>");
			stringBuilder.Append("<body>");
			stringBuilder.AppendFormat("<p STYLE=\"font-family: Arial; font-size: 13px\">{0}</p>", string.Format(ResReader.Reader.GetString("EinFehlerIstAufgetretenProgrammWirdBeendet"), supportEmailAdresse));
			stringBuilder.AppendFormat("<form METHOD=\"POST\" ACTION=\"{0}\">", supportWebScript);
			stringBuilder.AppendFormat("<P STYLE=\"font-family: Arial; font-size: 13px\"><INPUT TYPE=\"SUBMIT\" NAME=\"Submit1\" VALUE=\"{0}\"></P>", ResReader.Reader.GetString("FehlerMelden"));
			stringBuilder.AppendFormat("<P STYLE=\"font-family: Arial; font-size: 13px\">{0}:<br>", ResReader.Reader.GetString("WasHabenSieGetan"));
			stringBuilder.Append("<TEXTAREA NAME=\"wasgetan\" STYLE=\"width: 100%; height: 50px; font-family: Arial; font-size: 10px\"></TEXTAREA></p>");
			stringBuilder.AppendFormat("<P STYLE=\"font-family: Arial; font-size: 13px\">{0}:<br>", ResReader.Reader.GetString("DetailierteFehlermeldung"));
			stringBuilder.AppendFormat("<TEXTAREA NAME=\"fehlermeldung\" STYLE=\"width: 100%; height: 100px; font-family: Arial; font-size: 10px\">{0}</TEXTAREA></p>", meldung);
			stringBuilder.AppendFormat("<INPUT TYPE=\"HIDDEN\" NAME=\"betreff\" VALUE=\"Error Report {0}\">", Application.ProductName + " " + Application.ProductVersion);
			stringBuilder.AppendFormat("<INPUT TYPE=\"HIDDEN\" NAME=\"danke\" VALUE=\"{0}\">", ResReader.Reader.GetString("DankeFuerIhreFehlermeldung"));
			stringBuilder.Append("</form>");
			stringBuilder.Append("</body></html>");
			this.webBrowserSenden.DocumentText = stringBuilder.ToString();
		}

		private void frmFehlerReport_Load(object sender, EventArgs e)
		{
			this.Text = ResReader.Reader.GetString("AnwendungsFehler");
			this.webBrowserSenden.WebBrowserShortcutsEnabled = false;
			this.webBrowserSenden.ScriptErrorsSuppressed = false;
			this.webBrowserSenden.IsWebBrowserContextMenuEnabled = false;
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(frmFehlerReport));
			this.webBrowserSenden = new WebBrowser();
			base.SuspendLayout();
			this.webBrowserSenden.Dock = DockStyle.Fill;
			this.webBrowserSenden.Location = new Point(0, 0);
			this.webBrowserSenden.MinimumSize = new Size(20, 20);
			this.webBrowserSenden.Name = "webBrowserSenden";
			this.webBrowserSenden.ScrollBarsEnabled = false;
			this.webBrowserSenden.Size = new Size(592, 423);
			this.webBrowserSenden.TabIndex = 5;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = SystemColors.Control;
			base.ClientSize = new Size(592, 423);
			base.Controls.Add(this.webBrowserSenden);
			//base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "frmFehlerReport";
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "frmFehlerReport";
			base.TopMost = true;
			base.Load += this.frmFehlerReport_Load;
			base.ResumeLayout(false);
		}
	}
}
