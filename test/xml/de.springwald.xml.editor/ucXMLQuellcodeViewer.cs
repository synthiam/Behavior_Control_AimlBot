using System;
using System.Drawing;
using System.Windows.Forms;

namespace de.springwald.xml.editor
{
	public class ucXMLQuellcodeViewer : UserControl
	{
		private RichTextBox txtQuellcode;

		private string _quellcodeAlsRTF;

		public string XMLCodeAlsRTF
		{
			set
			{
				this._quellcodeAlsRTF = value;
				this.Zeichnen();
			}
		}

		private void InitializeComponent()
		{
			this.txtQuellcode = new RichTextBox();
			base.SuspendLayout();
			this.txtQuellcode.BackColor = Color.White;
			this.txtQuellcode.BorderStyle = BorderStyle.None;
			this.txtQuellcode.DetectUrls = false;
			this.txtQuellcode.Location = new Point(8, 8);
			this.txtQuellcode.Name = "txtQuellcode";
			this.txtQuellcode.ReadOnly = true;
			this.txtQuellcode.Size = new Size(600, 392);
			this.txtQuellcode.TabIndex = 0;
			this.txtQuellcode.Text = "txtQuellcode";
			this.BackColor = SystemColors.Control;
			base.Controls.Add(this.txtQuellcode);
			base.Name = "ucXMLQuellcodeViewer";
			base.Size = new Size(624, 416);
			base.Load += this.ucXMLQuellcodeViewer_Load;
			base.ResumeLayout(false);
		}

		public ucXMLQuellcodeViewer()
		{
			this.InitializeComponent();
			this.txtQuellcode.Text = "";
			base.Resize += this.ucXMLQuellcodeViewer_Resize;
		}

		private void ucXMLQuellcodeViewer_Load(object sender, EventArgs e)
		{
			this.BackColor = this.txtQuellcode.BackColor;
			this.Zeichnen();
		}

		private void ucXMLQuellcodeViewer_Resize(object sender, EventArgs e)
		{
			int num = 5;
			this.txtQuellcode.Left = num;
			this.txtQuellcode.Top = num;
			RichTextBox richTextBox = this.txtQuellcode;
			Size clientSize = base.ClientSize;
			richTextBox.Width = clientSize.Width - 2 * num;
			RichTextBox richTextBox2 = this.txtQuellcode;
			clientSize = base.ClientSize;
			richTextBox2.Height = clientSize.Height - 2 * num;
			this.Zeichnen();
		}

		private void Zeichnen()
		{
			if (!(this._quellcodeAlsRTF == ""))
			{
				this.txtQuellcode.Rtf = this._quellcodeAlsRTF;
			}
		}
	}
}
