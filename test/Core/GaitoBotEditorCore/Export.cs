using de.springwald.toolbox;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

namespace GaitoBotEditorCore
{
	public class Export : UserControl
	{
		private Arbeitsbereich _arbeitsbereich;

		private IContainer components = null;

		private FolderBrowserDialog folderBrowserDialog;

		private TextBox textBoxExportVerzeichnis;

		private Button buttonDurchsuchen;

		private Label labelExportVerzeichnis;

		private Button buttonExportStarten;

		private CheckBox checkBoxVorExportVerzeichnisLeeren;

		private CheckBox checkBoxAlleStarTagsInEineDatei;

		public Arbeitsbereich Arbeitsbereich
		{
			set
			{
				if (this._arbeitsbereich != null)
				{
					throw new ApplicationException("Diesem ExportSteuerelement wurde bereits ein Arbeitsbereich zugeordnet");
				}
				this._arbeitsbereich = value;
				this.Anzeigen();
			}
		}

		public Export()
		{
			this.InitializeComponent();
		}

		private void Export_Load(object sender, EventArgs e)
		{
			this.SteuerelementeBenennen();
		}

		private void buttonExportStarten_Click(object sender, EventArgs e)
		{
			if (this._arbeitsbereich.Exportverzeichnis != this.textBoxExportVerzeichnis.Text)
			{
				this._arbeitsbereich.Exportverzeichnis = this.textBoxExportVerzeichnis.Text;
			}
			if (this._arbeitsbereich.Dateiverwaltung.DateienIsChanged)
			{
				DialogResult dialogResult = MessageBox.Show(ResReader.Reader.GetString("ArbeitsBereichMussVorExportGespeichertWerden"), "Export", MessageBoxButtons.YesNoCancel);
				if (dialogResult == DialogResult.Yes)
				{
					this._arbeitsbereich.SaveAll();
					goto IL_00a2;
				}
				MessageBox.Show(ResReader.Reader.GetString("ExportAbgebrochen"));
				return;
			}
			goto IL_00a2;
			IL_00a2:
			string exportverzeichnis = this._arbeitsbereich.Exportverzeichnis;
			try
			{
				if (!Directory.Exists(exportverzeichnis))
				{
					DialogResult dialogResult2 = MessageBox.Show(ResReader.Reader.GetString("SollExportVerzeichnisAngelegtWerden"), "Export", MessageBoxButtons.YesNoCancel);
					if (dialogResult2 == DialogResult.Yes)
					{
						Directory.CreateDirectory(exportverzeichnis);
						goto end_IL_00ae;
					}
					MessageBox.Show(ResReader.Reader.GetString("ExportAbgebrochen"));
					return;
				}
				end_IL_00ae:;
			}
			catch (Exception ex)
			{
				Debugger.GlobalDebugger.FehlerZeigen(ex.Message, "CreateExportDirectory '" + exportverzeichnis + "'", "Export");
				MessageBox.Show(ResReader.Reader.GetString("ExportAbgebrochen"));
				return;
			}
			base.Enabled = false;
			bool flag = false;
			Arbeitsbereich.VerzeichnisVorherLeeren verzeichnisVorherLeeren = Arbeitsbereich.VerzeichnisVorherLeeren.nichtLeeren;
			if (this.checkBoxVorExportVerzeichnisLeeren.Checked)
			{
				verzeichnisVorherLeeren = Arbeitsbereich.VerzeichnisVorherLeeren.leerenUndVorherFragen;
			}
			this._arbeitsbereich.Exportieren(exportverzeichnis, verzeichnisVorherLeeren, this._arbeitsbereich.AlleStarTagsInExtraDateiExportieren, false, out flag);
			if (flag)
			{
				MessageBox.Show(ResReader.Reader.GetString("ExportAbgebrochen"));
			}
			else
			{
				MessageBox.Show(ResReader.Reader.GetString("ExportErfolgreich"));
			}
			base.Enabled = true;
		}

		private void Anzeigen()
		{
			if (this._arbeitsbereich == null)
			{
				base.Enabled = false;
			}
			else
			{
				base.Enabled = true;
				this.checkBoxVorExportVerzeichnisLeeren.Checked = this._arbeitsbereich.ExportverzeichnisVorExportLeeren;
				this.checkBoxAlleStarTagsInEineDatei.Checked = this._arbeitsbereich.AlleStarTagsInExtraDateiExportieren;
				this.textBoxExportVerzeichnis.Text = this._arbeitsbereich.Exportverzeichnis;
			}
		}

		private void buttonDurchsuchen_Click(object sender, EventArgs e)
		{
			this.folderBrowserDialog.ShowNewFolderButton = true;
			this.folderBrowserDialog.SelectedPath = this.textBoxExportVerzeichnis.Text;
			DialogResult dialogResult = this.folderBrowserDialog.ShowDialog();
			DialogResult dialogResult2 = dialogResult;
			if (dialogResult2 != DialogResult.Cancel)
			{
				this._arbeitsbereich.Exportverzeichnis = this.folderBrowserDialog.SelectedPath;
				this.Anzeigen();
			}
		}

