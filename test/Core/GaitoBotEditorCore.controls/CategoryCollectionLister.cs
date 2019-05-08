using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace GaitoBotEditorCore.controls
{
	public class CategoryCollectionLister : UserControl
	{
		private static object _lock = new object();

		private bool _zeilenWerdenGeradeAufgefuellt;

		private bool _wirdGeradeNeuGezeichnet;

		private List<AIMLCategory> _categories;

		private IContainer components = null;

		private DataGridView dataGridViewCategorien;

		public List<AIMLCategory> Categories
		{
			set
			{
				this._categories = value;
				if (this._categories == null || this._categories.Count == 0)
				{
					this.dataGridViewCategorien.Visible = false;
				}
				else
				{
					this.NeuZeichnen();
				}
				this.ResizeAll();
			}
		}

		private void NeuZeichnen()
		{
			if (!this._wirdGeradeNeuGezeichnet)
			{
				this._wirdGeradeNeuGezeichnet = true;
				if (this._categories == null || this._categories.Count == 0)
				{
					this.AnzahlZeilenAuffuellen(0);
				}
				else
				{
					this.AnzahlZeilenAuffuellen(this._categories.Count);
				}
				this.dataGridViewCategorien.Visible = true;
				this.ZeilenInhalteRefreshen();
				this._wirdGeradeNeuGezeichnet = false;
			}
		}

		public CategoryCollectionLister()
		{
			this.InitializeComponent();
			this.dataGridViewCategorien.Columns.Add(ResReader.Reader.GetString("CategoryListeCellAIMLDatei"), ResReader.Reader.GetString("SuchenCellAIMLDatei"));
			this.dataGridViewCategorien.Columns.Add(ResReader.Reader.GetString("CategoryListeCellAIMLDateiThema"), ResReader.Reader.GetString("SuchenCellAIMLDateiThema"));
			this.dataGridViewCategorien.Columns.Add(ResReader.Reader.GetString("CategoryListeCellAIMLCategory"), ResReader.Reader.GetString("SuchenCellAIMLCategory"));
			this.dataGridViewCategorien.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
			this.dataGridViewCategorien.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
			this.dataGridViewCategorien.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
			base.Resize += this.CategoryCollectionLister_Resize;
		}

		private void CategoryCollectionLister_Load(object sender, EventArgs e)
		{
			this.dataGridViewCategorien.AllowUserToAddRows = false;
			this.dataGridViewCategorien.AllowUserToResizeColumns = false;
			this.dataGridViewCategorien.AllowUserToResizeRows = false;
			this.dataGridViewCategorien.RowHeadersVisible = false;
			this.dataGridViewCategorien.MultiSelect = false;
			this.dataGridViewCategorien.ScrollBars = ScrollBars.Both;
			this.dataGridViewCategorien.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
			this.dataGridViewCategorien.Scroll += this.dataGridViewCategorien_Scroll;
			this.dataGridViewCategorien.SelectionChanged += this.dataGridViewCategorien_SelectionChanged;
			this.ResizeAll();
		}

		private void ZeilenInhalteRefreshen()
		{
			if (!this._zeilenWerdenGeradeAufgefuellt && this._categories != null && this._categories.Count != 0)
			{
				int firstDisplayedScrollingRowIndex = this.dataGridViewCategorien.FirstDisplayedScrollingRowIndex;
				int val = firstDisplayedScrollingRowIndex + this.dataGridViewCategorien.DisplayedRowCount(true);
				firstDisplayedScrollingRowIndex = Math.Max(0, firstDisplayedScrollingRowIndex);
				val = Math.Min(val, this._categories.Count);
				for (int i = firstDisplayedScrollingRowIndex; i < val; i++)
				{
					AIMLCategory aIMLCategory = this._categories[i];
					this.dataGridViewCategorien.Rows[i].Cells[0].Value = aIMLCategory.AIMLTopic.AIMLDatei.Titel;
					this.dataGridViewCategorien.Rows[i].Cells[1].Value = aIMLCategory.AIMLTopic.Name;
					this.dataGridViewCategorien.Rows[i].Cells[2].Value = aIMLCategory.AutoKomplettZusammenfassung;
				}
				Application.DoEvents();
			}
		}

		private void ResizeAll()
		{
			this.dataGridViewCategorien.Left = 0;
			this.dataGridViewCategorien.Top = 0;
			DataGridView dataGridView = this.dataGridViewCategorien;
			Size clientSize = base.ClientSize;
			dataGridView.Height = clientSize.Height;
			DataGridView dataGridView2 = this.dataGridViewCategorien;
			clientSize = base.ClientSize;
			dataGridView2.Width = clientSize.Width;
			clientSize = this.dataGridViewCategorien.ClientSize;
			int num = clientSize.Width - 3;
			int num2 = num;
			if (this.dataGridViewCategorien.Columns.Count == 3)
			{
				this.dataGridViewCategorien.Columns[0].Width = num / 7;
				num2 -= this.dataGridViewCategorien.Columns[0].Width;
				this.dataGridViewCategorien.Columns[1].Width = num / 7;
				num2 -= this.dataGridViewCategorien.Columns[1].Width;
				this.dataGridViewCategorien.Columns[2].Width = num2;
			}
			this.ZeilenInhalteRefreshen();
		}

		private void CategoryCollectionLister_Resize(object sender, EventArgs e)
		{
			this.ResizeAll();
		}

		private void dataGridViewCategorien_SelectionChanged(object sender, EventArgs e)
		{
			if (this._categories != null && this._categories.Count != 0 && this.dataGridViewCategorien.SelectedRows.Count == 1)
			{
				int index = this.dataGridViewCategorien.SelectedRows[0].Index;
				if (index <= this._categories.Count - 1)
				{
					if (this._categories[index] != null)
					{
						AIMLCategory aIMLCategory = this._categories[index];
						aIMLCategory.AIMLTopic.AIMLDatei.Arbeitsbereich.Fokus.AktAIMLCategory = aIMLCategory;
					}
					Application.DoEvents();
				}
			}
		}

		private void dataGridViewCategorien_Scroll(object sender, ScrollEventArgs e)
		{
			this.ZeilenInhalteRefreshen();
		}

		private void AnzahlZeilenAuffuellen(int anzahlKategorien)
		{
			object @lock = CategoryCollectionLister._lock;
			Monitor.Enter(@lock);
			try
			{
				this._zeilenWerdenGeradeAufgefuellt = true;
				if (anzahlKategorien == 0)
				{
					this.dataGridViewCategorien.Rows.Clear();
				}
				int num = anzahlKategorien - this.dataGridViewCategorien.Rows.Count;
				if (num < 0 && -num > anzahlKategorien)
				{
					this.dataGridViewCategorien.Rows.Clear();
				}
				if (anzahlKategorien > this.dataGridViewCategorien.Rows.Count)
				{
					int count = anzahlKategorien - this.dataGridViewCategorien.Rows.Count;
					this.dataGridViewCategorien.Rows.Add(count);
				}
				while (anzahlKategorien < this.dataGridViewCategorien.Rows.Count)
				{
					this.dataGridViewCategorien.Rows.RemoveAt(this.dataGridViewCategorien.Rows.Count - 1);
				}
				this._zeilenWerdenGeradeAufgefuellt = false;
			}
			finally
			{
				Monitor.Exit(@lock);
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(CategoryCollectionLister));
			this.dataGridViewCategorien = new DataGridView();
			((ISupportInitialize)this.dataGridViewCategorien).BeginInit();
			base.SuspendLayout();
			this.dataGridViewCategorien.AllowUserToAddRows = false;
			this.dataGridViewCategorien.AllowUserToDeleteRows = false;
			this.dataGridViewCategorien.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			componentResourceManager.ApplyResources(this.dataGridViewCategorien, "dataGridViewCategorien");
			this.dataGridViewCategorien.Name = "dataGridViewCategorien";
			this.dataGridViewCategorien.ReadOnly = true;
			componentResourceManager.ApplyResources(this, "$this");
			base.AutoScaleMode = AutoScaleMode.Font;
			base.Controls.Add(this.dataGridViewCategorien);
			base.Name = "CategoryCollectionLister";
			base.Load += this.CategoryCollectionLister_Load;
			((ISupportInitialize)this.dataGridViewCategorien).EndInit();
			base.ResumeLayout(false);
		}
	}
}
