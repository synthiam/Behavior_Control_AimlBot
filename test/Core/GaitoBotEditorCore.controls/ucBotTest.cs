using de.springwald.gaitobot2;
using de.springwald.toolbox;
using GaitoBotEditorCore.Properties;
using MultiLang;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

namespace GaitoBotEditorCore.controls
{
	public class ucBotTest : UserControl
	{
		private Arbeitsbereich _arbeitsbereich;

		private GaitoBotInterpreter _bot;

		private GaitoBotSession _session;

		private IContainer components = null;

		private TextBox textBoxBenutzerEingabe;

		private Button buttonAbsenden;

		private ToolStrip toolStrip;

		private ToolStripButton toolStripButtonWissenNeuLaden;

		private ToolStripSeparator toolStripSeparatorBitteWarten;

		private ToolStripLabel toolStripLabelWissenWirdGeladen;

		private ToolStripProgressBar toolStripProgressBarWissenLaden;

		private WebBrowser webBrowserBotAusgabe;

		private ucBotDenkprotokoll ucBotDenkprotokoll;

		private ToolStripButton toolStripButtonLadeProtokollZeigen;

		private ToolStripButton toolStripButtonZeigeStart;

		private ToolStripSeparator toolStripSeparator1;

		public Arbeitsbereich Arbeitsbereich
		{
			set
			{
				if (this._arbeitsbereich != null)
				{
					throw new ApplicationException("Diesem BotTester wurde bereits ein Arbeitsbereich zugeordnet");
				}
				this._arbeitsbereich = value;
				this._arbeitsbereich.WissenSpracheChanged += this._arbeitsbereich_WissenSpracheChangedEvent;
				this._arbeitsbereich.UseOneWordSRAIChanged += this._arbeitsbereich_UseOneWordSRAIChanged;
				this._arbeitsbereich.Dateiverwaltung.AimlDateienContentChanged += this._arbeitsbereich_AIMLDateienContentChanged;
				this._arbeitsbereich.MetaInfosChangedEvent += this._arbeitsbereich_MetaInfosChangedEvent;
				this.ucBotDenkprotokoll.Arbeitsbereich = this._arbeitsbereich;
			}
		}

		public ucBotTest()
		{
			this.InitializeComponent();
			this.SteuerelementeBeschriften();
			this.webBrowserBotAusgabe.WebBrowserShortcutsEnabled = false;
			this.webBrowserBotAusgabe.IsWebBrowserContextMenuEnabled = false;
			this.textBoxBenutzerEingabe.KeyDown += this.textBoxBenutzerEingabe_KeyDown;
			this.ucBotDenkprotokoll.CategoryAngewaehlt += this.ucBotDenkprotokoll1_CategoryAngewaehlt;
			this.BitteWartenZeigen(false);
			this.toolStripButtonLadeProtokollZeigen.Visible = false;
			base.Resize += this.ucBotTest_Resize;
		}

		private void ucBotTest_Resize(object sender, EventArgs e)
		{
			this.webBrowserBotAusgabe.Height = this.buttonAbsenden.Top - this.webBrowserBotAusgabe.Top - 5;
			this.ucBotDenkprotokoll.Left = this.webBrowserBotAusgabe.Left + this.webBrowserBotAusgabe.Width + 5;
			this.ucBotDenkprotokoll.Top = this.toolStrip.Top + this.toolStrip.Height;
			ucBotDenkprotokoll obj = this.ucBotDenkprotokoll;
			Size clientSize = base.ClientSize;
			obj.Width = clientSize.Width - 5 - this.ucBotDenkprotokoll.Left;
			ucBotDenkprotokoll obj2 = this.ucBotDenkprotokoll;
			clientSize = base.ClientSize;
			obj2.Height = clientSize.Height - this.ucBotDenkprotokoll.Top;
		}

		private void WissenLaden()
		{
			if (this._arbeitsbereich != null)
			{
				base.Enabled = false;
				this.BitteWartenZeigen(true);
				Application.DoEvents();
				List<DomDocLadePaket> list = new List<DomDocLadePaket>();
				StartupInfos startupInfos = new StartupInfos();
				foreach (IArbeitsbereichDatei item2 in this._arbeitsbereich.Dateiverwaltung.Dateien)
				{
					if (!this._arbeitsbereich.DieseAIMLDateiNichtExportieren(item2))
					{
						if (item2 is AIMLDatei)
						{
							DomDocLadePaket item = new DomDocLadePaket(item2.XML, item2.Titel);
							list.Add(item);
						}
						else if (item2 is StartupDatei)
						{
							startupInfos.FuegeEintraegeAusStartupDateiHinzu(item2.XML);
						}
					}
				}
				this.toolStripProgressBarWissenLaden.Value = 0;
				this.toolStripProgressBarWissenLaden.Maximum = list.Count + 1;
				CultureInfo currentUICulture = Thread.CurrentThread.CurrentUICulture;
				this._bot = new GaitoBotInterpreter(currentUICulture, startupInfos);
				this._bot.AIMLDateiWirdGeladen += this._bot_AIMLDateiWirdGeladen;
				this._bot.LadenAusXMLDoms(list);
				this._bot.AIMLDateiWirdGeladen -= this._bot_AIMLDateiWirdGeladen;
				this._session = new GaitoBotSession(currentUICulture);
				this.BitteWartenZeigen(false);
				base.Enabled = true;
			}
		}

