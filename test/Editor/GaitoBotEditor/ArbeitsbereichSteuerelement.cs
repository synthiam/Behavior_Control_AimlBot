using de.springwald.toolbox;
using de.springwald.xml;
using de.springwald.xml.editor;
using GaitoBotEditor.Properties;
using GaitoBotEditorCore;
using GaitoBotEditorCore.ContentHinzulinken;
using GaitoBotEditorCore.controls;
using MultiLang;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using TD.SandDock;
using TD.SandDock.Rendering;

namespace GaitoBotEditor
{
	public class ArbeitsbereichSteuerelement : UserControl
	{
		private bool _erstesResize = true;

		private Arbeitsbereich _arbeitsbereich;

		private IContainer components = null;

		private DockContainer dockContainerUntenSuchenTestEtc;

		private DockingRules dockingRules1;

		private ucCategoryXMLEditor ucXMLEditor;

		private DockableWindow dockWindowTestBot;

		private SandDockManager sandDockManager;

		private DocumentContainer documentContainerCategoryEdit;

		private TabbedDocument tabbedDocumentGrafischeAnsicht;

		private TabbedDocument tabbedDocumentAIMLQuellcode;

		private ucXMLQuellcodeDebugger ucXMLQuellcodeDebugger;

		private DockableWindow dockWindowAIMLDateiListe;

		private ucAIMLDateiListe ucProjektAIMLDateien;

		private DockableWindow dockWindowTopicListe;

		private ucTopicListe ucTopicListe;

		private DockContainer dockContainer1;

		private DockableWindow dockWindowCategoryListe;

		private ucCategoryListe ucCategoryListe;

		private DockContainer dockContainer3;

		private DockableWindow dockWindowXMLInsertElement;

		private DockableWindow dockWindowEditXMLAttribute;

		private ucXMLEditAttributes ucXMLEditAttributes;

		private DockContainer dockContainer6;

		private ucXMLToolbar ucXMLToolbar;

		private DockableWindow dockWindowSuchen;

		private ucArbeitsbereichToolbar ucArbeitsbereichToolbar;

		private Suchen SuchSteuerelement;

		private DockableWindow dockableWindowExport;

		private Export ExportArbeitsbereich;

		private ucBotTest ucBotTest;

		private DockableWindow dockableWindowWorkflow;

		private ucWorkflowToolbar ucWorkflowToolbar;

		private DockableWindow dockableWindowMostSRAIS;

		private ucMeisteSRAIs ucBesteSRAIZiele;

		private ucXMLAddElement2 ucXMLAddElement;

		private ucWorkflowScrollbox ucWorkflowScrollbox;

		private DockableWindow dockableWindowPublish;

		private Publizieren PubliziereArbeitsbereich;

		public Arbeitsbereich Arbeitsbereich
		{
			set
			{
				if (this._arbeitsbereich != null)
				{
					throw new ApplicationException("Diesem ArbeitsbereichSteuerelement wurde bereits ein Arbeitsbereich zugewiesen");
				}
				this._arbeitsbereich = value;
				this.ucWorkflowToolbar.Arbeitsbereich = this._arbeitsbereich;
				this.ucWorkflowScrollbox.Arbeitsbereich = this._arbeitsbereich;
				this.ucXMLEditor.Arbeitsbereich = this._arbeitsbereich;
				this.ucCategoryListe.Arbeitsbereich = this._arbeitsbereich;
				this.ucTopicListe.Arbeitsbereich = this._arbeitsbereich;
				this.ucProjektAIMLDateien.Arbeitsbereich = this._arbeitsbereich;
				this.ucArbeitsbereichToolbar.Arbeitsbereich = this._arbeitsbereich;
				this.ucBesteSRAIZiele.Arbeitsbereich = this._arbeitsbereich;
				this.SuchSteuerelement.Arbeitsbereich = this._arbeitsbereich;
				this.ExportArbeitsbereich.Arbeitsbereich = this._arbeitsbereich;
				this.PubliziereArbeitsbereich.AktArbeitsbereich = this._arbeitsbereich;
				this.ucBotTest.Arbeitsbereich = this._arbeitsbereich;
				this._arbeitsbereich.SucheImArbeitsbereich += this._arbeitsbereich_SucheImArbeitsbereich;
				this.Cursor = Cursors.Default;
				if (value.ContentElementUniqueIds.Length == 0)
				{
					ContentHinzulinken contentHinzulinken = new ContentHinzulinken(value);
					contentHinzulinken.ShowDialog();
				}
				else
				{
					this._arbeitsbereich.Dateiverwaltung.VordefinierteDateienHinzulinken(this._arbeitsbereich);
				}
				if (!(from a in this._arbeitsbereich.Dateiverwaltung.AlleAimlDateien
				from t in a.getTopics()
				from c in t.Categories
				where c.Pattern == "TARGET BOTSTART"
				select c).Any())
				{
					AIMLDatei aIMLDatei = this._arbeitsbereich.Dateiverwaltung.AddAimlLeereDatei(this._arbeitsbereich, true);
					if (aIMLDatei != null)
					{
						aIMLDatei.MitTargetBotStartFuellen();
					}
				}
			}
		}