		private void SteuerelementeBenennen()
		{
			this.labelExportVerzeichnis.Text = ResReader.Reader.GetString("labelExportVerzeichnis");
			this.buttonDurchsuchen.Text = ResReader.Reader.GetString("ExportButtonDurchsuchenText");
			this.buttonExportStarten.Text = ResReader.Reader.GetString("ExportStarten");
			this.checkBoxVorExportVerzeichnisLeeren.Text = ResReader.Reader.GetString("checkBoxVorExportVerzeichnisLeeren");
			this.checkBoxAlleStarTagsInEineDatei.Text = ResReader.Reader.GetString("checkBoxAlleStarTagsInEineDatei");
		}

		private void checkBoxVorExportVerzeichnisLeeren_CheckedChanged(object sender, EventArgs e)
		{
			if (this.checkBoxVorExportVerzeichnisLeeren.Checked != this._arbeitsbereich.ExportverzeichnisVorExportLeeren)
			{
				this._arbeitsbereich.ExportverzeichnisVorExportLeeren = this.checkBoxVorExportVerzeichnisLeeren.Checked;
			}
		}

		private void checkBoxAlleStarTagsInEineDatei_CheckedChanged(object sender, EventArgs e)
		{
			if (this.checkBoxAlleStarTagsInEineDatei.Checked != this._arbeitsbereich.AlleStarTagsInExtraDateiExportieren)
			{
				this._arbeitsbereich.AlleStarTagsInExtraDateiExportieren = this.checkBoxAlleStarTagsInEineDatei.Checked;
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(Export));
			this.folderBrowserDialog = new FolderBrowserDialog();
			this.textBoxExportVerzeichnis = new TextBox();
			this.buttonDurchsuchen = new Button();
			this.labelExportVerzeichnis = new Label();
			this.buttonExportStarten = new Button();
			this.checkBoxVorExportVerzeichnisLeeren = new CheckBox();
			this.checkBoxAlleStarTagsInEineDatei = new CheckBox();
			base.SuspendLayout();
			componentResourceManager.ApplyResources(this.textBoxExportVerzeichnis, "textBoxExportVerzeichnis");
			this.textBoxExportVerzeichnis.Name = "textBoxExportVerzeichnis";
			componentResourceManager.ApplyResources(this.buttonDurchsuchen, "buttonDurchsuchen");
			this.buttonDurchsuchen.Name = "buttonDurchsuchen";
			this.buttonDurchsuchen.UseVisualStyleBackColor = true;
			this.buttonDurchsuchen.Click += this.buttonDurchsuchen_Click;
			componentResourceManager.ApplyResources(this.labelExportVerzeichnis, "labelExportVerzeichnis");
			this.labelExportVerzeichnis.Name = "labelExportVerzeichnis";
			componentResourceManager.ApplyResources(this.buttonExportStarten, "buttonExportStarten");
			this.buttonExportStarten.Name = "buttonExportStarten";
			this.buttonExportStarten.UseVisualStyleBackColor = true;
			this.buttonExportStarten.Click += this.buttonExportStarten_Click;
			componentResourceManager.ApplyResources(this.checkBoxVorExportVerzeichnisLeeren, "checkBoxVorExportVerzeichnisLeeren");
			this.checkBoxVorExportVerzeichnisLeeren.Name = "checkBoxVorExportVerzeichnisLeeren";
			this.checkBoxVorExportVerzeichnisLeeren.UseVisualStyleBackColor = true;
			this.checkBoxVorExportVerzeichnisLeeren.CheckedChanged += this.checkBoxVorExportVerzeichnisLeeren_CheckedChanged;
			componentResourceManager.ApplyResources(this.checkBoxAlleStarTagsInEineDatei, "checkBoxAlleStarTagsInEineDatei");
			this.checkBoxAlleStarTagsInEineDatei.Name = "checkBoxAlleStarTagsInEineDatei";
			this.checkBoxAlleStarTagsInEineDatei.UseVisualStyleBackColor = true;
			this.checkBoxAlleStarTagsInEineDatei.CheckedChanged += this.checkBoxAlleStarTagsInEineDatei_CheckedChanged;
			componentResourceManager.ApplyResources(this, "$this");
			base.AutoScaleMode = AutoScaleMode.Font;
			base.Controls.Add(this.checkBoxAlleStarTagsInEineDatei);
			base.Controls.Add(this.checkBoxVorExportVerzeichnisLeeren);
			base.Controls.Add(this.buttonExportStarten);
			base.Controls.Add(this.labelExportVerzeichnis);
			base.Controls.Add(this.buttonDurchsuchen);
			base.Controls.Add(this.textBoxExportVerzeichnis);
			base.Name = "Export";
			base.Load += this.Export_Load;
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