		private void _bot_AIMLDateiWirdGeladen(object sender, GaitoBotInterpreter.AimlDateiWirdGeladenEventArgs e)
		{
			try
			{
				this.toolStripProgressBarWissenLaden.Value++;
				this.toolStripLabelWissenWirdGeladen.Text = string.Format(ResReader.Reader.GetString("toolStripLabelWissenWirdGeladen"), this.toolStripProgressBarWissenLaden.Value, this.toolStripProgressBarWissenLaden.Maximum - 1);
			}
			catch (Exception)
			{
			}
			Application.DoEvents();
		}

		private void SteuerelementeBeschriften()
		{
			this.toolStripButtonZeigeStart.Text = global::MultiLang.ml.ml_string(72, "Begrüßung");
			this.toolStripButtonZeigeStart.ToolTipText = global::MultiLang.ml.ml_string(72, "Begrüßung");
			this.toolStripButtonWissenNeuLaden.Text = ResReader.Reader.GetString("toolStripButtonWissenNeuLaden");
			this.buttonAbsenden.Text = ResReader.Reader.GetString("BotTestButtonAbsenden");
			this.toolStripButtonLadeProtokollZeigen.Text = ResReader.Reader.GetString("BotTestLadeProtokollZeigenButton");
		}

		private void toolStripButtonWissenNeuLaden_Click(object sender, EventArgs e)
		{
			this.WissenLaden();
			if (this.webBrowserBotAusgabe != null)
			{
				this.BegruessungAnzeigen();
			}
		}

		private void BenutzerEingabe(string eingabe)
		{
			if (this._bot == null)
			{
				this.WissenLaden();
			}
			this._session.DenkprotokollLeeren();
			string antwort = this._bot.GetAntwort(eingabe, this._session);
			this.webBrowserBotAusgabe.DocumentText = antwort;
			this.ucBotDenkprotokoll.Denkprotokoll = this._session.Denkprotokoll;
			this.textBoxBenutzerEingabe.Text = "";
			this.textBoxBenutzerEingabe.Focus();
		}

		private void buttonAbsenden_Click(object sender, EventArgs e)
		{
			this.BenutzerEingabe(this.textBoxBenutzerEingabe.Text);
		}

		private void ucBotDenkprotokoll1_CategoryAngewaehlt(object sender, ucBotDenkprotokoll.CategoryAngewaehltEventArgs e)
		{
			WissensCategory category = e.Category;
			XmlNode categoryNode = category.CategoryNode;
			if (categoryNode != null)
			{
				AIMLCategory categoryForCategoryNode = this._arbeitsbereich.GetCategoryForCategoryNode(categoryNode);
				if (categoryForCategoryNode != null)
				{
					this._arbeitsbereich.Fokus.AktAIMLCategory = categoryForCategoryNode;
				}
			}
		}

