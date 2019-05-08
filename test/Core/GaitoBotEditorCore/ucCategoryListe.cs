using de.springwald.toolbox;
using GaitoBotEditorCore.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

namespace GaitoBotEditorCore
{
	public class ucCategoryListe : UserControl
	{
		private ImageList imgListToolbar;

		private ToolStrip toolStrip1;

		private ToolStripButton toolStripButtonDeleteKategory;

		private DataGridView dataGridViewCategories;

		private ToolStripButton toolStripButtonSortieren;

		private ToolStripSplitButton toolStripButtonNeueKategorie;

		private ToolStripMenuItem toolStripButtonCreateTHATReference;

		private ToolStripMenuItem toolStripButtonPassendesSRAI;

		private ToolStripMenuItem toolStripButtonTHATKlonen;

		private ToolStripMenuItem toolStripButtonCloneTemplate;

		private ToolStripMenuItem cloneToolStripMenuItem;

		private IContainer components;

		private bool _zeilenWerdenGeradeAufgefuellt;

		private bool _wirdGeradeNeuGezeichnet;

		private List<AIMLCategory> _merkeCategories;

		private Arbeitsbereich _arbeitsbereich;

		private AIMLTopic aimlTopic
		{
			get
			{
				if (this._arbeitsbereich == null)
				{
					return null;
				}
				return this._arbeitsbereich.Fokus.AktAIMLTopic;
			}
		}

		private AIMLCategory AktuelleCategory
		{
			get
			{
				if (this._arbeitsbereich == null)
				{
					throw new ApplicationException("Noch kein Arbeitsbereich zugewiesen!");
				}
				return this._arbeitsbereich.Fokus.AktAIMLCategory;
			}
			set
			{
				if (this._arbeitsbereich == null)
				{
					throw new ApplicationException("Noch kein Arbeitsbereich zugewiesen!");
				}
				this._arbeitsbereich.Fokus.AktAIMLCategory = value;
			}
		}

		public Arbeitsbereich Arbeitsbereich
		{
			set
			{
				if (this._arbeitsbereich != null)
				{
					throw new ApplicationException("Dieser AIMLCategoryListe wurde bereits ein Arbeitsbereich zugeordnet");
				}
				this._arbeitsbereich = value;
				this._arbeitsbereich.Fokus.AktAIMLTopicChanged += this.Fokus_AktAIMLTopicChanged;
				this._arbeitsbereich.Fokus.AktAIMLCategoryChanged += this.Fokus_AktAIMLCategoryChanged;
				this.AIMLCategoriesNeuAnzeigen(false);
			}
		}

