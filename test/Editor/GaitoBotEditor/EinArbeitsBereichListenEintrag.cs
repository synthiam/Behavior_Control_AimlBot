using de.springwald.toolbox;
using GaitoBotEditorCore;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace GaitoBotEditor
{
	public class EinArbeitsBereichListenEintrag : UserControl
	{
		private IContainer components = null;

		private GroupBox groupBoxArbeitsbereich;

		private Button buttonOeffnen;

		private Label labelTitel;

		private Button buttonLoeschen;

		private Arbeitsbereich Arbeitsbereich
		{
			get;
			set;
		}

		public event EventHandler<EventArgs<Arbeitsbereich>> ArbeitsbereichOeffnen;

		public event EventHandler<EventArgs<Arbeitsbereich>> ArbeitsbereichLoeschen;

		public EinArbeitsBereichListenEintrag()
		{
			this.InitializeComponent();
		}

		public void Anzeigen(Arbeitsbereich arbeitsbereich)
		{
			this.Arbeitsbereich = arbeitsbereich;
			string[] array = arbeitsbereich.Arbeitsverzeichnis.Split(new char[1]
			{
				'\\'
			}, StringSplitOptions.RemoveEmptyEntries);
			this.groupBoxArbeitsbereich.Text = array[array.Length - 1];
			this.labelTitel.Text = arbeitsbereich.Name;
		}

		private void buttonOeffnen_Click(object sender, EventArgs e)
		{
			if (this.ArbeitsbereichOeffnen != null)
			{
				this.ArbeitsbereichOeffnen(this, new EventArgs<Arbeitsbereich>(this.Arbeitsbereich));
			}
		}

		private void buttonLoeschen_Click(object sender, EventArgs e)
		{
			if (this.ArbeitsbereichLoeschen != null)
			{
				this.ArbeitsbereichLoeschen(this, new EventArgs<Arbeitsbereich>(this.Arbeitsbereich));
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(EinArbeitsBereichListenEintrag));
			this.groupBoxArbeitsbereich = new GroupBox();
			this.buttonLoeschen = new Button();
			this.labelTitel = new Label();
			this.buttonOeffnen = new Button();
			this.groupBoxArbeitsbereich.SuspendLayout();
			base.SuspendLayout();
			componentResourceManager.ApplyResources(this.groupBoxArbeitsbereich, "groupBoxArbeitsbereich");
			this.groupBoxArbeitsbereich.BackColor = Color.White;
			this.groupBoxArbeitsbereich.Controls.Add(this.buttonLoeschen);
			this.groupBoxArbeitsbereich.Controls.Add(this.labelTitel);
			this.groupBoxArbeitsbereich.Controls.Add(this.buttonOeffnen);
			this.groupBoxArbeitsbereich.ForeColor = SystemColors.ControlText;
			this.groupBoxArbeitsbereich.Name = "groupBoxArbeitsbereich";
			this.groupBoxArbeitsbereich.TabStop = false;
			componentResourceManager.ApplyResources(this.buttonLoeschen, "buttonLoeschen");
			this.buttonLoeschen.BackColor = SystemColors.Control;
			this.buttonLoeschen.Name = "buttonLoeschen";
			this.buttonLoeschen.UseVisualStyleBackColor = false;
			this.buttonLoeschen.Click += this.buttonLoeschen_Click;
			componentResourceManager.ApplyResources(this.labelTitel, "labelTitel");
			this.labelTitel.Name = "labelTitel";
			componentResourceManager.ApplyResources(this.buttonOeffnen, "buttonOeffnen");
			this.buttonOeffnen.BackColor = SystemColors.Control;
			this.buttonOeffnen.Name = "buttonOeffnen";
			this.buttonOeffnen.UseVisualStyleBackColor = false;
			this.buttonOeffnen.Click += this.buttonOeffnen_Click;
			componentResourceManager.ApplyResources(this, "$this");
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = Color.Transparent;
			base.Controls.Add(this.groupBoxArbeitsbereich);
			base.Name = "EinArbeitsBereichListenEintrag";
			this.groupBoxArbeitsbereich.ResumeLayout(false);
			base.ResumeLayout(false);
		}
	}
}
