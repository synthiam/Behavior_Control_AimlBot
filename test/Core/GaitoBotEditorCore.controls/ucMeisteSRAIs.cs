using de.springwald.gaitobot2;
using de.springwald.toolbox;
using GaitoBotEditorCore.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace GaitoBotEditorCore.controls
{
	public class ucMeisteSRAIs : UserControl
	{
		private Arbeitsbereich _arbeitsbereich;

		private IContainer components = null;

		private ToolStrip toolStripTop;

		private DataGridView dataGridViewBesteSRAIs;

		private ToolStripButton toolStripButtonRefresh;

		private ToolStripProgressBar toolStripProgressBar;

		private CategoryCollectionLister categoryCollectionLister;

		public Arbeitsbereich Arbeitsbereich
		{
			set
			{
				if (this._arbeitsbereich != null)
				{
					throw new ApplicationException("Diesem BesteSRAIZiele wurde bereits ein Arbeitsbereich zugeordnet");
				}
				this._arbeitsbereich = value;
				this.NeuBerechnen();
			}
		}

		public ucMeisteSRAIs()
		{
			this.InitializeComponent();
			this.toolStripProgressBar.Visible = false;
			this.dataGridViewBesteSRAIs.Columns.Add(ResReader.Reader.GetString("BestSRAIAnzahl"), ResReader.Reader.GetString("BestSRAIAnzahl"));
			this.dataGridViewBesteSRAIs.Columns.Add(ResReader.Reader.GetString("BestSRAISRAI"), ResReader.Reader.GetString("BestSRAISRAI"));
			this.dataGridViewBesteSRAIs.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
			this.dataGridViewBesteSRAIs.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
			base.Resize += this.ucBesteSRAIZiele_Resize;
		}

		private void ucBesteSRAIZiele_Load(object sender, EventArgs e)
		{
			this.dataGridViewBesteSRAIs.AllowUserToAddRows = false;
			this.dataGridViewBesteSRAIs.AllowUserToResizeColumns = false;
			this.dataGridViewBesteSRAIs.AllowUserToResizeRows = false;
			this.dataGridViewBesteSRAIs.RowHeadersVisible = false;
			this.dataGridViewBesteSRAIs.MultiSelect = false;
			this.dataGridViewBesteSRAIs.ScrollBars = ScrollBars.Both;
			this.dataGridViewBesteSRAIs.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
			this.dataGridViewBesteSRAIs.SelectionChanged += this.dataGridViewBesteSRAIs_SelectionChanged;
			this.toolStripButtonRefresh.Text = ResReader.Reader.GetString("ucMeisteSRAIStoolStripButtonRefresh");
		}

		public void NeuBerechnen()
		{
			if (this._arbeitsbereich == null)
			{
				this.dataGridViewBesteSRAIs.Visible = false;
			}
			else
			{
				Hashtable hashtable = new Hashtable();
				try
				{
					this.toolStripProgressBar.Maximum = this._arbeitsbereich.Dateiverwaltung.AlleAimlDateien.Count;
					this.toolStripProgressBar.Value = 0;
					this.toolStripProgressBar.Visible = true;
				}
				finally
				{
				}
				foreach (AIMLDatei item in this._arbeitsbereich.Dateiverwaltung.AlleAimlDateien)
				{
					try
					{
						this.toolStripProgressBar.Value++;
					}
					finally
					{
					}
					foreach (AIMLTopic topic in item.getTopics())
					{
						foreach (AIMLCategory category in topic.Categories)
						{
							string[] srais = category.Srais;
							foreach (string text in srais)
							{
								if (text != "*")
								{
									string key = text.ToLower();
									if (hashtable.ContainsKey(key))
									{
										hashtable[key] = (int)hashtable[key] + 1;
									}
									else
									{
										hashtable[key] = 1;
									}
								}
							}
						}
					}
				}
				this.toolStripProgressBar.Visible = false;
				ArrayList arrayList = new ArrayList();
				IDictionaryEnumerator enumerator4 = hashtable.GetEnumerator();
				try
				{
					while (enumerator4.MoveNext())
					{
						DictionaryEntry dictionaryEntry = (DictionaryEntry)enumerator4.Current;
						int sortWert = (int)dictionaryEntry.Value;
						string inhalt = (string)dictionaryEntry.Key;
						arrayList.Add(new IntSorterItem(sortWert, inhalt));
					}
				}
				finally
				{
					IDisposable disposable = enumerator4 as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}
				}
				arrayList.Sort(new IntSorterComparerReverse());
				this.dataGridViewBesteSRAIs.Rows.Clear();
				if (arrayList.Count != 0)
				{
					int num = Math.Min(50, arrayList.Count);
					this.dataGridViewBesteSRAIs.Rows.Add(num);
					for (int j = 0; j < num; j++)
					{
						this.dataGridViewBesteSRAIs.Rows[j].Cells[1].Value = (string)((IntSorterItem)arrayList[j]).Inhalt;
						this.dataGridViewBesteSRAIs.Rows[j].Cells[0].Value = ((IntSorterItem)arrayList[j]).SortWert.ToString();
					}
				}
				this.dataGridViewBesteSRAIs.Visible = true;
			}
			this.AktuelleCategoryListeAnzeigen();
		}

		private void dataGridViewBesteSRAIs_SelectionChanged(object sender, EventArgs e)
		{
			this.AktuelleCategoryListeAnzeigen();
		}

		private void AktuelleCategoryListeAnzeigen()
		{
			if (this.dataGridViewBesteSRAIs.SelectedRows.Count != 1)
			{
				this.categoryCollectionLister.Categories = null;
			}
			else if (this.dataGridViewBesteSRAIs.SelectedRows[0].Cells[1].Value == null)
			{
				this.categoryCollectionLister.Categories = null;
			}
			else
			{
				string text = this.dataGridViewBesteSRAIs.SelectedRows[0].Cells[1].Value.ToString();
				text = text.ToLower();
				List<AIMLCategory> list = new List<AIMLCategory>();
				this.toolStripProgressBar.Maximum = this._arbeitsbereich.Dateiverwaltung.AlleAimlDateien.Count;
				this.toolStripProgressBar.Value = 0;
				this.toolStripProgressBar.Visible = true;
				foreach (AIMLDatei item in this._arbeitsbereich.Dateiverwaltung.AlleAimlDateien)
				{
					this.toolStripProgressBar.Value++;
					foreach (AIMLTopic topic in item.getTopics())
					{
						foreach (AIMLCategory category in topic.Categories)
						{
							bool flag = false;
							string[] srais = category.Srais;
							foreach (string text2 in srais)
							{
								if (text2.ToLower() == text)
								{
									flag = true;
								}
							}
							if (flag)
							{
								list.Add(category);
							}
						}
					}
				}
				this.toolStripProgressBar.Visible = false;
				this.categoryCollectionLister.Categories = list;
			}
		}

		private void AktuelleCategoryListeAnzeigenAlt()
		{
			if (this.dataGridViewBesteSRAIs.SelectedRows.Count != 1)
			{
				this.categoryCollectionLister.Categories = null;
			}
			else if (this.dataGridViewBesteSRAIs.SelectedRows[0].Cells[1].Value == null)
			{
				this.categoryCollectionLister.Categories = null;
			}
			else
			{
				string text = this.dataGridViewBesteSRAIs.SelectedRows[0].Cells[1].Value.ToString();
				string text2 = Normalisierung.EingabePatternOptimieren(text, true);
				text2 = text.Replace("*", ".*?");
				text2 = "\\A" + text2 + "\\z";
				List<AIMLCategory> list = new List<AIMLCategory>();
				this.toolStripProgressBar.Maximum = this._arbeitsbereich.Dateiverwaltung.AlleAimlDateien.Count;
				this.toolStripProgressBar.Value = 0;
				this.toolStripProgressBar.Visible = true;
				foreach (AIMLDatei item in this._arbeitsbereich.Dateiverwaltung.AlleAimlDateien)
				{
					this.toolStripProgressBar.Value++;
					foreach (AIMLTopic topic in item.getTopics())
					{
						foreach (AIMLCategory category in topic.Categories)
						{
							if (Regex.IsMatch(category.Pattern, text2, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
							{
								list.Add(category);
							}
						}
					}
				}
				this.toolStripProgressBar.Visible = false;
				this.categoryCollectionLister.Categories = list;
			}
		}

		private void toolStripButtonRefresh_Click(object sender, EventArgs e)
		{
			this.NeuBerechnen();
		}

		private void ucBesteSRAIZiele_Resize(object sender, EventArgs e)
		{
			this.dataGridViewBesteSRAIs.Top = this.toolStripTop.Top + this.toolStripTop.Height;
			this.dataGridViewBesteSRAIs.Left = 0;
			DataGridView dataGridView = this.dataGridViewBesteSRAIs;
			Size clientSize = base.ClientSize;
			dataGridView.Height = clientSize.Height - this.toolStripTop.Top - this.toolStripTop.Height;
			DataGridView dataGridView2 = this.dataGridViewBesteSRAIs;
			clientSize = base.ClientSize;
			dataGridView2.Width = clientSize.Width / 3;
			this.ResizeAll();
			this.categoryCollectionLister.Top = this.toolStripTop.Top + this.toolStripTop.Height;
			this.categoryCollectionLister.Left = this.dataGridViewBesteSRAIs.Left + this.dataGridViewBesteSRAIs.Width;
			CategoryCollectionLister obj = this.categoryCollectionLister;
			clientSize = base.ClientSize;
			obj.Height = clientSize.Height - this.toolStripTop.Top - this.toolStripTop.Height;
			CategoryCollectionLister obj2 = this.categoryCollectionLister;
			clientSize = base.ClientSize;
			obj2.Width = clientSize.Width - this.dataGridViewBesteSRAIs.Width;
		}

		private void ResizeAll()
		{
			int num = this.dataGridViewBesteSRAIs.ClientSize.Width - 3;
			int num2 = num;
			if (this.dataGridViewBesteSRAIs.Columns.Count == 2)
			{
				this.dataGridViewBesteSRAIs.Columns[0].Width = 40;
				num2 -= this.dataGridViewBesteSRAIs.Columns[0].Width;
				this.dataGridViewBesteSRAIs.Columns[1].Width = num2;
				num2 -= this.dataGridViewBesteSRAIs.Columns[1].Width;
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ucMeisteSRAIs));
			this.toolStripTop = new ToolStrip();
			this.toolStripButtonRefresh = new ToolStripButton();
			this.toolStripProgressBar = new ToolStripProgressBar();
			this.dataGridViewBesteSRAIs = new DataGridView();
			this.categoryCollectionLister = new CategoryCollectionLister();
			this.toolStripTop.SuspendLayout();
			((ISupportInitialize)this.dataGridViewBesteSRAIs).BeginInit();
			base.SuspendLayout();
			this.toolStripTop.Items.AddRange(new ToolStripItem[2]
			{
				this.toolStripButtonRefresh,
				this.toolStripProgressBar
			});
			componentResourceManager.ApplyResources(this.toolStripTop, "toolStripTop");
			this.toolStripTop.Name = "toolStripTop";
			this.toolStripButtonRefresh.Image = Resources.sync;
			componentResourceManager.ApplyResources(this.toolStripButtonRefresh, "toolStripButtonRefresh");
			this.toolStripButtonRefresh.Name = "toolStripButtonRefresh";
			this.toolStripButtonRefresh.Click += this.toolStripButtonRefresh_Click;
			this.toolStripProgressBar.Name = "toolStripProgressBar";
			componentResourceManager.ApplyResources(this.toolStripProgressBar, "toolStripProgressBar");
			this.dataGridViewBesteSRAIs.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			componentResourceManager.ApplyResources(this.dataGridViewBesteSRAIs, "dataGridViewBesteSRAIs");
			this.dataGridViewBesteSRAIs.Name = "dataGridViewBesteSRAIs";
			componentResourceManager.ApplyResources(this.categoryCollectionLister, "categoryCollectionLister");
			this.categoryCollectionLister.Name = "categoryCollectionLister";
			componentResourceManager.ApplyResources(this, "$this");
			base.AutoScaleMode = AutoScaleMode.Font;
			base.Controls.Add(this.categoryCollectionLister);
			base.Controls.Add(this.dataGridViewBesteSRAIs);
			base.Controls.Add(this.toolStripTop);
			base.Name = "ucMeisteSRAIs";
			base.Load += this.ucBesteSRAIZiele_Load;
			this.toolStripTop.ResumeLayout(false);
			this.toolStripTop.PerformLayout();
			((ISupportInitialize)this.dataGridViewBesteSRAIs).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
