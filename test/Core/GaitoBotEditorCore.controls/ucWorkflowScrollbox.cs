using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace GaitoBotEditorCore.controls
{
	public class ucWorkflowScrollbox : UserControl
	{
		private Arbeitsbereich _arbeitsbereich;

		private IContainer components = null;

		private ucWorkflow ucWorkflow;

		public Arbeitsbereich Arbeitsbereich
		{
			set
			{
				if (this._arbeitsbereich != null)
				{
					throw new ApplicationException("Dieser WorkflowScrollbox wurde bereits ein Arbeitsbereich zugewiesen");
				}
				this._arbeitsbereich = value;
				this.ucWorkflow.Arbeitsbereich = this._arbeitsbereich;
			}
		}

		public AIMLCategory Category
		{
			get
			{
				return this.ucWorkflow.Category;
			}
			set
			{
				this.ucWorkflow.Category = value;
			}
		}

		public ucWorkflowScrollbox()
		{
			this.InitializeComponent();
		}

		private void ucWorkflowScrollbox_Load(object sender, EventArgs e)
		{
			this.BackColor = Color.White;
		}

		public void RefreshAnzeige()
		{
			this.ucWorkflow.RefreshAnzeige();
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ucWorkflowScrollbox));
			this.ucWorkflow = new ucWorkflow();
			base.SuspendLayout();
			this.ucWorkflow.BackColor = Color.FromArgb(255, 192, 192);
			this.ucWorkflow.Category = null;
			componentResourceManager.ApplyResources(this.ucWorkflow, "ucWorkflow");
			this.ucWorkflow.Name = "ucWorkflow";
			componentResourceManager.ApplyResources(this, "$this");
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = Color.FromArgb(192, 255, 192);
			base.Controls.Add(this.ucWorkflow);
			base.Name = "ucWorkflowScrollbox";
			base.Load += this.ucWorkflowScrollbox_Load;
			base.ResumeLayout(false);
		}
	}
}
