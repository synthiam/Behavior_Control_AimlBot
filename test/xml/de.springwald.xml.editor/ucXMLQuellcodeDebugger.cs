using de.springwald.xml.Properties;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace de.springwald.xml.editor
{
	public class ucXMLQuellcodeDebugger : UserControl
	{
		private ucXMLQuellcodeViewer ucXMLQuellcodeViewer;

		private ucXMLQuellCodeFehlerliste ucXMLQuellCodeFehlerliste;

		private ToolStrip toolStripTop;

		private ToolStripButton toolStripButtonAktualisieren;

		private IContainer components;

		private XMLEditor _xmlEditor;

		public XMLEditor XMLEditor
		{
			set
			{
				this._xmlEditor = value;
				this._xmlEditor.ContentChangedEvent += this._xmlEditor_ContentChangedEvent;
				this.Veraendert();
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
			this.toolStripTop = new ToolStrip();
			this.toolStripButtonAktualisieren = new ToolStripButton();
			this.ucXMLQuellCodeFehlerliste = new ucXMLQuellCodeFehlerliste();
			this.ucXMLQuellcodeViewer = new ucXMLQuellcodeViewer();
			this.toolStripTop.SuspendLayout();
			base.SuspendLayout();
			this.toolStripTop.Items.AddRange(new ToolStripItem[1]
			{
				this.toolStripButtonAktualisieren
			});
			this.toolStripTop.Location = new Point(0, 0);
			this.toolStripTop.Name = "toolStripTop";
			this.toolStripTop.Size = new Size(528, 25);
			this.toolStripTop.TabIndex = 2;
			this.toolStripTop.Text = "toolStrip1";
			this.toolStripButtonAktualisieren.Image = Resources.sync;
			this.toolStripButtonAktualisieren.ImageTransparentColor = Color.Magenta;
			this.toolStripButtonAktualisieren.Name = "toolStripButtonAktualisieren";
			this.toolStripButtonAktualisieren.Size = new Size(160, 22);
			this.toolStripButtonAktualisieren.Text = "toolStripButtonAktualisieren";
			this.toolStripButtonAktualisieren.Click += this.toolStripButtonAktualisieren_Click;
			this.ucXMLQuellCodeFehlerliste.Location = new Point(328, 64);
			this.ucXMLQuellCodeFehlerliste.Name = "ucXMLQuellCodeFehlerliste";
			this.ucXMLQuellCodeFehlerliste.Size = new Size(200, 256);
			this.ucXMLQuellCodeFehlerliste.TabIndex = 1;
			this.ucXMLQuellcodeViewer.BackColor = Color.White;
			this.ucXMLQuellcodeViewer.Location = new Point(0, 85);
			this.ucXMLQuellcodeViewer.Name = "ucXMLQuellcodeViewer";
			this.ucXMLQuellcodeViewer.Size = new Size(254, 235);
			this.ucXMLQuellcodeViewer.TabIndex = 0;
			base.Controls.Add(this.toolStripTop);
			base.Controls.Add(this.ucXMLQuellCodeFehlerliste);
			base.Controls.Add(this.ucXMLQuellcodeViewer);
			base.Name = "ucXMLQuellcodeDebugger";
			base.Size = new Size(528, 320);
			base.Resize += this.ucXMLQuellcodeDebugger_Resize;
			this.toolStripTop.ResumeLayout(false);
			this.toolStripTop.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		public ucXMLQuellcodeDebugger()
		{
			this.InitializeComponent();
			this.toolStripButtonAktualisieren.Text = ResReader.Reader.GetString("toolStripButtonAktualisieren");
		}

		private void Veraendert()
		{
			this.ucXMLQuellCodeFehlerliste.Visible = false;
			this.ucXMLQuellcodeViewer.Visible = false;
		}

		private void ucXMLQuellcodeDebugger_Resize(object sender, EventArgs e)
		{
			this.ucXMLQuellcodeViewer.Top = this.toolStripTop.Top + this.toolStripTop.Height;
			this.ucXMLQuellcodeViewer.Left = 0;
			this.ucXMLQuellcodeViewer.Width = (int)((double)base.Width * 0.6);
			this.ucXMLQuellcodeViewer.Height = base.Height - this.ucXMLQuellcodeViewer.Top;
			this.ucXMLQuellCodeFehlerliste.Top = this.toolStripTop.Top + this.toolStripTop.Height;
			this.ucXMLQuellCodeFehlerliste.Left = this.ucXMLQuellcodeViewer.Left + this.ucXMLQuellcodeViewer.Width;
			this.ucXMLQuellCodeFehlerliste.Width = base.Width - this.ucXMLQuellCodeFehlerliste.Left;
			this.ucXMLQuellCodeFehlerliste.Height = base.Height - this.ucXMLQuellCodeFehlerliste.Top;
		}

		private void _xmlEditor_ContentChangedEvent(object sender, EventArgs e)
		{
			this.Veraendert();
		}

		private void toolStripButtonAktualisieren_Click(object sender, EventArgs e)
		{
			this.ZeichnenIntern();
		}

		private void ZeichnenIntern()
		{
			if (this._xmlEditor != null && this._xmlEditor.ReadOnly)
			{
				base.Visible = false;
			}
			else
			{
				base.Visible = true;
				XMLQuellcodeAlsRTF xMLQuellcodeAlsRTF = new XMLQuellcodeAlsRTF();
				xMLQuellcodeAlsRTF.Regelwerk = this._xmlEditor.Regelwerk;
				xMLQuellcodeAlsRTF.Rootnode = this._xmlEditor.RootNode;
				this.ucXMLQuellcodeViewer.XMLCodeAlsRTF = xMLQuellcodeAlsRTF.QuellCodeAlsRTF;
				this.ucXMLQuellCodeFehlerliste.FehlerProtokollAlsText = xMLQuellcodeAlsRTF.FehlerProtokollAlsText;
				this.ucXMLQuellCodeFehlerliste.Visible = true;
				this.ucXMLQuellcodeViewer.Visible = true;
			}
		}
	}
}