		private void textBoxBenutzerEingabe_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyData == Keys.Return)
			{
				this.BenutzerEingabe(this.textBoxBenutzerEingabe.Text);
			}
		}

		private void BitteWartenZeigen(bool zeigen)
		{
			try
			{
				this.toolStripLabelWissenWirdGeladen.Visible = zeigen;
				this.toolStripProgressBarWissenLaden.Visible = zeigen;
				this.toolStripSeparatorBitteWarten.Visible = zeigen;
				this.toolStripButtonLadeProtokollZeigen.Visible = !zeigen;
			}
			catch (Exception)
			{
				if (!ToolboxSonstiges.IstEntwicklungsmodus)
				{
					goto end_IL_003c;
				}
				throw new ApplicationException(global::MultiLang.ml.ml_string(69, "Wahrscheinlich wird das Programm gerade während des Ladens geschlossen?"));
				end_IL_003c:;
			}
		}

		private void _arbeitsbereich_WissenSpracheChangedEvent(object sender, EventArgs e)
		{
			this._bot = null;
			this.BegruessungAnzeigen();
		}

		private void BegruessungAnzeigen()
		{
			this.BenutzerEingabe("TARGET BOTSTART");
		}

		private void _arbeitsbereich_UseOneWordSRAIChanged(object sender, EventArgs e)
		{
			this._bot = null;
		}

		private void _arbeitsbereich_AIMLDateienContentChanged(object sender, EventArgs e)
		{
			this._bot = null;
		}

		private void _arbeitsbereich_MetaInfosChangedEvent(object sender, EventArgs e)
		{
			this._bot = null;
		}

		private void toolStripButtonLadeProtokollZeigen_Click(object sender, EventArgs e)
		{
			if (this._bot != null)
			{
				MessageBox.Show(this._bot.LadeProtokoll);
			}
		}

		private void toolStripButtonZeigeStart_Click(object sender, EventArgs e)
		{
			this.BegruessungAnzeigen();
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ucBotTest));
			this.textBoxBenutzerEingabe = new TextBox();
			this.buttonAbsenden = new Button();
			this.toolStrip = new ToolStrip();
			this.toolStripButtonZeigeStart = new ToolStripButton();
			this.toolStripSeparator1 = new ToolStripSeparator();
			this.toolStripButtonWissenNeuLaden = new ToolStripButton();
			this.toolStripSeparatorBitteWarten = new ToolStripSeparator();
			this.toolStripLabelWissenWirdGeladen = new ToolStripLabel();
			this.toolStripProgressBarWissenLaden = new ToolStripProgressBar();
			this.toolStripButtonLadeProtokollZeigen = new ToolStripButton();
			this.webBrowserBotAusgabe = new WebBrowser();
			this.ucBotDenkprotokoll = new ucBotDenkprotokoll();
			this.toolStrip.SuspendLayout();
			base.SuspendLayout();
			componentResourceManager.ApplyResources(this.textBoxBenutzerEingabe, "textBoxBenutzerEingabe");
			this.textBoxBenutzerEingabe.Name = "textBoxBenutzerEingabe";
			componentResourceManager.ApplyResources(this.buttonAbsenden, "buttonAbsenden");
			this.buttonAbsenden.Name = "buttonAbsenden";
			this.buttonAbsenden.UseVisualStyleBackColor = true;
			this.buttonAbsenden.Click += this.buttonAbsenden_Click;
			this.toolStrip.Items.AddRange(new ToolStripItem[7]
			{
				this.toolStripButtonZeigeStart,
				this.toolStripSeparator1,
				this.toolStripButtonWissenNeuLaden,
				this.toolStripSeparatorBitteWarten,
				this.toolStripLabelWissenWirdGeladen,
				this.toolStripProgressBarWissenLaden,
				this.toolStripButtonLadeProtokollZeigen
			});
			componentResourceManager.ApplyResources(this.toolStrip, "toolStrip");
			this.toolStrip.Name = "toolStrip";
			this.toolStripButtonZeigeStart.DisplayStyle = ToolStripItemDisplayStyle.Text;
			componentResourceManager.ApplyResources(this.toolStripButtonZeigeStart, "toolStripButtonZeigeStart");
			this.toolStripButtonZeigeStart.Name = "toolStripButtonZeigeStart";
			this.toolStripButtonZeigeStart.Click += this.toolStripButtonZeigeStart_Click;
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			componentResourceManager.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
			this.toolStripButtonWissenNeuLaden.Image = Resources.undo_16;
			componentResourceManager.ApplyResources(this.toolStripButtonWissenNeuLaden, "toolStripButtonWissenNeuLaden");
			this.toolStripButtonWissenNeuLaden.Name = "toolStripButtonWissenNeuLaden";
			this.toolStripButtonWissenNeuLaden.Click += this.toolStripButtonWissenNeuLaden_Click;
			this.toolStripSeparatorBitteWarten.Name = "toolStripSeparatorBitteWarten";
			componentResourceManager.ApplyResources(this.toolStripSeparatorBitteWarten, "toolStripSeparatorBitteWarten");
			this.toolStripLabelWissenWirdGeladen.Name = "toolStripLabelWissenWirdGeladen";
			componentResourceManager.ApplyResources(this.toolStripLabelWissenWirdGeladen, "toolStripLabelWissenWirdGeladen");
			this.toolStripProgressBarWissenLaden.Name = "toolStripProgressBarWissenLaden";
			componentResourceManager.ApplyResources(this.toolStripProgressBarWissenLaden, "toolStripProgressBarWissenLaden");
			this.toolStripButtonLadeProtokollZeigen.Image = Resources.documents_161;
			componentResourceManager.ApplyResources(this.toolStripButtonLadeProtokollZeigen, "toolStripButtonLadeProtokollZeigen");
			this.toolStripButtonLadeProtokollZeigen.Name = "toolStripButtonLadeProtokollZeigen";
			this.toolStripButtonLadeProtokollZeigen.Click += this.toolStripButtonLadeProtokollZeigen_Click;
			componentResourceManager.ApplyResources(this.webBrowserBotAusgabe, "webBrowserBotAusgabe");
			this.webBrowserBotAusgabe.MinimumSize = new Size(20, 20);
			this.webBrowserBotAusgabe.Name = "webBrowserBotAusgabe";
			componentResourceManager.ApplyResources(this.ucBotDenkprotokoll, "ucBotDenkprotokoll");
			this.ucBotDenkprotokoll.Name = "ucBotDenkprotokoll";
			componentResourceManager.ApplyResources(this, "$this");
			base.AutoScaleMode = AutoScaleMode.Font;
			base.Controls.Add(this.textBoxBenutzerEingabe);
			base.Controls.Add(this.ucBotDenkprotokoll);
			base.Controls.Add(this.toolStrip);
			base.Controls.Add(this.buttonAbsenden);
			base.Controls.Add(this.webBrowserBotAusgabe);
			base.Name = "ucBotTest";
			this.toolStrip.ResumeLayout(false);
			this.toolStrip.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
