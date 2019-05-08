using de.springwald.toolbox;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace GaitoBotEditorCore
{
	public class Suchen : UserControl
	{
		public class SuchTitelChangedEventArgs : EventArgs
		{
			public string Titel;

			public SuchTitelChangedEventArgs(string titel)
			{
				this.Titel = titel;
			}
		}

		public delegate void SuchTitelChangedEventHandler(object sender, SuchTitelChangedEventArgs e);

		private Arbeitsbereich _arbeitsbereich;

		private bool _sucheLaeuft;

		private ArrayList _treffer;

		private string _titel;

		private string _wonachGesucht;

		private Arbeitsbereich.WoSuchenOrte _woGesucht;

		private IContainer components = null;

		private ToolStrip toolStripSuchen;

		private ToolStripTextBox txtboxSucheingabe;

		private ToolStripProgressBar toolStripProgressBarSuchen;

		private ToolStripSplitButton toolStripSplitButtonSucheImArbeitsbereich;

		private ToolStripMenuItem searchInActualAimlfileToolStripMenuItem;

		private ToolStripMenuItem searchInActualTopicToolStripMenuItem;

		private DataGridView dataGridViewTrefferliste;

		private string SuchBeschreibung
		{
			get
			{
				if (this._wonachGesucht == null)
				{
					return ResReader.Reader.GetString("NochKeineSucheAusgefuehrt");
				}
				return string.Format(ResReader.Reader.GetString("SucheNachIn"), this._wonachGesucht, Arbeitsbereich.WoSuchenOrt2Name(this._woGesucht));
			}
		}

		public Arbeitsbereich Arbeitsbereich
		{
			set
			{
				if (this._arbeitsbereich != null)
				{
					throw new ApplicationException("Diesem SuchControl wurde bereits ein Arbeitsbereich zugeordnet");
				}
				this._arbeitsbereich = value;
				this._arbeitsbereich.SucheImArbeitsbereich += this._arbeitsbereich_SucheImArbeitsbereich;
			}
		}

		public string Titel
		{
			get
			{
				return this._titel;
			}
			set
			{
				bool flag = this._titel != value;
				this._titel = value;
				if (flag)
				{
					this.SuchTitelChangedEvent(this._titel);
				}
			}
		}

		public event SuchTitelChangedEventHandler SuchTitelChanged;

		protected virtual void SuchTitelChangedEvent(string titel)
		{
			if (this.SuchTitelChanged != null)
			{
				this.SuchTitelChanged(this, new SuchTitelChangedEventArgs(titel));
			}
		}

		public Suchen()
		{
			this.InitializeComponent();
			this.dataGridViewTrefferliste.Columns.Add(ResReader.Reader.GetString("SuchenCellAIMLDatei"), ResReader.Reader.GetString("SuchenCellAIMLDatei"));
			this.dataGridViewTrefferliste.Columns.Add(ResReader.Reader.GetString("SuchenCellAIMLDateiThema"), ResReader.Reader.GetString("SuchenCellAIMLDateiThema"));
			this.dataGridViewTrefferliste.Columns.Add(ResReader.Reader.GetString("SuchenCellAIMLCategory"), ResReader.Reader.GetString("SuchenCellAIMLCategory"));
			this.dataGridViewTrefferliste.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
			this.dataGridViewTrefferliste.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
			this.dataGridViewTrefferliste.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
		}

		private void Suchen_Load(object sender, EventArgs e)
		{
			this.SteuerElementeBeschriften();
			this.SucheFertig();
			this.dataGridViewTrefferliste.AllowUserToAddRows = false;
			this.dataGridViewTrefferliste.AllowUserToResizeColumns = false;
			this.dataGridViewTrefferliste.AllowUserToResizeRows = false;
			this.dataGridViewTrefferliste.RowHeadersVisible = false;
			this.dataGridViewTrefferliste.MultiSelect = false;
			this.dataGridViewTrefferliste.ScrollBars = ScrollBars.Both;
			this.dataGridViewTrefferliste.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
			this.dataGridViewTrefferliste.Scroll += this.dataGridViewTrefferliste_Scroll;
			this.dataGridViewTrefferliste.SelectionChanged += this.dataGridViewTrefferliste_SelectionChanged;
			this.txtboxSucheingabe.KeyDown += this.txtboxSucheingabe_KeyDown;
			base.Resize += this.Suchen_Resize;
			this.ResizeAll();
		}

		private void Suchen_Resize(object sender, EventArgs e)
		{
			this.ResizeAll();
		}

		private void txtboxSucheingabe_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Return)
			{
				this.SucheStarten(this.txtboxSucheingabe.Text, Arbeitsbereich.WoSuchenOrte.ImArbeitsbereich);
			}
		}

		private void ResizeAll()
		{
			this.dataGridViewTrefferliste.Top = this.toolStripSuchen.Top + this.toolStripSuchen.Height;
			this.dataGridViewTrefferliste.Left = 0;
			DataGridView dataGridView = this.dataGridViewTrefferliste;
			Size clientSize = base.ClientSize;
			dataGridView.Height = clientSize.Height - this.dataGridViewTrefferliste.Top;
			DataGridView dataGridView2 = this.dataGridViewTrefferliste;
			clientSize = base.ClientSize;
			dataGridView2.Width = clientSize.Width;
			clientSize = this.dataGridViewTrefferliste.ClientSize;
			int num = clientSize.Width - 3;
			int num2 = num;
			if (this.dataGridViewTrefferliste.Columns.Count == 3)
			{
				this.dataGridViewTrefferliste.Columns[0].Width = num / 7;
				num2 -= this.dataGridViewTrefferliste.Columns[0].Width;
				this.dataGridViewTrefferliste.Columns[1].Width = num / 7;
				num2 -= this.dataGridViewTrefferliste.Columns[1].Width;
				this.dataGridViewTrefferliste.Columns[2].Width = num2;
			}
			this.ZeilenInhalteRefreshen();
		}

		private void _arbeitsbereich_SucheImArbeitsbereich(object sender, Arbeitsbereich.SucheImArbeitsbereichEventArgs e)
		{
			this.txtboxSucheingabe.Text = e.SuchEingabe;
			this.SucheStarten(e.SuchEingabe, e.WoSuchen);
		}

		private void SucheStarten(string suchEingabe, Arbeitsbereich.WoSuchenOrte woSuchen)
		{
			if (this._sucheLaeuft)
			{
				this.SuchFehlerZeigen(ResReader.Reader.GetString("EsLaeuftBereitsEineSuche"));
			}
			else
			{
				this.Titel = this.SuchBeschreibung;
				if (this.IstSuchEingabeGueltig(suchEingabe))
				{
					this._sucheLaeuft = true;
					this._wonachGesucht = suchEingabe.Trim();
					this._woGesucht = woSuchen;
					this.toolStripProgressBarSuchen.Value = 0;
					this.toolStripProgressBarSuchen.Maximum = this._arbeitsbereich.Dateiverwaltung.AlleAimlDateien.Count;
					this.toolStripProgressBarSuchen.Visible = false;
					this.toolStripSuchen.Enabled = false;
					this.dataGridViewTrefferliste.Visible = false;
					this._treffer = new ArrayList();
					switch (woSuchen)
					{
					case Arbeitsbereich.WoSuchenOrte.ImArbeitsbereich:
						this.ArbeitsbereichDurchsuchen(this._arbeitsbereich, suchEingabe);
						break;
					case Arbeitsbereich.WoSuchenOrte.InAktuellerAIMLDatei:
						if (this._arbeitsbereich.Fokus.AktDatei is AIMLDatei)
						{
							this.AIMLDateiDurchsuchen((AIMLDatei)this._arbeitsbereich.Fokus.AktDatei, suchEingabe);
						}
						break;
					case Arbeitsbereich.WoSuchenOrte.InAktuellemTopic:
						this.AIMLTopicDurchsuchen(this._arbeitsbereich.Fokus.AktAIMLTopic, suchEingabe);
						break;
					default:
						throw new ApplicationException("Unbekannter Suchort " + woSuchen + "!");
					}
					this.SucheFertig();
				}
			}
		}

		private void SucheFertig()
		{
			this.toolStripProgressBarSuchen.Visible = false;
			this.toolStripSuchen.Enabled = true;
			int num;
			if (this._treffer == null || this._treffer.Count == 0)
			{
				num = 0;
				this.dataGridViewTrefferliste.Visible = false;
			}
			else
			{
				num = this._treffer.Count;
				this.dataGridViewTrefferliste.Rows.Clear();
				this.dataGridViewTrefferliste.Rows.Add(this._treffer.Count);
				this.dataGridViewTrefferliste.Visible = true;
				this.ResizeAll();
				this.ZeilenInhalteRefreshen();
			}
			this.Titel = string.Format(ResReader.Reader.GetString("SuchTitelMitTreffer"), this.SuchBeschreibung, num);
			this._sucheLaeuft = false;
		}

		private void ZeilenInhalteRefreshen()
		{
			if (this._treffer != null && this._treffer.Count != 0)
			{
				int firstDisplayedScrollingRowIndex = this.dataGridViewTrefferliste.FirstDisplayedScrollingRowIndex;
				int val = firstDisplayedScrollingRowIndex + this.dataGridViewTrefferliste.DisplayedRowCount(true);
				firstDisplayedScrollingRowIndex = Math.Max(0, firstDisplayedScrollingRowIndex);
				val = Math.Min(val, this._treffer.Count);
				for (int i = firstDisplayedScrollingRowIndex; i < val; i++)
				{
					if (this._treffer[i] is AIMLCategory)
					{
						AIMLCategory aIMLCategory = (AIMLCategory)this._treffer[i];
						this.dataGridViewTrefferliste.Rows[i].Cells[0].Value = aIMLCategory.AIMLTopic.AIMLDatei.Titel;
						this.dataGridViewTrefferliste.Rows[i].Cells[1].Value = aIMLCategory.AIMLTopic.Name;
						this.dataGridViewTrefferliste.Rows[i].Cells[2].Value = aIMLCategory.AutoKomplettZusammenfassung;
					}
				}
				Application.DoEvents();
			}
		}

		private bool IstSuchEingabeGueltig(string suchEingabe)
		{
			if (suchEingabe == null)
			{
				this.SuchFehlerZeigen(string.Format(ResReader.Reader.GetString("UngueltigeSuchEingabe"), ""));
				return false;
			}
			suchEingabe = suchEingabe.Trim();
			if (suchEingabe == "")
			{
				this.SuchFehlerZeigen(string.Format(ResReader.Reader.GetString("UngueltigeSuchEingabe"), ""));
				return false;
			}
			try
			{
				string text = "hallo test test";
				if (text.IndexOf(suchEingabe, StringComparison.OrdinalIgnoreCase) == -1)
				{
					goto end_IL_006e;
				}
				end_IL_006e:;
			}
			catch (Exception)
			{
				this.SuchFehlerZeigen(string.Format(ResReader.Reader.GetString("UngueltigeSuchEingabe"), suchEingabe));
				return false;
			}
			return true;
		}

		private void SuchFehlerZeigen(string fehler)
		{
			MessageBox.Show(fehler);
			Debugger.GlobalDebugger.Protokolliere("Suchfehler:" + fehler);
		}

		private void ArbeitsbereichDurchsuchen(Arbeitsbereich arbeitsbereich, string suchEingabe)
		{
			if (arbeitsbereich != null)
			{
				foreach (IArbeitsbereichDatei item in arbeitsbereich.Dateiverwaltung.Dateien)
				{
					if (item is AIMLDatei)
					{
						this.AIMLDateiDurchsuchen((AIMLDatei)item, suchEingabe);
					}
				}
			}
		}

		private void AIMLDateiDurchsuchen(AIMLDatei datei, string suchEingabe)
		{
			if (datei != null)
			{
				try
				{
					this.toolStripProgressBarSuchen.Value++;
				}
				catch (Exception)
				{
				}
				Application.DoEvents();
				foreach (AIMLTopic topic in datei.getTopics())
				{
					this.AIMLTopicDurchsuchen(topic, suchEingabe);
				}
			}
		}

		private void AIMLTopicDurchsuchen(AIMLTopic topic, string suchEingabe)
		{
			if (topic != null)
			{
				foreach (AIMLCategory category in topic.Categories)
				{
					this.AIMLCategoryDurchsuchen(category, suchEingabe);
				}
			}
		}

		private void AIMLCategoryDurchsuchen(AIMLCategory category, string suchEingabe)
		{
			try
			{
				if (category.ContentNode.InnerXml.IndexOf(suchEingabe, StringComparison.OrdinalIgnoreCase) != -1)
				{
					this._treffer.Add(category);
				}
			}
			catch (Exception)
			{
			}
		}

		private void SteuerElementeBeschriften()
		{
			this.toolStripSplitButtonSucheImArbeitsbereich.Text = ResReader.Reader.GetString("toolStripSplitButtonSucheImArbeitsbereich");
			this.searchInActualAimlfileToolStripMenuItem.Text = ResReader.Reader.GetString("searchInActualAimlfileToolStripMenuItem");
			this.searchInActualTopicToolStripMenuItem.Text = ResReader.Reader.GetString("searchInActualTopicToolStripMenuItem");
		}

		private void dataGridViewTrefferliste_SelectionChanged(object sender, EventArgs e)
		{
			if (this._treffer != null && this._treffer.Count != 0 && this.dataGridViewTrefferliste.SelectedRows.Count == 1)
			{
				int index = this.dataGridViewTrefferliste.SelectedRows[0].Index;
				if (index <= this._treffer.Count - 1)
				{
					if (this._treffer[index] is AIMLCategory)
					{
						AIMLCategory aktAIMLCategory = (AIMLCategory)this._treffer[index];
						this._arbeitsbereich.Fokus.AktAIMLCategory = aktAIMLCategory;
					}
					Application.DoEvents();
				}
			}
		}

		private void dataGridViewTrefferliste_Scroll(object sender, ScrollEventArgs e)
		{
			this.ZeilenInhalteRefreshen();
		}

		private void toolStripSplitButtonSucheImArbeitsbereich_ButtonClick(object sender, EventArgs e)
		{
			this.SucheStarten(this.txtboxSucheingabe.Text, Arbeitsbereich.WoSuchenOrte.ImArbeitsbereich);
		}

		private void searchInActualAimlfileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.SucheStarten(this.txtboxSucheingabe.Text, Arbeitsbereich.WoSuchenOrte.InAktuellerAIMLDatei);
		}

		private void searchInActualTopicToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.SucheStarten(this.txtboxSucheingabe.Text, Arbeitsbereich.WoSuchenOrte.InAktuellemTopic);
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(Suchen));
			this.toolStripSuchen = new ToolStrip();
			this.txtboxSucheingabe = new ToolStripTextBox();
			this.toolStripSplitButtonSucheImArbeitsbereich = new ToolStripSplitButton();
			this.searchInActualAimlfileToolStripMenuItem = new ToolStripMenuItem();
			this.searchInActualTopicToolStripMenuItem = new ToolStripMenuItem();
			this.toolStripProgressBarSuchen = new ToolStripProgressBar();
			this.dataGridViewTrefferliste = new DataGridView();
			this.toolStripSuchen.SuspendLayout();
			((ISupportInitialize)this.dataGridViewTrefferliste).BeginInit();
			base.SuspendLayout();
			this.toolStripSuchen.Items.AddRange(new ToolStripItem[3]
			{
				this.txtboxSucheingabe,
				this.toolStripSplitButtonSucheImArbeitsbereich,
				this.toolStripProgressBarSuchen
			});
			componentResourceManager.ApplyResources(this.toolStripSuchen, "toolStripSuchen");
			this.toolStripSuchen.Name = "toolStripSuchen";
			this.txtboxSucheingabe.Name = "txtboxSucheingabe";
			componentResourceManager.ApplyResources(this.txtboxSucheingabe, "txtboxSucheingabe");
			this.toolStripSplitButtonSucheImArbeitsbereich.DisplayStyle = ToolStripItemDisplayStyle.Text;
			this.toolStripSplitButtonSucheImArbeitsbereich.DropDownItems.AddRange(new ToolStripItem[2]
			{
				this.searchInActualAimlfileToolStripMenuItem,
				this.searchInActualTopicToolStripMenuItem
			});
			componentResourceManager.ApplyResources(this.toolStripSplitButtonSucheImArbeitsbereich, "toolStripSplitButtonSucheImArbeitsbereich");
			this.toolStripSplitButtonSucheImArbeitsbereich.Name = "toolStripSplitButtonSucheImArbeitsbereich";
			this.toolStripSplitButtonSucheImArbeitsbereich.ButtonClick += this.toolStripSplitButtonSucheImArbeitsbereich_ButtonClick;
			this.searchInActualAimlfileToolStripMenuItem.Name = "searchInActualAimlfileToolStripMenuItem";
			componentResourceManager.ApplyResources(this.searchInActualAimlfileToolStripMenuItem, "searchInActualAimlfileToolStripMenuItem");
			this.searchInActualAimlfileToolStripMenuItem.Click += this.searchInActualAimlfileToolStripMenuItem_Click;
			this.searchInActualTopicToolStripMenuItem.Name = "searchInActualTopicToolStripMenuItem";
			componentResourceManager.ApplyResources(this.searchInActualTopicToolStripMenuItem, "searchInActualTopicToolStripMenuItem");
			this.searchInActualTopicToolStripMenuItem.Click += this.searchInActualTopicToolStripMenuItem_Click;
			this.toolStripProgressBarSuchen.ForeColor = Color.FromArgb(128, 128, 255);
			this.toolStripProgressBarSuchen.Name = "toolStripProgressBarSuchen";
			componentResourceManager.ApplyResources(this.toolStripProgressBarSuchen, "toolStripProgressBarSuchen");
			this.toolStripProgressBarSuchen.Value = 50;
			this.dataGridViewTrefferliste.AllowUserToAddRows = false;
			this.dataGridViewTrefferliste.AllowUserToDeleteRows = false;
			this.dataGridViewTrefferliste.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			componentResourceManager.ApplyResources(this.dataGridViewTrefferliste, "dataGridViewTrefferliste");
			this.dataGridViewTrefferliste.Name = "dataGridViewTrefferliste";
			this.dataGridViewTrefferliste.ReadOnly = true;
			componentResourceManager.ApplyResources(this, "$this");
			base.AutoScaleMode = AutoScaleMode.Font;
			base.Controls.Add(this.dataGridViewTrefferliste);
			base.Controls.Add(this.toolStripSuchen);
			base.Name = "Suchen";
			base.Load += this.Suchen_Load;
			this.toolStripSuchen.ResumeLayout(false);
			this.toolStripSuchen.PerformLayout();
			((ISupportInitialize)this.dataGridViewTrefferliste).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
