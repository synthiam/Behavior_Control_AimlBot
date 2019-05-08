using de.springwald.toolbox;
using de.springwald.xml.editor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

namespace de.springwald.xml
{
	public class ucXMLAddElementGruppe : UserControl
	{
		private bool _buttonWirdGeradeGeklickt = false;

		private XMLEditor _xmlEditor;

		private ArrayList _buttons;

		private string[] _elemente;

		private XMLElementGruppe _gruppe;

		private IContainer components = null;

		private KlappbareGroupBox klappbareGroupBox;

		public ucXMLAddElementGruppe(XMLElementGruppe gruppe, XMLEditor xmlEditor)
		{
			this._xmlEditor = xmlEditor;
			this._gruppe = gruppe;
			this.InitializeComponent();
		}

		private void ucXMLAddElementGruppe_Load(object sender, EventArgs e)
		{
			if (this._gruppe == null)
			{
				this.klappbareGroupBox.Visible = false;
			}
			else
			{
				this.klappbareGroupBox.Visible = true;
				this.klappbareGroupBox.Text = this._gruppe.Titel;
				this.klappbareGroupBox.IsCollapsed = this._gruppe.StandardMaessigZusammengeklappt;
				this.klappbareGroupBox.CollapseBoxClickedEvent += this.klappbareGroupBox_CollapseBoxClickedEvent;
			}
			this._buttons = new ArrayList();
			base.Resize += this.ucXMLAddElementGruppe_Resize;
		}

		private void klappbareGroupBox_CollapseBoxClickedEvent(object sender)
		{
			this.ButtonsAnzeigen();
		}

		public string[] VerfuegbareElementeZuweisenUndRestElementeZurueckGeben(string[] elemente)
		{
			List<string> list = new List<string>();
			List<string> list2 = new List<string>();
			foreach (string text in elemente)
			{
				if (this._gruppe == null)
				{
					list2.Add(text);
				}
				else if (this._gruppe.ContainsElement(text))
				{
					list2.Add(text);
				}
				else
				{
					list.Add(text);
				}
			}
			this._elemente = list2.ToArray();
			this.ButtonsAnzeigen();
			if (this._gruppe != null)
			{
				string text2 = string.Format("{0} ({1})", this._gruppe.Titel, this._elemente.Length);
				if (this.klappbareGroupBox.Text != text2)
				{
					this.klappbareGroupBox.Text = text2;
				}
			}
			return list.ToArray();
		}

		private void ButtonsAnzeigen()
		{
			if (this._gruppe != null && this.klappbareGroupBox.IsCollapsed)
			{
				this.NichtMehrBenoetigteButtonsLoeschen();
				base.Height = this.klappbareGroupBox.Top + this.klappbareGroupBox.Height;
			}
			else if (this._elemente == null)
			{
				this.NichtMehrBenoetigteButtonsLoeschen();
			}
			else
			{
				int num = 5;
				int num2 = 0;
				num2 = ((this._gruppe != null) ? 20 : num);
				for (int i = 0; i < this._elemente.Length; i++)
				{
					Button button;
					if (i < this._buttons.Count)
					{
						button = (Button)this._buttons[i];
					}
					else
					{
						button = new Button();
						button.Top = num2;
						button.Left = num;
						button.Visible = true;
						button.Click += this.Button_Click;
						this._buttons.Add(button);
						if (this._gruppe == null)
						{
							button.Parent = this;
						}
						else
						{
							button.Parent = this.klappbareGroupBox;
						}
					}
					int num3 = 0;
					Size clientSize;
					if (this._gruppe == null)
					{
						clientSize = base.ClientSize;
						num3 = clientSize.Width - num * 2;
					}
					else
					{
						clientSize = base.ClientSize;
						num3 = clientSize.Width - num * 3;
					}
					if (button.Width != num3)
					{
						button.Width = num3;
					}
					if (button.Text != this._elemente[i])
					{
						button.Text = this._elemente[i];
					}
					num2 += button.Height + num;
				}
				if (this._gruppe == null)
				{
					if (base.Height != num2)
					{
						base.Height = num2;
					}
				}
				else
				{
					if (this.klappbareGroupBox.Height != num2 + num + num)
					{
						this.klappbareGroupBox.Height = num2 + num + num;
					}
					if (base.Height != this.klappbareGroupBox.Top + this.klappbareGroupBox.Height)
					{
						base.Height = this.klappbareGroupBox.Top + this.klappbareGroupBox.Height;
					}
				}
				this.NichtMehrBenoetigteButtonsLoeschen();
			}
			bool flag = this._elemente != null && this._elemente.Length != 0;
			if (base.Visible != flag)
			{
				base.Visible = flag;
			}
		}

