using de.springwald.toolbox;
using GaitoBotEditorCore.Publizieren;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace GaitoBotEditorCore.controls
{
	public class Publizieren : UserControl
	{
		private Arbeitsbereich _arbeitsbereich;

		private string _protokollReverse;

		private IContainer components = null;

		private Button buttonPublish;

		private TextBox textBoxGaitoBotID;

		private Label labelGaitoBotID;

		private LinkLabel linkLabelKeineBotID;

		private TextBox textBoxProtokoll;

		public Arbeitsbereich AktArbeitsbereich
		{
			set
			{
				if (this._arbeitsbereich != null)
				{
					throw new ApplicationException("Diesem PublizierungSteuerelement wurde bereits ein Arbeitsbereich zugeordnet");
				}
				this._arbeitsbereich = value;
				this.Anzeigen();
			}
		}

		public Publizieren()
		{
			this.InitializeComponent();
			this.SteuerElementeBeschriften();
			this.textBoxProtokoll.Visible = false;
			base.Enabled = true;
		}

		private void SteuerElementeBeschriften()
		{
			this.labelGaitoBotID.Text = ResReader.Reader.GetString("PublizierenLabelGaitoBotID");
			this.buttonPublish.Text = ResReader.Reader.GetString("PublizierenbuttonPublish");
			this.linkLabelKeineBotID.Text = ResReader.Reader.GetString("PublizierenlinkLabelKeineBotID");
		}

		private void buttonPublish_Click(object sender, EventArgs e)
		{
			this.buttonPublish.Enabled = false;
			this.textBoxGaitoBotID.Enabled = false;
			this._protokollReverse = "";
			ArbeitsbereichPublizieren arbeitsbereichPublizieren = new ArbeitsbereichPublizieren(this._arbeitsbereich);
			arbeitsbereichPublizieren.PublizerEvent += this.publizierer_PublizerEvent;
			arbeitsbereichPublizieren.Publizieren();
			ArbeitsbereichPublizieren.ergebnisse ergebnis = arbeitsbereichPublizieren.Ergebnis;
			if (ergebnis != ArbeitsbereichPublizieren.ergebnisse.erfolgreich && ergebnis != ArbeitsbereichPublizieren.ergebnisse.fehlerhaft)
			{
				throw new ApplicationException("Unbehandeltes Publizierungsergebnis");
			}
			arbeitsbereichPublizieren.PublizerEvent -= this.publizierer_PublizerEvent;
			arbeitsbereichPublizieren.Dispose();
			this.buttonPublish.Enabled = true;
			this.textBoxGaitoBotID.Enabled = true;
		}

		private void Anzeigen()
		{
			this.textBoxGaitoBotID.Text = this._arbeitsbereich.GaitoBotID;
		}

		private void publizierer_PublizerEvent(object sender, PublizierenEventArgs e)
		{
			this.textBoxProtokoll.Visible = true;
			this._protokollReverse = string.Format("{0}\r\n{1}", e.Meldung, this._protokollReverse);
			this.textBoxProtokoll.Text = this._protokollReverse;
		}

		private void textBoxGaitoBotID_TextChanged(object sender, EventArgs e)
		{
			this._arbeitsbereich.GaitoBotID = this.textBoxGaitoBotID.Text;
		}

		private void linkLabelKeineBotID_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			ToolboxSonstiges.HTMLSeiteAufrufen(Config.GlobalConfig.URLNochKeineGaitoBotID);
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(Publizieren));
			this.buttonPublish = new Button();
			this.textBoxGaitoBotID = new TextBox();
			this.labelGaitoBotID = new Label();
			this.linkLabelKeineBotID = new LinkLabel();
			this.textBoxProtokoll = new TextBox();
			base.SuspendLayout();
			componentResourceManager.ApplyResources(this.buttonPublish, "buttonPublish");
			this.buttonPublish.Name = "buttonPublish";
			this.buttonPublish.UseVisualStyleBackColor = true;
			this.buttonPublish.Click += this.buttonPublish_Click;
			componentResourceManager.ApplyResources(this.textBoxGaitoBotID, "textBoxGaitoBotID");
			this.textBoxGaitoBotID.Name = "textBoxGaitoBotID";
			this.textBoxGaitoBotID.TextChanged += this.textBoxGaitoBotID_TextChanged;
			componentResourceManager.ApplyResources(this.labelGaitoBotID, "labelGaitoBotID");
			this.labelGaitoBotID.Name = "labelGaitoBotID";
			componentResourceManager.ApplyResources(this.linkLabelKeineBotID, "linkLabelKeineBotID");
			this.linkLabelKeineBotID.Name = "linkLabelKeineBotID";
			this.linkLabelKeineBotID.TabStop = true;
			this.linkLabelKeineBotID.LinkClicked += this.linkLabelKeineBotID_LinkClicked;
			componentResourceManager.ApplyResources(this.textBoxProtokoll, "textBoxProtokoll");
			this.textBoxProtokoll.Name = "textBoxProtokoll";
			componentResourceManager.ApplyResources(this, "$this");
			base.AutoScaleMode = AutoScaleMode.Font;
			base.Controls.Add(this.textBoxProtokoll);
			base.Controls.Add(this.linkLabelKeineBotID);
			base.Controls.Add(this.labelGaitoBotID);
			base.Controls.Add(this.textBoxGaitoBotID);
			base.Controls.Add(this.buttonPublish);
			base.Name = "Publizieren";
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
