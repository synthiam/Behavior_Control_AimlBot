using de.springwald.toolbox;
using GaitoBotEditorCore;
using MultiLang;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using TD.SandDock;

namespace GaitoBotEditor {

  public class frmMain : Form {

    private SandDockManager sandDockManager;

    private MainMenu mainMenu;

    private MenuItem mnuProgramm;

    private MenuItem mnuProgrammBeenden;

    private MenuItem mnuAnsicht;

    private MenuItem mnuDebugger;

    private MenuItem mnuHilfe;

    private MenuItem mnuHandbuch;

    private MenuItem mnuHomepage;

    private MenuItem menuItem3;

    private MenuItem mnuInfo;

    private DocumentContainer documentContainer;

    private TabbedDocument tabbedDocumentStartseite;

    private DockContainer dockContainer1;

    private DockableWindow dockableWindowDebug;

    private TextBox txtDebuggerAusgabe;

    private IContainer components;

    private Startseite startseite;

    private ArbeitsbereichVerwaltung _arbeitsbereichVerwaltung;

    private frmAbout _aboutForm;

    protected override void Dispose(bool disposing) {
      if (disposing && this.components != null) {
        this.components.Dispose();
      }
      base.Dispose(disposing);
    }

    private void InitializeComponent() {
      this.components = new Container();
      ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(frmMain));
      this.sandDockManager = new SandDockManager();
      this.mainMenu = new MainMenu(this.components);
      this.mnuProgramm = new MenuItem();
      this.mnuProgrammBeenden = new MenuItem();
      this.mnuAnsicht = new MenuItem();
      this.mnuDebugger = new MenuItem();
      this.mnuHilfe = new MenuItem();
      this.mnuHandbuch = new MenuItem();
      this.mnuHomepage = new MenuItem();
      this.menuItem3 = new MenuItem();
      this.mnuInfo = new MenuItem();
      this.tabbedDocumentStartseite = new TabbedDocument();
      this.startseite = new Startseite();
      this.documentContainer = new DocumentContainer();
      this.dockableWindowDebug = new DockableWindow();
      this.txtDebuggerAusgabe = new TextBox();
      this.dockContainer1 = new DockContainer();
      this.tabbedDocumentStartseite.SuspendLayout();
      this.documentContainer.SuspendLayout();
      this.dockableWindowDebug.SuspendLayout();
      this.dockContainer1.SuspendLayout();
      base.SuspendLayout();
      this.sandDockManager.DockSystemContainer = this;
      this.sandDockManager.OwnerForm = this;
      this.mainMenu.MenuItems.AddRange(new MenuItem[3]
      {
        this.mnuProgramm,
        this.mnuAnsicht,
        this.mnuHilfe
      });
      this.mnuProgramm.Index = 0;
      this.mnuProgramm.MenuItems.AddRange(new MenuItem[1]
      {
        this.mnuProgrammBeenden
      });
      componentResourceManager.ApplyResources(this.mnuProgramm, "mnuProgramm");
      this.mnuProgrammBeenden.Index = 0;
      componentResourceManager.ApplyResources(this.mnuProgrammBeenden, "mnuProgrammBeenden");
      this.mnuProgrammBeenden.Click += this.mnuProgrammBeenden_Click;
      this.mnuAnsicht.Index = 1;
      this.mnuAnsicht.MenuItems.AddRange(new MenuItem[1]
      {
        this.mnuDebugger
      });
      componentResourceManager.ApplyResources(this.mnuAnsicht, "mnuAnsicht");
      this.mnuDebugger.Index = 0;
      componentResourceManager.ApplyResources(this.mnuDebugger, "mnuDebugger");
      this.mnuDebugger.Click += this.mnuDebugger_Click_1;
      this.mnuHilfe.Index = 2;
      this.mnuHilfe.MenuItems.AddRange(new MenuItem[4]
      {
        this.mnuHandbuch,
        this.mnuHomepage,
        this.menuItem3,
        this.mnuInfo
      });
      componentResourceManager.ApplyResources(this.mnuHilfe, "mnuHilfe");
      this.mnuHandbuch.Index = 0;
      componentResourceManager.ApplyResources(this.mnuHandbuch, "mnuHandbuch");
      this.mnuHandbuch.Click += this.mnuHandbuch_Click;
      this.mnuHomepage.Index = 1;
      componentResourceManager.ApplyResources(this.mnuHomepage, "mnuHomepage");
      this.mnuHomepage.Click += this.mnuHomepage_Click;
      this.menuItem3.Index = 2;
      componentResourceManager.ApplyResources(this.menuItem3, "menuItem3");
      this.mnuInfo.Index = 3;
      componentResourceManager.ApplyResources(this.mnuInfo, "mnuInfo");
      this.mnuInfo.Click += this.mnuInfo_Click;
      this.tabbedDocumentStartseite.AllowClose = false;
      this.tabbedDocumentStartseite.AllowCollapse = false;
      this.tabbedDocumentStartseite.Controls.Add(this.startseite);
      this.tabbedDocumentStartseite.FloatingSize = new Size(550, 400);
      this.tabbedDocumentStartseite.Guid = new Guid("9ee7f539-b109-4417-80cb-02aa6c3c738d");
      componentResourceManager.ApplyResources(this.tabbedDocumentStartseite, "tabbedDocumentStartseite");
      this.tabbedDocumentStartseite.Name = "tabbedDocumentStartseite";
      this.tabbedDocumentStartseite.ShowOptions = false;
      this.startseite.BackColor = Color.FromArgb(192, 192, 255);
      componentResourceManager.ApplyResources(this.startseite, "startseite");
      this.startseite.Name = "startseite";
      this.documentContainer.ContentSize = 400;
      this.documentContainer.Controls.Add(this.tabbedDocumentStartseite);
      this.documentContainer.LayoutSystem = new SplitLayoutSystem(new SizeF(250f, 400f), Orientation.Horizontal, new LayoutSystemBase[1]
      {
        new DocumentLayoutSystem(new SizeF(763f, 388f), new DockControl[1]
        {
          this.tabbedDocumentStartseite
        }, this.tabbedDocumentStartseite)
      });
      componentResourceManager.ApplyResources(this.documentContainer, "documentContainer");
      this.documentContainer.Manager = this.sandDockManager;
      this.documentContainer.Name = "documentContainer";
      this.dockableWindowDebug.Controls.Add(this.txtDebuggerAusgabe);
      this.dockableWindowDebug.Guid = new Guid("8098dec4-2a99-4d25-86aa-a35f31fa49d6");
      componentResourceManager.ApplyResources(this.dockableWindowDebug, "dockableWindowDebug");
      this.dockableWindowDebug.Name = "dockableWindowDebug";
      this.txtDebuggerAusgabe.AcceptsReturn = true;
      this.txtDebuggerAusgabe.AcceptsTab = true;
      this.txtDebuggerAusgabe.BorderStyle = BorderStyle.None;
      this.txtDebuggerAusgabe.Cursor = Cursors.IBeam;
      componentResourceManager.ApplyResources(this.txtDebuggerAusgabe, "txtDebuggerAusgabe");
      this.txtDebuggerAusgabe.Name = "txtDebuggerAusgabe";
      this.dockContainer1.ContentSize = 400;
      this.dockContainer1.Controls.Add(this.dockableWindowDebug);
      componentResourceManager.ApplyResources(this.dockContainer1, "dockContainer1");
      this.dockContainer1.LayoutSystem = new SplitLayoutSystem(new SizeF(250f, 400f), Orientation.Vertical, new LayoutSystemBase[1]
      {
        new ControlLayoutSystem(new SizeF(765f, 127f), new DockControl[1]
        {
          this.dockableWindowDebug
        }, this.dockableWindowDebug)
      });
      this.dockContainer1.Manager = this.sandDockManager;
      this.dockContainer1.Name = "dockContainer1";
      componentResourceManager.ApplyResources(this, "$this");
      this.BackColor = Color.DodgerBlue;
      base.Controls.Add(this.documentContainer);
      base.Controls.Add(this.dockContainer1);
      base.Menu = this.mainMenu;
      base.Name = "frmMain";
      base.WindowState = FormWindowState.Maximized;
      base.Load += this.frmMain_Load;
      this.tabbedDocumentStartseite.ResumeLayout(false);
      this.documentContainer.ResumeLayout(false);
      this.dockableWindowDebug.ResumeLayout(false);
      this.dockableWindowDebug.PerformLayout();
      this.dockContainer1.ResumeLayout(false);
      base.ResumeLayout(false);
    }

