using de.springwald.toolbox;
using de.springwald.xml.dtd;
using de.springwald.xml.editor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace de.springwald.xml
{
	public class ucXMLAddElement2 : UserControl
	{
		private XMLEditor _xmlEditor;

		private Timer timerButtonListeAnzeigen;

		private Label lblFehler;

		private bool _nichtAufResizeReagieren = false;

		private List<ucXMLAddElementGruppe> _gruppenControls;

		private IContainer components;

		public XMLEditor XMLEditor
		{
			set
			{
				if (this._xmlEditor != null)
				{
					throw new ApplicationException("There is already a XMLEditor object attached!");
				}
				this._xmlEditor = value;
				this._xmlEditor.ContentChangedEvent += this._xmlEditor_ContentChangedEvent;
				this._xmlEditor.CursorRoh.ChangedEvent += this.Cursor_ChangedEvent;
				this.VeraenderungAnmelden_();
				this._gruppenControls = new List<ucXMLAddElementGruppe>();
				for (int i = 0; i < this._xmlEditor.Regelwerk.ElementGruppen.Count; i++)
				{
					XMLElementGruppe gruppe = this._xmlEditor.Regelwerk.ElementGruppen[i];
					ucXMLAddElementGruppe ucXMLAddElementGruppe = new ucXMLAddElementGruppe(gruppe, this._xmlEditor);
					ucXMLAddElementGruppe.Parent = this;
					ucXMLAddElementGruppe.Left = 0;
					ucXMLAddElementGruppe.Top = 50 * i;
					ucXMLAddElementGruppe.Visible = true;
					ucXMLAddElementGruppe.Resize += this.gruppenControl_Resize;
					this._gruppenControls.Add(ucXMLAddElementGruppe);
				}
				ucXMLAddElementGruppe ucXMLAddElementGruppe2 = new ucXMLAddElementGruppe(null, this._xmlEditor);
				ucXMLAddElementGruppe2.Parent = this;
				ucXMLAddElementGruppe2.Left = 0;
				ucXMLAddElementGruppe2.Top = 0;
				ucXMLAddElementGruppe2.Visible = true;
				ucXMLAddElementGruppe2.Resize += this.gruppenControl_Resize;
				this._gruppenControls.Add(ucXMLAddElementGruppe2);
			}
		}

		private void _xmlEditor_ContentChangedEvent(object sender, EventArgs e)
		{
			this.ButtonsNeuAnzeigen();
			if (this._xmlEditor.RootNode == null)
			{
				if (base.Enabled)
				{
					base.Enabled = false;
				}
			}
			else if (!base.Enabled)
			{
				base.Enabled = true;
			}
		}

		public ucXMLAddElement2()
		{
			this.InitializeComponent();
			base.Resize += this.ucXMLAddElement_Resize;
		}

		private void ucXMLAddElement_Load(object sender, EventArgs e)
		{
		}

		private void ucXMLAddElement_Resize(object sender, EventArgs e)
		{
			if (!this._nichtAufResizeReagieren)
			{
				this.VeraenderungAnmelden_();
			}
		}

		private void gruppenControl_Resize(object sender, EventArgs e)
		{
			if (!this._nichtAufResizeReagieren)
			{
				this.VeraenderungAnmelden_();
			}
		}

		private void VeraenderungAnmelden_()
		{
			this.timerButtonListeAnzeigen.Enabled = false;
			this.timerButtonListeAnzeigen.Interval = 100;
			this.timerButtonListeAnzeigen.Enabled = true;
		}

		private void Cursor_ChangedEvent(object sender, EventArgs e)
		{
			this.VeraenderungAnmelden_();
		}

		private void timerButtonListeAnzeigen_Tick(object sender, EventArgs e)
		{
			this.timerButtonListeAnzeigen.Enabled = false;
			this.ButtonsNeuAnzeigen();
		}

		private void ButtonsNeuAnzeigen()
		{
			this.AutoScroll = true;
			if (this.lblFehler.Visible)
			{
				this.lblFehler.Visible = false;
			}
			this._nichtAufResizeReagieren = true;
			if (this._xmlEditor != null && this._xmlEditor.RootNode != null)
			{
				string[] array = null;
				try
				{
					bool kommentareMitAuflisten = false;
					array = this._xmlEditor.Regelwerk.ErlaubteEinfuegeElemente_(this._xmlEditor.CursorOptimiert.StartPos, false, kommentareMitAuflisten);
				}
				catch (DTD.XMLUnknownElementException ex)
				{
					Debugger.GlobalDebugger.Protokolliere(string.Format("unknown element {0} in {1}->{2}", ex.ElementName, base.Name, "Aktualisieren"));
					this.lblFehler.Text = string.Format("unknown element '{0}'", ex.ElementName);
					this.lblFehler.Visible = true;
				}
				if (array != null)
				{
					string[] elemente = (from e in array
					orderby e
					select e).ToArray();
					int num = 3;
					int num2 = 0;
					for (int i = 0; i < this._gruppenControls.Count; i++)
					{
						elemente = this._gruppenControls[i].VerfuegbareElementeZuweisenUndRestElementeZurueckGeben(elemente);
						if (this._gruppenControls[i].Visible)
						{
							this._gruppenControls[i].Top = num2;
							this._gruppenControls[i].Left = num;
							int width = base.ClientSize.Width - num * 2;
							this._gruppenControls[i].Width = width;
							num2 += this._gruppenControls[i].Height;
						}
					}
				}
			}
			this._nichtAufResizeReagieren = false;
		}

		protected void InitializeComponent()
		{
			this.components = new Container();
			this.timerButtonListeAnzeigen = new Timer(this.components);
			this.lblFehler = new Label();
			base.SuspendLayout();
			this.timerButtonListeAnzeigen.Tick += this.timerButtonListeAnzeigen_Tick;
			this.lblFehler.AutoSize = true;
			this.lblFehler.ForeColor = Color.FromArgb(192, 0, 0);
			this.lblFehler.Location = new Point(3, 0);
			this.lblFehler.Name = "lblFehler";
			this.lblFehler.Size = new Size(46, 13);
			this.lblFehler.TabIndex = 0;
			this.lblFehler.Text = "lblFehler";
			base.Controls.Add(this.lblFehler);
			this.DoubleBuffered = true;
			base.Margin = new Padding(0);
			base.Name = "ucXMLAddElement";
			base.Size = new Size(361, 348);
			base.Load += this.ucXMLAddElement_Load;
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