		private bool DateiIstReadOnly
		{
			get
			{
				return this.aimlTopic != null && this.aimlTopic.AIMLDatei != null && this.aimlTopic.AIMLDatei.ReadOnly;
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ucCategoryListe));
			this.imgListToolbar = new ImageList(this.components);
			this.toolStrip1 = new ToolStrip();
			this.toolStripButtonNeueKategorie = new ToolStripSplitButton();
			this.toolStripButtonCreateTHATReference = new ToolStripMenuItem();
			this.toolStripButtonPassendesSRAI = new ToolStripMenuItem();
			this.toolStripButtonTHATKlonen = new ToolStripMenuItem();
			this.toolStripButtonCloneTemplate = new ToolStripMenuItem();
			this.cloneToolStripMenuItem = new ToolStripMenuItem();
			this.toolStripButtonDeleteKategory = new ToolStripButton();
			this.toolStripButtonSortieren = new ToolStripButton();
			this.dataGridViewCategories = new DataGridView();
			this.toolStrip1.SuspendLayout();
			((ISupportInitialize)this.dataGridViewCategories).BeginInit();
			base.SuspendLayout();
			this.imgListToolbar.ImageStream = (ImageListStreamer)componentResourceManager.GetObject("imgListToolbar.ImageStream");
			this.imgListToolbar.TransparentColor = Color.Transparent;
			this.imgListToolbar.Images.SetKeyName(0, "");
			this.imgListToolbar.Images.SetKeyName(1, "");
			this.toolStrip1.Items.AddRange(new ToolStripItem[3]
			{
				this.toolStripButtonNeueKategorie,
				this.toolStripButtonDeleteKategory,
				this.toolStripButtonSortieren
			});
			componentResourceManager.ApplyResources(this.toolStrip1, "toolStrip1");
			this.toolStrip1.Name = "toolStrip1";
			this.toolStripButtonNeueKategorie.DropDownItems.AddRange(new ToolStripItem[5]
			{
				this.toolStripButtonCreateTHATReference,
				this.toolStripButtonPassendesSRAI,
				this.toolStripButtonTHATKlonen,
				this.toolStripButtonCloneTemplate,
				this.cloneToolStripMenuItem
			});
			this.toolStripButtonNeueKategorie.Image = Resources.add_161;
			componentResourceManager.ApplyResources(this.toolStripButtonNeueKategorie, "toolStripButtonNeueKategorie");
			this.toolStripButtonNeueKategorie.Name = "toolStripButtonNeueKategorie";
			this.toolStripButtonNeueKategorie.ButtonClick += this.toolStripButtonNeueKategorie_ButtonClick;
			this.toolStripButtonCreateTHATReference.Name = "toolStripButtonCreateTHATReference";
			componentResourceManager.ApplyResources(this.toolStripButtonCreateTHATReference, "toolStripButtonCreateTHATReference");
			this.toolStripButtonCreateTHATReference.Click += this.toolStripButtonCreateTHATReference_Click_1;
			this.toolStripButtonPassendesSRAI.Name = "toolStripButtonPassendesSRAI";
			componentResourceManager.ApplyResources(this.toolStripButtonPassendesSRAI, "toolStripButtonPassendesSRAI");
			this.toolStripButtonPassendesSRAI.Click += this.toolStripButtonPassendesSRAI_Click_1;
			this.toolStripButtonTHATKlonen.Name = "toolStripButtonTHATKlonen";
			componentResourceManager.ApplyResources(this.toolStripButtonTHATKlonen, "toolStripButtonTHATKlonen");
			this.toolStripButtonTHATKlonen.Click += this.toolStripButtonTHATKlonen_Click_1;
			this.toolStripButtonCloneTemplate.Name = "toolStripButtonCloneTemplate";
			componentResourceManager.ApplyResources(this.toolStripButtonCloneTemplate, "toolStripButtonCloneTemplate");
			this.toolStripButtonCloneTemplate.Click += this.toolStripButtonCloneTemplate_Click_1;
			this.cloneToolStripMenuItem.Name = "cloneToolStripMenuItem";
			componentResourceManager.ApplyResources(this.cloneToolStripMenuItem, "cloneToolStripMenuItem");
			this.cloneToolStripMenuItem.Click += this.cloneToolStripMenuItem_Click;
			componentResourceManager.ApplyResources(this.toolStripButtonDeleteKategory, "toolStripButtonDeleteKategory");
			this.toolStripButtonDeleteKategory.Name = "toolStripButtonDeleteKategory";
			this.toolStripButtonDeleteKategory.Click += this.toolStripButtonDeleteKategory_Click;
			this.toolStripButtonSortieren.Image = Resources.KEY06;
			componentResourceManager.ApplyResources(this.toolStripButtonSortieren, "toolStripButtonSortieren");
			this.toolStripButtonSortieren.Name = "toolStripButtonSortieren";
			this.toolStripButtonSortieren.Click += this.toolStripButtonSortieren_Click;
			this.dataGridViewCategories.AllowUserToAddRows = false;
			this.dataGridViewCategories.AllowUserToDeleteRows = false;
			this.dataGridViewCategories.AllowUserToResizeColumns = false;
			this.dataGridViewCategories.AllowUserToResizeRows = false;
			this.dataGridViewCategories.BackgroundColor = Color.White;
			this.dataGridViewCategories.ClipboardCopyMode = DataGridViewClipboardCopyMode.Disable;
			this.dataGridViewCategories.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridViewCategories.EditMode = DataGridViewEditMode.EditProgrammatically;
			componentResourceManager.ApplyResources(this.dataGridViewCategories, "dataGridViewCategories");
			this.dataGridViewCategories.MultiSelect = false;
			this.dataGridViewCategories.Name = "dataGridViewCategories";
			this.dataGridViewCategories.ReadOnly = true;
			this.dataGridViewCategories.RowHeadersVisible = false;
			this.dataGridViewCategories.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
			this.dataGridViewCategories.RowTemplate.ReadOnly = true;
			this.dataGridViewCategories.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
			this.dataGridViewCategories.ShowCellErrors = false;
			this.dataGridViewCategories.ShowCellToolTips = false;
			this.dataGridViewCategories.ShowEditingIcon = false;
			this.dataGridViewCategories.ShowRowErrors = false;
			this.dataGridViewCategories.CellContentClick += this.dataGridViewCategories_CellContentClick;
			base.Controls.Add(this.dataGridViewCategories);
			base.Controls.Add(this.toolStrip1);
			this.DoubleBuffered = true;
			base.Name = "ucCategoryListe";
			componentResourceManager.ApplyResources(this, "$this");
			base.Load += this.ucCategoryListe_Load;
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			((ISupportInitialize)this.dataGridViewCategories).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		public ucCategoryListe()
		{
			this.InitializeComponent();
			base.Resize += this.ucCategoryListe_Resize;
			this.dataGridViewCategories.Columns.Add(ResReader.Reader.GetString("Pattern"), ResReader.Reader.GetString("Pattern"));
			this.dataGridViewCategories.Columns.Add(ResReader.Reader.GetString("That"), ResReader.Reader.GetString("That"));
			this.dataGridViewCategories.Columns.Add(ResReader.Reader.GetString("Template"), ResReader.Reader.GetString("Template"));
			this.dataGridViewCategories.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
			this.dataGridViewCategories.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
			this.dataGridViewCategories.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
			this.dataGridViewCategories.SelectionChanged += this.dataGridViewCategories_SelectionChanged;
			this.dataGridViewCategories.Scroll += this.dataGridViewCategories_Scroll;
			this.dataGridViewCategories.Resize += this.dataGridViewCategories_Resize;
		}

