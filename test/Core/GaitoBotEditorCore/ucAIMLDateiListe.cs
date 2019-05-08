using de.springwald.toolbox;
using GaitoBotEditorCore.ContentHinzulinken;
using GaitoBotEditorCore.Properties;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace GaitoBotEditorCore
{
	public class ucAIMLDateiListe : UserControl
	{
		private Timer tmrRefresh;

		private MenuItem mnuSave;

		private ToolStrip toolStrip1;

		private ToolStripSeparator toolStripSeparator1;

		private ToolStripButton toolStripButtonOpenFile;

		private ToolStripButton toolStripButtonDelete;

		private ToolStripButton toolStripButtonSave;

		private OpenFileDialog openFileDialog;

		private ToolStripButton toolStripButtonFehlerZeigen;

		private ToolStripSeparator toolStripSeparator2;

		private ToolStripButton toolStripButtonAIMLDateiUmbenennen;

		private TreeView treeViewDateien;

		private ToolStripDropDownButton toolStripButtonNewFile;

		private ToolStripMenuItem newAimlFileToolStripMenuItem;

		private ToolStripMenuItem newReplaceConfigFileToolStripMenuItem;

		private ToolStripButton toolStripButtonHinzugelinkteDateienWaehlen;

		private ImageList imageListDateien;

		private IContainer components;

		private bool _wirdGeradeNeuGezeichnet;

		private Arbeitsbereich _arbeitsbereich;

		private IArbeitsbereichDatei AktuelleDatei
		{
			get
			{
				if (this._arbeitsbereich == null)
				{
					throw new ApplicationException("Noch kein Arbeitsbereich zugewiesen!");
				}
				return this._arbeitsbereich.Fokus.AktDatei;
			}
			set
			{
				if (this._arbeitsbereich == null)
				{
					throw new ApplicationException("Noch kein Arbeitsbereich zugewiesen!");
				}
				this._arbeitsbereich.Fokus.AktDatei = value;
			}
		}

		public Arbeitsbereich Arbeitsbereich
		{
			private get
			{
				return this._arbeitsbereich;
			}
			set
			{
				if (this._arbeitsbereich != null)
				{
					throw new ApplicationException("Dieser AIMLDateiListe wurde bereits ein Arbeitsbereich zugeordnet");
				}
				this._arbeitsbereich = value;
				this._arbeitsbereich.Fokus.AktDateiChanged += this.Fokus_AktAIMLDateiChanged;
				this._arbeitsbereich.Dateiverwaltung.DateiAddedEvent += this.Dateiverwaltung_AimlDateiAddedEvent;
				this._arbeitsbereich.Dateiverwaltung.DateiRemovedEvent += this.Dateiverwaltung_AimlDateiRemovedEvent;
				this.AIMLDateienNeuAnzeigen(true);
				this.ErsteDateiSelektieren();
				this.tmrRefresh.Tick += this.tmrRefresh_Tick;
				this.tmrRefresh.Enabled = true;
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ucAIMLDateiListe));
			this.tmrRefresh = new Timer(this.components);
			this.mnuSave = new MenuItem();
			this.toolStrip1 = new ToolStrip();
			this.toolStripButtonSave = new ToolStripButton();
			this.toolStripSeparator1 = new ToolStripSeparator();
			this.toolStripButtonOpenFile = new ToolStripButton();
			this.toolStripButtonNewFile = new ToolStripDropDownButton();
			this.newAimlFileToolStripMenuItem = new ToolStripMenuItem();
			this.newReplaceConfigFileToolStripMenuItem = new ToolStripMenuItem();
			this.toolStripButtonDelete = new ToolStripButton();
			this.toolStripButtonAIMLDateiUmbenennen = new ToolStripButton();
			this.toolStripSeparator2 = new ToolStripSeparator();
			this.toolStripButtonFehlerZeigen = new ToolStripButton();
			this.toolStripButtonHinzugelinkteDateienWaehlen = new ToolStripButton();
			this.openFileDialog = new OpenFileDialog();
			this.treeViewDateien = new TreeView();
			this.imageListDateien = new ImageList(this.components);
			this.toolStrip1.SuspendLayout();
			base.SuspendLayout();
			this.tmrRefresh.Interval = 2000;
			this.mnuSave.Index = -1;
			this.mnuSave.Text = MultiLang._75;
			this.toolStrip1.Items.AddRange(new ToolStripItem[9]
			{
				this.toolStripButtonSave,
				this.toolStripSeparator1,
				this.toolStripButtonOpenFile,
				this.toolStripButtonNewFile,
				this.toolStripButtonDelete,
				this.toolStripButtonAIMLDateiUmbenennen,
				this.toolStripSeparator2,
				this.toolStripButtonFehlerZeigen,
				this.toolStripButtonHinzugelinkteDateienWaehlen
			});
			componentResourceManager.ApplyResources(this.toolStrip1, "toolStrip1");
			this.toolStrip1.Name = "toolStrip1";
			this.toolStripButtonSave.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.toolStripButtonSave.Image = Resources.save_16;
			componentResourceManager.ApplyResources(this.toolStripButtonSave, "toolStripButtonSave");
			this.toolStripButtonSave.Name = "toolStripButtonSave";
			this.toolStripButtonSave.Click += this.toolStripButtonSave_Click;
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			componentResourceManager.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
			this.toolStripButtonOpenFile.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.toolStripButtonOpenFile.Image = Resources.folder_open_16;
			componentResourceManager.ApplyResources(this.toolStripButtonOpenFile, "toolStripButtonOpenFile");
			this.toolStripButtonOpenFile.Name = "toolStripButtonOpenFile";
			this.toolStripButtonOpenFile.Click += this.toolStripButtonOpenFile_Click;
			this.toolStripButtonNewFile.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.toolStripButtonNewFile.DropDownItems.AddRange(new ToolStripItem[2]
			{
				this.newAimlFileToolStripMenuItem,
				this.newReplaceConfigFileToolStripMenuItem
			});
			this.toolStripButtonNewFile.Image = Resources.add_161;
			componentResourceManager.ApplyResources(this.toolStripButtonNewFile, "toolStripButtonNewFile");
			this.toolStripButtonNewFile.Name = "toolStripButtonNewFile";
			this.newAimlFileToolStripMenuItem.Name = "newAimlFileToolStripMenuItem";
			componentResourceManager.ApplyResources(this.newAimlFileToolStripMenuItem, "newAimlFileToolStripMenuItem");
			this.newAimlFileToolStripMenuItem.Click += this.newAimlFileToolStripMenuItem_Click;
			this.newReplaceConfigFileToolStripMenuItem.Name = "newReplaceConfigFileToolStripMenuItem";
			componentResourceManager.ApplyResources(this.newReplaceConfigFileToolStripMenuItem, "newReplaceConfigFileToolStripMenuItem");
			this.newReplaceConfigFileToolStripMenuItem.Click += this.newStartupDateiToolStripMenuItem_Click;
			this.toolStripButtonDelete.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.toolStripButtonDelete.Image = Resources.delete_161;
			componentResourceManager.ApplyResources(this.toolStripButtonDelete, "toolStripButtonDelete");
			this.toolStripButtonDelete.Name = "toolStripButtonDelete";
			this.toolStripButtonDelete.Click += this.toolStripButtonDelete_Click;
			this.toolStripButtonAIMLDateiUmbenennen.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.toolStripButtonAIMLDateiUmbenennen.Image = Resources.rename1;
			componentResourceManager.ApplyResources(this.toolStripButtonAIMLDateiUmbenennen, "toolStripButtonAIMLDateiUmbenennen");
			this.toolStripButtonAIMLDateiUmbenennen.Name = "toolStripButtonAIMLDateiUmbenennen";
			this.toolStripButtonAIMLDateiUmbenennen.Click += this.toolStripButtonUmbenennen_Click;
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			componentResourceManager.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
			this.toolStripButtonFehlerZeigen.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.toolStripButtonFehlerZeigen.Image = Resources.W95MBX03;
			componentResourceManager.ApplyResources(this.toolStripButtonFehlerZeigen, "toolStripButtonFehlerZeigen");
			this.toolStripButtonFehlerZeigen.Name = "toolStripButtonFehlerZeigen";
			this.toolStripButtonFehlerZeigen.Click += this.toolStripButtonFehlerZeigen_Click;
			this.toolStripButtonHinzugelinkteDateienWaehlen.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.toolStripButtonHinzugelinkteDateienWaehlen.Image = Resources.App;
			componentResourceManager.ApplyResources(this.toolStripButtonHinzugelinkteDateienWaehlen, "toolStripButtonHinzugelinkteDateienWaehlen");
			this.toolStripButtonHinzugelinkteDateienWaehlen.Name = "toolStripButtonHinzugelinkteDateienWaehlen";
			this.toolStripButtonHinzugelinkteDateienWaehlen.Click += this.toolStripButtonHinzugelinkteDateienWaehlen_Click;
			this.openFileDialog.FileName = "openFileDialog";
			componentResourceManager.ApplyResources(this.treeViewDateien, "treeViewDateien");
			this.treeViewDateien.ImageList = this.imageListDateien;
			this.treeViewDateien.Name = "treeViewDateien";
			this.imageListDateien.ImageStream = (ImageListStreamer)componentResourceManager.GetObject("imageListDateien.ImageStream");
			this.imageListDateien.TransparentColor = Color.Transparent;
			this.imageListDateien.Images.SetKeyName(0, "aiml");
			this.imageListDateien.Images.SetKeyName(1, "startup");
			this.imageListDateien.Images.SetKeyName(2, "paket");
			this.BackColor = SystemColors.Control;
			base.Controls.Add(this.treeViewDateien);
			base.Controls.Add(this.toolStrip1);
			base.Name = "ucAIMLDateiListe";
			componentResourceManager.ApplyResources(this, "$this");
			base.Load += this.uAIMLDateiListe_Load;
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		public ucAIMLDateiListe()
		{
			this.InitializeComponent();
			this.treeViewDateien.FullRowSelect = true;
			this.treeViewDateien.HideSelection = false;
			base.Resize += this.uAIMLDateiListe_Resize;
		}

		private void uAIMLDateiListe_Load(object sender, EventArgs e)
		{
			this.SteuerElementeBenennen();
			this.treeViewDateien.AfterSelect += this.treeViewDateien_AfterSelect;
			this.treeViewDateien.AllowDrop = true;
			this.treeViewDateien.DragEnter += this.DateiListenAnzeige_DragEnter;
			this.treeViewDateien.DragOver += this.DateiListenAnzeige_DragEnter;
			this.treeViewDateien.DragDrop += this.DateiListenAnzeige_DragDrop;
		}

		private void Dateiverwaltung_AimlDateiRemovedEvent(object sender, EventArgs<IArbeitsbereichDatei> e)
		{
			this.AIMLDateienNeuAnzeigen(true);
		}

		private void Dateiverwaltung_AimlDateiAddedEvent(object sender, EventArgs<IArbeitsbereichDatei> e)
		{
			this.AIMLDateienNeuAnzeigen(true);
		}

		private void DateiListenAnzeige_DragDrop(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(typeof(AIMLCategory)))
			{
				AIMLCategory aIMLCategory = (AIMLCategory)e.Data.GetData(typeof(AIMLCategory));
				if (aIMLCategory != null)
				{
					IArbeitsbereichDatei arbeitsbereichDatei = this.DateiUnterPos(new Point(e.X, e.Y));
					if (arbeitsbereichDatei != null && !arbeitsbereichDatei.ReadOnly)
					{
						if (arbeitsbereichDatei != aIMLCategory.AIMLTopic.AIMLDatei)
						{
							if (arbeitsbereichDatei is AIMLDatei)
							{
								AIMLDatei aIMLDatei = (AIMLDatei)arbeitsbereichDatei;
								string innerXml = aIMLCategory.ContentNode.InnerXml;
								AIMLCategory aIMLCategory2 = aIMLDatei.RootTopic.AddNewCategory();
								aIMLCategory2.ContentNode.InnerXml = innerXml;
								e.Effect = DragDropEffects.Move;
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
			else if (e.Data.GetDataPresent(typeof(AIMLTopic)))
			{
				AIMLTopic aIMLTopic = (AIMLTopic)e.Data.GetData(typeof(AIMLTopic));
				if (aIMLTopic != null)
				{
					IArbeitsbereichDatei arbeitsbereichDatei2 = this.DateiUnterPos(new Point(e.X, e.Y));
					if (arbeitsbereichDatei2 != null && !arbeitsbereichDatei2.ReadOnly)
					{
						if (arbeitsbereichDatei2 != aIMLTopic.AIMLDatei)
						{
							if (arbeitsbereichDatei2 is AIMLDatei)
							{
								AIMLDatei aIMLDatei2 = (AIMLDatei)arbeitsbereichDatei2;
								AIMLTopic aIMLTopic2 = aIMLDatei2.AddNewTopic();
								aIMLTopic2.TopicNode.InnerXml = aIMLTopic.TopicNode.InnerXml;
								aIMLTopic2.Name = aIMLTopic.Name;
								e.Effect = DragDropEffects.Move;
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
			else
			{
				e.Effect = DragDropEffects.None;
			}
		}

		private void DateiListenAnzeige_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(typeof(AIMLTopic)))
			{
				AIMLTopic aIMLTopic = (AIMLTopic)e.Data.GetData(typeof(AIMLTopic));
				if (aIMLTopic != null)
				{
					IArbeitsbereichDatei arbeitsbereichDatei = this.DateiUnterPos(new Point(e.X, e.Y));
					if (arbeitsbereichDatei != null && !arbeitsbereichDatei.ReadOnly)
					{
						if (arbeitsbereichDatei != aIMLTopic.AIMLDatei)
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
			else if (e.Data.GetDataPresent(typeof(AIMLCategory)))
			{
				AIMLCategory aIMLCategory = (AIMLCategory)e.Data.GetData(typeof(AIMLCategory));
				if (aIMLCategory != null)
				{
					IArbeitsbereichDatei arbeitsbereichDatei2 = this.DateiUnterPos(new Point(e.X, e.Y));
					if (arbeitsbereichDatei2 != null && !arbeitsbereichDatei2.ReadOnly)
					{
						if (arbeitsbereichDatei2 != aIMLCategory.AIMLTopic.AIMLDatei)
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

		private void Fokus_AktAIMLDateiChanged(object sender, EventArgs<IArbeitsbereichDatei> e)
		{
			this.AIMLDateienNeuAnzeigen(false);
		}

		private void treeViewDateien_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (!this._wirdGeradeNeuGezeichnet)
			{
				TreeNode selectedNode = this.treeViewDateien.SelectedNode;
				if (selectedNode == null)
				{
					this.AktuelleDatei = null;
				}
				else if (selectedNode.Tag != null)
				{
					this.AktuelleDatei = (IArbeitsbereichDatei)selectedNode.Tag;
				}
				else
				{
					this.AktuelleDatei = null;
				}
			}
		}

		private void toolStripButtonSave_Click(object sender, EventArgs e)
		{
			this.Save();
		}

		private void toolStripButtonOpenFile_Click(object sender, EventArgs e)
		{
			this.openFileDialog.FileName = "";
			this.openFileDialog.Multiselect = false;
			this.openFileDialog.AddExtension = false;
			this.openFileDialog.CheckFileExists = true;
			this.openFileDialog.CheckPathExists = true;
			this.openFileDialog.DefaultExt = "aiml";
			this.openFileDialog.Filter = "aiml files (*.aiml)|*.aiml|xml files (*.xml)|*.xml|startup files (*.startup)|*.startup";
			this.openFileDialog.Title = ResReader.Reader.GetString("BitteAIMLDateiWaehlen");
			this.openFileDialog.ShowDialog();
			string fileName = this.openFileDialog.FileName;
			if (!string.IsNullOrEmpty(fileName))
			{
				string text = fileName.ToLower();
				if (text.EndsWith(".aiml") || text.EndsWith(".xml"))
				{
					this.VorhandeneAIMLDateiHinzufuegen(fileName, true);
				}
				else if (text.EndsWith(".startup"))
				{
					this.VorhandeneStartupDateiHinzufuegen(fileName, true);
				}
			}
		}

		private void VorhandeneStartupDateiHinzufuegen(string dateiname, bool dateiAlsChangedMarkieren)
		{
			if (File.Exists(dateiname))
			{
				try
				{
					StartupDatei startupDatei = (StartupDatei)(this.AktuelleDatei = this._arbeitsbereich.Dateiverwaltung.VorhandeneExterneStartupDateiImportieren(dateiname, this._arbeitsbereich));
					if (dateiAlsChangedMarkieren && startupDatei != null)
					{
						startupDatei.IsChanged = true;
					}
					this.AIMLDateienNeuAnzeigen(true);
				}
				catch (AIMLDatei.AIMLDateiNichtGeladenException ex)
				{
					Debugger.GlobalDebugger.FehlerZeigen(string.Format(ResReader.Reader.GetString("AIMLDateiLadeFehler"), dateiname, ex.Message), this, "LadeDatei");
				}
			}
		}

		private void VorhandeneAIMLDateiHinzufuegen(string dateiname, bool dateiAlsChangedMarkieren)
		{
			if (File.Exists(dateiname))
			{
				try
				{
					AIMLDatei aIMLDatei = (AIMLDatei)(this.AktuelleDatei = this._arbeitsbereich.Dateiverwaltung.VorhandeneExterneAimlDateiImportieren(dateiname, this._arbeitsbereich));
					if (dateiAlsChangedMarkieren && aIMLDatei != null)
					{
						aIMLDatei.IsChanged = true;
					}
					this.AIMLDateienNeuAnzeigen(true);
				}
				catch (AIMLDatei.AIMLDateiNichtGeladenException ex)
				{
					Debugger.GlobalDebugger.FehlerZeigen(string.Format(ResReader.Reader.GetString("AIMLDateiLadeFehler"), dateiname, ex.Message), this, "LadeDatei");
				}
			}
		}

		private void newStartupDateiToolStripMenuItem_Click(object sender, EventArgs e)
		{
			StartupDatei startupDatei = (StartupDatei)(this.AktuelleDatei = this._arbeitsbereich.Dateiverwaltung.AddLeereStartupDatei(this._arbeitsbereich));
			this.AIMLDateienNeuAnzeigen(true);
		}

		private void newAimlFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.AktuelleDatei = this._arbeitsbereich.Dateiverwaltung.AddAimlLeereDatei(this._arbeitsbereich, false);
			this.AIMLDateienNeuAnzeigen(true);
		}

		private void toolStripButtonDelete_Click(object sender, EventArgs e)
		{
			IArbeitsbereichDatei aktuelleDatei = this.AktuelleDatei;
			if (aktuelleDatei != null && this._arbeitsbereich.Dateiverwaltung.DateiLoeschen(aktuelleDatei, this._arbeitsbereich))
			{
				this.AIMLDateienNeuAnzeigen(true);
				Application.DoEvents();
				this.ErsteDateiSelektieren();
			}
		}

		private void AIMLDateienNeuAnzeigen(bool vorherLeeren)
		{
			vorherLeeren = true;
			if (!this._wirdGeradeNeuGezeichnet)
			{
				this._wirdGeradeNeuGezeichnet = true;
				if (vorherLeeren)
				{
					IOrderedEnumerable<IArbeitsbereichDatei> orderedEnumerable = from IArbeitsbereichDatei a in this._arbeitsbereich.Dateiverwaltung.Dateien
					orderby a.SortKey
					select a;
					this.treeViewDateien.Nodes.Clear();
					int num = 0;
					foreach (IArbeitsbereichDatei item2 in orderedEnumerable)
					{
						string titel = item2.Titel;
						if (string.IsNullOrEmpty(titel))
						{
							titel = item2.Titel;
						}
						TreeNode treeNode = new TreeNode(titel);
						treeNode.Tag = item2;
						this.ListenEintragBeschriften(treeNode, false);
						TreeNode parentTreeNode = this.GetParentTreeNode(item2.Unterverzeichnisse);
						if (parentTreeNode == null)
						{
							this.treeViewDateien.Nodes.Add(treeNode);
						}
						else
						{
							parentTreeNode.Nodes.Add(treeNode);
						}
					}
				}
				else
				{
					foreach (TreeNode node in this.treeViewDateien.Nodes)
					{
						this.ListenEintragBeschriften(node, true);
					}
				}
				this._wirdGeradeNeuGezeichnet = false;
			}
		}

		private TreeNode GetParentTreeNode(string[] unterverzeichnisse)
		{
			if (unterverzeichnisse == null || unterverzeichnisse.Length == 0)
			{
				return null;
			}
			TreeNode treeNode = null;
			int i = 0;
			ArrayList arrayList = new ArrayList();
			foreach (TreeNode node in this.treeViewDateien.Nodes)
			{
				if (node.Parent == null)
				{
					arrayList.Add(node);
				}
			}
			for (; i < unterverzeichnisse.Length; i++)
			{
				TreeNode treeNode3 = null;
				foreach (TreeNode item in arrayList)
				{
					if (item.Text == unterverzeichnisse[i])
					{
						treeNode3 = item;
						break;
					}
				}
				if (treeNode3 == null)
				{
					TreeNode treeNode5 = new TreeNode(unterverzeichnisse[i]);
					if (treeNode == null)
					{
						this.treeViewDateien.Nodes.Add(treeNode5);
					}
					else
					{
						treeNode.Nodes.Add(treeNode5);
					}
					treeNode = treeNode5;
				}
				else
				{
					treeNode = treeNode3;
				}
				arrayList = new ArrayList();
				foreach (TreeNode node2 in treeNode.Nodes)
				{
					arrayList.Add(node2);
				}
			}
			treeNode.ImageKey = "paket";
			treeNode.SelectedImageKey = treeNode.ImageKey;
			return treeNode;
		}

		private void ListenEintragBeschriften(TreeNode item, bool unterNodesAuch)
		{
			if (item.Tag != null && item.Tag is IArbeitsbereichDatei)
			{
				IArbeitsbereichDatei arbeitsbereichDatei = (IArbeitsbereichDatei)item.Tag;
				Color color;
				Color color2;
				string text;
				if (arbeitsbereichDatei.HatFehler)
				{
					color = Color.Red;
					color2 = Color.Black;
					text = string.Format("{0} ({1})", arbeitsbereichDatei.Titel, ResReader.Reader.GetString("ungueltig"));
				}
				else if (this._arbeitsbereich.DieseAIMLDateiNichtExportieren(arbeitsbereichDatei))
				{
					color = this.treeViewDateien.BackColor;
					color2 = Color.LightGray;
					text = string.Format("[{0}] ({1})", arbeitsbereichDatei.Titel, ResReader.Reader.GetString("DieseDateiNichtExportieren"));
				}
				else if (arbeitsbereichDatei.ReadOnly)
				{
					color2 = Color.Black;
					color = Color.FromArgb(0, 240, 240, 255);
					text = arbeitsbereichDatei.Titel;
				}
				else
				{
					color2 = Color.Black;
					color = this.treeViewDateien.BackColor;
					text = arbeitsbereichDatei.Titel;
				}
				if (arbeitsbereichDatei is AIMLDatei)
				{
					item.ImageKey = "aiml";
				}
				if (arbeitsbereichDatei is StartupDatei)
				{
					item.ImageKey = "startup";
				}
				if (item.Text != text)
				{
					item.Text = text;
				}
				if (item.ForeColor != color2)
				{
					item.ForeColor = color2;
				}
				if (item.BackColor != color)
				{
					item.BackColor = color;
				}
				bool flag = false;
				if (this.AktuelleDatei != null)
				{
					flag = (arbeitsbereichDatei == this.AktuelleDatei);
				}
				if (flag)
				{
					this.treeViewDateien.SelectedNode = item;
				}
			}
			if (unterNodesAuch)
			{
				foreach (TreeNode node in item.Nodes)
				{
					this.ListenEintragBeschriften(node, true);
				}
			}
			item.SelectedImageKey = item.ImageKey;
		}

		private void SteuerElementeBenennen()
		{
			this.toolStripButtonSave.Text = ResReader.Reader.GetString("toolStripButtonSave");
			this.toolStripButtonDelete.Text = ResReader.Reader.GetString("toolStripButtonDelete");
			this.toolStripButtonNewFile.Text = ResReader.Reader.GetString("toolStripButtonNewFile");
			this.toolStripButtonOpenFile.Text = ResReader.Reader.GetString("toolStripButtonOpenFile");
			this.toolStripButtonFehlerZeigen.Text = ResReader.Reader.GetString("toolStripButtonFehlerzeigen");
			this.toolStripButtonAIMLDateiUmbenennen.Text = ResReader.Reader.GetString("toolStripButtonAIMLDateiUmbenennen");
		}

		private void uAIMLDateiListe_Resize(object sender, EventArgs e)
		{
			this.treeViewDateien.Top = this.toolStrip1.Top + this.toolStrip1.Height;
			this.treeViewDateien.Left = 0;
			TreeView treeView = this.treeViewDateien;
			Size clientSize = base.ClientSize;
			treeView.Height = clientSize.Height - this.treeViewDateien.Top;
			TreeView treeView2 = this.treeViewDateien;
			clientSize = base.ClientSize;
			treeView2.Width = clientSize.Width;
			clientSize = this.treeViewDateien.ClientSize;
			int num = clientSize.Width - 3;
			int num2 = num;
		}

		private void AIMLDateiChangedZeigen()
		{
			IArbeitsbereichDatei aktuelleDatei = this.AktuelleDatei;
			if (aktuelleDatei != null)
			{
				if (aktuelleDatei.ReadOnly)
				{
					this.toolStripButtonSave.Enabled = false;
					this.toolStripButtonDelete.Enabled = false;
					this.toolStripButtonFehlerZeigen.Enabled = aktuelleDatei.HatFehler;
					this.toolStripButtonAIMLDateiUmbenennen.Enabled = false;
				}
				else
				{
					this.toolStripButtonSave.Enabled = aktuelleDatei.IsChanged;
					this.toolStripButtonDelete.Enabled = true;
					this.toolStripButtonFehlerZeigen.Enabled = aktuelleDatei.HatFehler;
					this.toolStripButtonAIMLDateiUmbenennen.Enabled = true;
				}
			}
			else
			{
				this.toolStripButtonFehlerZeigen.Enabled = false;
				this.toolStripButtonSave.Enabled = false;
				this.toolStripButtonDelete.Enabled = false;
				this.toolStripButtonAIMLDateiUmbenennen.Enabled = false;
			}
		}

		private void Save()
		{
			IArbeitsbereichDatei aktuelleDatei = this.AktuelleDatei;
			if (aktuelleDatei != null)
			{
				bool flag = default(bool);
				aktuelleDatei.Save(out flag);
				this.AIMLDateiChangedZeigen();
			}
		}

		private void tmrRefresh_Tick(object sender, EventArgs e)
		{
			this.AIMLDateiChangedZeigen();
		}

		private void toolStripButtonFehlerZeigen_Click(object sender, EventArgs e)
		{
			IArbeitsbereichDatei aktuelleDatei = this.AktuelleDatei;
			if (aktuelleDatei != null && aktuelleDatei is AIMLDatei)
			{
				((AIMLDatei)aktuelleDatei).GegenAIMLDTDPruefen(AIMLDatei.PruefFehlerVerhalten.FehlerDirektZeigen);
			}
		}

		private void toolStripButtonUmbenennen_Click(object sender, EventArgs e)
		{
			IArbeitsbereichDatei aktuelleDatei = this.AktuelleDatei;
			if (aktuelleDatei != null)
			{
				bool flag = false;
				bool flag2 = false;
				while (!flag2)
				{
					string text = InputBox.Show(ResReader.Reader.GetString("BitteNeuenNamenFuerAIMLDatei"), ResReader.Reader.GetString("AIMLDateiUmbenennen"), aktuelleDatei.Titel, out flag);
					if (string.IsNullOrEmpty(text))
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
						if (aktuelleDatei.Titel == text)
						{
							flag2 = true;
						}
						else
						{
							string text2 = string.Format("\\{0}.aiml", text);
							string[] unterverzeichnisse = aktuelleDatei.Unterverzeichnisse;
							foreach (string arg in unterverzeichnisse)
							{
								text2 = string.Format("\\{0}", arg) + text2;
							}
							text2 = this._arbeitsbereich.Arbeitsverzeichnis + "\\" + text2;
							bool flag3 = false;
							aktuelleDatei.Save(out flag3);
							if (!flag3)
							{
								string text3 = default(string);
								if (this.AIMLDateiUmbenennen(aktuelleDatei.Dateiname, text2, out text3))
								{
									this._arbeitsbereich.Dateiverwaltung.Dateien.Remove(aktuelleDatei);
									IArbeitsbereichDatei arbeitsbereichDatei2 = this.AktuelleDatei = this._arbeitsbereich.Dateiverwaltung.LadeEinzelneAimlDateiDirektOhneKopieEin(text2, this._arbeitsbereich);
									flag2 = true;
									this.AIMLDateienNeuAnzeigen(true);
								}
								else
								{
									MessageBox.Show(text3, ResReader.Reader.GetString("KonnteAIMLDateiNichtUmbenennen"));
									Debugger.GlobalDebugger.Protokolliere("Konnte AIMLDatei '" + aktuelleDatei.Dateiname + "' nicht umbenennen:" + text3, Debugger.ProtokollTypen.Fehlermeldung);
								}
							}
						}
					}
				}
			}
		}

		public bool AIMLDateiUmbenennen(string alterDateiname, string neuerDateiname, out string grundWennNichtErfolgreich)
		{
			try
			{
				if (File.Exists(neuerDateiname))
				{
					grundWennNichtErfolgreich = ResReader.Reader.GetString("AIMLDateiSchonVorhanden");
					return false;
				}
				File.Move(alterDateiname, neuerDateiname);
			}
			catch (Exception ex)
			{
				grundWennNichtErfolgreich = ex.Message;
				return false;
			}
			grundWennNichtErfolgreich = null;
			return true;
		}

		private void ErsteDateiSelektieren()
		{
			if (this._arbeitsbereich != null)
			{
				if (this.treeViewDateien.Nodes.Count > 0)
				{
					TreeNode treeNode = this.treeViewDateien.Nodes[0];
					this._arbeitsbereich.Fokus.AktDatei = (IArbeitsbereichDatei)treeNode.Tag;
				}
				else
				{
					this._arbeitsbereich.Fokus.AktDatei = null;
				}
			}
		}

		private IArbeitsbereichDatei DateiUnterPos(Point screenPos)
		{
			Point point = this.treeViewDateien.PointToClient(screenPos);
			TreeNode nodeAt = this.treeViewDateien.GetNodeAt(point.X, point.Y);
			if (nodeAt != null)
			{
				IArbeitsbereichDatei arbeitsbereichDatei = (IArbeitsbereichDatei)nodeAt.Tag;
				if (arbeitsbereichDatei != null)
				{
					return arbeitsbereichDatei;
				}
				return null;
			}
			return null;
		}

		private void toolStripButtonHinzugelinkteDateienWaehlen_Click(object sender, EventArgs e)
		{
			GaitoBotEditorCore.ContentHinzulinken.ContentHinzulinken contentHinzulinken = new GaitoBotEditorCore.ContentHinzulinken.ContentHinzulinken(this.Arbeitsbereich);
			contentHinzulinken.ShowDialog();
		}
	}
}