		public ArbeitsbereichSteuerelement()
		{
			this.InitializeComponent();
			base.Load += this.ProjektSteuerelement_Load;
		}

		private void ProjektSteuerelement_Load(object sender, EventArgs e)
		{
			this.sandDockManager.Renderer = new Office2003Renderer();
			this.SteuerElementeVerheiraten();
			this.ElementeBenennen();
			this.dockWindowTestBot.Collapsed = true;
			this.dockWindowSuchen.Collapsed = true;
			this.dockableWindowPublish.Collapsed = true;
			this.dockableWindowWorkflow.Collapsed = false;
			this.dockWindowTestBot.Collapsed = false;
			base.Resize += this.ArbeitsbereichSteuerelement_Resize;
			this.dockableWindowWorkflow.Resize += this.dockableWindowWorkflow_Resize;
			foreach (Control control in base.Controls)
			{
				if (control.Name == "dockContainerUntenSuchenTestEtc")
				{
					control.Height = (ToolboxSonstiges.IstEntwicklungsmodus ? 400 : 300);
				}
			}
		}

		private void ArbeitsbereichSteuerelement_Resize(object sender, EventArgs e)
		{
			if (this._erstesResize)
			{
				this._erstesResize = false;
				if (base.Height > 800)
				{
					this.dockableWindowWorkflow.Collapsed = false;
					this.dockWindowTestBot.Collapsed = false;
				}
				else
				{
					this.dockableWindowWorkflow.Collapsed = true;
					this.dockWindowTestBot.Collapsed = true;
				}
			}
			this.dockContainerUntenSuchenTestEtc.Height = Math.Max(200, Math.Min(400, (int)((double)base.Height * 0.3)));
		}

		private void _arbeitsbereich_SucheImArbeitsbereich(object sender, Arbeitsbereich.SucheImArbeitsbereichEventArgs e)
		{
			this.dockWindowSuchen.Open();
		}

		private void SteuerElementeVerheiraten()
		{
			this.ucXMLAddElement.XMLEditor = this.ucXMLEditor.XmlEditor;
			this.ucXMLEditAttributes.XMLEditor = this.ucXMLEditor.XmlEditor;
			this.ucXMLQuellcodeDebugger.XMLEditor = this.ucXMLEditor.XmlEditor;
			this.ucXMLToolbar.XMLEditor = this.ucXMLEditor.XmlEditor;
			this.SuchSteuerelement.SuchTitelChanged += this.SuchSteuerelement_SuchTitelChanged;
			this.ucArbeitsbereichToolbar.ExportAnzeigen += this.ucArbeitsbereichToolbar_ExportAnzeigen;
			this.ucArbeitsbereichToolbar.ManuellenTestAnzeigen += this.ucArbeitsbereichToolbar_ManuellenTestAnzeigen;
			this.ucArbeitsbereichToolbar.PublizierenAnzeigen += this.ucArbeitsbereichToolbar_PublizierenAnzeigen;
			this.ucWorkflowToolbar.SyncCategoryAngewaehlt += this.ucWorkflowToolbar_SyncCategoryAngewaehlt;
			this.ucWorkflowToolbar.AnzeigeRefreshen += this.ucWorkflowToolbar_AnzeigeRefreshen;
		}

		private void SuchSteuerelement_SuchTitelChanged(object sender, Suchen.SuchTitelChangedEventArgs e)
		{
			this.dockWindowSuchen.Text = e.Titel;
		}