		private void ucCategoryListe_Load(object sender, EventArgs e)
		{
			this.dataGridViewCategories.AllowUserToAddRows = false;
			this.dataGridViewCategories.AllowUserToResizeColumns = false;
			this.dataGridViewCategories.AllowUserToResizeRows = false;
			this.dataGridViewCategories.MouseMove += this.dataGridViewCategories_MouseMove;
			this.dataGridViewCategories.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
			this.SteuerelementeBeschriften();
		}

		private AIMLCategory GetNaechstLiegendeCategory(AIMLCategory category)
		{
			if (category.ContentNode.NextSibling != null)
			{
				AIMLCategory categoryForCategoryNode = this._arbeitsbereich.GetCategoryForCategoryNode(category.ContentNode.NextSibling);
				if (categoryForCategoryNode != null)
				{
					return categoryForCategoryNode;
				}
			}
			else if (category.ContentNode.PreviousSibling != null)
			{
				AIMLCategory categoryForCategoryNode2 = this._arbeitsbereich.GetCategoryForCategoryNode(category.ContentNode.PreviousSibling);
				if (categoryForCategoryNode2 != null)
				{
					return categoryForCategoryNode2;
				}
			}
			return null;
		}

		private void dataGridViewCategories_MouseMove(object sender, MouseEventArgs e)
		{
			if (!this.DateiIstReadOnly && e.Button == MouseButtons.Left && this.AktuelleCategory != null)
			{
				AIMLCategory aktuelleCategory = this.AktuelleCategory;
				DragDropEffects dragDropEffects = this.dataGridViewCategories.DoDragDrop(aktuelleCategory, DragDropEffects.Copy | DragDropEffects.Move);
				if (dragDropEffects == DragDropEffects.Move && aktuelleCategory != null)
				{
					AIMLTopic aIMLTopic = aktuelleCategory.AIMLTopic;
					AIMLCategory naechstLiegendeCategory = this.GetNaechstLiegendeCategory(aktuelleCategory);
					aIMLTopic.LoescheCategory(aktuelleCategory);
					this.AIMLCategoriesNeuAnzeigen(false);
					if (naechstLiegendeCategory != null)
					{
						this.AktuelleCategory = naechstLiegendeCategory;
					}
					else
					{
						this._arbeitsbereich.Fokus.BesteCategorySelektieren();
					}
					this.ZeilenInhalteRefreshen();
				}
			}
		}

