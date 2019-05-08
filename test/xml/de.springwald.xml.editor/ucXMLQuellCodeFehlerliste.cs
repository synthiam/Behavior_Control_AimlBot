using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace de.springwald.xml.editor
{
	public class ucXMLQuellCodeFehlerliste : UserControl
	{
		private Container components = null;

		private TextBox txtFehlerProtokoll;

		private string _fehlerProtokollAlsText;

		public string FehlerProtokollAlsText
		{
			set
			{
				this._fehlerProtokollAlsText = value;
				this.Zeichnen();
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
			this.txtFehlerProtokoll = new TextBox();
			base.SuspendLayout();
			this.txtFehlerProtokoll.Location = new Point(8, 16);
			this.txtFehlerProtokoll.Multiline = true;
			this.txtFehlerProtokoll.Name = "txtFehlerProtokoll";
			this.txtFehlerProtokoll.ScrollBars = ScrollBars.Both;
			this.txtFehlerProtokoll.Size = new Size(304, 216);
			this.txtFehlerProtokoll.TabIndex = 0;
			this.txtFehlerProtokoll.Text = "txtFehlerProtokoll";
			base.Controls.Add(this.txtFehlerProtokoll);
			base.Name = "ucXMLQuellCodeFehlerliste";
			base.Size = new Size(392, 264);
			base.Load += this.ucXMLQuellCodeFehlerliste_Load;
			base.ResumeLayout(false);
		}

		public ucXMLQuellCodeFehlerliste()
		{
			this.InitializeComponent();
			base.Resize += this.ucXMLQuellCodeFehlerliste_Resize;
		}

		private void Zeichnen()
		{
			this.txtFehlerProtokoll.Text = this._fehlerProtokollAlsText;
			this.txtFehlerProtokoll.Select(0, 0);
		}

		private void ucXMLQuellCodeFehlerliste_Load(object sender, EventArgs e)
		{
		}

		private void ucXMLQuellCodeFehlerliste_Resize(object sender, EventArgs e)
		{
			this.txtFehlerProtokoll.Left = 0;
			this.txtFehlerProtokoll.Top = 0;
			TextBox textBox = this.txtFehlerProtokoll;
			Size clientSize = base.ClientSize;
			textBox.Width = clientSize.Width;
			TextBox textBox2 = this.txtFehlerProtokoll;
			clientSize = base.ClientSize;
			textBox2.Height = clientSize.Height;
		}
	}
}
