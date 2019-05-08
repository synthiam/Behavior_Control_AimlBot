using GaitoBotEditor.Properties;
using GaitoBotEditorCore;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace GaitoBotEditor
{
	public class SpracheAuswaehlen : Form
	{
		private IContainer components = null;

		private Button buttonDeutschSprache;

		private Button buttonSpracheEnglisch;

		public SpracheAuswaehlen()
		{
			this.InitializeComponent();
		}

		private void buttonDeutschSprache_Click(object sender, EventArgs e)
		{
			Config.GlobalConfig.ProgrammSprache = "de";
			this.Starten();
		}

		private void buttonSpracheEnglisch_Click(object sender, EventArgs e)
		{
			Config.GlobalConfig.ProgrammSprache = "en";
			this.Starten();
		}

		private void Starten()
		{
			base.Close();
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(SpracheAuswaehlen));
			this.buttonSpracheEnglisch = new Button();
			this.buttonDeutschSprache = new Button();
			base.SuspendLayout();
			//componentResourceManager.ApplyResources(this.buttonSpracheEnglisch, "buttonSpracheEnglisch");
			//this.buttonSpracheEnglisch.Image = Resources.United_Kingdom;
			this.buttonSpracheEnglisch.Name = "buttonSpracheEnglisch";
			this.buttonSpracheEnglisch.UseVisualStyleBackColor = true;
			this.buttonSpracheEnglisch.Click += this.buttonSpracheEnglisch_Click;
			//this.buttonDeutschSprache.Image = Resources.Germany;
			//componentResourceManager.ApplyResources(this.buttonDeutschSprache, "buttonDeutschSprache");
			this.buttonDeutschSprache.Name = "buttonDeutschSprache";
			this.buttonDeutschSprache.UseVisualStyleBackColor = true;
			this.buttonDeutschSprache.Click += this.buttonDeutschSprache_Click;
			//componentResourceManager.ApplyResources(this, "$this");
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ControlBox = false;
			base.Controls.Add(this.buttonSpracheEnglisch);
			base.Controls.Add(this.buttonDeutschSprache);
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "SpracheAuswaehlen";
			base.ResumeLayout(false);
		}
	}
}