    public frmMain() {
      if (Config.GlobalConfig.ProgrammSprache == "") {
        Config.GlobalConfig.ProgrammSprache = "en";
      }
      Thread.CurrentThread.CurrentUICulture = new CultureInfo(Config.GlobalConfig.ProgrammSprache);
      this.InitializeComponent();
      base.FormClosing += this.frmMain_FormClosing;
    }

    private void frmMain_Load(object sender, EventArgs e) {
      this.ProgrammTitelUndVorgangTitelAnzeigen(global::MultiLang.ml.ml_string(59, "laed"));
      if (this.LizenzChecken()) {
        if (!ToolboxSonstiges.IstEntwicklungsmodus) {
          frmSplashScreen frmSplashScreen = new frmSplashScreen();
          frmSplashScreen.Show();
          Application.DoEvents();
        }
        this.dockableWindowDebug.Close();
        this.tabbedDocumentStartseite.Enter += this.tabbedDocumentStartseite_Enter;
        Debugger.GlobalDebugger.NeueProtokollZeileEvent += this.GlobalDebugger_NeueProtokollZeileEvent;
        this.txtDebuggerAusgabe.Text = "";
        Debugger.GlobalDebugger.SetzeProtokollDateiname(Config.GlobalConfig.SpeicherVerzeichnis + "\\debuglog.txt", true);
        this.ProgrammTitelUndVorgangTitelAnzeigen();
        this._arbeitsbereichVerwaltung = new ArbeitsbereichVerwaltung();
        this._arbeitsbereichVerwaltung.ArbeitsbereichAddedEvent += this._arbeitsbereichVerwaltung_ArbeitsbereichAddedEvent;
        this.startseite.ArbeitsbereichVerwaltung = this._arbeitsbereichVerwaltung;
        if (ToolboxSonstiges.IstEntwicklungsmodus) {
          this._arbeitsbereichVerwaltung.VorhandenenArbeitsbereichOeffnen(Path.Combine(Config.GlobalConfig.ArbeitsbereicheSpeicherVerzeichnis, "1"));
        }
      } else {
        base.Close();
        base.Dispose();
      }
    }