		private void ucXMLAddElementGruppe_Resize(object sender, EventArgs e)
		{
			if (this.klappbareGroupBox.Top != 0)
			{
				this.klappbareGroupBox.Top = 0;
			}
			if (this.klappbareGroupBox.Left != 0)
			{
				this.klappbareGroupBox.Left = 0;
			}
			int width = this.klappbareGroupBox.Width;
			Size clientSize = base.ClientSize;
			if (width != clientSize.Width)
			{
				KlappbareGroupBox obj = this.klappbareGroupBox;
				clientSize = base.ClientSize;
				obj.Width = clientSize.Width;
			}
			this.ButtonsAnzeigen();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.components != null)
				{
					this.components.Dispose();
				}
				this.DisposeButtons_();
			}
			base.Dispose(disposing);
		}

		private void DisposeButtons_()
		{
			if (this._buttons != null)
			{
				foreach (Button button in this._buttons)
				{
					if (button != null)
					{
						button.Click -= this.Button_Click;
						button.Dispose();
					}
				}
				this._buttons = null;
			}
		}

		private void Button_Click(object sender, EventArgs e)
		{
			if (!this._buttonWirdGeradeGeklickt)
			{
				this._buttonWirdGeradeGeklickt = true;
				if (this._buttons != null)
				{
					string text = null;
					for (int i = 0; i < this._buttons.Count; i++)
					{
						if (this._buttons[i] == sender)
						{
							text = this._elemente[i];
						}
					}
					if (text != null)
					{
						XmlNode xmlNode = this._xmlEditor.AktionNeuesElementAnAktCursorPosEinfuegen(text, XMLEditor.UndoSnapshotSetzenOptionen.ja, false);
						if (xmlNode != null)
						{
							goto IL_008f;
						}
					}
				}
				goto IL_008f;
			}
			return;
			IL_008f:
			this._xmlEditor.FokusAufEingabeFormularSetzen();
			this._buttonWirdGeradeGeklickt = false;
		}

		private void NichtMehrBenoetigteButtonsLoeschen()
		{
			if (this._buttons != null)
			{
				int num = (this._xmlEditor != null && this._xmlEditor.RootNode != null && this._elemente != null) ? ((this._gruppe == null || !this.klappbareGroupBox.IsCollapsed) ? this._elemente.Length : 0) : 0;
				while (this._buttons.Count > num)
				{
					Button button = (Button)this._buttons[this._buttons.Count - 1];
					this._buttons.Remove(button);
					button.Click -= this.Button_Click;
					button.Dispose();
				}
			}
		}

		private void klappbareGroupBox_Enter(object sender, EventArgs e)
		{
		}

		private void InitializeComponent()
		{
			this.klappbareGroupBox = new KlappbareGroupBox();
			base.SuspendLayout();
			this.klappbareGroupBox.BorderColor = Color.Black;
			this.klappbareGroupBox.Location = new Point(0, 0);
			this.klappbareGroupBox.Margin = new Padding(0);
			this.klappbareGroupBox.MinimumSize = new Size(0, 25);
			this.klappbareGroupBox.Name = "klappbareGroupBox";
			this.klappbareGroupBox.Padding = new Padding(0);
			this.klappbareGroupBox.Size = new Size(134, 139);
			this.klappbareGroupBox.TabIndex = 0;
			this.klappbareGroupBox.TabStop = false;
			this.klappbareGroupBox.Text = "klappbareGroupBox";
			this.klappbareGroupBox.Enter += this.klappbareGroupBox_Enter;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.BackColor = SystemColors.Control;
			base.Controls.Add(this.klappbareGroupBox);
			base.Margin = new Padding(0);
			base.Name = "ucXMLAddElementGruppe";
			base.Size = new Size(140, 170);
			base.Load += this.ucXMLAddElementGruppe_Load;
			base.ResumeLayout(false);
		}
	}
}