		private void SteuerelementeBeschriften()
		{
			this.toolStripButtonDeleteKategory.Text = ResReader.Reader.GetString("toolStripButtonDeleteKategory");
			this.toolStripButtonNeueKategorie.Text = ResReader.Reader.GetString("toolStripButtonNeueKategorie");
			this.toolStripButtonCreateTHATReference.Text = ResReader.Reader.GetString("toolStripButtonCreateTHATReferenceText");
			this.toolStripButtonCreateTHATReference.ToolTipText = ResReader.Reader.GetString("toolStripButtonCreateTHATReferenceTooltip");
			this.toolStripButtonTHATKlonen.Text = ResReader.Reader.GetString("toolStripButtonTHATKlonenText");
			this.toolStripButtonTHATKlonen.ToolTipText = ResReader.Reader.GetString("toolStripButtonTHATKlonenToolTipp");
			this.toolStripButtonCloneTemplate.Text = ResReader.Reader.GetString("toolStripButtonCloneTemplateText");
			this.toolStripButtonPassendesSRAI.Text = ResReader.Reader.GetString("toolStripButtonPassendesSRAIText");
			this.toolStripButtonPassendesSRAI.ToolTipText = ResReader.Reader.GetString("toolStripButtonPassendesSRAITooltip");
			this.toolStripButtonSortieren.Text = ResReader.Reader.GetString("toolStripButtonSortieren");
			this.cloneToolStripMenuItem.Text = ResReader.Reader.GetString("cloneToolStripMenuItem");
		}

		private void Fokus_AktAIMLTopicChanged(object sender, ArbeitsbereichFokus.AktAIMLTopicChangedEventArgs e)
		{
			if (this.AktuelleCategory == null)
			{
				this.AIMLCategoriesNeuAnzeigen(false);
			}
		}

		private void Fokus_AktAIMLCategoryChanged(object sender, EventArgs<AIMLCategory> e)
		{
			this.AIMLCategoriesNeuAnzeigen(false);
		}

		private void AIMLCategoriesNeuAnzeigen(bool markierteZeileAufJedenFallInDenSichtbarenBereichScrollen)
		{
			if (!this._wirdGeradeNeuGezeichnet)
			{
				this._wirdGeradeNeuGezeichnet = true;
				int num = 0;
				AIMLTopic aimlTopic = this.aimlTopic;
				if (aimlTopic == null)
				{
					num = 0;
					this._merkeCategories = null;
					base.Enabled = false;
					this.dataGridViewCategories.Rows.Clear();
				}
				else
				{
					this._merkeCategories = aimlTopic.Categories;
					num = this._merkeCategories.Count;
					this.AnzahlZeilenAuffuellen(num);
					AIMLCategory aktuelleCategory = this.AktuelleCategory;
					if (aktuelleCategory != null)
					{
						int num2 = this._merkeCategories.IndexOf(aktuelleCategory);
						if (num2 != -1)
						{
							DataGridViewRow dataGridViewRow = this.dataGridViewCategories.Rows[num2];
							if (!dataGridViewRow.Selected)
							{
								dataGridViewRow.Selected = true;
							}
							if (!dataGridViewRow.Displayed | markierteZeileAufJedenFallInDenSichtbarenBereichScrollen)
							{
								try
								{
									this.dataGridViewCategories.FirstDisplayedScrollingRowIndex = num2;
								}
								catch (Exception)
								{
								}
							}
						}
					}
					base.Enabled = true;
				}
				this._wirdGeradeNeuGezeichnet = false;
				this.ToolStripButtonsAnzeigen();
				this.ZeilenInhalteRefreshen();
			}
		}

		private void AnzahlZeilenAuffuellen(int anzahlKategorien)
		{
			this._zeilenWerdenGeradeAufgefuellt = true;
			if (anzahlKategorien == 0)
			{
				this.dataGridViewCategories.Rows.Clear();
			}
			int num = anzahlKategorien - this.dataGridViewCategories.Rows.Count;
			if (num < 0 && -num > anzahlKategorien)
			{
				this.dataGridViewCategories.Rows.Clear();
			}
			if (anzahlKategorien > this.dataGridViewCategories.Rows.Count)
			{
				int count = anzahlKategorien - this.dataGridViewCategories.Rows.Count;
				this.dataGridViewCategories.Rows.Add(count);
			}
			while (anzahlKategorien < this.dataGridViewCategories.Rows.Count)
			{
				this.dataGridViewCategories.Rows.RemoveAt(this.dataGridViewCategories.Rows.Count - 1);
			}
			this._zeilenWerdenGeradeAufgefuellt = false;
		}

