using de.springwald.toolbox;
using GaitoBotEditorCore.Properties;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace GaitoBotEditorCore.controls
{
	public class ucWorkflowToolbar : UserControl
	{
		public class SyncCategoryAngewaehltEventArgs : EventArgs
		{
			public AIMLCategory Category;

			public SyncCategoryAngewaehltEventArgs(AIMLCategory category)
			{
				this.Category = category;
			}
		}

		public delegate void SyncCategoryAngewaehltEventHandler(object sender, SyncCategoryAngewaehltEventArgs e);

		private const int _bisAnzahlAutofresh = 3000;

		private const int _bisAnzahlAutoSync = 5000;

		private Arbeitsbereich _arbeitsbereich;

		private AIMLCategory _aktSyncCategory;

		private IContainer components = null;

		private Timer timerRefresh;

		private ToolStripButton toolStripButtonSyncronisieren;

		private ToolStrip toolStripTop;

		public AIMLCategory AktSyncCategory
		{
			get
			{
				return this._aktSyncCategory;
			}
		}

		public Arbeitsbereich Arbeitsbereich
		{
			set
			{
				if (this._arbeitsbereich != null)
				{
					throw new ApplicationException("Dieser Workflow-Toolbar wurde bereits ein Arbeitsbereich zugewiesen");
				}
				this._arbeitsbereich = value;
				this._arbeitsbereich.Fokus.AktAIMLCategoryChanged += this.Fokus_AktAIMLCategoryChanged;
			}
		}

		public event EventHandler AnzeigeRefreshen;

		public event SyncCategoryAngewaehltEventHandler SyncCategoryAngewaehlt;

		protected virtual void AnzeigeRefreshenEvent()
		{
			if (this.AnzeigeRefreshen != null)
			{
				this.AnzeigeRefreshen(this, EventArgs.Empty);
			}
		}

		protected virtual void SyncCategoryAngewaehltEvent(AIMLCategory category)
		{
			if (this.SyncCategoryAngewaehlt != null)
			{
				this.SyncCategoryAngewaehlt(this, new SyncCategoryAngewaehltEventArgs(category));
			}
		}

		public ucWorkflowToolbar()
		{
			this.InitializeComponent();
			base.Load += this.ucWorkflowToolbar_Load;
		}

		private void ucWorkflowToolbar_Load(object sender, EventArgs e)
		{
			this.toolStripButtonSyncronisieren.Text = ResReader.Reader.GetString("toolStripButtonSyncronisieren");
		}

		private void toolStripButtonSyncronisieren_Click(object sender, EventArgs e)
		{
			this._aktSyncCategory = this._arbeitsbereich.Fokus.AktAIMLCategory;
			this.SyncCategoryAngewaehltEvent(this._aktSyncCategory);
		}

		private void Fokus_AktAIMLCategoryChanged(object sender, EventArgs<AIMLCategory> e)
		{
			if (this._arbeitsbereich != null && this._arbeitsbereich.AnzahlKategorien < 5000)
			{
				this._aktSyncCategory = e.Value;
				this.SyncCategoryAngewaehltEvent(this._aktSyncCategory);
			}
		}

		private void timerRefresh_Tick(object sender, EventArgs e)
		{
			if (this._arbeitsbereich != null)
			{
				if (this._arbeitsbereich.AnzahlKategorien < 3000)
				{
					this.AnzeigeRefreshenEvent();
				}
				this.toolStripButtonSyncronisieren.Enabled = (this._arbeitsbereich.AnzahlKategorien > 3000);
			}
			else
			{
				this.toolStripButtonSyncronisieren.Enabled = false;
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ucWorkflowToolbar));
			this.timerRefresh = new Timer(this.components);
			this.toolStripButtonSyncronisieren = new ToolStripButton();
			this.toolStripTop = new ToolStrip();
			this.toolStripTop.SuspendLayout();
			base.SuspendLayout();
			this.timerRefresh.Enabled = true;
			this.timerRefresh.Interval = 3000;
			this.timerRefresh.Tick += this.timerRefresh_Tick;
			this.toolStripButtonSyncronisieren.Image = Resources.sync;
			componentResourceManager.ApplyResources(this.toolStripButtonSyncronisieren, "toolStripButtonSyncronisieren");
			this.toolStripButtonSyncronisieren.Name = "toolStripButtonSyncronisieren";
			this.toolStripButtonSyncronisieren.Click += this.toolStripButtonSyncronisieren_Click;
			componentResourceManager.ApplyResources(this.toolStripTop, "toolStripTop");
			this.toolStripTop.Items.AddRange(new ToolStripItem[1]
			{
				this.toolStripButtonSyncronisieren
			});
			this.toolStripTop.Name = "toolStripTop";
			componentResourceManager.ApplyResources(this, "$this");
			base.AutoScaleMode = AutoScaleMode.Font;
			base.Controls.Add(this.toolStripTop);
			base.Name = "ucWorkflowToolbar";
			this.toolStripTop.ResumeLayout(false);
			this.toolStripTop.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
