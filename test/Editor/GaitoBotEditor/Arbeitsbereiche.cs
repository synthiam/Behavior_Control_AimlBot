using de.springwald.toolbox;
using GaitoBotEditorCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace GaitoBotEditor
{
	public class Arbeitsbereiche : UserControl
	{
		private ArbeitsbereichVerwaltung _arbeitsbereichVerwaltung;

		private IContainer components = null;

		private Label labelTitelArbeitsbereiche;

		private LinkLabel linkLabelNeuenArbeitsbereich;

		private Panel panelArbeitsbereiche;

		public ArbeitsbereichVerwaltung ArbeitsbereichVerwaltung
		{
			set
			{
				this._arbeitsbereichVerwaltung = value;
				this._arbeitsbereichVerwaltung.ArbeitsbereichAddedEvent += this._arbeitsbereichVerwaltung_ArbeitsbereichAddedEvent;
				this._arbeitsbereichVerwaltung.ArbeitsbereichEntferntEvent += this._arbeitsbereichVerwaltung_ArbeitsbereichEntferntEvent;
				this.VorhandeneArbeitsbereicheAuflisten();
			}
		}

		private void _arbeitsbereichVerwaltung_ArbeitsbereichEntferntEvent(object sender, EventArgs<Arbeitsbereich> e)
		{
			this.VorhandeneArbeitsbereicheAuflisten();
		}

		private void _arbeitsbereichVerwaltung_ArbeitsbereichAddedEvent(object sender, EventArgs<Arbeitsbereich> e)
		{
			this.VorhandeneArbeitsbereicheAuflisten();
		}

		public Arbeitsbereiche()
		{
			this.InitializeComponent();
		}

		private void Arbeitsbereiche_Load(object sender, EventArgs e)
		{
		}

		private void VorhandeneArbeitsbereicheAuflisten()
		{
			List<Arbeitsbereich> vorhandeneNochNichtGeladeneArbeitsbereiche = this._arbeitsbereichVerwaltung.VorhandeneNochNichtGeladeneArbeitsbereiche;
			this.panelArbeitsbereiche.Controls.Clear();
			foreach (Arbeitsbereich item in vorhandeneNochNichtGeladeneArbeitsbereiche)
			{
				EinArbeitsBereichListenEintrag einArbeitsBereichListenEintrag = new EinArbeitsBereichListenEintrag();
				einArbeitsBereichListenEintrag.Anzeigen(item);
				this.panelArbeitsbereiche.Controls.Add(einArbeitsBereichListenEintrag);
				einArbeitsBereichListenEintrag.Dock = DockStyle.Top;
				einArbeitsBereichListenEintrag.ArbeitsbereichOeffnen += this.eintrag_ArbeitsbereichOeffnen;
				einArbeitsBereichListenEintrag.ArbeitsbereichLoeschen += this.eintrag_ArbeitsbereichLoeschen;
			}
		}

		private void eintrag_ArbeitsbereichLoeschen(object sender, EventArgs<Arbeitsbereich> e)
		{
			DialogResult dialogResult = MessageBox.Show(string.Format("Sind Sie sicher, dass Sie den Arbeitsbereich '{0}' inklusive aller seiner Inhalte löschen möchten?", e.Value.Name), "Warnung", MessageBoxButtons.YesNoCancel);
			if (dialogResult == DialogResult.Yes)
			{
				try
				{
					e.Value.Loeschen();
					this._arbeitsbereichVerwaltung.ArbeitsbereichEntfernen(e.Value);
					this.VorhandeneArbeitsbereicheAuflisten();
					MessageBox.Show("Arbeitsbereich erfolgreich gelöscht");
				}
				catch (Exception ex)
				{
					MessageBox.Show(string.Format("Konnte Arbeitsbereich nicht oder nur teilweise löschen: {0}", ex.Message));
				}
			}
		}

		private void eintrag_ArbeitsbereichOeffnen(object sender, EventArgs<Arbeitsbereich> e)
		{
			this._arbeitsbereichVerwaltung.VorhandenenArbeitsbereichOeffnen(e.Value.Arbeitsverzeichnis);
			this.VorhandeneArbeitsbereicheAuflisten();
		}

		private void linkLabelNeuenArbeitsbereich_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			if (this._arbeitsbereichVerwaltung == null)
			{
				Debugger.GlobalDebugger.FehlerZeigen("_arbeitsbereichVerwaltung == null", this, "GaitoBotEditor.Arbeitsbereiche.NeuenArbeitsbereichErstellenLinkLabel_LinkClicked");
			}
			else
			{
				this._arbeitsbereichVerwaltung.NeuenArbeitsbereichErstellenUndOeffnen();
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(Arbeitsbereiche));
			this.labelTitelArbeitsbereiche = new Label();
			this.linkLabelNeuenArbeitsbereich = new LinkLabel();
			this.panelArbeitsbereiche = new Panel();
			base.SuspendLayout();
			componentResourceManager.ApplyResources(this.labelTitelArbeitsbereiche, "labelTitelArbeitsbereiche");
			this.labelTitelArbeitsbereiche.BackColor = Color.WhiteSmoke;
			this.labelTitelArbeitsbereiche.Name = "labelTitelArbeitsbereiche";
			componentResourceManager.ApplyResources(this.linkLabelNeuenArbeitsbereich, "linkLabelNeuenArbeitsbereich");
			this.linkLabelNeuenArbeitsbereich.BackColor = Color.AliceBlue;
			this.linkLabelNeuenArbeitsbereich.Name = "linkLabelNeuenArbeitsbereich";
			this.linkLabelNeuenArbeitsbereich.TabStop = true;
			this.linkLabelNeuenArbeitsbereich.LinkClicked += this.linkLabelNeuenArbeitsbereich_LinkClicked;
			componentResourceManager.ApplyResources(this.panelArbeitsbereiche, "panelArbeitsbereiche");
			this.panelArbeitsbereiche.BackColor = Color.Transparent;
			this.panelArbeitsbereiche.Name = "panelArbeitsbereiche";
			componentResourceManager.ApplyResources(this, "$this");
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = Color.LightSteelBlue;
			base.Controls.Add(this.panelArbeitsbereiche);
			base.Controls.Add(this.linkLabelNeuenArbeitsbereich);
			base.Controls.Add(this.labelTitelArbeitsbereiche);
			base.Name = "Arbeitsbereiche";
			base.Load += this.Arbeitsbereiche_Load;
			base.ResumeLayout(false);
		}
	}
}