    private void frmMain_FormClosing(object sender, FormClosingEventArgs e) {
      bool flag = false;
      if (this._arbeitsbereichVerwaltung != null) {
        this._arbeitsbereichVerwaltung.ProgrammSollBeendetWerden(out flag);
      }
      if (flag) {
        e.Cancel = true;
      }
    }

    private bool LizenzChecken() {
      if (LizenzManager.DarfProgrammUeberhauptGestartetWerden) {
        base.Visible = false;
        if (!frmMain.EULAOk()) {
          return false;
        }
        base.Visible = true;
        return true;
      }
      base.Visible = false;
      MessageBox.Show(LizenzManager.WarumDarfProgrammNichtGestartetWerden, Application.ProductName + " " + Application.ProductVersion, MessageBoxButtons.OK);
      return false;
    }

    private static bool EULAOk() {
      if (ToolboxSonstiges.IstEntwicklungsmodus) {
        return true;
      }
      string productVersion = Application.ProductVersion;
      if (Config.GlobalConfig.EULABestaetigtFuer == productVersion) {
        return true;
      }
      EULA eULA = new EULA();
      eULA.Show();
      Application.DoEvents();
      while (eULA.Visible) {
        Application.DoEvents();
      }
      eULA.DarfSchliessen = true;
      if (eULA.Bestaetigt) {
        Config.GlobalConfig.EULABestaetigtFuer = productVersion;
        eULA.Close();
        return true;
      }
      eULA.Close();
      return false;
    }

    private void _arbeitsbereichVerwaltung_ArbeitsbereichAddedEvent(object sender, EventArgs<Arbeitsbereich> e) {
      ArbeitsbereichSteuerelement arbeitsbereichSteuerelement = new ArbeitsbereichSteuerelement();
      TabbedDocument tabbedDocument = new TabbedDocument(this.sandDockManager, arbeitsbereichSteuerelement, global::MultiLang.ml.ml_string(60, "es wird geladen..."));
      tabbedDocument.Enter += this._ArbeitsbereichFenster_Enter;
      arbeitsbereichSteuerelement.Visible = false;
      tabbedDocument.Open();
      tabbedDocument.Show();
      tabbedDocument.AllowClose = true;
      tabbedDocument.Closing += this._ArbeitsbereichFenster_Closing;
      tabbedDocument.Cursor = Cursors.WaitCursor;
      Arbeitsbereich arbeitsbereich = (Arbeitsbereich)(tabbedDocument.Tag = e.Value);
      arbeitsbereich.Dateiverwaltung.AimlDateiWirdGeladenEvent += this.Dateiverwaltung_AimlDateiWirdGeladenEvent;
      arbeitsbereich.Oeffnen();
      arbeitsbereich.NameChangedEvent += this.arbeitsbereich_NameChangedEvent;
      arbeitsbereichSteuerelement.Visible = true;
      arbeitsbereichSteuerelement.Arbeitsbereich = arbeitsbereich;
      Application.DoEvents();
      this.arbeitsbereich_NameChangedEvent(null, null);
      this.ProgrammTitelUndVorgangTitelAnzeigen();
      tabbedDocument.Cursor = Cursors.Default;
    }

