using de.springwald.toolbox;
using de.springwald.xml.cursor;
using de.springwald.xml.dtd;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

namespace de.springwald.xml.editor
{
	public class ucXMLEditAttributes : UserControl
	{
		private IContainer components;

		private XMLEditor _xmlEditor;

		private Timer timerAttributListeAnzeigen;

		private ArrayList _attributEditFelder;

		private Label lblFehler;

		private List<DTDAttribut> _attribute;

		public XMLEditor XMLEditor
		{
			set
			{
				this._attributEditFelder = new ArrayList();
				this._attribute = null;
				this._xmlEditor = value;
				this._xmlEditor.CursorRoh.ChangedEvent += this.Cursor_ChangedEvent;
				this._xmlEditor.ContentChangedEvent += this._xmlEditor_ContentChangedEvent;
				this.VeraenderungAnmelden();
			}
		}

		private void InitializeComponent()
		{
			this.components = new Container();
			this.timerAttributListeAnzeigen = new Timer(this.components);
			this.lblFehler = new Label();
			base.SuspendLayout();
			this.timerAttributListeAnzeigen.Tick += this.timerAttributListeAnzeigen_Tick;
			this.lblFehler.AutoSize = true;
			this.lblFehler.ForeColor = Color.FromArgb(192, 0, 0);
			this.lblFehler.Location = new Point(3, 0);
			this.lblFehler.Name = "lblFehler";
			this.lblFehler.Size = new Size(46, 13);
			this.lblFehler.TabIndex = 0;
			this.lblFehler.Text = "lblFehler";
			base.Controls.Add(this.lblFehler);
			base.Name = "ucXMLEditAttributes";
			base.Load += this.ucXMLEditAttributes_Load;
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private void _xmlEditor_ContentChangedEvent(object sender, EventArgs e)
		{
			if (this._xmlEditor.RootNode == null)
			{
				this.AttributeAnzeigen();
				base.Enabled = false;
			}
			else
			{
				base.Enabled = true;
			}
		}

		public ucXMLEditAttributes()
		{
			this.InitializeComponent();
		}

		private void ucXMLEditAttributes_Load(object sender, EventArgs e)
		{
		}

		public void NeuZeichnen()
		{
			this.VeraenderungAnmelden();
		}

		private void VeraenderungAnmelden()
		{
			this.timerAttributListeAnzeigen.Enabled = false;
			this.timerAttributListeAnzeigen.Interval = 100;
			this.timerAttributListeAnzeigen.Enabled = true;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.components != null)
				{
					this.components.Dispose();
				}
				if (this._xmlEditor != null)
				{
					this._xmlEditor.CursorRoh.ChangedEvent -= this.Cursor_ChangedEvent;
				}
				this.DisposeAttribute();
			}
			base.Dispose(disposing);
		}

		private void DisposeAttribute()
		{
			this._attribute = null;
			if (this._attributEditFelder != null)
			{
				foreach (ucXMLEditAttribut item in this._attributEditFelder)
				{
					item.Dispose();
				}
				this._attributEditFelder.Clear();
			}
		}

		private void timerAttributListeAnzeigen_Tick(object sender, EventArgs e)
		{
			this.timerAttributListeAnzeigen.Enabled = false;
			this.AttributeAnzeigen();
		}

		private void Cursor_ChangedEvent(object sender, EventArgs e)
		{
			this.VeraenderungAnmelden();
		}

		public void AttributeAnzeigen()
		{
			this._attribute = null;
			this.lblFehler.Visible = false;
			if (this._xmlEditor != null && this._xmlEditor.RootNode != null && this._xmlEditor.CursorRoh.StartPos.Equals(this._xmlEditor.CursorRoh.EndPos))
			{
				int num = 5;
				int num2 = num;
				XmlNode xmlNode;
				switch (this._xmlEditor.CursorRoh.StartPos.PosAmNode)
				{
				case XMLCursorPositionen.CursorAufNodeSelbstVorderesTag:
				case XMLCursorPositionen.CursorAufNodeSelbstHinteresTag:
				case XMLCursorPositionen.CursorInDemLeeremNode:
					xmlNode = this._xmlEditor.CursorRoh.StartPos.AktNode;
					break;
				case XMLCursorPositionen.CursorInnerhalbDesTextNodes:
					xmlNode = this._xmlEditor.CursorRoh.StartPos.AktNode;
					break;
				case XMLCursorPositionen.CursorVorDemNode:
				case XMLCursorPositionen.CursorHinterDemNode:
					xmlNode = this._xmlEditor.CursorRoh.StartPos.AktNode.ParentNode;
					break;
				default:
					xmlNode = null;
					break;
				}
				while (xmlNode is XmlText)
				{
					xmlNode = xmlNode.ParentNode;
				}
				if (xmlNode != null)
				{
					DTDElement dTDElement = null;
					try
					{
						dTDElement = this._xmlEditor.Regelwerk.DTD.DTDElementByName(xmlNode.Name, true);
					}
					catch (DTD.XMLUnknownElementException ex)
					{
						Debugger.GlobalDebugger.Protokolliere(string.Format("unknown element {0} in {1}->{2}", ex.ElementName, base.Name, "AttributeNeuAnzeigen"));
						this.lblFehler.Text = string.Format("unknown element '{0}'", ex.ElementName);
						this.lblFehler.Visible = true;
					}
					if (dTDElement != null)
					{
						this._attribute = dTDElement.Attribute;
						if (this._attribute.Count > 0)
						{
							for (int i = 0; i < this._attribute.Count; i++)
							{
								ucXMLEditAttribut ucXMLEditAttribut;
								if (i < this._attributEditFelder.Count)
								{
									ucXMLEditAttribut = (ucXMLEditAttribut)this._attributEditFelder[i];
								}
								else
								{
									ucXMLEditAttribut = new ucXMLEditAttribut();
									ucXMLEditAttribut.Parent = this;
									ucXMLEditAttribut.Top = num2;
									ucXMLEditAttribut.Left = num;
									ucXMLEditAttribut.Width = 10;
									ucXMLEditAttribut.Visible = true;
									this._attributEditFelder.Add(ucXMLEditAttribut);
								}
								ucXMLEditAttribut.XMLEditor = this._xmlEditor;
								ucXMLEditAttribut.Node = xmlNode;
								ucXMLEditAttribut.Attribut = this._attribute[i];
								ucXMLEditAttribut.NeuZeichnen();
								num2 += ucXMLEditAttribut.Height;
								this.NichtMehrBenoetigteAttributEditFelderLoeschen();
								foreach (ucXMLEditAttribut item in this._attributEditFelder)
								{
									item.Width = base.ClientSize.Width - num * 2;
								}
							}
						}
					}
				}
			}
			this.NichtMehrBenoetigteAttributEditFelderLoeschen();
		}

		private void NichtMehrBenoetigteAttributEditFelderLoeschen()
		{
			if (this._attributEditFelder != null)
			{
				int num = (this._attribute != null) ? this._attribute.Count : 0;
				while (this._attributEditFelder.Count > num)
				{
					ucXMLEditAttribut ucXMLEditAttribut = (ucXMLEditAttribut)this._attributEditFelder[this._attributEditFelder.Count - 1];
					this._attributEditFelder.Remove(ucXMLEditAttribut);
					ucXMLEditAttribut.Dispose();
				}
			}
		}
	}
}