		private void ucArbeitsbereichToolbar_ExportAnzeigen(object sender, EventArgs e)
		{
			this.dockableWindowExport.Open();
		}

		private void ucArbeitsbereichToolbar_PublizierenAnzeigen(object sender, EventArgs e)
		{
			this.dockableWindowPublish.Open();
		}

		private void ucArbeitsbereichToolbar_ManuellenTestAnzeigen(object sender, EventArgs e)
		{
			this.dockWindowTestBot.Open();
		}

		private void ElementeBenennen()
		{
			string text = "";
			string str = text;
			int num = Assembly.GetExecutingAssembly().GetName().Version.Major;
			text = str + num.ToString();
			string str2 = text;
			num = Assembly.GetExecutingAssembly().GetName().Version.Minor;
			text = str2 + "." + num.ToString();
			this.Text = string.Format(global::MultiLang.ml.ml_string(58, "GaitoBot AIML Editor V {0} - {1}"), text, "");
		}

		private void tabbedDocumentGrafischeAnsicht_Resize(object sender, EventArgs e)
		{
			this.ucXMLEditor.Left = 0;
			this.ucXMLEditor.Top = this.ucXMLToolbar.Top + this.ucXMLToolbar.Height;
			ucCategoryXMLEditor ucCategoryXMLEditor = this.ucXMLEditor;
			Size clientSize = this.tabbedDocumentGrafischeAnsicht.ClientSize;
			ucCategoryXMLEditor.Width = clientSize.Width;
			ucCategoryXMLEditor ucCategoryXMLEditor2 = this.ucXMLEditor;
			clientSize = this.tabbedDocumentGrafischeAnsicht.ClientSize;
			ucCategoryXMLEditor2.Height = clientSize.Height - this.ucXMLEditor.Top;
		}

		private void dockableWindowWorkflow_Resize(object sender, EventArgs e)
		{
			this.ucWorkflowScrollbox.Left = 0;
			this.ucWorkflowScrollbox.Top = this.ucWorkflowToolbar.Top + this.ucWorkflowToolbar.Height;
			ucWorkflowScrollbox obj = this.ucWorkflowScrollbox;
			Size clientSize = this.dockableWindowWorkflow.ClientSize;
			obj.Width = clientSize.Width;
			ucWorkflowScrollbox obj2 = this.ucWorkflowScrollbox;
			clientSize = this.dockableWindowWorkflow.ClientSize;
			obj2.Height = clientSize.Height - this.ucWorkflowScrollbox.Top;
		}

		private void ucWorkflowToolbar_SyncCategoryAngewaehlt(object sender, ucWorkflowToolbar.SyncCategoryAngewaehltEventArgs e)
		{
			this.ucWorkflowScrollbox.Category = e.Category;
		}