		private void dataGridViewCategories_Scroll(object sender, ScrollEventArgs e)
		{
			this.ZeilenInhalteRefreshen();
		}

		private void ZeilenInhalteRefreshen()
		{
			if (!this._zeilenWerdenGeradeAufgefuellt && this._merkeCategories != null && !this._wirdGeradeNeuGezeichnet)
			{
				this._wirdGeradeNeuGezeichnet = true;
				int firstDisplayedScrollingRowIndex = this.dataGridViewCategories.FirstDisplayedScrollingRowIndex;
				int val = firstDisplayedScrollingRowIndex + this.dataGridViewCategories.DisplayedRowCount(true);
				firstDisplayedScrollingRowIndex = Math.Max(0, firstDisplayedScrollingRowIndex);
				val = Math.Min(val, this._merkeCategories.Count);
				for (int i = firstDisplayedScrollingRowIndex; i < val; i++)
				{
					this.dataGridViewCategories.Rows[i].Cells[0].Value = this._merkeCategories[i].AutoKurzNamePattern;
					this.dataGridViewCategories.Rows[i].Cells[1].Value = this._merkeCategories[i].AutoThatZusammenfassung;
					this.dataGridViewCategories.Rows[i].Cells[2].Value = this._merkeCategories[i].AutoTemplateZusammenfassung;
				}
				this._wirdGeradeNeuGezeichnet = false;
				Application.DoEvents();
			}
		}

		private void dataGridViewCategories_SelectionChanged(object sender, EventArgs e)
		{
			if (!this._wirdGeradeNeuGezeichnet)
			{
				DataGridViewSelectedRowCollection selectedRows = this.dataGridViewCategories.SelectedRows;
				if (selectedRows.Count == 1)
				{
					if (this._merkeCategories != null)
					{
						if (this._merkeCategories.Count == this.dataGridViewCategories.Rows.Count)
						{
							int index = selectedRows[0].Index;
							this.AktuelleCategory = this._merkeCategories[index];
						}
						else
						{
							this.AIMLCategoriesNeuAnzeigen(true);
							this.AktuelleCategory = null;
						}
					}
					else
					{
						this.AktuelleCategory = null;
					}
				}
				else
				{
					this.AktuelleCategory = null;
				}
				this.ZeilenInhalteRefreshen();
			}
		}

		private void NeueCategoryHinzufuegen()
		{
			AIMLCategory aIMLCategory2 = this.AktuelleCategory = this.aimlTopic.AddNewCategory();
			this._arbeitsbereich.Fokus.FokusAufXmlEditorSetzen();
			this.AIMLCategoriesNeuAnzeigen(true);
		}

		private void AktuelleCategoryLoeschen()
		{
			AIMLCategory aktuelleCategory = this.AktuelleCategory;
			if (aktuelleCategory != null)
			{
				DialogResult dialogResult = MessageBox.Show(ResReader.Reader.GetString("AktuelleCategoryWirklichLoeschen"), ResReader.Reader.GetString("AktuelleCategoryLoeschen"), MessageBoxButtons.YesNoCancel);
				if (dialogResult == DialogResult.Yes)
				{
					AIMLCategory naechstLiegendeCategory = this.GetNaechstLiegendeCategory(aktuelleCategory);
					if (this.aimlTopic == null)
					{
						throw new ApplicationException("aimlTopic==null!");
					}
					if (aktuelleCategory == null)
					{
						throw new ApplicationException("Es ist keine aktuelle Category mehr zum Löschen gewählt!");
					}
					this.aimlTopic.LoescheCategory(aktuelleCategory);
					if (naechstLiegendeCategory != null)
					{
						this.AktuelleCategory = naechstLiegendeCategory;
					}
					else
					{
						if (this._arbeitsbereich == null)
						{
							throw new ApplicationException("_arbeitsbereich==null!");
						}
						this._arbeitsbereich.Fokus.BesteCategorySelektieren();
					}
				}
			}
		}

