using de.springwald.xml.dtd;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

namespace de.springwald.xml.editor
{
	public class ucXMLEditAttribut : UserControl
	{
		private TextBox txtEingabe;

		private ComboBox comboAuswahl;

		private Label lblTitel;

		private Container components = null;

		private XMLEditor _xmlEditor;

		private XmlNode _node;

		private DTDAttribut _attribut;

		private string XMLInhaltInNode
		{
			get
			{
				XmlAttribute xmlAttribute = this._node.Attributes[this._attribut.Name];
				if (xmlAttribute == null)
				{
					return "";
				}
				return xmlAttribute.Value;
			}
		}

		public XMLEditor XMLEditor
		{
			set
			{
				this._xmlEditor = value;
			}
		}

		public XmlNode Node
		{
			set
			{
				this._node = value;
			}
		}

		public DTDAttribut Attribut
		{
			get
			{
				return this._attribut;
			}
			set
			{
				this._attribut = value;
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
			this.txtEingabe = new TextBox();
			this.comboAuswahl = new ComboBox();
			this.lblTitel = new Label();
			base.SuspendLayout();
			this.txtEingabe.Location = new Point(88, 16);
			this.txtEingabe.Name = "txtEingabe";
			this.txtEingabe.TabIndex = 0;
			this.txtEingabe.Text = "txtEingabe";
			this.txtEingabe.TextChanged += this.txtEingabe_TextChanged;
			this.comboAuswahl.Location = new Point(0, 16);
			this.comboAuswahl.Name = "comboAuswahl";
			this.comboAuswahl.Size = new Size(88, 21);
			this.comboAuswahl.TabIndex = 1;
			this.comboAuswahl.Text = "comboAuswahl";
			this.comboAuswahl.SelectedIndexChanged += this.comboAuswahl_SelectedIndexChanged;
			this.lblTitel.Location = new Point(0, 0);
			this.lblTitel.Name = "lblTitel";
			this.lblTitel.Size = new Size(184, 16);
			this.lblTitel.TabIndex = 2;
			this.lblTitel.Text = "lblTitel";
			base.Controls.Add(this.lblTitel);
			base.Controls.Add(this.comboAuswahl);
			base.Controls.Add(this.txtEingabe);
			base.Name = "ucXMLEditAttribut";
			base.Size = new Size(192, 40);
			base.ResumeLayout(false);
		}

		public ucXMLEditAttribut()
		{
			this.InitializeComponent();
			base.Resize += this.ucXMLEditAttribut_Resize;
		}

		public void NeuZeichnen()
		{
			if (this._attribut == null)
			{
				throw new ApplicationException("Es wurde noch kein Attribut zugewiesen!");
			}
			if (this._node == null)
			{
				throw new ApplicationException("Es wurde noch kein Node zugewiesen!");
			}
			this.lblTitel.Text = this._attribut.Name;
			if (this._attribut.ErlaubteWerte.Count > 0)
			{
				this.comboAuswahl.Visible = true;
				this.txtEingabe.Visible = false;
				this.comboAuswahl.Items.Clear();
				if (this._attribut.Pflicht == DTDAttribut.PflichtArten.Optional)
				{
					this.comboAuswahl.Items.Add("");
				}
				StringEnumerator enumerator = this._attribut.ErlaubteWerte.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						string current = enumerator.Current;
						this.comboAuswahl.Items.Add(current);
					}
				}
				finally
				{
					IDisposable disposable = enumerator as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}
				}
				this.comboAuswahl.Text = this.XMLInhaltInNode;
			}
			else
			{
				this.comboAuswahl.Visible = false;
				this.txtEingabe.Visible = true;
				this.txtEingabe.Text = this.XMLInhaltInNode;
			}
		}

		private void comboAuswahl_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this._attribut == null)
			{
				throw new ApplicationException("Es wurde noch kein Attribut zugewiesen!");
			}
			if (this._node == null)
			{
				throw new ApplicationException("Es wurde noch kein Node zugewiesen!");
			}
			this._xmlEditor.AktionAttributWertInNodeSetzen(this._node, this._attribut.Name, this.comboAuswahl.Text, XMLEditor.UndoSnapshotSetzenOptionen.ja);
		}

		private void txtEingabe_TextChanged(object sender, EventArgs e)
		{
			if (this._attribut == null)
			{
				throw new ApplicationException("Es wurde noch kein Attribut zugewiesen!");
			}
			if (this._node == null)
			{
				throw new ApplicationException("Es wurde noch kein Node zugewiesen!");
			}
			this._xmlEditor.AktionAttributWertInNodeSetzen(this._node, this._attribut.Name, this.txtEingabe.Text, XMLEditor.UndoSnapshotSetzenOptionen.ja);
		}

		private void ucXMLEditAttribut_Resize(object sender, EventArgs e)
		{
			this.lblTitel.Top = 0;
			this.lblTitel.Left = 0;
			Label label = this.lblTitel;
			Size clientSize = base.ClientSize;
			label.Width = clientSize.Width;
			this.txtEingabe.Left = 0;
			this.txtEingabe.Top = this.lblTitel.Top + this.lblTitel.Height;
			TextBox textBox = this.txtEingabe;
			clientSize = base.ClientSize;
			textBox.Width = clientSize.Width;
			this.comboAuswahl.Left = 0;
			this.comboAuswahl.Top = this.lblTitel.Top + this.lblTitel.Height;
			ComboBox comboBox = this.comboAuswahl;
			clientSize = base.ClientSize;
			comboBox.Width = clientSize.Width;
		}
	}
}
