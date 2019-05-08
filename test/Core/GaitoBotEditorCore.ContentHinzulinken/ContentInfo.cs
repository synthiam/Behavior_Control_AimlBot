using de.springwald.gaitobot.content;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace GaitoBotEditorCore.ContentHinzulinken
{
	public class ContentInfo : UserControl
	{
		private ContentElementInfo _info;

		private IContainer components = null;

		private CheckBox checkBox1;

		private Label label1;

		public ContentElementInfo Info
		{
			get
			{
				return this._info;
			}
			set
			{
				this.label1.Text = value.Beschreibung;
				this.checkBox1.Text = value.Name;
				this._info = value;
			}
		}

		public bool WahlEnabled
		{
			get
			{
				return this.checkBox1.Enabled;
			}
			set
			{
				this.checkBox1.Enabled = value;
			}
		}

		public bool Gewaehlt
		{
			get
			{
				return this.checkBox1.Checked;
			}
			set
			{
				this.checkBox1.Checked = value;
			}
		}

		public event EventHandler GewaehltChanged;

		public ContentInfo()
		{
			this.InitializeComponent();
			this.checkBox1.CheckedChanged += this.checkBox1_CheckedChanged;
			base.Resize += this.ContentInfo_Resize;
		}

		private void ContentInfo_Resize(object sender, EventArgs e)
		{
			this.label1.Width = base.Width - 20;
		}

		private void checkBox1_CheckedChanged(object sender, EventArgs e)
		{
			if (this.GewaehltChanged != null)
			{
				this.GewaehltChanged(this, EventArgs.Empty);
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ContentInfo));
			this.checkBox1 = new CheckBox();
			this.label1 = new Label();
			base.SuspendLayout();
			componentResourceManager.ApplyResources(this.checkBox1, "checkBox1");
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.UseVisualStyleBackColor = true;
			this.label1.AutoEllipsis = true;
			componentResourceManager.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			componentResourceManager.ApplyResources(this, "$this");
			base.AutoScaleMode = AutoScaleMode.Font;
			base.Controls.Add(this.label1);
			base.Controls.Add(this.checkBox1);
			base.Name = "ContentInfo";
			base.ResumeLayout(false);
		}
	}
}
