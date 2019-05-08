using de.springwald.gaitobot.content;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace GaitoBotEditorCore.ContentHinzulinken
{
	public class ContentHinzulinken : Form
	{
		private Arbeitsbereich _arbeitbereich;

		private List<ContentInfo> _infos;

		private IContainer components = null;

		private Panel panel1;

		private Button buttonFertig;

		private Label labelAnweisung;

		public ContentHinzulinken(Arbeitsbereich arbeitsbereich)
		{
			this._arbeitbereich = arbeitsbereich;
			this.InitializeComponent();
			base.Load += this.ContentHinzulinken_Load;
			base.Closing += this.ContentHinzulinken_Closing;
		}

		private void ContentHinzulinken_Load(object sender, EventArgs e)
		{
			this.GewaehlteBausteineAnzeigen();
			this.AbhaengigkeitenAnzeigen();
		}

		private void ContentHinzulinken_Closing(object sender, CancelEventArgs e)
		{
			ContentManager contentManager = new ContentManager();
			List<string> list = new List<string>();
			foreach (ContentInfo info in this._infos)
			{
				if (info.Gewaehlt)
				{
					list.Add(info.Info.UniqueKey);
				}
			}
			this._arbeitbereich.ContentElementUniqueIds = list.ToArray();
			this._arbeitbereich.Dateiverwaltung.VordefinierteDateienHinzulinken(this._arbeitbereich);
		}

		private void GewaehlteBausteineAnzeigen()
		{
			this._infos = new List<ContentInfo>();
			int num = 0;
			ContentManager contentManager = new ContentManager();
			ContentElementInfo[] enthalteneContentElementInfos = contentManager.EnthalteneContentElementInfos;
			foreach (ContentElementInfo contentElementInfo in enthalteneContentElementInfos)
			{
				ContentInfo contentInfo = new ContentInfo();
				this.panel1.Controls.Add(contentInfo);
				this._infos.Add(contentInfo);
				contentInfo.Info = contentElementInfo;
				contentInfo.Gewaehlt = this._arbeitbereich.ContentElementUniqueIds.Contains(contentElementInfo.UniqueKey);
				contentInfo.Width = this.panel1.Width - 10;
				contentInfo.Top = num;
				num += contentInfo.Height;
				contentInfo.GewaehltChanged += this.info_GewaehltChanged;
			}
		}

		private void info_GewaehltChanged(object sender, EventArgs e)
		{
			this.AbhaengigkeitenAnzeigen();
		}

		private void AbhaengigkeitenAnzeigen()
		{
			foreach (ContentInfo info in this._infos)
			{
				info.WahlEnabled = true;
			}
			foreach (ContentInfo info2 in this._infos)
			{
				if (info2.Gewaehlt)
				{
					foreach (ContentInfo info3 in this._infos)
					{
						if (info2.Info.AbhaengigkeitenUniqueIds.Contains(info3.Info.UniqueKey))
						{
							info3.Gewaehlt = true;
							info3.WahlEnabled = false;
						}
					}
				}
			}
		}

		private void buttonFertig_Click(object sender, EventArgs e)
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ContentHinzulinken));
			this.panel1 = new Panel();
			this.buttonFertig = new Button();
			this.labelAnweisung = new Label();
			base.SuspendLayout();
			componentResourceManager.ApplyResources(this.panel1, "panel1");
			this.panel1.Name = "panel1";
			componentResourceManager.ApplyResources(this.buttonFertig, "buttonFertig");
			this.buttonFertig.Name = "buttonFertig";
			this.buttonFertig.UseVisualStyleBackColor = true;
			this.buttonFertig.Click += this.buttonFertig_Click;
			componentResourceManager.ApplyResources(this.labelAnweisung, "labelAnweisung");
			this.labelAnweisung.Name = "labelAnweisung";
			componentResourceManager.ApplyResources(this, "$this");
			base.AutoScaleMode = AutoScaleMode.Font;
			base.Controls.Add(this.labelAnweisung);
			base.Controls.Add(this.buttonFertig);
			base.Controls.Add(this.panel1);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "ContentHinzulinken";
			base.SizeGripStyle = SizeGripStyle.Hide;
			base.TopMost = true;
			base.ResumeLayout(false);
		}
	}
}
