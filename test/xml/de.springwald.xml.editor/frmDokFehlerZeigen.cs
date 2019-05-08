using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace de.springwald.xml.editor
{
	public class frmDokFehlerZeigen : Form
	{
		private Container components = null;

		private ucXMLQuellcodeViewer ucXMLQuellcodeViewer;

		private Splitter splitter1;

		private ucXMLQuellCodeFehlerliste ucXMLQuellCodeFehlerliste;

		public frmDokFehlerZeigen()
		{
			this.InitializeComponent();
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(frmDokFehlerZeigen));
			this.ucXMLQuellCodeFehlerliste = new ucXMLQuellCodeFehlerliste();
			this.ucXMLQuellcodeViewer = new ucXMLQuellcodeViewer();
			this.splitter1 = new Splitter();
			base.SuspendLayout();
			this.ucXMLQuellCodeFehlerliste.Dock = DockStyle.Left;
			this.ucXMLQuellCodeFehlerliste.Location = new Point(0, 0);
			this.ucXMLQuellCodeFehlerliste.Name = "ucXMLQuellCodeFehlerliste";
			this.ucXMLQuellCodeFehlerliste.Size = new Size(352, 357);
			this.ucXMLQuellCodeFehlerliste.TabIndex = 0;
			this.ucXMLQuellcodeViewer.BackColor = SystemColors.Control;
			this.ucXMLQuellcodeViewer.Dock = DockStyle.Fill;
			this.ucXMLQuellcodeViewer.Location = new Point(352, 0);
			this.ucXMLQuellcodeViewer.Name = "ucXMLQuellcodeViewer";
			this.ucXMLQuellcodeViewer.Size = new Size(240, 357);
			this.ucXMLQuellcodeViewer.TabIndex = 1;
			this.splitter1.Location = new Point(352, 0);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new Size(3, 357);
			this.splitter1.TabIndex = 2;
			this.splitter1.TabStop = false;
			this.AutoScaleBaseSize = new Size(5, 13);
			base.ClientSize = new Size(592, 357);
			base.Controls.Add(this.splitter1);
			base.Controls.Add(this.ucXMLQuellcodeViewer);
			base.Controls.Add(this.ucXMLQuellCodeFehlerliste);
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.Name = "frmDokFehlerZeigen";
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "frmDokFehlerZeigen";
			base.WindowState = FormWindowState.Maximized;
			base.Load += this.frmDokFehlerZeigen_Load;
			base.ResumeLayout(false);
		}

		private void frmDokFehlerZeigen_Load(object sender, EventArgs e)
		{
			this.Text = ResReader.Reader.GetString("QuellcodeFehleranzeige");
		}

		public void Anzeigen(string xMLQuellCodeAlsRTF, string fehlerProtokollAlsText)
		{
			this.ucXMLQuellcodeViewer.XMLCodeAlsRTF = xMLQuellCodeAlsRTF;
			this.ucXMLQuellCodeFehlerliste.FehlerProtokollAlsText = fehlerProtokollAlsText;
		}
	}
}
