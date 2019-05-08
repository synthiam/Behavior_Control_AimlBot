using de.springwald.toolbox;
using GaitoBotEditorCore.Properties;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace GaitoBotEditorCore
{
	public class ucTopicListe : UserControl
	{
		private ListView listThemen;

		private ColumnHeader Thema;

		private ImageList imgListToolbar;

		private ToolStrip toolStrip1;

		private ToolStripButton toolStripButtonNeuesThema;

		private ToolStripButton toolStripButtonTopicLoeschen;

		private ToolStripButton toolStripButtonTopicUmbenennen;

		private IContainer components;

		private bool _wirdGeradeNeuGezeichnet;

		private Arbeitsbereich _arbeitsbereich;

		private AIMLDatei AimlDatei
		{
			get
			{
				if (this._arbeitsbereich == null)
				{
					return null;
				}
				if (this._arbeitsbereich.Fokus.AktDatei is AIMLDatei)
				{
					return (AIMLDatei)this._arbeitsbereich.Fokus.AktDatei;
				}
				return null;
			}
		}

		private AIMLTopic AktuellesThema
		{
			get
			{
				if (this._arbeitsbereich == null)
				{
					throw new ApplicationException("Noch kein Arbeitsbereich zugewiesen!");
				}
				return this._arbeitsbereich.Fokus.AktAIMLTopic;
			}
			set
			{
				if (this._arbeitsbereich == null)
				{
					throw new ApplicationException("Noch kein Arbeitsbereich zugewiesen!");
				}
				this._arbeitsbereich.Fokus.AktAIMLTopic = value;
			}
		}

		public Arbeitsbereich Arbeitsbereich
		{
			set
			{
				if (this._arbeitsbereich != null)
				{
					throw new ApplicationException("Dieser AIMLTopicListe wurde bereits ein Arbeitsbereich zugeordnet");
				}
				this._arbeitsbereich = value;
				this._arbeitsbereich.Fokus.AktAIMLTopicChanged += this.Fokus_AktAIMLTopicChanged;
				this.AIMLTopicsNeuAnzeigen();
			}
		}

		private bool DateiIstReadOnly
		{
			get
			{
				return this.AimlDatei != null && this.AimlDatei.ReadOnly;
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ucTopicListe));
			this.listThemen = new ListView();
			this.Thema = new ColumnHeader();
			this.imgListToolbar = new ImageList(this.components);
			this.toolStrip1 = new ToolStrip();
			this.toolStripButtonNeuesThema = new ToolStripButton();
			this.toolStripButtonTopicLoeschen = new ToolStripButton();
			this.toolStripButtonTopicUmbenennen = new ToolStripButton();
			this.toolStrip1.SuspendLayout();
			base.SuspendLayout();
			this.listThemen.BorderStyle = BorderStyle.FixedSingle;
			this.listThemen.Columns.AddRange(new ColumnHeader[1]
			{
				this.Thema
			});
			this.listThemen.FullRowSelect = true;
			this.listThemen.HeaderStyle = ColumnHeaderStyle.Nonclickable;
			this.listThemen.HideSelection = false;
			this.listThemen.Items.AddRange(new ListViewItem[6]
			{
				(ListViewItem)componentResourceManager.GetObject("listThemen.Items"),
				(ListViewItem)componentResourceManager.GetObject("listThemen.Items1"),
				(ListViewItem)componentResourceManager.GetObject("listThemen.Items2"),
				(ListViewItem)componentResourceManager.GetObject("listThemen.Items3"),
				(ListViewItem)componentResourceManager.GetObject("listThemen.Items4"),
				(ListViewItem)componentResourceManager.GetObject("listThemen.Items5")
			});
			componentResourceManager.ApplyResources(this.listThemen, "listThemen");
			this.listThemen.MultiSelect = false;
			this.listThemen.Name = "listThemen";
			this.listThemen.UseCompatibleStateImageBehavior = false;
			this.listThemen.View = View.List;
			this.listThemen.SelectedIndexChanged += this.listThemen_SelectedIndexChanged;
			componentResourceManager.ApplyResources(this.Thema, "Thema");
			this.imgListToolbar.ImageStream = (ImageListStreamer)componentResourceManager.GetObject("imgListToolbar.ImageStream");
			this.imgListToolbar.TransparentColor = Color.Transparent;
			this.imgListToolbar.Images.SetKeyName(0, "");
			this.imgListToolbar.Images.SetKeyName(1, "");
			this.toolStrip1.Items.AddRange(new ToolStripItem[3]
			{
				this.toolStripButtonNeuesThema,
				this.toolStripButtonTopicLoeschen,
				this.toolStripButtonTopicUmbenennen
			});
			componentResourceManager.ApplyResources(this.toolStrip1, "toolStrip1");
			this.toolStrip1.Name = "toolStrip1";
			this.toolStripButtonNeuesThema.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.toolStripButtonNeuesThema.Image = Resources.add_161;
			componentResourceManager.ApplyResources(this.toolStripButtonNeuesThema, "toolStripButtonNeuesThema");
			this.toolStripButtonNeuesThema.Name = "toolStripButtonNeuesThema";
			this.toolStripButtonNeuesThema.Click += this.toolStripButtonNeuesThema_Click;
			this.toolStripButtonTopicLoeschen.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.toolStripButtonTopicLoeschen.Image = Resources.delete_161;
			componentResourceManager.ApplyResources(this.toolStripButtonTopicLoeschen, "toolStripButtonTopicLoeschen");
			this.toolStripButtonTopicLoeschen.Name = "toolStripButtonTopicLoeschen";
			this.toolStripButtonTopicLoeschen.Click += this.toolStripButtonLoeschen_Click;
			this.toolStripButtonTopicUmbenennen.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.toolStripButtonTopicUmbenennen.Image = Resources.rename1;
			componentResourceManager.ApplyResources(this.toolStripButtonTopicUmbenennen, "toolStripButtonTopicUmbenennen");
			this.toolStripButtonTopicUmbenennen.Name = "toolStripButtonTopicUmbenennen";
			this.toolStripButtonTopicUmbenennen.Click += this.toolStripButtonTopicUmbenennen_Click;
			base.Controls.Add(this.toolStrip1);
			base.Controls.Add(this.listThemen);
			base.Name = "ucTopicListe";
			componentResourceManager.ApplyResources(this, "$this");
			base.Load += this.ucThemenListe_Load;
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		public ucTopicListe()
		{
			this.InitializeComponent();
			base.Resize += this.ucThemenListe_Resize;
		}

		private void ucThemenListe_Load(object sender, EventArgs e)
		{
			this.SteuerElementeBeschriften();
			this.listThemen.Columns.Add("Datei", "Datei");
			this.listThemen.FullRowSelect = true;
			this.listThemen.HideSelection = false;
			this.listThemen.MultiSelect = false;
			this.listThemen.HeaderStyle = ColumnHeaderStyle.None;
			this.listThemen.View = View.Details;
			this.ucThemenListe_Resize(null, null);
			this.listThemen.AllowDrop = true;
			this.listThemen.DragEnter += this.listThemen_DragOver;
			this.listThemen.DragOver += this.listThemen_DragOver;
			this.listThemen.DragDrop += this.listThemen_DragDrop;
			this.listThemen.MouseMove += this.listThemen_MouseMove;
		}

		private void listThemen_MouseMove(object sender, MouseEventArgs e)
		{
			if (!this.DateiIstReadOnly && e.Button == MouseButtons.Left)
			{
				AIMLTopic aktuellesThema = this.AktuellesThema;
				if (aktuellesThema != null && !aktuellesThema.IstRootTopic)
				{
					DragDropEffects dragDropEffects = this.listThemen.DoDragDrop(aktuellesThema, DragDropEffects.Copy | DragDropEffects.Move);
					if (dragDropEffects == DragDropEffects.Move)
					{
						this.AimlDatei.LoescheTopic(aktuellesThema);
						this._arbeitsbereich.Fokus.BestesTopicSelektieren();
					}
				}
			}
		}

		private void listThemen_DragOver(object sender, DragEventArgs e)
		{
			if (!this.DateiIstReadOnly)
			{
				if (e.Data.GetDataPresent(typeof(AIMLCategory)))
				{
					AIMLCategory aIMLCategory = (AIMLCategory)e.Data.GetData(typeof(AIMLCategory));
					if (aIMLCategory != null)
					{
						AIMLTopic aIMLTopic = this.TopicUnterPos(new Point(e.X, e.Y));
						if (aIMLTopic != null)
						{
							if (aIMLTopic != aIMLCategory.AIMLTopic)
							{
								e.Effect = DragDropEffects.Move;
							}
							else
							{
								e.Effect = DragDropEffects.None;
							}
						}
						else
						{
							e.Effect = DragDropEffects.None;
						}
					}
					else
					{
						e.Effect = DragDropEffects.None;
					}
				}
				else
				{
					e.Effect = DragDropEffects.None;
				}
			}
		}

		private void listThemen_DragDrop(object sender, DragEventArgs e)
		{
			if (!this.DateiIstReadOnly)
			{
				if (e.Data.GetDataPresent(typeof(AIMLCategory)))
				{
					AIMLCategory aIMLCategory = (AIMLCategory)e.Data.GetData(typeof(AIMLCategory));
					if (aIMLCategory != null)
					{
						AIMLTopic aIMLTopic = this.TopicUnterPos(new Point(e.X, e.Y));
						if (aIMLTopic != null)
						{
							if (aIMLTopic != aIMLCategory.AIMLTopic)
							{
								AIMLCategory aIMLCategory2 = aIMLTopic.AddNewCategory();
								aIMLCategory2.ContentNode.InnerXml = aIMLCategory.ContentNode.InnerXml;
								e.Effect = DragDropEffects.Move;
							}
							else
							{
								e.Effect = DragDropEffects.None;
							}
						}
						else
						{
							e.Effect = DragDropEffects.None;
						}
					}
				}
				else
				{
					e.Effect = DragDropEffects.None;
				}
			}
		}

		private AIMLTopic TopicUnterPos(Point screenPos)
		{
			Point point = this.listThemen.PointToClient(screenPos);
			ListViewItem itemAt = this.listThemen.GetItemAt(point.X, point.Y);
			if (itemAt != null)
			{
				AIMLTopic aIMLTopic = (AIMLTopic)itemAt.Tag;
				if (aIMLTopic != null)
				{
					return aIMLTopic;
				}
				return null;
			}
			return null;
		}

		private void Fokus_AktAIMLTopicChanged(object sender, ArbeitsbereichFokus.AktAIMLTopicChangedEventArgs e)
		{
			this.AIMLTopicsNeuAnzeigen();
		}

		private void ucThemenListe_Resize(object sender, EventArgs e)
		{
			this.listThemen.Top = this.toolStrip1.Top + this.toolStrip1.Height;
			this.listThemen.Left = 0;
			ListView listView = this.listThemen;
			Size clientSize = base.ClientSize;
			listView.Height = clientSize.Height - this.listThemen.Top;
			ListView listView2 = this.listThemen;
			clientSize = base.ClientSize;
			listView2.Width = clientSize.Width;
			ColumnHeader columnHeader = this.listThemen.Columns[0];
			clientSize = this.listThemen.ClientSize;
			columnHeader.Width = clientSize.Width;
		}

		private void listThemen_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!this._wirdGeradeNeuGezeichnet)
			{
				ListView.SelectedListViewItemCollection selectedItems = this.listThemen.SelectedItems;
				if (selectedItems.Count == 1)
				{
					if (selectedItems[0].Tag != null && selectedItems[0].Tag is AIMLTopic)
					{
						this.AktuellesThema = (AIMLTopic)selectedItems[0].Tag;
					}
					else
					{
						this.AktuellesThema = null;
					}
				}
				else
				{
					this.AktuellesThema = null;
				}
			}
		}

		private void AIMLTopicsNeuAnzeigen()
		{
			if (!this._wirdGeradeNeuGezeichnet)
			{
				this._wirdGeradeNeuGezeichnet = true;
				AIMLDatei aimlDatei = this.AimlDatei;
				if (aimlDatei == null)
				{
					base.Enabled = false;
					this.listThemen.Items.Clear();
				}
				else
				{
					IOrderedEnumerable<AIMLTopic> orderedEnumerable = from t in aimlDatei.getTopics()
					orderby t.Name
					select t;
					while (this.listThemen.Items.Count < orderedEnumerable.Count())
					{
						ListViewItem value = new ListViewItem();
						this.listThemen.Items.Add(value);
					}
					int num = 0;
					foreach (AIMLTopic item2 in orderedEnumerable)
					{
						ListViewItem listViewItem = this.listThemen.Items[num];
						listViewItem.Tag = item2;
						this.ListenEintragBeschriften(listViewItem);
						num++;
					}
					for (int count = this.listThemen.Items.Count; count > orderedEnumerable.Count(); count = this.listThemen.Items.Count)
					{
						ListViewItem item = this.listThemen.Items[count - 1];
						this.listThemen.Items.Remove(item);
					}
					base.Enabled = true;
				}
				this.ToolStripButtonsAnzeigen();
				this._wirdGeradeNeuGezeichnet = false;
			}
		}

		private void ListenEintragBeschriften(ListViewItem item)
		{
			AIMLTopic aIMLTopic = (AIMLTopic)item.Tag;
			string name = aIMLTopic.Name;
			if (item.Text != name)
			{
				item.Text = name;
			}
			bool flag = aIMLTopic == this.AktuellesThema;
			if (item.Selected != flag)
			{
				item.Selected = flag;
			}
		}

		private void NeuesTopicHinzufuegen()
		{
			if (!this.DateiIstReadOnly)
			{
				AIMLTopic aIMLTopic2 = this.AktuellesThema = this.AimlDatei.AddNewTopic();
			}
		}

		private void AktuellesTopicLoeschen()
		{
			if (!this.DateiIstReadOnly)
			{
				AIMLTopic aktuellesThema = this.AktuellesThema;
				if (aktuellesThema != null)
				{
					if (aktuellesThema.IstRootTopic)
					{
						MessageBox.Show(ResReader.Reader.GetString("RootTopicKannNichtGeloeschtWerden"));
					}
					else
					{
						DialogResult dialogResult = MessageBox.Show(string.Format(ResReader.Reader.GetString("SollDasThemaWirklichGeloeschtWerden"), aktuellesThema.Name), ResReader.Reader.GetString("ThemaLoeschen"), MessageBoxButtons.YesNoCancel);
						if (dialogResult == DialogResult.Yes)
						{
							this.AimlDatei.LoescheTopic(aktuellesThema);
							this._arbeitsbereich.Fokus.BestesTopicSelektieren();
						}
					}
				}
			}
		}

		private void SteuerElementeBeschriften()
		{
			this.toolStripButtonTopicLoeschen.Text = ResReader.Reader.GetString("toolStripButtonTopicLoeschen");
			this.toolStripButtonTopicUmbenennen.Text = ResReader.Reader.GetString("toolStripButtonTopicUmbenennen");
			this.toolStripButtonNeuesThema.Text = ResReader.Reader.GetString("toolStripButtonNeuesThema");
		}

		private void toolStripButtonLoeschen_Click(object sender, EventArgs e)
		{
			this.AktuellesTopicLoeschen();
		}

		private void ToolStripButtonsAnzeigen()
		{
			this.toolStripButtonNeuesThema.Enabled = !this.DateiIstReadOnly;
			AIMLTopic aktuellesThema = this.AktuellesThema;
			if (aktuellesThema == null || this.DateiIstReadOnly)
			{
				this.toolStripButtonTopicLoeschen.Enabled = false;
				this.toolStripButtonTopicUmbenennen.Enabled = false;
			}
			else
			{
				this.toolStripButtonTopicLoeschen.Enabled = !aktuellesThema.IstRootTopic;
				this.toolStripButtonTopicUmbenennen.Enabled = !aktuellesThema.IstRootTopic;
			}
		}

		private void toolStripButtonNeuesThema_Click(object sender, EventArgs e)
		{
			this.NeuesTopicHinzufuegen();
			this.AktuellesTopicUmbenennen();
		}

		private void toolStripButtonTopicUmbenennen_Click(object sender, EventArgs e)
		{
			if (!this.DateiIstReadOnly)
			{
				this.AktuellesTopicUmbenennen();
			}
		}

		public void AktuellesTopicUmbenennen()
		{
			AIMLTopic aktuellesThema = this.AktuellesThema;
			if (aktuellesThema != null)
			{
				if (aktuellesThema.IstRootTopic)
				{
					MessageBox.Show(ResReader.Reader.GetString("KannStandardTopicNichtUmbenennen"), ResReader.Reader.GetString("KonnteAIMLTopicNichtUmbenennen"));
				}
				else
				{
					bool flag = false;
					bool flag2 = false;
					while (!flag2)
					{
						string text = InputBox.Show(ResReader.Reader.GetString("BitteNamenFuerAIMLTopic"), ResReader.Reader.GetString("NameFuerAIMLTopicVergeben"), aktuellesThema.Name, out flag);
						if (text == "" || text == null)
						{
							flag = true;
						}
						if (flag)
						{
							flag2 = true;
						}
						else
						{
							text = text.Trim();
							if (aktuellesThema.Name == text)
							{
								flag2 = true;
							}
							else
							{
								string text2 = null;
								foreach (AIMLTopic topic in aktuellesThema.AIMLDatei.getTopics())
								{
									if (aktuellesThema != topic && topic.Name == text)
									{
										text2 = string.Format(ResReader.Reader.GetString("TopicNameSchonVorhanden"), text);
									}
								}
								if (text2 == null)
								{
									try
									{
										aktuellesThema.Name = text;
										this.AIMLTopicsNeuAnzeigen();
										flag2 = true;
									}
									catch (Exception ex)
									{
										text2 = ex.Message;
									}
								}
								if (text2 != null)
								{
									MessageBox.Show(text2, ResReader.Reader.GetString("KonnteAIMLTopicNichtUmbenennen"));
									Debugger.GlobalDebugger.Protokolliere("Konnte AIMLTopic '" + aktuellesThema.Name + "' nicht umbenennen:" + text2, Debugger.ProtokollTypen.Fehlermeldung);
								}
							}
						}
					}
				}
			}
		}
	}
}