		private void ToolStripButtonsAnzeigen()
		{
			bool flag = true;
			if (this.aimlTopic == null)
			{
				flag = false;
			}
			if (this.DateiIstReadOnly)
			{
				flag = false;
			}
			if (!flag)
			{
				this.toolStripButtonNeueKategorie.Enabled = false;
				this.toolStripButtonDeleteKategory.Enabled = false;
				this.toolStripButtonCreateTHATReference.Enabled = false;
				this.toolStripButtonTHATKlonen.Enabled = false;
				this.toolStripButtonCloneTemplate.Enabled = false;
				this.toolStripButtonPassendesSRAI.Enabled = false;
				this.cloneToolStripMenuItem.Enabled = false;
			}
			else
			{
				this.toolStripButtonNeueKategorie.Enabled = true;
				this.toolStripButtonDeleteKategory.Enabled = (this.AktuelleCategory != null);
				this.toolStripButtonCreateTHATReference.Enabled = (this.AktuelleCategory != null);
				this.toolStripButtonTHATKlonen.Enabled = (this.AktuelleCategory != null && this.AktuelleCategory.That != "");
				this.toolStripButtonCloneTemplate.Enabled = (this.AktuelleCategory != null);
				this.toolStripButtonPassendesSRAI.Enabled = (this.AktuelleCategory != null);
				this.cloneToolStripMenuItem.Enabled = (this.AktuelleCategory != null);
			}
		}

		private void toolStripButtonSortieren_Click(object sender, EventArgs e)
		{
			if (this._arbeitsbereich != null)
			{
				AIMLTopic aktAIMLTopic = this._arbeitsbereich.Fokus.AktAIMLTopic;
				if (aktAIMLTopic != null)
				{
					aktAIMLTopic.CategoriesSortieren_();
					this.AIMLCategoriesNeuAnzeigen(true);
					AIMLCategory aktuelleCategory = this.AktuelleCategory;
					this.AktuelleCategory = null;
					this.AktuelleCategory = aktuelleCategory;
				}
			}
		}

		private void toolStripButtonNeueKategorie_ButtonClick(object sender, EventArgs e)
		{
			this.NeueCategoryHinzufuegen();
		}

		private void toolStripButtonDeleteKategory_Click(object sender, EventArgs e)
		{
			this.AktuelleCategoryLoeschen();
		}

		private void toolStripButtonCreateTHATReference_Click_1(object sender, EventArgs e)
		{
			AIMLCategory aktuelleCategory = this.AktuelleCategory;
			AIMLCategory aIMLCategory = aktuelleCategory.AIMLTopic.AddNewCategory();
			XmlNode xmlNode = aIMLCategory.ContentNode.OwnerDocument.CreateElement("that");
			string text = xmlNode.InnerXml = aktuelleCategory.Template;
			XmlNode xmlNode2 = aIMLCategory.ContentNode.SelectSingleNode("pattern");
			if (xmlNode2 != null)
			{
				aIMLCategory.ContentNode.InsertAfter(xmlNode, xmlNode2);
				this.AktuelleCategory = aIMLCategory;
				this._arbeitsbereich.Fokus.FokusAufXmlEditorSetzen();
			}
			else
			{
				aktuelleCategory.AIMLTopic.LoescheCategory(aIMLCategory);
			}
			this.AIMLCategoriesNeuAnzeigen(true);
		}

		private void cloneToolStripMenuItem_Click(object sender, EventArgs e)
		{
			AIMLCategory aktuelleCategory = this.AktuelleCategory;
			XmlNode newCategoryNode = aktuelleCategory.ContentNode.Clone();
			AIMLCategory aIMLCategory2 = this.AktuelleCategory = aktuelleCategory.AIMLTopic.AddNewCategory(newCategoryNode);
			this._arbeitsbereich.Fokus.FokusAufXmlEditorSetzen();
			this.AIMLCategoriesNeuAnzeigen(true);
		}

