using de.springwald.toolbox;
using MultiLang;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace GaitoBotEditorCore
{
	public class Arbeitsbereich
	{
		public enum VerzeichnisVorherLeeren
		{
			nichtLeeren,
			leerenUndVorherFragen,
			leerenUndNichtVorherFragen
		}

		public enum WoSuchenOrte
		{
			ImArbeitsbereich,
			InAktuellerAIMLDatei,
			InAktuellemTopic
		}

		public class SucheImArbeitsbereichEventArgs : EventArgs
		{
			public string SuchEingabe;

			public WoSuchenOrte WoSuchen;

			public SucheImArbeitsbereichEventArgs(string sucheingabe, WoSuchenOrte woSuchen)
			{
				this.SuchEingabe = sucheingabe;
				this.WoSuchen = woSuchen;
			}
		}

		public delegate void SucheImArbeitsbereichEventHandler(object sender, SucheImArbeitsbereichEventArgs e);

		private bool _geoeffnet;

		private ArbeitsbereichFokus _fokus;

		private ArbeitsbereichVerlauf _verlauf;

		private readonly string _arbeitsVerzeichnis;

		private ArbeitsbereichMetaInfos _arbeitsbereichMetaInfos;

		public string GaitoBotID
		{
			get
			{
				return this._arbeitsbereichMetaInfos.GaitoBotID;
			}
			set
			{
				if (this._arbeitsbereichMetaInfos.GaitoBotID != value)
				{
					this._arbeitsbereichMetaInfos.GaitoBotID = value;
				}
			}
		}

		public string Exportverzeichnis
		{
			get
			{
				return this._arbeitsbereichMetaInfos.Exportverzeichnis;
			}
			set
			{
				this._arbeitsbereichMetaInfos.Exportverzeichnis = value;
			}
		}

		public bool ExportverzeichnisVorExportLeeren
		{
			get
			{
				return this._arbeitsbereichMetaInfos.ExportverzeichnisVorExportLeeren;
			}
			set
			{
				this._arbeitsbereichMetaInfos.ExportverzeichnisVorExportLeeren = value;
			}
		}

		public bool AlleStarTagsInExtraDateiExportieren
		{
			get
			{
				return this._arbeitsbereichMetaInfos.AlleStarTagsInExtraDateiExportieren;
			}
			set
			{
				this._arbeitsbereichMetaInfos.AlleStarTagsInExtraDateiExportieren = value;
			}
		}

		public string[] NichtExporierenDateinamen
		{
			get
			{
				return this._arbeitsbereichMetaInfos.NichtExportierenDateinamen;
			}
			set
			{
				this._arbeitsbereichMetaInfos.NichtExportierenDateinamen = value;
			}
		}

		public bool SindAlleZuExportierendenDateienFehlerFrei
		{
			get
			{
				foreach (AIMLDatei item in this.Dateiverwaltung.AlleAimlDateien)
				{
					if (!item.SeitLetztemIsChangedAufDTDFehlergeprueft)
					{
						item.GegenAIMLDTDPruefen(AIMLDatei.PruefFehlerVerhalten.NiemalsFehlerZeigen);
					}
				}
				if ((from IArbeitsbereichDatei d in this.GetZuExportierendeDateien(true)
				where d.HatFehler
				select d).Count() > 0)
				{
					return false;
				}
				return true;
			}
		}

		private string ArbeitsBereichMetaInfosDateiname
		{
			get
			{
				return Path.Combine(this._arbeitsVerzeichnis, "metainfos.ser");
			}
		}

		public string[] ContentElementUniqueIds
		{
			get
			{
				return this._arbeitsbereichMetaInfos.ContentElementUniqueIds;
			}
			set
			{
				this._arbeitsbereichMetaInfos.ContentElementUniqueIds = value;
				this.MetaInfosChanged();
			}
		}

		public ArbeitsbereichDateiverwaltung Dateiverwaltung
		{
			get;
			private set;
		}

		public bool MetaInfosIsChanged
		{
			get;
			set;
		}

		public int AnzahlKategorien
		{
			get
			{
				int num = 0;
				foreach (AIMLDatei item in this.Dateiverwaltung.AlleAimlDateien)
				{
					num += item.AnzahlCategories;
				}
				return num;
			}
		}

		public ArbeitsbereichFokus Fokus
		{
			get
			{
				return this._fokus;
			}
		}

		public ArbeitsbereichVerlauf Verlauf
		{
			get
			{
				return this._verlauf;
			}
		}

		public string Name
		{
			get
			{
				return this._arbeitsbereichMetaInfos.Name;
			}
			set
			{
				this._arbeitsbereichMetaInfos.Name = value;
				this.NameChanged();
			}
		}

		public string Arbeitsverzeichnis
		{
			get
			{
				return this._arbeitsVerzeichnis;
			}
		}

		public event SucheImArbeitsbereichEventHandler SucheImArbeitsbereich;

		public event EventHandler MetaInfosChangedEvent;

		public event EventHandler NameChangedEvent;

		public event EventHandler UseOneWordSRAIChanged;

		public event EventHandler WissenSpracheChanged;

		public List<IArbeitsbereichDatei> GetZuExportierendeDateien(bool fuerGaitoBotDePublizierungExportieren)
		{
			List<IArbeitsbereichDatei> list = new List<IArbeitsbereichDatei>();
			foreach (IArbeitsbereichDatei item in this.Dateiverwaltung.Dateien)
			{
				bool flag = false;
				flag = (fuerGaitoBotDePublizierungExportieren || !item.NurFuerGaitoBotExportieren);
				if (flag && !this.DieseAIMLDateiNichtExportieren(item))
				{
					list.Add(item);
				}
			}
			return list;
		}

		public bool DieseAIMLDateiNichtExportieren(IArbeitsbereichDatei datei)
		{
			if (datei is AIMLDatei)
			{
				string[] nichtExporierenDateinamen = this.NichtExporierenDateinamen;
				foreach (string arg in nichtExporierenDateinamen)
				{
					string value = string.Format("\\{0}.aiml", arg);
					if (!string.IsNullOrEmpty(datei.Dateiname) && datei.Dateiname.EndsWith(value, StringComparison.OrdinalIgnoreCase))
					{
						return true;
					}
				}
			}
			return false;
		}

		public void Exportieren(string exportVerzeichnis, VerzeichnisVorherLeeren verzeichnisVorherLeeren, bool alleStarTagsInExtraDateiExportieren, bool exportFuerGaitoBotDePublizierung, out bool abgebrochen)
		{
			if (!Directory.Exists(exportVerzeichnis))
			{
				throw new ApplicationException(string.Format("Exportverzeichnis '{0}' nicht vorhanden!", exportVerzeichnis));
			}
			switch (verzeichnisVorherLeeren)
			{
			case VerzeichnisVorherLeeren.leerenUndVorherFragen:
			{
				DialogResult dialogResult = MessageBox.Show(string.Format(ResReader.Reader.GetString("ExportVerzeichnisWirklichLeeren"), exportVerzeichnis), ResReader.Reader.GetString("Export"), MessageBoxButtons.YesNo);
				if (dialogResult != DialogResult.Yes)
				{
					abgebrochen = true;
					return;
				}
				goto default;
			}
			default:
			{
				string[] directories = Directory.GetDirectories(exportVerzeichnis);
				string[] array = directories;
				foreach (string text in array)
				{
					try
					{
						Directory.Delete(text, true);
					}
					catch (Exception ex)
					{
						MessageBox.Show(string.Format(global::MultiLang.ml.ml_string(70, "Das Verzeichnis '{0}' konnte nicht gelöscht werden: {1}"), text, ex.Message));
						abgebrochen = true;
						return;
					}
				}
				string[] files = Directory.GetFiles(exportVerzeichnis);
				string[] array2 = files;
				foreach (string text2 in array2)
				{
					try
					{
						File.Delete(text2);
					}
					catch (Exception ex2)
					{
						MessageBox.Show(string.Format(global::MultiLang.ml.ml_string(71, "Die Datei '{0}' konnte nicht gelöscht werden: {1}"), text2, ex2.Message));
						abgebrochen = true;
						return;
					}
				}
				break;
			}
			case VerzeichnisVorherLeeren.nichtLeeren:
				break;
			}
			if (alleStarTagsInExtraDateiExportieren)
			{
				StringCollection stringCollection = new StringCollection();
				foreach (IArbeitsbereichDatei item in this.GetZuExportierendeDateien(exportFuerGaitoBotDePublizierung))
				{
					if (item is AIMLDatei)
					{
						AIMLDatei aIMLDatei = (AIMLDatei)item;
						AIMLDatei aIMLDatei2 = new AIMLDatei(this);
						aIMLDatei2.LiesAusString(aIMLDatei.XML.OuterXml);
						List<AIMLCategory> list = new List<AIMLCategory>();
						foreach (AIMLCategory category in aIMLDatei2.RootTopic.Categories)
						{
							if (category.That == "" && category.Pattern == "*")
							{
								stringCollection.Add(category.Template);
								list.Add(category);
							}
						}
						foreach (AIMLCategory item2 in list)
						{
							aIMLDatei2.RootTopic.LoescheCategory(item2);
						}
						StringBuilder stringBuilder = new StringBuilder();
						string[] unterverzeichnisse = aIMLDatei.Unterverzeichnisse;
						foreach (string arg in unterverzeichnisse)
						{
							stringBuilder.AppendFormat("{0}_", arg);
						}
						string text4 = aIMLDatei2.Dateiname = string.Format("{0}\\{1}{2}.aiml", exportVerzeichnis, stringBuilder.ToString(), aIMLDatei.Titel);
						bool flag = false;
						aIMLDatei2.Save(out flag);
					}
				}
				if (stringCollection.Count != 0)
				{
					StringBuilder stringBuilder2 = new StringBuilder();
					stringBuilder2.Append("<pattern>*</pattern><template><random>");
					StringEnumerator enumerator4 = stringCollection.GetEnumerator();
					try
					{
						while (enumerator4.MoveNext())
						{
							string current4 = enumerator4.Current;
							stringBuilder2.AppendFormat("<li>{0}</li>", current4);
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
					stringBuilder2.Append("</random></template>");
					AIMLDatei aIMLDatei3 = new AIMLDatei(this);
					aIMLDatei3.LeerFuellen();
					aIMLDatei3.Dateiname = string.Format("{0}\\_stars_.aiml", exportVerzeichnis);
					AIMLCategory aIMLCategory = aIMLDatei3.RootTopic.AddNewCategory();
					aIMLCategory.ContentNode.InnerXml = stringBuilder2.ToString();
					bool flag2 = false;
					if (File.Exists(aIMLDatei3.Dateiname))
					{
						File.Delete(aIMLDatei3.Dateiname);
					}
					aIMLDatei3.Save(out flag2);
				}
				foreach (IArbeitsbereichDatei item3 in this.GetZuExportierendeDateien(exportFuerGaitoBotDePublizierung))
				{
					if (item3 is StartupDatei)
					{
						StartupDatei startupDatei = (StartupDatei)item3;
						StringBuilder stringBuilder3 = new StringBuilder();
						string[] unterverzeichnisse2 = item3.Unterverzeichnisse;
						foreach (string baustein in unterverzeichnisse2)
						{
							stringBuilder3.AppendFormat("{0}_", this.DateinameBausteinBereinigt(baustein));
						}
						string text5 = "startup";
						string text6 = string.Format("{0}\\{1}{2}.{3}", exportVerzeichnis, stringBuilder3.ToString(), this.DateinameBausteinBereinigt(startupDatei.Titel), text5);
						string dateiname = startupDatei.Dateiname;
						if (string.IsNullOrEmpty(dateiname))
						{
							ToolboxStrings.String2File(startupDatei.XML.OuterXml, text6);
						}
						else
						{
							File.Copy(dateiname, text6, true);
						}
					}
				}
			}
			else
			{
				foreach (IArbeitsbereichDatei item4 in this.GetZuExportierendeDateien(exportFuerGaitoBotDePublizierung))
				{
					StringBuilder stringBuilder4 = new StringBuilder();
					string[] unterverzeichnisse3 = item4.Unterverzeichnisse;
					foreach (string baustein2 in unterverzeichnisse3)
					{
						stringBuilder4.AppendFormat("{0}_", this.DateinameBausteinBereinigt(baustein2));
					}
					string text7 = "";
					if (item4 is AIMLDatei)
					{
						text7 = "aiml";
						goto IL_05c3;
					}
					if (item4 is StartupDatei)
					{
						text7 = "startup";
						goto IL_05c3;
					}
					throw new ApplicationException(string.Format("Datei '{0}' hat einen unbekannten Typ", item4.Titel));
					IL_05c3:
					string text8 = string.Format("{0}\\{1}{2}.{3}", exportVerzeichnis, stringBuilder4.ToString(), this.DateinameBausteinBereinigt(item4.Titel), text7);
					string dateiname2 = item4.Dateiname;
					if (string.IsNullOrEmpty(dateiname2))
					{
						ToolboxStrings.String2File(item4.XML.OuterXml, text8);
					}
					else
					{
						File.Copy(dateiname2, text8, true);
					}
				}
			}
			abgebrochen = false;
		}

		private string DateinameBausteinBereinigt(string baustein)
		{
			if (string.IsNullOrEmpty(baustein))
			{
				return baustein;
			}
			baustein = baustein.Replace("|", "_");
			baustein = baustein.Replace(":", "_");
			baustein = baustein.Replace("\\", "_");
			baustein = baustein.Replace("/", "_");
			baustein = baustein.Replace(" ", "_");
			baustein = baustein.Replace("@", "_");
			baustein = baustein.Replace("$", "_");
			baustein = baustein.Replace("§", "_");
			return baustein;
		}

		protected virtual void SucheImArbeitsbereichEvent(string sucheingabe, WoSuchenOrte woSuchen)
		{
			if (this.SucheImArbeitsbereich != null)
			{
				this.SucheImArbeitsbereich(this, new SucheImArbeitsbereichEventArgs(sucheingabe, woSuchen));
			}
		}

		public AIMLCategory GetCategoryForCategoryNode(XmlNode categoryNode)
		{
			foreach (AIMLDatei item in this.Dateiverwaltung.AlleAimlDateien)
			{
				foreach (AIMLTopic topic in item.getTopics())
				{
					foreach (AIMLCategory category in topic.Categories)
					{
						if (category.ContentNode == categoryNode)
						{
							return category;
						}
					}
				}
			}
			return null;
		}

		public void Suchen(string suchEingabe, WoSuchenOrte woSuchen)
		{
			suchEingabe = ToolboxStrings.UmlauteAussschreiben(suchEingabe);
			this.SucheImArbeitsbereichEvent(suchEingabe, woSuchen);
		}

		public static string WoSuchenOrt2Name(WoSuchenOrte ort)
		{
			switch (ort)
			{
			case WoSuchenOrte.ImArbeitsbereich:
				return ResReader.Reader.GetString("SuchenOrtNameImArbeitsbereich");
			case WoSuchenOrte.InAktuellemTopic:
				return ResReader.Reader.GetString("SuchenOrtNameInAktuellemTopic");
			case WoSuchenOrte.InAktuellerAIMLDatei:
				return ResReader.Reader.GetString("SuchenOrtNameInAktuellerAIMLDatei");
			default:
				throw new ApplicationException("Unbehandelter WoSuchenOrt '" + ort + "'!");
			}
		}

		public static void WoSuchenAuswahlFuellen(ToolStripComboBox combo, WoSuchenOrte welchenSelektieren)
		{
			Arbeitsbereich.WoSuchenAuswahlFuellenEiner(combo, WoSuchenOrte.ImArbeitsbereich, welchenSelektieren);
			Arbeitsbereich.WoSuchenAuswahlFuellenEiner(combo, WoSuchenOrte.InAktuellerAIMLDatei, welchenSelektieren);
			Arbeitsbereich.WoSuchenAuswahlFuellenEiner(combo, WoSuchenOrte.InAktuellemTopic, welchenSelektieren);
		}

		private static void WoSuchenAuswahlFuellenEiner(ToolStripComboBox combo, WoSuchenOrte ort, WoSuchenOrte welchenSelektieren)
		{
			int selectedIndex = combo.Items.Add(Arbeitsbereich.WoSuchenOrt2Name(ort));
			if (ort == welchenSelektieren)
			{
				combo.SelectedIndex = selectedIndex;
			}
		}

		protected virtual void MetaInfosChanged()
		{
			if (this.MetaInfosChangedEvent != null)
			{
				this.MetaInfosChangedEvent(this, EventArgs.Empty);
			}
		}

		protected virtual void NameChanged()
		{
			if (this.NameChangedEvent != null)
			{
				this.NameChangedEvent(this, EventArgs.Empty);
			}
		}

		protected virtual void UseOneWordSRAIChangedEvent()
		{
			if (this.UseOneWordSRAIChanged != null)
			{
				this.UseOneWordSRAIChanged(this, EventArgs.Empty);
			}
		}

		protected virtual void WissenSpracheChangedEvent()
		{
			if (this.WissenSpracheChanged != null)
			{
				this.WissenSpracheChanged(this, EventArgs.Empty);
			}
		}

		public Arbeitsbereich(string arbeitsverzeichnis)
		{
			this._arbeitsVerzeichnis = arbeitsverzeichnis;
			this.InitArbeitsbereich();
		}

		public Arbeitsbereich()
		{
			int num = 0;
			do
			{
				num++;
				this._arbeitsVerzeichnis = Path.Combine(Config.GlobalConfig.ArbeitsbereicheSpeicherVerzeichnis, num.ToString());
			}
			while (Directory.Exists(this._arbeitsVerzeichnis));
			Directory.CreateDirectory(this._arbeitsVerzeichnis);
			this.InitArbeitsbereich();
		}

		public void Loeschen()
		{
			Directory.Delete(this.Arbeitsverzeichnis, true);
		}

		public void SaveAll()
		{
			foreach (IArbeitsbereichDatei item in this.Dateiverwaltung.Dateien)
			{
				if (item.IsChanged)
				{
					bool flag = default(bool);
					item.Save(out flag);
					if (flag)
					{
						return;
					}
				}
			}
			this.MetaInfosSpeichern();
		}

		public void ArbeitsbereichSollGeschlossenWerden(out bool cancel)
		{
			cancel = false;
			foreach (IArbeitsbereichDatei item in this.Dateiverwaltung.Dateien)
			{
				if (item.IsChanged)
				{
					DialogResult dialogResult = MessageBox.Show(ResReader.Reader.GetString("SollenDieAenderungenGespeichertWerden"), string.Format(ResReader.Reader.GetString("DateiInhaltHatSichSeitSpeichernGeaendert"), item.Titel), MessageBoxButtons.YesNoCancel);
					switch (dialogResult)
					{
					case DialogResult.Yes:
					{
						bool flag = default(bool);
						item.Save(out flag);
						cancel = flag;
						break;
					}
					case DialogResult.Cancel:
						cancel = true;
						return;
					default:
						throw new ApplicationException("Unbekanntes Dialogergebnis '" + dialogResult + "'");
					case DialogResult.No:
						break;
					}
				}
			}
			if (this.MetaInfosIsChanged)
			{
				DialogResult dialogResult2 = MessageBox.Show(ResReader.Reader.GetString("SollenDieAenderungenGespeichertWerden"), string.Format(ResReader.Reader.GetString("DerInhaltDesArbeitsbereichesWurdeVeraendert"), this.Name), MessageBoxButtons.YesNoCancel);
				switch (dialogResult2)
				{
				case DialogResult.No:
					break;
				case DialogResult.Yes:
					this.MetaInfosSpeichern();
					break;
				case DialogResult.Cancel:
					cancel = true;
					break;
				default:
					throw new ApplicationException("Unbekanntes Dialogergebnis '" + dialogResult2 + "'");
				}
			}
		}

		public override string ToString()
		{
			return this.Name;
		}

		public void MetaInfosSpeichern()
		{
			ArbeitsbereichMetaInfos.SerialisiereInXMLDatei(this.ArbeitsBereichMetaInfosDateiname, this._arbeitsbereichMetaInfos);
			this.MetaInfosIsChanged = false;
		}

		public void Oeffnen()
		{
			if (this._geoeffnet)
			{
				throw new ApplicationException(global::MultiLang.ml.ml_string(65, "Dieser Arbeitsbereich wurde bereits geöffnet!"));
			}
			this._geoeffnet = true;
			this.LadeVerzeichnis(this._arbeitsVerzeichnis, true);
		}

		private void InitArbeitsbereich()
		{
			this.Dateiverwaltung = new ArbeitsbereichDateiverwaltung(this._arbeitsVerzeichnis);
			if (File.Exists(this.ArbeitsBereichMetaInfosDateiname))
			{
				this._arbeitsbereichMetaInfos = ArbeitsbereichMetaInfos.DeSerialisiereAusXMLDatei(this.ArbeitsBereichMetaInfosDateiname);
			}
			else
			{
				this._arbeitsbereichMetaInfos = new ArbeitsbereichMetaInfos();
			}
			this._arbeitsbereichMetaInfos.Changed += this.MetaInfos_Changed;
			this._fokus = new ArbeitsbereichFokus();
			this._verlauf = new ArbeitsbereichVerlauf(this._fokus);
		}

		private void LadeVerzeichnis(string verzeichnis, bool mitUnterverzeichnissen)
		{
			string[] files = Directory.GetFiles(verzeichnis, "*.xml");
			string[] array = files;
			foreach (string dateiname in array)
			{
				this.Dateiverwaltung.LadeEinzelneAimlDateiDirektOhneKopieEin(dateiname, this);
			}
			files = Directory.GetFiles(verzeichnis, "*.aiml");
			string[] array2 = files;
			foreach (string dateiname2 in array2)
			{
				this.Dateiverwaltung.LadeEinzelneAimlDateiDirektOhneKopieEin(dateiname2, this);
			}
			files = Directory.GetFiles(verzeichnis, "*.startup");
			string[] array3 = files;
			foreach (string dateiname3 in array3)
			{
				this.Dateiverwaltung.LadeEinzelneGaitoBotConfigDateiDirektOhneKopieEin(dateiname3, this);
			}
			if (mitUnterverzeichnissen)
			{
				string[] directories = Directory.GetDirectories(verzeichnis);
				string[] array4 = directories;
				foreach (string verzeichnis2 in array4)
				{
					this.LadeVerzeichnis(verzeichnis2, true);
				}
			}
		}

		private void MetaInfos_Changed(object sender, EventArgs e)
		{
			this.MetaInfosIsChanged = true;
			this.MetaInfosChanged();
		}
	}
}