    private void arbeitsbereich_NameChangedEvent(object sender, EventArgs e) {
      DockControl[] documents = this.sandDockManager.Documents;
      for (int i = 0; i < documents.Length; i++) {
        TabbedDocument tabbedDocument = (TabbedDocument)documents[i];
        if (tabbedDocument.Tag != null && tabbedDocument.Tag is Arbeitsbereich) {
          Arbeitsbereich arbeitsbereich = (Arbeitsbereich)tabbedDocument.Tag;
          tabbedDocument.Text = string.Format(global::MultiLang.ml.ml_string(62, "Arbeitsbereich \"{0}\""), arbeitsbereich.Name);
        }
      }
    }

    private void _ArbeitsbereichFenster_Closing(object sender, DockControlClosingEventArgs e) {
      if (e.DockControl.Tag != null) {
        Arbeitsbereich arbeitsbereich = (Arbeitsbereich)e.DockControl.Tag;
        bool flag = default(bool);
        arbeitsbereich.ArbeitsbereichSollGeschlossenWerden(out flag);
        if (flag) {
          e.Cancel = true;
        } else {
          arbeitsbereich.NameChangedEvent -= this.arbeitsbereich_NameChangedEvent;
          this._arbeitsbereichVerwaltung.ArbeitsbereichEntfernen(arbeitsbereich);
        }
      }
    }

    private void _ArbeitsbereichFenster_Enter(object sender, EventArgs e) {
      this.ProgrammTitelUndVorgangTitelAnzeigen();
    }

    private void Dateiverwaltung_AimlDateiWirdGeladenEvent(object sender, EventArgs<string> e) {
      string text = e.Value;
      try {
        string[] array = text.Split('\\');
        text = array[array.Length - 1];
      } catch (Exception) {
      }
      string vorgangTitel = string.Format(global::MultiLang.ml.ml_string(63, "Öffne Datei {0}"), text);
      this.ProgrammTitelUndVorgangTitelAnzeigen(vorgangTitel);
      vorgangTitel = string.Format(global::MultiLang.ml.ml_string(63, "Öffne Datei {0}"), e.Value);
      Debugger.GlobalDebugger.Protokolliere(vorgangTitel);
      Application.DoEvents();
    }

    private void mnuDebugger_Click_1(object sender, EventArgs e) {
      this.dockableWindowDebug.Open();
    }

    private void GlobalDebugger_NeueProtokollZeileEvent(object sender, DebuggerNeueZeileEventArgs ae) {
      this.txtDebuggerAusgabe.Text = string.Format("{0}\r\n{1}", ae._inhalt, this.txtDebuggerAusgabe.Text);
      if (this.txtDebuggerAusgabe.Text.Length > 2000) {
        this.txtDebuggerAusgabe.Text = this.txtDebuggerAusgabe.Text.Substring(0, 1500);
      }
      Application.DoEvents();
    }

    private void mnuProgrammBeenden_Click(object sender, EventArgs e) {
      base.Close();
    }

    private void mnuInfo_Click(object sender, EventArgs e) {
      if (this._aboutForm != null) {
        if (!this._aboutForm.Visible) {
          this._aboutForm.Dispose();
          this._aboutForm = null;
        } else {
          this._aboutForm.Show();
          this._aboutForm.BringToFront();
        }
      }
      if (this._aboutForm == null) {
        this._aboutForm = new frmAbout();
        this._aboutForm.Show();
      }
    }

    private void mnuHandbuch_Click(object sender, EventArgs e) {
      ToolboxSonstiges.HTMLSeiteAufrufen(Config.GlobalConfig.URLHandbuch);
    }

    private void mnuHomepage_Click(object sender, EventArgs e) {
      ToolboxSonstiges.HTMLSeiteAufrufen(global::MultiLang.ml.ml_string(54, "http://www.gaitobot.de"));
    }

    private void tabbedDocumentStartseite_Enter(object sender, EventArgs e) {
      this.ProgrammTitelUndVorgangTitelAnzeigen();
    }

    private void ProgrammTitelUndVorgangTitelAnzeigen(string vorgangTitel) {
      this.Text = string.Format(global::MultiLang.ml.ml_string(58, "GaitoBot AIML Editor V {0} - {1}"), LizenzManager.ProgrammVersionNummerAnzeige, vorgangTitel);
    }

    private void ProgrammTitelUndVorgangTitelAnzeigen() {
      if (this.documentContainer.ActiveControl != null) {
        this.ProgrammTitelUndVorgangTitelAnzeigen(this.documentContainer.ActiveControl.Text);
      } else {
        this.ProgrammTitelUndVorgangTitelAnzeigen("");
      }
    }
  }
}
