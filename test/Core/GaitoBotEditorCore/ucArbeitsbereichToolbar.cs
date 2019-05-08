using de.springwald.toolbox;
using GaitoBotEditorCore.Properties;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace GaitoBotEditorCore
{
	public class ucArbeitsbereichToolbar : UserControl
	{
		private Arbeitsbereich _arbeitsbereich;

		private IContainer components = null;

		private ToolStrip toolStrip1;

		private ToolStripButton toolStripButtonSaveAll;

		private ToolStripSeparator toolStripSeparator1;

		private ToolStripButton toolStripButtonExport;

		private ToolStripSeparator toolStripSeparator2;

		private ToolStripTextBox toolStripTextBoxSearch;

		private ToolStripButton toolStripButtonSearch;

		private ToolStripSeparator toolStripSeparator3;

		private ToolStripSplitButton toolStripSplitButtonTest;

		private ToolStripMenuItem manuellerTestToolStripMenuItem;

		private ToolStripMenuItem automatischeTestsToolStripMenuItem;

		private ToolStripSeparator toolStripSeparator4;

		private ToolStripButton toolStripButtonUmbenennen;

		private Timer timerAkualisieren;

		private ToolStripSeparator toolStripSeparator5;

		private ToolStripButton toolStripButtonHistoryBack;

		private ToolStripButton toolStripButtonHistoryForward;

		private ToolStripButton toolStripButtonPublish;

		public Arbeitsbereich Arbeitsbereich
		{
			set
			{
				if (this._arbeitsbereich != null)
				{
					throw new ApplicationException("Dieser Arbeitsbereich-Toolbar wurde bereits ein Arbeitsbereich zugeordnet");
				}
				this._arbeitsbereich = value;
				this._arbeitsbereich.Verlauf.Changed += this.Verlauf_Changed;
				this.Aktualisieren();
			}
		}

		public event EventHandler ExportAnzeigen;

		public event EventHandler PublizierenAnzeigen;

		public event EventHandler ManuellenTestAnzeigen;

		protected virtual void ExportAnzeigenEvent()
		{
			if (this.ExportAnzeigen != null)
			{
				this.ExportAnzeigen(this, EventArgs.Empty);
			}
		}

		protected virtual void PublizierenAnzeigenEvent()
		{
			if (this.PublizierenAnzeigen != null)
			{
				this.PublizierenAnzeigen(this, EventArgs.Empty);
			}
		}

		protected virtual void ManuellenTestAnzeigenEvent()
		{
			if (this.ManuellenTestAnzeigen != null)
			{
				this.ManuellenTestAnzeigen(this, EventArgs.Empty);
			}
		}

		public ucArbeitsbereichToolbar()
		{
			this.InitializeComponent();
			base.Load += this.ucArbeitsbereich_Load;
		}

		private void ucArbeitsbereich_Load(object sender, EventArgs e)
		{
			if (ToolboxSonstiges.IstEntwicklungsmodus)
			{
				this.automatischeTestsToolStripMenuItem.Visible = false;
			}
			this.toolStripTextBoxSearch.TextChanged += this.toolStripTextBoxSearch_TextChanged;
			this.toolStripTextBoxSearch.KeyDown += this.toolStripTextBoxSearch_KeyDown;
			this.toolStripButtonSearch.Enabled = false;
			this.SteuerElementeBeschriften();
			this.Aktualisieren();
		}

		private void Verlauf_Changed(object sender, EventArgs e)
		{
			this.Aktualisieren();
		}

		private void toolStripTextBoxSearch_TextChanged(object sender, EventArgs e)
		{
			if (this.toolStripTextBoxSearch.Text == null || this.toolStripTextBoxSearch.Text.Trim() == "")
			{
				this.toolStripButtonSearch.Enabled = false;
			}
			else
			{
				this.toolStripButtonSearch.Enabled = true;
			}
		}

		private void timerAkualisieren_Tick(object sender, EventArgs e)
		{
			this.Aktualisieren();
		}

		private void Aktualisieren()
		{
			if (this._arbeitsbereich == null)
			{
				base.Enabled = false;
			}
			else
			{
				base.Enabled = true;
				this.toolStripButtonSaveAll.Enabled = this._arbeitsbereich.Dateiverwaltung.DateienIsChanged;
				this.toolStripButtonHistoryBack.Enabled = this._arbeitsbereich.Verlauf.ZurueckVerfuegbar;
				this.toolStripButtonHistoryForward.Enabled = this._arbeitsbereich.Verlauf.VorwaertsVerfuegbar;
			}
		}

		private void SteuerElementeBeschriften()
		{
			this.toolStripButtonSaveAll.Text = ResReader.Reader.GetString("toolStripButtonSaveAll");
			this.toolStripButtonPublish.Text = ResReader.Reader.GetString("toolStripButtonPublish");
			this.toolStripButtonExport.Text = ResReader.Reader.GetString("toolStripButtonExport");
			this.toolStripButtonSearch.Text = ResReader.Reader.GetString("toolStripButtonSearch");
			this.toolStripSplitButtonTest.Text = ResReader.Reader.GetString("toolStripSplitButtonTest");
			this.manuellerTestToolStripMenuItem.Text = ResReader.Reader.GetString("manuellerTestToolStripMenuItem");
			this.automatischeTestsToolStripMenuItem.Text = ResReader.Reader.GetString("automatischeTestsToolStripMenuItem");
			this.toolStripButtonUmbenennen.Text = ResReader.Reader.GetString("toolStripButtonUmbenennen");
			this.toolStripButtonUmbenennen.ToolTipText = ResReader.Reader.GetString("ArbeitsbereichUmbenennen");
			this.toolStripButtonHistoryBack.Text = ResReader.Reader.GetString("toolStripButtonHistoryBack");
			this.toolStripButtonHistoryForward.Text = ResReader.Reader.GetString("toolStripButtonHistoryForward");
		}

		private void toolStripButtonSaveAll_Click(object sender, EventArgs e)
		{
			if (this._arbeitsbereich != null)
			{
				this._arbeitsbereich.SaveAll();
			}
			this.Aktualisieren();
		}

		private void toolStripButtonExport_Click(object sender, EventArgs e)
		{
			this.ExportAnzeigenEvent();
		}

		private void toolStripButtonPublish_Click(object sender, EventArgs e)
		{
			this.PublizierenAnzeigenEvent();
		}

		private void toolStripSplitButtonTest_ButtonClick(object sender, EventArgs e)
		{
			this.ManuellenTestAnzeigenEvent();
		}

		private void manuellerTestToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.ManuellenTestAnzeigenEvent();
		}

		private void automatischeTestsToolStripMenuItem_Click(object sender, EventArgs e)
		{
		}

		private void toolStripButtonUmbenennen_Click(object sender, EventArgs e)
		{
			if (this._arbeitsbereich != null)
			{
				bool flag = false;
				string name = InputBox.Show(ResReader.Reader.GetString("BitteNeuenNamenFuerArbeitsbereich"), ResReader.Reader.GetString("ArbeitsbereichUmbenennen"), this._arbeitsbereich.Name, out flag);
				if (!flag)
				{
					this._arbeitsbereich.Name = name;
				}
			}
		}

		private void toolStripButtonSearch_Click(object sender, EventArgs e)
		{
			this._arbeitsbereich.Suchen(this.toolStripTextBoxSearch.Text, Arbeitsbereich.WoSuchenOrte.ImArbeitsbereich);
		}

		private void toolStripTextBoxSearch_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Return)
			{
				this._arbeitsbereich.Suchen(this.toolStripTextBoxSearch.Text, Arbeitsbereich.WoSuchenOrte.ImArbeitsbereich);
			}
		}

		private void toolStripButtonHistoryBack_Click(object sender, EventArgs e)
		{
			if (this._arbeitsbereich != null)
			{
				this._arbeitsbereich.Verlauf.Zurueck();
			}
		}

		private void toolStripButtonHistoryForward_Click(object sender, EventArgs e)
		{
			if (this._arbeitsbereich != null)
			{
				this._arbeitsbereich.Verlauf.Vorwaerts();
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
			this.components = new Container();
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ucArbeitsbereichToolbar));
			this.toolStrip1 = new ToolStrip();
			this.toolStripButtonSaveAll = new ToolStripButton();
			this.toolStripSeparator1 = new ToolStripSeparator();
			this.toolStripButtonPublish = new ToolStripButton();
			this.toolStripButtonExport = new ToolStripButton();
			this.toolStripSeparator2 = new ToolStripSeparator();
			this.toolStripSplitButtonTest = new ToolStripSplitButton();
			this.manuellerTestToolStripMenuItem = new ToolStripMenuItem();
			this.automatischeTestsToolStripMenuItem = new ToolStripMenuItem();
			this.toolStripSeparator3 = new ToolStripSeparator();
			this.toolStripTextBoxSearch = new ToolStripTextBox();
			this.toolStripButtonSearch = new ToolStripButton();
			this.toolStripSeparator4 = new ToolStripSeparator();
			this.toolStripButtonUmbenennen = new ToolStripButton();
			this.toolStripSeparator5 = new ToolStripSeparator();
			this.toolStripButtonHistoryBack = new ToolStripButton();
			this.toolStripButtonHistoryForward = new ToolStripButton();
			this.timerAkualisieren = new Timer(this.components);
			this.toolStrip1.SuspendLayout();
			base.SuspendLayout();
			this.toolStrip1.Items.AddRange(new ToolStripItem[14]
			{
				this.toolStripButtonSaveAll,
				this.toolStripSeparator1,
				this.toolStripButtonPublish,
				this.toolStripButtonExport,
				this.toolStripSeparator2,
				this.toolStripSplitButtonTest,
				this.toolStripSeparator3,
				this.toolStripTextBoxSearch,
				this.toolStripButtonSearch,
				this.toolStripSeparator4,
				this.toolStripButtonUmbenennen,
				this.toolStripSeparator5,
				this.toolStripButtonHistoryBack,
				this.toolStripButtonHistoryForward
			});
			componentResourceManager.ApplyResources(this.toolStrip1, "toolStrip1");
			this.toolStrip1.Name = "toolStrip1";
			this.toolStripButtonSaveAll.Image = Resources.save_16;
			componentResourceManager.ApplyResources(this.toolStripButtonSaveAll, "toolStripButtonSaveAll");
			this.toolStripButtonSaveAll.Name = "toolStripButtonSaveAll";
			this.toolStripButtonSaveAll.Click += this.toolStripButtonSaveAll_Click;
			this.toolStripSeparator1.Margin = new Padding(2, 0, 2, 0);
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			componentResourceManager.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
			this.toolStripButtonPublish.Image = Resources.Globe;
			componentResourceManager.ApplyResources(this.toolStripButtonPublish, "toolStripButtonPublish");
			this.toolStripButtonPublish.Name = "toolStripButtonPublish";
			this.toolStripButtonPublish.Click += this.toolStripButtonPublish_Click;
			this.toolStripButtonExport.Image = Resources.redo_16;
			componentResourceManager.ApplyResources(this.toolStripButtonExport, "toolStripButtonExport");
			this.toolStripButtonExport.Name = "toolStripButtonExport";
			this.toolStripButtonExport.Click += this.toolStripButtonExport_Click;
			this.toolStripSeparator2.Margin = new Padding(2, 0, 2, 0);
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			componentResourceManager.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
			this.toolStripSplitButtonTest.DropDownItems.AddRange(new ToolStripItem[2]
			{
				this.manuellerTestToolStripMenuItem,
				this.automatischeTestsToolStripMenuItem
			});
			this.toolStripSplitButtonTest.Image = Resources.applications_16;
			componentResourceManager.ApplyResources(this.toolStripSplitButtonTest, "toolStripSplitButtonTest");
			this.toolStripSplitButtonTest.Name = "toolStripSplitButtonTest";
			this.toolStripSplitButtonTest.ButtonClick += this.toolStripSplitButtonTest_ButtonClick;
			this.manuellerTestToolStripMenuItem.Name = "manuellerTestToolStripMenuItem";
			componentResourceManager.ApplyResources(this.manuellerTestToolStripMenuItem, "manuellerTestToolStripMenuItem");
			this.manuellerTestToolStripMenuItem.Click += this.manuellerTestToolStripMenuItem_Click;
			this.automatischeTestsToolStripMenuItem.Name = "automatischeTestsToolStripMenuItem";
			componentResourceManager.ApplyResources(this.automatischeTestsToolStripMenuItem, "automatischeTestsToolStripMenuItem");
			this.automatischeTestsToolStripMenuItem.Click += this.automatischeTestsToolStripMenuItem_Click;
			this.toolStripSeparator3.Margin = new Padding(2, 0, 2, 0);
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			componentResourceManager.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
			this.toolStripTextBoxSearch.Name = "toolStripTextBoxSearch";
			componentResourceManager.ApplyResources(this.toolStripTextBoxSearch, "toolStripTextBoxSearch");
			this.toolStripButtonSearch.DisplayStyle = ToolStripItemDisplayStyle.Text;
			componentResourceManager.ApplyResources(this.toolStripButtonSearch, "toolStripButtonSearch");
			this.toolStripButtonSearch.Name = "toolStripButtonSearch";
			this.toolStripButtonSearch.Click += this.toolStripButtonSearch_Click;
			this.toolStripSeparator4.Margin = new Padding(2, 0, 2, 0);
			this.toolStripSeparator4.Name = "toolStripSeparator4";
			componentResourceManager.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
			this.toolStripButtonUmbenennen.Image = Resources.rename1;
			componentResourceManager.ApplyResources(this.toolStripButtonUmbenennen, "toolStripButtonUmbenennen");
			this.toolStripButtonUmbenennen.Name = "toolStripButtonUmbenennen";
			this.toolStripButtonUmbenennen.Click += this.toolStripButtonUmbenennen_Click;
			this.toolStripSeparator5.Name = "toolStripSeparator5";
			componentResourceManager.ApplyResources(this.toolStripSeparator5, "toolStripSeparator5");
			this.toolStripButtonHistoryBack.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.toolStripButtonHistoryBack.Image = Resources.arrow_back_16;
			componentResourceManager.ApplyResources(this.toolStripButtonHistoryBack, "toolStripButtonHistoryBack");
			this.toolStripButtonHistoryBack.Name = "toolStripButtonHistoryBack";
			this.toolStripButtonHistoryBack.Click += this.toolStripButtonHistoryBack_Click;
			this.toolStripButtonHistoryForward.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.toolStripButtonHistoryForward.Image = Resources.arrow_forward_16;
			componentResourceManager.ApplyResources(this.toolStripButtonHistoryForward, "toolStripButtonHistoryForward");
			this.toolStripButtonHistoryForward.Name = "toolStripButtonHistoryForward";
			this.toolStripButtonHistoryForward.Click += this.toolStripButtonHistoryForward_Click;
			this.timerAkualisieren.Enabled = true;
			this.timerAkualisieren.Interval = 2000;
			this.timerAkualisieren.Tick += this.timerAkualisieren_Tick;
			componentResourceManager.ApplyResources(this, "$this");
			base.AutoScaleMode = AutoScaleMode.Font;
			base.Controls.Add(this.toolStrip1);
			base.Name = "ucArbeitsbereichToolbar";
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
