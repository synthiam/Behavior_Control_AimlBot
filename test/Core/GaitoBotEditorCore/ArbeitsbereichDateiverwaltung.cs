using de.springwald.gaitobot.content;
using de.springwald.toolbox;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace GaitoBotEditorCore
{
	public class ArbeitsbereichDateiverwaltung
	{
		private readonly List<IArbeitsbereichDatei> _dateien;

		private readonly string _arbeitsVerzeichnis;

		public List<IArbeitsbereichDatei> Dateien
		{
			get
			{
				return this._dateien;
			}
		}

		public List<AIMLDatei> AlleAimlDateien
		{
			get
			{
				return (from d in this._dateien
				where d is AIMLDatei
				select (AIMLDatei)d).ToList();
			}
		}

		public bool DateienIsChanged
		{
			get
			{
				return this._dateien.Any((IArbeitsbereichDatei datei) => datei.IsChanged);
			}
		}

		public event EventHandler AimlDateienContentChanged;

		public event EventHandler<EventArgs<IArbeitsbereichDatei>> DateiAddedEvent;

		public event EventHandler<EventArgs<IArbeitsbereichDatei>> DateiRemovedEvent;

		public event EventHandler<EventArgs<string>> AimlDateiWirdGeladenEvent;

		public event EventHandler AimlDateiFertigGeladenEvent;

		protected virtual void OnAimlDateienContentChangedEvent()
		{
			if (this.AimlDateienContentChanged != null)
			{
				this.AimlDateienContentChanged(this, EventArgs.Empty);
			}
		}

		protected virtual void DateiAdded(IArbeitsbereichDatei datei)
		{
			if (this.DateiAddedEvent != null)
			{
				this.DateiAddedEvent(this, new EventArgs<IArbeitsbereichDatei>(datei));
			}
		}

		protected virtual void AimlDateiRemoved(IArbeitsbereichDatei datei)
		{
			if (this.DateiRemovedEvent != null)
			{
				this.DateiRemovedEvent(this, new EventArgs<IArbeitsbereichDatei>(datei));
			}
		}

		protected virtual void AimlDateiWirdGeladen(string dateiname)
		{
			if (this.AimlDateiWirdGeladenEvent != null)
			{
				this.AimlDateiWirdGeladenEvent(this, new EventArgs<string>(dateiname));
			}
		}

		protected virtual void AimlDateiFertigGeladen()
		{
			if (this.AimlDateiFertigGeladenEvent != null)
			{
				this.AimlDateiFertigGeladenEvent(this, EventArgs.Empty);
			}
		}

		public ArbeitsbereichDateiverwaltung(string arbeitsverzeichnis)
		{
			this._arbeitsVerzeichnis = arbeitsverzeichnis;
			this._dateien = new List<IArbeitsbereichDatei>();
		}

		public void VordefinierteDateienHinzulinken(Arbeitsbereich arbeitsbereich)
		{
			if (arbeitsbereich.Dateiverwaltung != this)
			{
				throw new ApplicationException("Arbeitsbereich passt nicht zur ArbeitsbereichDateiverwaltung!");
			}
			ContentManager contentManager = new ContentManager();
			string[] contentElementUniqueIds = arbeitsbereich.ContentElementUniqueIds;
			foreach (string uniqueId in contentElementUniqueIds)
			{
				if (!(from d in this._dateien
				where !string.IsNullOrEmpty(d.ZusatzContentUniqueId) && d.ZusatzContentUniqueId == uniqueId
				select d).Any())
				{
					List<ContentDokument> dokumente = contentManager.GetDokumente(uniqueId);
					foreach (ContentDokument item in dokumente)
					{
						string dateiExtension = item.DateiExtension;
						if (!(dateiExtension == "aiml"))
						{
							if (dateiExtension == "startup")
							{
								StartupDatei datei = this.AddStartUpdateiHinzugelinkteContent(item, arbeitsbereich);
								this.DateiAdded(datei);
								continue;
							}
							throw new ApplicationException(string.Format("Unbekannte Content-Datei-Extension '{0}' in '{1}'", item.DateiExtension, item.Titel));
						}
						AIMLDatei datei2 = this.AddAimlHinzugelinkteContentDatei(item, arbeitsbereich);
						this.DateiAdded(datei2);
					}
				}
			}
			List<IArbeitsbereichDatei> list = new List<IArbeitsbereichDatei>();
			foreach (IArbeitsbereichDatei item2 in this.Dateien)
			{
				if (!string.IsNullOrEmpty(item2.ZusatzContentUniqueId) && !arbeitsbereich.ContentElementUniqueIds.Contains(item2.ZusatzContentUniqueId))
				{
					list.Add(item2);
				}
			}
			foreach (IArbeitsbereichDatei item3 in list)
			{
				this.DateiLoeschen(item3, arbeitsbereich);
			}
		}

		public AIMLDatei VorhandeneExterneAimlDateiImportieren(string externerDateiname, Arbeitsbereich zielArbeitsbereich)
		{
			if (zielArbeitsbereich.Dateiverwaltung != this)
			{
				throw new ApplicationException("Arbeitsbereich passt nicht zur ArbeitsbereichDateiverwaltung!");
			}
			string vorgabe = AIMLDatei.TitelAusDateinameHerleiten(externerDateiname);
			bool flag = false;
			string text = this.ErmittleFreienNamenFuerAimlDatei(vorgabe, "aiml", out flag);
			if (flag)
			{
				return null;
			}
			try
			{
				File.Copy(externerDateiname, text);
			}
			catch (Exception ex)
			{
				throw new AIMLDatei.AIMLDateiNichtGeladenException(ex.Message);
			}
			this.AimlDateiWirdGeladen(text);
			AIMLDatei aIMLDatei = new AIMLDatei(zielArbeitsbereich);
			aIMLDatei.OnChanged += this.AimlDateiOnChanged;
			try
			{
				aIMLDatei.LiesAusDatei(text, this._arbeitsVerzeichnis);
				this._dateien.Add(aIMLDatei);
				this.AimlDateiFertigGeladen();
				return aIMLDatei;
			}
			catch (AIMLDatei.AIMLDateiNichtGeladenException ex3)
			{
				try
				{
					File.Delete(text);
				}
				catch (Exception)
				{
				}
				throw new AIMLDatei.AIMLDateiNichtGeladenException(ex3.Message);
			}
		}

		public StartupDatei VorhandeneExterneStartupDateiImportieren(string externerDateiname, Arbeitsbereich zielArbeitsbereich)
		{
			if (zielArbeitsbereich.Dateiverwaltung != this)
			{
				throw new ApplicationException("Arbeitsbereich passt nicht zur ArbeitsbereichDateiverwaltung!");
			}
			string vorgabe = StartupDatei.TitelAusDateinameHerleiten(externerDateiname);
			bool flag = false;
			string text = this.ErmittleFreienNamenFuerAimlDatei(vorgabe, "startup", out flag);
			if (flag)
			{
				return null;
			}
			try
			{
				File.Copy(externerDateiname, text);
			}
			catch (Exception ex)
			{
				throw new AIMLDatei.AIMLDateiNichtGeladenException(ex.Message);
			}
			this.AimlDateiWirdGeladen(text);
			StartupDatei startupDatei = new StartupDatei(zielArbeitsbereich);
			startupDatei.OnChanged += this.AimlDateiOnChanged;
			try
			{
				startupDatei.LiesAusDatei(text, this._arbeitsVerzeichnis);
				this._dateien.Add(startupDatei);
				this.AimlDateiFertigGeladen();
				return startupDatei;
			}
			catch (AIMLDatei.AIMLDateiNichtGeladenException ex3)
			{
				try
				{
					File.Delete(text);
				}
				catch (Exception)
				{
				}
				throw new AIMLDatei.AIMLDateiNichtGeladenException(ex3.Message);
			}
		}

		public bool DateiLoeschen(IArbeitsbereichDatei datei, Arbeitsbereich arbeitsbereich)
		{
			if (arbeitsbereich.Dateiverwaltung != this)
			{
				throw new ApplicationException("Arbeitsbereich passt nicht zur ArbeitsbereichDateiverwaltung!");
			}
			bool flag = false;
			bool flag2 = !string.IsNullOrEmpty(datei.ZusatzContentUniqueId);
			if (this._dateien.Contains(datei))
			{
				if (flag2 || MessageBox.Show(string.Format(ResReader.Reader.GetString("AIMLDateiWirklichLoeschen"), datei.Dateiname), "", MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
				{
					try
					{
						if (!flag2)
						{
							File.Delete(datei.Dateiname);
						}
						this._dateien.Remove(datei);
						arbeitsbereich.Verlauf.AlleVerweiseDieserDateiEntfernen(datei);
						flag = true;
					}
					catch (Exception)
					{
						flag = false;
					}
				}
				else
				{
					flag = false;
				}
			}
			else
			{
				flag = false;
			}
			this.AimlDateiRemoved(datei);
			return flag;
		}

		public IArbeitsbereichDatei LadeEinzelneAimlDateiDirektOhneKopieEin(string dateiname, Arbeitsbereich arbeitsbereich)
		{
			if (arbeitsbereich.Dateiverwaltung != this)
			{
				throw new ApplicationException("Arbeitsbereich passt nicht zur ArbeitsbereichDateiverwaltung!");
			}
			try
			{
				return this.AddAimlVorhandeneDatei(dateiname, arbeitsbereich);
			}
			catch (AIMLDatei.AIMLDateiNichtGeladenException ex)
			{
				Debugger.GlobalDebugger.FehlerZeigen(string.Format(ResReader.Reader.GetString("AIMLDateiLadeFehler"), dateiname, ex.Message), this, "LadeDatei");
				return null;
			}
		}

		public IArbeitsbereichDatei LadeEinzelneGaitoBotConfigDateiDirektOhneKopieEin(string dateiname, Arbeitsbereich arbeitsbereich)
		{
			if (arbeitsbereich.Dateiverwaltung != this)
			{
				throw new ApplicationException("Arbeitsbereich passt nicht zur ArbeitsbereichDateiverwaltung!");
			}
			try
			{
				return this.AddGaitoBotConfig_VorhandeneDatei(dateiname, arbeitsbereich);
			}
			catch (AIMLDatei.AIMLDateiNichtGeladenException ex)
			{
				Debugger.GlobalDebugger.FehlerZeigen(string.Format(ResReader.Reader.GetString("AIMLDateiLadeFehler"), dateiname, ex.Message), this, "LadeDatei");
				return null;
			}
		}

		public StartupDatei AddLeereStartupDatei(Arbeitsbereich arbeitsbereich)
		{
			if (arbeitsbereich.Dateiverwaltung != this)
			{
				throw new ApplicationException("Arbeitsbereich passt nicht zur ArbeitsbereichDateiverwaltung!");
			}
			bool flag = default(bool);
			string dateiname = this.ErmittleFreienNamenFuerAimlDatei((string)null, "startup", out flag);
			if (flag)
			{
				return null;
			}
			StartupDatei startupDatei = new StartupDatei(arbeitsbereich);
			startupDatei.OnChanged += this.AimlDateiOnChanged;
			startupDatei.LeerFuellen();
			startupDatei.Dateiname = dateiname;
			bool flag2 = default(bool);
			startupDatei.Save(out flag2);
			this._dateien.Add(startupDatei);
			this.DateiAdded(startupDatei);
			return startupDatei;
		}

		public AIMLDatei AddAimlLeereDatei(Arbeitsbereich arbeitsbereich, bool ersteDateiMitBeispielen)
		{
			if (arbeitsbereich.Dateiverwaltung != this)
			{
				throw new ApplicationException("Arbeitsbereich passt nicht zur ArbeitsbereichDateiverwaltung!");
			}
			string vorgabe = null;
			if (ersteDateiMitBeispielen)
			{
				vorgabe = ResReader.Reader.GetString("MeineErsteAimlDatei");
			}
			bool flag = default(bool);
			string dateiname = this.ErmittleFreienNamenFuerAimlDatei(vorgabe, "aiml", out flag);
			if (flag)
			{
				return null;
			}
			AIMLDatei aIMLDatei = new AIMLDatei(arbeitsbereich);
			aIMLDatei.OnChanged += this.AimlDateiOnChanged;
			if (ersteDateiMitBeispielen)
			{
				aIMLDatei.MitTargetBotStartFuellen();
			}
			else
			{
				aIMLDatei.LeerFuellen();
			}
			aIMLDatei.Dateiname = dateiname;
			bool flag2 = default(bool);
			aIMLDatei.Save(out flag2);
			this._dateien.Add(aIMLDatei);
			this.DateiAdded(aIMLDatei);
			return aIMLDatei;
		}

		public AIMLDatei AddAimlVorhandeneDatei(string dateiname, Arbeitsbereich arbeitsbereich)
		{
			if (arbeitsbereich.Dateiverwaltung != this)
			{
				throw new ApplicationException("Arbeitsbereich passt nicht zur ArbeitsbereichDateiverwaltung!");
			}
			this.AimlDateiWirdGeladen(dateiname);
			AIMLDatei aIMLDatei = new AIMLDatei(arbeitsbereich);
			aIMLDatei.OnChanged += this.AimlDateiOnChanged;
			try
			{
				aIMLDatei.LiesAusDatei(dateiname, this._arbeitsVerzeichnis);
				this._dateien.Add(aIMLDatei);
				this.AimlDateiFertigGeladen();
				this.DateiAdded(aIMLDatei);
				return aIMLDatei;
			}
			catch (AIMLDatei.AIMLDateiNichtGeladenException ex)
			{
				throw new AIMLDatei.AIMLDateiNichtGeladenException(ex.Message);
			}
		}

		private AIMLDatei AddAimlHinzugelinkteContentDatei(ContentDokument dokument, Arbeitsbereich arbeitsbereich)
		{
			if (arbeitsbereich.Dateiverwaltung != this)
			{
				throw new ApplicationException("Arbeitsbereich passt nicht zur ArbeitsbereichDateiverwaltung!");
			}
			this.AimlDateiWirdGeladen(dokument.Titel);
			AIMLDatei aIMLDatei = new AIMLDatei(arbeitsbereich);
			aIMLDatei.OnChanged += this.AimlDateiOnChanged;
			try
			{
				aIMLDatei.LiesAusString(dokument.Inhalt);
				aIMLDatei.ZusatzContentUniqueId = dokument.ZusatzContentUniqueId;
				aIMLDatei.Titel = dokument.Titel;
				aIMLDatei.NurFuerGaitoBotExportieren = true;
				aIMLDatei.ReadOnly = true;
				aIMLDatei.Unterverzeichnisse = dokument.Unterverzeichnise;
				this._dateien.Add(aIMLDatei);
				this.AimlDateiFertigGeladen();
				this.DateiAdded(aIMLDatei);
				return aIMLDatei;
			}
			catch (AIMLDatei.AIMLDateiNichtGeladenException ex)
			{
				throw new AIMLDatei.AIMLDateiNichtGeladenException(ex.Message);
			}
		}

		private StartupDatei AddStartUpdateiHinzugelinkteContent(ContentDokument dokument, Arbeitsbereich arbeitsbereich)
		{
			if (arbeitsbereich.Dateiverwaltung != this)
			{
				throw new ApplicationException("Arbeitsbereich passt nicht zur ArbeitsbereichDateiverwaltung!");
			}
			this.AimlDateiWirdGeladen(dokument.Titel);
			StartupDatei startupDatei = new StartupDatei(arbeitsbereich);
			startupDatei.OnChanged += this.AimlDateiOnChanged;
			try
			{
				startupDatei.LiesAusString(dokument.Inhalt);
				startupDatei.Titel = dokument.Titel;
				startupDatei.ZusatzContentUniqueId = dokument.ZusatzContentUniqueId;
				startupDatei.NurFuerGaitoBotExportieren = true;
				startupDatei.ReadOnly = true;
				startupDatei.Unterverzeichnisse = dokument.Unterverzeichnise;
				this._dateien.Add(startupDatei);
				this.AimlDateiFertigGeladen();
				this.DateiAdded(startupDatei);
				return startupDatei;
			}
			catch (AIMLDatei.AIMLDateiNichtGeladenException ex)
			{
				throw new AIMLDatei.AIMLDateiNichtGeladenException(ex.Message);
			}
		}

		private StartupDatei AddGaitoBotConfig_VorhandeneDatei(string dateiname, Arbeitsbereich arbeitsbereich)
		{
			if (arbeitsbereich.Dateiverwaltung != this)
			{
				throw new ApplicationException("Arbeitsbereich passt nicht zur ArbeitsbereichDateiverwaltung!");
			}
			this.AimlDateiWirdGeladen(dateiname);
			StartupDatei startupDatei = new StartupDatei(arbeitsbereich);
			startupDatei.OnChanged += this.AimlDateiOnChanged;
			try
			{
				startupDatei.LiesAusDatei(dateiname, this._arbeitsVerzeichnis);
				this._dateien.Add(startupDatei);
				this.AimlDateiFertigGeladen();
				this.DateiAdded(startupDatei);
				return startupDatei;
			}
			catch (AIMLDatei.AIMLDateiNichtGeladenException ex)
			{
				throw new AIMLDatei.AIMLDateiNichtGeladenException(ex.Message);
			}
		}

		private void AimlDateiOnChanged(object sender, EventArgs e)
		{
			this.OnAimlDateienContentChangedEvent();
		}

		private string ErmittleFreienNamenFuerAimlDatei(string vorgabe, string extension, out bool abgebrochen)
		{
			abgebrochen = false;
			bool flag = false;
			string text = "";
			if (string.IsNullOrEmpty(vorgabe))
			{
				vorgabe = ResReader.Reader.GetString("unbenannt");
			}
			do
			{
				string text2 = InputBox.Show(ResReader.Reader.GetString("NamenFuerNeueAIMLDateiAngeben"), ResReader.Reader.GetString("NeueAIMLDateiErzeugen"), vorgabe, out abgebrochen);
				if (string.IsNullOrEmpty(text2))
				{
					abgebrochen = true;
				}
				if (!abgebrochen)
				{
					flag = false;
					text2 = ToolboxStrings.ReplaceEx(text2, string.Format(".{0}", "aiml"), "");
					text2 = ToolboxStrings.ReplaceEx(text2, string.Format(".{0}", "startup"), "");
					text2 = ToolboxStrings.ReplaceEx(text2, ".xml", "");
					text2 = ToolboxStrings.ReplaceEx(text2, ".", "_");
					text2 = ToolboxStrings.ReplaceEx(text2, "\\", "_");
					text2 = ToolboxStrings.ReplaceEx(text2, "/", "_");
					text2 = text2.Trim();
					if (text2 == "")
					{
						flag = true;
					}
					else
					{
						text = string.Format("{0}\\{1}.{2}", this._arbeitsVerzeichnis, text2, extension);
						if (File.Exists(text))
						{
							MessageBox.Show(ResReader.Reader.GetString("AIMLDateiSchonVorhanden"));
							flag = true;
						}
						else
						{
							try
							{
								ToolboxStrings.String2File("test", text);
								File.Delete(text);
							}
							catch (Exception ex)
							{
								MessageBox.Show(string.Format(ResReader.Reader.GetString("AIMLDateiNameUngueltigOderKeinZugriff"), ex.Message));
								flag = true;
							}
						}
					}
				}
			}
			while (!abgebrochen & flag);
			if (abgebrochen)
			{
				text = null;
			}
			return text;
		}
	}
}
