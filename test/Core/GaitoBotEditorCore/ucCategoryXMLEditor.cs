using de.springwald.toolbox;
using de.springwald.xml;
using de.springwald.xml.editor;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace GaitoBotEditorCore
{
	public class ucCategoryXMLEditor : UserControl
	{
		private XMLRegelwerk _aimlRegelwerk;

		private XMLRegelwerk _startupRegelwerk;

		private IArbeitsbereichDatei _aktDatei;

		private Arbeitsbereich _arbeitsbereich;

		private AIMLCategory _category;

		private IArbeitsbereichDatei AktDatei
		{
			get
			{
				return this._aktDatei;
			}
			set
			{
				this._aktDatei = value;
				if (value != null)
				{
					this.XmlEditor.ReadOnly = this._aktDatei.ReadOnly;
					if (this.XmlEditor.RootNode != value.XML.DocumentElement)
					{
						this.XmlEditor.RootNode = value.XML.DocumentElement;
					}
					if (value is StartupDatei)
					{
						this.XmlEditor.Regelwerk = this._startupRegelwerk;
						return;
					}
					if (value is AIMLDatei)
					{
						this.XmlEditor.Regelwerk = this._aimlRegelwerk;
						return;
					}
					throw new ApplicationException("Unbehandelte Dateiart " + this.AktDatei.Dateiname);
				}
			}
		}

		private AIMLCategory AktCategory
		{
			get
			{
				return this._category;
			}
			set
			{
				if (this._arbeitsbereich.Fokus.AktDatei is AIMLDatei)
				{
					this._category = value;
					if (this._category == null)
					{
						if (this.XmlEditor.RootNode != null)
						{
							this.XmlEditor.RootNode = null;
						}
					}
					else if (this.XmlEditor.RootNode != this._category.ContentNode)
					{
						this.XmlEditor.RootNode = this._category.ContentNode;
					}
				}
			}
		}

		public Arbeitsbereich Arbeitsbereich
		{
			set
			{
				if (this._arbeitsbereich != null)
				{
					throw new ApplicationException("Diesem ucCategoryXMLEditor wurde bereits ein Arbeitsbereich zugeordnet");
				}
				this._arbeitsbereich = value;
				this._arbeitsbereich.Fokus.XmlEditor = this.XmlEditor;
				this._arbeitsbereich.Fokus.AktAIMLCategoryChanged += this.Fokus_AktAIMLCategoryChanged;
				this._arbeitsbereich.Fokus.AktDateiChanged += this.Fokus_AktDateiChanged;
			}
		}

		public XMLEditor XmlEditor
		{
			get;
			private set;
		}

		private void InitializeComponent()
		{
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ucCategoryXMLEditor));
			base.SuspendLayout();
			componentResourceManager.ApplyResources(this, "$this");
			this.BackColor = SystemColors.Control;
			this.ForeColor = SystemColors.Control;
			base.Name = "ucCategoryXMLEditor";
			base.ResumeLayout(false);
		}

		public ucCategoryXMLEditor()
		{
			this.InitializeComponent();
			this._aimlRegelwerk = new AimlXmlRegelwerk(AIMLDTD.GetAIMLDTD());
			this._startupRegelwerk = new StartupDatei_XmlRegelwerk(StartupDateiDtd.GetStartupDtd());
			this.XmlEditor = new XMLEditor(this._aimlRegelwerk, this);
			base.SetStyle(ControlStyles.UserPaint, true);
			base.SetStyle(ControlStyles.DoubleBuffer, true);
			base.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			base.SetStyle(ControlStyles.ResizeRedraw, true);
			base.UpdateStyles();
		}

		private void Fokus_AktAIMLCategoryChanged(object sender, EventArgs<AIMLCategory> e)
		{
			this.AktCategory = e.Value;
		}

		private void Fokus_AktDateiChanged(object sender, EventArgs<IArbeitsbereichDatei> e)
		{
			this.AktDatei = e.Value;
		}

		protected override bool IsInputKey(Keys keyData)
		{
			bool result = true;
			switch (keyData)
			{
			default:
				result = base.IsInputKey(keyData);
				break;
			case Keys.Left:
			case Keys.Up:
			case Keys.Right:
			case Keys.Down:
				break;
			}
			return result;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			this.XmlEditor.Paint(e);
		}
	}
}
