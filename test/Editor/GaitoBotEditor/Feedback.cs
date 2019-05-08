using de.springwald.toolbox;
using GaitoBotEditorCore;
using MultiLang;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace GaitoBotEditor
{
	public class Feedback : UserControl
	{
		private frmFeedback _feedbackFormular;

		private IContainer components = null;

		private LinkLabel FeedbackGebenLinkLabel;

		private Label labelTitelFeedback;

		public Feedback()
		{
			this.InitializeComponent();
			this.FeedbackBoxFuellen();
		}

		private void FeedbackBoxFuellen()
		{
			this.FeedbackGebenLinkLabel.Text = string.Format(global::MultiLang.ml.ml_string(47, "Teilen Sie uns Ihre Meinung, Verbesserungsvorschläge und mögliche Fehler in {0} mit."), Application.ProductName);
		}

		private void FeedbackGebenLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			if (this._feedbackFormular != null)
			{
				if (!this._feedbackFormular.Visible)
				{
					this._feedbackFormular.Dispose();
					this._feedbackFormular = null;
				}
				else
				{
					this._feedbackFormular.Show();
					this._feedbackFormular.BringToFront();
				}
			}
			if (this._feedbackFormular == null)
			{
				this._feedbackFormular = new frmFeedback();
				this._feedbackFormular.ZeigeFeedbackFormular(Config.GlobalConfig.FeedbackEmailAdresse, Config.GlobalConfig.FeedbackWebScriptURL);
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(Feedback));
			this.FeedbackGebenLinkLabel = new LinkLabel();
			this.labelTitelFeedback = new Label();
			base.SuspendLayout();
			this.FeedbackGebenLinkLabel.ActiveLinkColor = Color.Blue;
			componentResourceManager.ApplyResources(this.FeedbackGebenLinkLabel, "FeedbackGebenLinkLabel");
			this.FeedbackGebenLinkLabel.BackColor = Color.White;
			this.FeedbackGebenLinkLabel.LinkBehavior = LinkBehavior.NeverUnderline;
			this.FeedbackGebenLinkLabel.LinkColor = Color.Black;
			this.FeedbackGebenLinkLabel.Name = "FeedbackGebenLinkLabel";
			this.FeedbackGebenLinkLabel.TabStop = true;
			this.FeedbackGebenLinkLabel.LinkClicked += this.FeedbackGebenLinkLabel_LinkClicked;
			componentResourceManager.ApplyResources(this.labelTitelFeedback, "labelTitelFeedback");
			this.labelTitelFeedback.BackColor = Color.WhiteSmoke;
			this.labelTitelFeedback.Name = "labelTitelFeedback";
			componentResourceManager.ApplyResources(this, "$this");
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = Color.LightSteelBlue;
			base.Controls.Add(this.labelTitelFeedback);
			base.Controls.Add(this.FeedbackGebenLinkLabel);
			base.Name = "Feedback";
			base.ResumeLayout(false);
		}
	}
}