		private void ucWorkflowToolbar_AnzeigeRefreshen(object sender, EventArgs e)
		{
			this.ucWorkflowScrollbox.RefreshAnzeige();
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ArbeitsbereichSteuerelement));
			DockingRules dockingRules = new DockingRules();
			this.dockWindowTestBot = new DockableWindow();
			this.ucBotTest = new ucBotTest();
			this.dockWindowSuchen = new DockableWindow();
			this.SuchSteuerelement = new Suchen();
			this.dockableWindowExport = new DockableWindow();
			this.ExportArbeitsbereich = new Export();
			this.dockableWindowPublish = new DockableWindow();
			this.PubliziereArbeitsbereich = new Publizieren();
			this.dockableWindowWorkflow = new DockableWindow();
			this.ucWorkflowToolbar = new ucWorkflowToolbar();
			this.ucWorkflowScrollbox = new ucWorkflowScrollbox();
			this.dockableWindowMostSRAIS = new DockableWindow();
			this.ucBesteSRAIZiele = new ucMeisteSRAIs();
			this.sandDockManager = new SandDockManager();
			this.dockWindowAIMLDateiListe = new DockableWindow();
			this.ucProjektAIMLDateien = new ucAIMLDateiListe();
			this.dockWindowTopicListe = new DockableWindow();
			this.ucTopicListe = new ucTopicListe();
			this.tabbedDocumentGrafischeAnsicht = new TabbedDocument();
			this.ucXMLToolbar = new ucXMLToolbar();
			this.ucXMLEditor = new ucCategoryXMLEditor();
			this.documentContainerCategoryEdit = new DocumentContainer();
			this.tabbedDocumentAIMLQuellcode = new TabbedDocument();
			this.ucXMLQuellcodeDebugger = new ucXMLQuellcodeDebugger();
			this.dockWindowCategoryListe = new DockableWindow();
			this.ucCategoryListe = new ucCategoryListe();
			this.dockContainer1 = new DockContainer();
			this.dockContainer3 = new DockContainer();
			this.dockWindowXMLInsertElement = new DockableWindow();
			this.ucXMLAddElement = new ucXMLAddElement2();
			this.dockWindowEditXMLAttribute = new DockableWindow();
			this.ucXMLEditAttributes = new ucXMLEditAttributes();
			this.dockContainer6 = new DockContainer();
			this.ucArbeitsbereichToolbar = new ucArbeitsbereichToolbar();
			this.dockContainerUntenSuchenTestEtc = new DockContainer();
			this.dockWindowTestBot.SuspendLayout();
			this.dockWindowSuchen.SuspendLayout();
			this.dockableWindowExport.SuspendLayout();
			this.dockableWindowPublish.SuspendLayout();
			this.dockableWindowWorkflow.SuspendLayout();
			this.dockableWindowMostSRAIS.SuspendLayout();
			this.dockWindowAIMLDateiListe.SuspendLayout();
			this.dockWindowTopicListe.SuspendLayout();
			this.tabbedDocumentGrafischeAnsicht.SuspendLayout();
			this.documentContainerCategoryEdit.SuspendLayout();
			this.tabbedDocumentAIMLQuellcode.SuspendLayout();
			this.dockWindowCategoryListe.SuspendLayout();
			this.dockContainer1.SuspendLayout();
			this.dockContainer3.SuspendLayout();
			this.dockWindowXMLInsertElement.SuspendLayout();
			this.dockWindowEditXMLAttribute.SuspendLayout();
			this.dockContainer6.SuspendLayout();
			this.dockContainerUntenSuchenTestEtc.SuspendLayout();
			base.SuspendLayout();
			this.dockWindowTestBot.AllowClose = false;
			this.dockWindowTestBot.Controls.Add(this.ucBotTest);
			this.dockWindowTestBot.FloatingSize = new Size(250, 116);
			this.dockWindowTestBot.Guid = new Guid("b5e67ccb-f99b-4e45-8e8b-53afe1c26699");
			componentResourceManager.ApplyResources(this.dockWindowTestBot, "dockWindowTestBot");
			this.dockWindowTestBot.Name = "dockWindowTestBot";
			this.dockWindowTestBot.ShowOptions = false;
			this.dockWindowTestBot.TabImage = Resources.applications_16;
			componentResourceManager.ApplyResources(this.ucBotTest, "ucBotTest");
			this.ucBotTest.Name = "ucBotTest";
			this.dockWindowSuchen.AllowClose = false;
			this.dockWindowSuchen.Controls.Add(this.SuchSteuerelement);
			dockingRules.AllowDockBottom = true;
			dockingRules.AllowDockLeft = true;
			dockingRules.AllowDockRight = true;
			dockingRules.AllowDockTop = true;
			dockingRules.AllowFloat = false;
			dockingRules.AllowTab = false;
			this.dockWindowSuchen.DockingRules = dockingRules;
			this.dockWindowSuchen.FloatingSize = new Size(450, 400);
			this.dockWindowSuchen.Guid = new Guid("c2581e6a-d96f-49c2-a2b9-a7b4cd022403");
			componentResourceManager.ApplyResources(this.dockWindowSuchen, "dockWindowSuchen");
			this.dockWindowSuchen.Name = "dockWindowSuchen";
			this.dockWindowSuchen.ShowOptions = false;
			this.dockWindowSuchen.TabImage = Resources.BINOCULR;
			componentResourceManager.ApplyResources(this.SuchSteuerelement, "SuchSteuerelement");
			this.SuchSteuerelement.Name = "SuchSteuerelement";
			this.SuchSteuerelement.Titel = "No search started.: 0 hit(s)";
			this.dockableWindowExport.AllowClose = false;
			this.dockableWindowExport.Controls.Add(this.ExportArbeitsbereich);
			this.dockableWindowExport.Cursor = Cursors.Default;
			this.dockableWindowExport.Guid = new Guid("4b077e64-38cc-4b5e-a99f-da9b2730f692");
			componentResourceManager.ApplyResources(this.dockableWindowExport, "dockableWindowExport");
			this.dockableWindowExport.Name = "dockableWindowExport";
			this.dockableWindowExport.ShowOptions = false;
			this.dockableWindowExport.TabImage = Resources.redo_16;
			componentResourceManager.ApplyResources(this.ExportArbeitsbereich, "ExportArbeitsbereich");
			this.ExportArbeitsbereich.Name = "ExportArbeitsbereich";
			this.dockableWindowPublish.AllowClose = false;
			this.dockableWindowPublish.Controls.Add(this.PubliziereArbeitsbereich);
			this.dockableWindowPublish.Guid = new Guid("cf60f5be-8603-4733-9032-662b0d05c24d");
			componentResourceManager.ApplyResources(this.dockableWindowPublish, "dockableWindowPublish");
			this.dockableWindowPublish.Name = "dockableWindowPublish";
			this.dockableWindowPublish.ShowOptions = false;
			this.dockableWindowPublish.TabImage = Resources.Globe;
			componentResourceManager.ApplyResources(this.PubliziereArbeitsbereich, "PubliziereArbeitsbereich");
			this.PubliziereArbeitsbereich.Name = "PubliziereArbeitsbereich";
			this.dockableWindowWorkflow.AllowClose = false;
			this.dockableWindowWorkflow.Controls.Add(this.ucWorkflowToolbar);
			this.dockableWindowWorkflow.Controls.Add(this.ucWorkflowScrollbox);
			this.dockableWindowWorkflow.Guid = new Guid("50ef7a81-ad45-4994-881e-e423baee0104");
			componentResourceManager.ApplyResources(this.dockableWindowWorkflow, "dockableWindowWorkflow");
			this.dockableWindowWorkflow.Name = "dockableWindowWorkflow";
			this.dockableWindowWorkflow.ShowOptions = false;
			this.dockableWindowWorkflow.TabImage = Resources.GRAPH14;
			componentResourceManager.ApplyResources(this.ucWorkflowToolbar, "ucWorkflowToolbar");
			this.ucWorkflowToolbar.Name = "ucWorkflowToolbar";
			componentResourceManager.ApplyResources(this.ucWorkflowScrollbox, "ucWorkflowScrollbox");
			this.ucWorkflowScrollbox.BackColor = Color.White;
			this.ucWorkflowScrollbox.Category = null;
			this.ucWorkflowScrollbox.Name = "ucWorkflowScrollbox";
			this.dockableWindowMostSRAIS.AllowClose = false;
			this.dockableWindowMostSRAIS.Controls.Add(this.ucBesteSRAIZiele);
			this.dockableWindowMostSRAIS.Guid = new Guid("668e99cf-2422-41cf-9838-70d0007347a7");
			componentResourceManager.ApplyResources(this.dockableWindowMostSRAIS, "dockableWindowMostSRAIS");
			this.dockableWindowMostSRAIS.Name = "dockableWindowMostSRAIS";
			this.dockableWindowMostSRAIS.ShowOptions = false;
			componentResourceManager.ApplyResources(this.ucBesteSRAIZiele, "ucBesteSRAIZiele");
			this.ucBesteSRAIZiele.Name = "ucBesteSRAIZiele";
			this.sandDockManager.DockSystemContainer = this;
			this.sandDockManager.OwnerForm = null;
			this.dockWindowAIMLDateiListe.AllowClose = false;
			this.dockWindowAIMLDateiListe.Controls.Add(this.ucProjektAIMLDateien);
			this.dockWindowAIMLDateiListe.Guid = new Guid("51cde5d8-7920-4b1a-86aa-f14518ee0e47");
			componentResourceManager.ApplyResources(this.dockWindowAIMLDateiListe, "dockWindowAIMLDateiListe");
			this.dockWindowAIMLDateiListe.Name = "dockWindowAIMLDateiListe";
			this.dockWindowAIMLDateiListe.ShowOptions = false;
			this.ucProjektAIMLDateien.BackColor = SystemColors.Control;
			componentResourceManager.ApplyResources(this.ucProjektAIMLDateien, "ucProjektAIMLDateien");
			this.ucProjektAIMLDateien.Name = "ucProjektAIMLDateien";
			this.dockWindowTopicListe.AllowClose = false;
			this.dockWindowTopicListe.Controls.Add(this.ucTopicListe);
			this.dockWindowTopicListe.Guid = new Guid("ea5081bf-2f8a-4752-b387-ec19f8831882");
			componentResourceManager.ApplyResources(this.dockWindowTopicListe, "dockWindowTopicListe");
			this.dockWindowTopicListe.Name = "dockWindowTopicListe";
			this.dockWindowTopicListe.ShowOptions = false;
			componentResourceManager.ApplyResources(this.ucTopicListe, "ucTopicListe");
			this.ucTopicListe.Name = "ucTopicListe";
			this.tabbedDocumentGrafischeAnsicht.AllowClose = false;
			this.tabbedDocumentGrafischeAnsicht.BackColor = Color.White;
			this.tabbedDocumentGrafischeAnsicht.Controls.Add(this.ucXMLToolbar);
			this.tabbedDocumentGrafischeAnsicht.Controls.Add(this.ucXMLEditor);
			this.tabbedDocumentGrafischeAnsicht.FloatingSize = new Size(550, 400);
			this.tabbedDocumentGrafischeAnsicht.Guid = new Guid("dd4dda3d-4da1-4a08-8187-813737830be1");
			componentResourceManager.ApplyResources(this.tabbedDocumentGrafischeAnsicht, "tabbedDocumentGrafischeAnsicht");
			this.tabbedDocumentGrafischeAnsicht.Name = "tabbedDocumentGrafischeAnsicht";
			this.tabbedDocumentGrafischeAnsicht.Resize += this.tabbedDocumentGrafischeAnsicht_Resize;
			componentResourceManager.ApplyResources(this.ucXMLToolbar, "ucXMLToolbar");
			this.ucXMLToolbar.Name = "ucXMLToolbar";
			componentResourceManager.ApplyResources(this.ucXMLEditor, "ucXMLEditor");
			this.ucXMLEditor.BackColor = Color.White;
			this.ucXMLEditor.ForeColor = SystemColors.Control;
			this.ucXMLEditor.Name = "ucXMLEditor";
			this.documentContainerCategoryEdit.ContentSize = 400;
			this.documentContainerCategoryEdit.Controls.Add(this.tabbedDocumentGrafischeAnsicht);
			this.documentContainerCategoryEdit.Controls.Add(this.tabbedDocumentAIMLQuellcode);
			this.documentContainerCategoryEdit.LayoutSystem = new SplitLayoutSystem(new SizeF(250f, 400f), Orientation.Horizontal, new LayoutSystemBase[1]
			{
				new DocumentLayoutSystem(new SizeF(281f, 150f), new DockControl[2]
				{
					this.tabbedDocumentGrafischeAnsicht,
					this.tabbedDocumentAIMLQuellcode
				}, this.tabbedDocumentGrafischeAnsicht)
			});
			componentResourceManager.ApplyResources(this.documentContainerCategoryEdit, "documentContainerCategoryEdit");
			this.documentContainerCategoryEdit.Manager = this.sandDockManager;
			this.documentContainerCategoryEdit.Name = "documentContainerCategoryEdit";
			this.tabbedDocumentAIMLQuellcode.AllowClose = false;
			this.tabbedDocumentAIMLQuellcode.BackColor = Color.White;
			this.tabbedDocumentAIMLQuellcode.Controls.Add(this.ucXMLQuellcodeDebugger);
			this.tabbedDocumentAIMLQuellcode.FloatingSize = new Size(550, 400);
			this.tabbedDocumentAIMLQuellcode.Guid = new Guid("ee004c2a-1eda-45f3-aa57-bfd00714eb65");
			componentResourceManager.ApplyResources(this.tabbedDocumentAIMLQuellcode, "tabbedDocumentAIMLQuellcode");
			this.tabbedDocumentAIMLQuellcode.Name = "tabbedDocumentAIMLQuellcode";
			this.ucXMLQuellcodeDebugger.BackColor = SystemColors.Control;
			componentResourceManager.ApplyResources(this.ucXMLQuellcodeDebugger, "ucXMLQuellcodeDebugger");
			this.ucXMLQuellcodeDebugger.Name = "ucXMLQuellcodeDebugger";
			this.dockWindowCategoryListe.AllowClose = false;
			this.dockWindowCategoryListe.Controls.Add(this.ucCategoryListe);
			this.dockWindowCategoryListe.Guid = new Guid("71593c02-4597-41dc-8df3-eff7d26bacc2");
			componentResourceManager.ApplyResources(this.dockWindowCategoryListe, "dockWindowCategoryListe");
			this.dockWindowCategoryListe.Name = "dockWindowCategoryListe";
			this.dockWindowCategoryListe.ShowOptions = false;
			componentResourceManager.ApplyResources(this.ucCategoryListe, "ucCategoryListe");
			this.ucCategoryListe.Name = "ucCategoryListe";
			this.dockContainer1.ContentSize = 400;
			this.dockContainer1.Controls.Add(this.dockWindowCategoryListe);
			componentResourceManager.ApplyResources(this.dockContainer1, "dockContainer1");
			this.dockContainer1.LayoutSystem = new SplitLayoutSystem(new SizeF(250f, 400f), Orientation.Vertical, new LayoutSystemBase[1]
			{
				new ControlLayoutSystem(new SizeF(283f, 272f), new DockControl[1]
				{
					this.dockWindowCategoryListe
				}, this.dockWindowCategoryListe)
			});
			this.dockContainer1.Manager = this.sandDockManager;
			this.dockContainer1.Name = "dockContainer1";
			this.dockContainer3.ContentSize = 250;
			this.dockContainer3.Controls.Add(this.dockWindowXMLInsertElement);
			this.dockContainer3.Controls.Add(this.dockWindowEditXMLAttribute);
			componentResourceManager.ApplyResources(this.dockContainer3, "dockContainer3");
			this.dockContainer3.LayoutSystem = new SplitLayoutSystem(new SizeF(250f, 400f), Orientation.Horizontal, new LayoutSystemBase[2]
			{
				new ControlLayoutSystem(new SizeF(164f, 311f), new DockControl[1]
				{
					this.dockWindowXMLInsertElement
				}, this.dockWindowXMLInsertElement),
				new ControlLayoutSystem(new SizeF(164f, 114f), new DockControl[1]
				{
					this.dockWindowEditXMLAttribute
				}, this.dockWindowEditXMLAttribute)
			});
			this.dockContainer3.Manager = this.sandDockManager;
			this.dockContainer3.Name = "dockContainer3";
			this.dockWindowXMLInsertElement.AllowClose = false;
			this.dockWindowXMLInsertElement.Controls.Add(this.ucXMLAddElement);
			this.dockWindowXMLInsertElement.Guid = new Guid("537d1bfb-ec52-421e-844c-230c301c3825");
			componentResourceManager.ApplyResources(this.dockWindowXMLInsertElement, "dockWindowXMLInsertElement");
			this.dockWindowXMLInsertElement.Name = "dockWindowXMLInsertElement";
			this.dockWindowXMLInsertElement.ShowOptions = false;
			componentResourceManager.ApplyResources(this.ucXMLAddElement, "ucXMLAddElement");
			this.ucXMLAddElement.Name = "ucXMLAddElement";
			this.dockWindowEditXMLAttribute.AllowClose = false;
			this.dockWindowEditXMLAttribute.Controls.Add(this.ucXMLEditAttributes);
			this.dockWindowEditXMLAttribute.Guid = new Guid("89e6e7ee-6ca5-4907-a2aa-fd2748ae354e");
			componentResourceManager.ApplyResources(this.dockWindowEditXMLAttribute, "dockWindowEditXMLAttribute");
			this.dockWindowEditXMLAttribute.Name = "dockWindowEditXMLAttribute";
			this.dockWindowEditXMLAttribute.ShowOptions = false;
			componentResourceManager.ApplyResources(this.ucXMLEditAttributes, "ucXMLEditAttributes");
			this.ucXMLEditAttributes.Name = "ucXMLEditAttributes";
			this.dockContainer6.ContentSize = 250;
			this.dockContainer6.Controls.Add(this.dockWindowAIMLDateiListe);
			this.dockContainer6.Controls.Add(this.dockWindowTopicListe);
			componentResourceManager.ApplyResources(this.dockContainer6, "dockContainer6");
			this.dockContainer6.LayoutSystem = new SplitLayoutSystem(new SizeF(250f, 400f), Orientation.Horizontal, new LayoutSystemBase[2]
			{
				new ControlLayoutSystem(new SizeF(225f, 280f), new DockControl[1]
				{
					this.dockWindowAIMLDateiListe
				}, this.dockWindowAIMLDateiListe),
				new ControlLayoutSystem(new SizeF(225f, 145f), new DockControl[1]
				{
					this.dockWindowTopicListe
				}, this.dockWindowTopicListe)
			});
			this.dockContainer6.Manager = this.sandDockManager;
			this.dockContainer6.Name = "dockContainer6";
			componentResourceManager.ApplyResources(this.ucArbeitsbereichToolbar, "ucArbeitsbereichToolbar");
			this.ucArbeitsbereichToolbar.Name = "ucArbeitsbereichToolbar";
			this.dockContainerUntenSuchenTestEtc.ContentSize = 176;
			this.dockContainerUntenSuchenTestEtc.Controls.Add(this.dockWindowTestBot);
			this.dockContainerUntenSuchenTestEtc.Controls.Add(this.dockWindowSuchen);
			this.dockContainerUntenSuchenTestEtc.Controls.Add(this.dockableWindowExport);
			this.dockContainerUntenSuchenTestEtc.Controls.Add(this.dockableWindowPublish);
			this.dockContainerUntenSuchenTestEtc.Controls.Add(this.dockableWindowWorkflow);
			this.dockContainerUntenSuchenTestEtc.Controls.Add(this.dockableWindowMostSRAIS);
			componentResourceManager.ApplyResources(this.dockContainerUntenSuchenTestEtc, "dockContainerUntenSuchenTestEtc");
			this.dockContainerUntenSuchenTestEtc.LayoutSystem = new SplitLayoutSystem(new SizeF(250f, 400f), Orientation.Vertical, new LayoutSystemBase[2]
			{
				new ControlLayoutSystem(new SizeF(309.3716f, 100f), new DockControl[4]
				{
					this.dockWindowTestBot,
					this.dockWindowSuchen,
					this.dockableWindowExport,
					this.dockableWindowPublish
				}, this.dockWindowTestBot),
				new ControlLayoutSystem(new SizeF(366.6284f, 100f), new DockControl[2]
				{
					this.dockableWindowWorkflow,
					this.dockableWindowMostSRAIS
				}, this.dockableWindowWorkflow)
			});
			this.dockContainerUntenSuchenTestEtc.Manager = this.sandDockManager;
			this.dockContainerUntenSuchenTestEtc.Name = "dockContainerUntenSuchenTestEtc";
			componentResourceManager.ApplyResources(this, "$this");
			base.AutoScaleMode = AutoScaleMode.Font;
			base.Controls.Add(this.documentContainerCategoryEdit);
			base.Controls.Add(this.dockContainer1);
			base.Controls.Add(this.dockContainer3);
			base.Controls.Add(this.dockContainer6);
			base.Controls.Add(this.ucArbeitsbereichToolbar);
			base.Controls.Add(this.dockContainerUntenSuchenTestEtc);
			this.Cursor = Cursors.WaitCursor;
			base.Name = "ArbeitsbereichSteuerelement";
			this.dockWindowTestBot.ResumeLayout(false);
			this.dockWindowSuchen.ResumeLayout(false);
			this.dockableWindowExport.ResumeLayout(false);
			this.dockableWindowPublish.ResumeLayout(false);
			this.dockableWindowWorkflow.ResumeLayout(false);
			this.dockableWindowMostSRAIS.ResumeLayout(false);
			this.dockWindowAIMLDateiListe.ResumeLayout(false);
			this.dockWindowTopicListe.ResumeLayout(false);
			this.tabbedDocumentGrafischeAnsicht.ResumeLayout(false);
			this.documentContainerCategoryEdit.ResumeLayout(false);
			this.tabbedDocumentAIMLQuellcode.ResumeLayout(false);
			this.dockWindowCategoryListe.ResumeLayout(false);
			this.dockContainer1.ResumeLayout(false);
			this.dockContainer3.ResumeLayout(false);
			this.dockWindowXMLInsertElement.ResumeLayout(false);
			this.dockWindowEditXMLAttribute.ResumeLayout(false);
			this.dockContainer6.ResumeLayout(false);
			this.dockContainerUntenSuchenTestEtc.ResumeLayout(false);
			base.ResumeLayout(false);
		}
	}
}