		private void toolStripButtonTHATKlonen_Click_1(object sender, EventArgs e)
		{
			AIMLCategory aktuelleCategory = this.AktuelleCategory;
			AIMLCategory aIMLCategory = aktuelleCategory.AIMLTopic.AddNewCategory();
			XmlNode xmlNode = aIMLCategory.ContentNode.OwnerDocument.CreateElement("that");
			string text = xmlNode.InnerXml = aktuelleCategory.That;
			XmlNode xmlNode2 = aIMLCategory.ContentNode.SelectSingleNode("pattern");
			if (xmlNode2 != null)
			{
				aIMLCategory.ContentNode.InsertAfter(xmlNode, xmlNode2);
				this.AktuelleCategory = aIMLCategory;
				this._arbeitsbereich.Fokus.FokusAufXmlEditorSetzen();
			}
			else
			{
				aktuelleCategory.AIMLTopic.LoescheCategory(aIMLCategory);
			}
			this.AIMLCategoriesNeuAnzeigen(true);
		}

		private void toolStripButtonCloneTemplate_Click_1(object sender, EventArgs e)
		{
			AIMLCategory aktuelleCategory = this.AktuelleCategory;
			AIMLCategory aIMLCategory = aktuelleCategory.AIMLTopic.AddNewCategory();
			XmlNode xmlNode = aktuelleCategory.ContentNode.SelectSingleNode("template");
			XmlNode xmlNode2 = aIMLCategory.ContentNode.SelectSingleNode("template");
			if (xmlNode != null)
			{
				foreach (XmlNode childNode in xmlNode.ChildNodes)
				{
					XmlNode newChild = childNode.Clone();
					xmlNode2.AppendChild(newChild);
				}
				this.AktuelleCategory = aIMLCategory;
				this._arbeitsbereich.Fokus.FokusAufXmlEditorSetzen();
			}
			else
			{
				aktuelleCategory.AIMLTopic.LoescheCategory(aIMLCategory);
			}
			this.AIMLCategoriesNeuAnzeigen(true);
		}

		private void toolStripButtonPassendesSRAI_Click_1(object sender, EventArgs e)
		{
			AIMLCategory aktuelleCategory = this.AktuelleCategory;
			AIMLCategory aIMLCategory = aktuelleCategory.AIMLTopic.AddNewCategory();
			XmlNode xmlNode = aktuelleCategory.ContentNode.SelectSingleNode("pattern");
			if (xmlNode != null)
			{
				XmlNode xmlNode2 = aIMLCategory.ContentNode.OwnerDocument.CreateElement("srai");
				xmlNode2.InnerText = xmlNode.InnerXml;
				aIMLCategory.ContentNode.SelectSingleNode("template").AppendChild(xmlNode2);
				this.AktuelleCategory = aIMLCategory;
				this._arbeitsbereich.Fokus.FokusAufXmlEditorSetzen();
			}
			else
			{
				aktuelleCategory.AIMLTopic.LoescheCategory(aIMLCategory);
			}
			this.AIMLCategoriesNeuAnzeigen(true);
		}

		private void dataGridViewCategories_Resize(object sender, EventArgs e)
		{
			this.ResizeAll();
		}

		private void ucCategoryListe_Resize(object sender, EventArgs e)
		{
			this.ResizeAll();
		}

		private void ResizeAll()
		{
			this.dataGridViewCategories.Top = this.toolStrip1.Top + this.toolStrip1.Height;
			this.dataGridViewCategories.Left = 0;
			DataGridView dataGridView = this.dataGridViewCategories;
			Size clientSize = base.ClientSize;
			dataGridView.Height = clientSize.Height - this.dataGridViewCategories.Top;
			DataGridView dataGridView2 = this.dataGridViewCategories;
			clientSize = base.ClientSize;
			dataGridView2.Width = clientSize.Width;
			clientSize = this.dataGridViewCategories.ClientSize;
			int num = clientSize.Width - 3;
			int num2 = num;
			if (this.dataGridViewCategories.Columns.Count == 3)
			{
				this.dataGridViewCategories.Columns[0].Width = num / 3;
				num2 -= this.dataGridViewCategories.Columns[0].Width;
				this.dataGridViewCategories.Columns[1].Width = num / 3;
				num2 -= this.dataGridViewCategories.Columns[1].Width;
				this.dataGridViewCategories.Columns[2].Width = num2;
			}
			this.ZeilenInhalteRefreshen();
		}

		private void dataGridViewCategories_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{
		}
	}
}
