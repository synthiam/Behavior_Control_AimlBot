using de.springwald.gaitobot.publizierung;
using de.springwald.toolbox;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GaitoBotEditorCore.Publizieren
{
	public class ArbeitsbereichPublizieren : IDisposable
	{
		public enum ergebnisse
		{
			undefiniert,
			erfolgreich,
			fehlerhaft
		}

		private string _arbeitsVerzeichnis;

		private Arbeitsbereich _arbeitsbereich;

		private StringBuilder _ergebnisProtokoll;

		private long GroesseExportInByte
		{
			get
			{
				long num = 0L;
				string[] files = Directory.GetFiles(this.ExportVerzeichnis);
				foreach (string fileName in files)
				{
					FileInfo fileInfo = new FileInfo(fileName);
					num += fileInfo.Length;
				}
				return num;
			}
		}

		public ergebnisse Ergebnis
		{
			get;
			set;
		}

		private string ExportVerzeichnis
		{
			get
			{
				if (this._arbeitsVerzeichnis == null)
				{
					int num = 0;
					do
					{
						num++;
						this._arbeitsVerzeichnis = Path.Combine(Config.GlobalConfig.ArbeitsbereicheSpeicherVerzeichnis, string.Format("{0}_temp", num));
					}
					while (Directory.Exists(this._arbeitsVerzeichnis));
					Directory.CreateDirectory(this._arbeitsVerzeichnis);
				}
				return this._arbeitsVerzeichnis;
			}
		}

		public event EventHandler<PublizierenEventArgs> PublizerEvent;

		public ArbeitsbereichPublizieren(Arbeitsbereich arbeitsbereich)
		{
			this._arbeitsbereich = arbeitsbereich;
			this.Ergebnis = ergebnisse.undefiniert;
			this._ergebnisProtokoll = new StringBuilder();
		}

		public void Publizieren()
		{
			this.Ergebnis = ergebnisse.erfolgreich;
			this._ergebnisProtokoll = new StringBuilder();
			this.Protkolliere(ResReader.Reader.GetString("PublizierungGestartet"));
			if (!this._arbeitsbereich.SindAlleZuExportierendenDateienFehlerFrei)
			{
				this.ZeigeFehler(ResReader.Reader.GetString("PublizierenNichtAlleAIMLDateienKorrekt"), "Publizieren:Checking AIML-Files");
				this.Ergebnis = ergebnisse.fehlerhaft;
			}
			else
			{
				string text = this._arbeitsbereich.GaitoBotID;
				if (text != null)
				{
					text = text.Trim();
				}
				if (string.IsNullOrEmpty(text))
				{
					this.ZeigeFehler(ResReader.Reader.GetString("KeineGaitoBotIDEingegeben"), "Publizieren:Checking GaitoBotID");
					this.Ergebnis = ergebnisse.fehlerhaft;
				}
				else
				{
					bool flag = false;
					try
					{
						flag = PublizierDienst.ExistsGaitoBotID(this._arbeitsbereich.GaitoBotID);
					}
					catch (Exception ex)
					{
						this.ZeigeFehler(string.Format(ResReader.Reader.GetString("FehlerBeiGaitoBotWebServiceZugriff"), ex.Message), "Publizieren");
						this.Ergebnis = ergebnisse.fehlerhaft;
						goto IL_04ab;
					}
					if (flag)
					{
						this.Protkolliere(ResReader.Reader.GetString("GaitotbotIDExistiert"));
						this.Protkolliere(ResReader.Reader.GetString("AIMLDateienWerdenExportiert"));
						bool flag2 = false;
						this._arbeitsbereich.Exportieren(this.ExportVerzeichnis, Arbeitsbereich.VerzeichnisVorherLeeren.leerenUndNichtVorherFragen, true, true, out flag2);
						if (flag2)
						{
							this.Ergebnis = ergebnisse.fehlerhaft;
						}
						else
						{
							long num = (int)(this.GroesseExportInByte / 1024);
							this.Protkolliere(string.Format(ResReader.Reader.GetString("AIMLExportGroesseLautetKB"), num));
							long num2 = PublizierDienst.MaxKBWissen(this._arbeitsbereich.GaitoBotID);
							this.Protkolliere(string.Format(ResReader.Reader.GetString("AIMLExportGroesseErlaubtKB"), num2));
							if (num > num2)
							{
								this.ZeigeFehler(ResReader.Reader.GetString("AIMLExportZuGrossFuerPublizieren"), "Publizieren:Checking export size");
								this.Ergebnis = ergebnisse.fehlerhaft;
							}
							else
							{
								this.Protkolliere(ResReader.Reader.GetString("LoescheAlteAIMLDateienRemote"));
								List<DateiPublizierungsInfos> list = new List<DateiPublizierungsInfos>();
								string[] files = Directory.GetFiles(this.ExportVerzeichnis, "*.*");
								foreach (string dateinameMitPfad in files)
								{
									DateiPublizierungsInfos dateiPublizierungsInfos = new DateiPublizierungsInfos();
									dateiPublizierungsInfos.SetzeWerte(dateinameMitPfad);
									DateiPublizierungsInfos item = new DateiPublizierungsInfos
									{
										CRC32Checksumme = dateiPublizierungsInfos.CRC32Checksumme,
										Dateiname = dateiPublizierungsInfos.Dateiname,
										Groesse = dateiPublizierungsInfos.Groesse,
										GesamtCheckString = dateiPublizierungsInfos.GesamtCheckString
									};
									list.Add(item);
								}
								string[] toDoDateinamen = null;
								try
								{
									toDoDateinamen = PublizierDienst.AlteOderGeloeschteDateienLoeschenUndHochzuladendeZurueckGeben(this._arbeitsbereich.GaitoBotID, list.ToArray());
								}
								catch (Exception ex2)
								{
									this.ZeigeFehler(string.Format(ResReader.Reader.GetString("FehlerBeimRemoteLoeschen"), ex2.Message), "Publizieren:deleting old remote files");
									this.Ergebnis = ergebnisse.fehlerhaft;
									goto IL_04ab;
								}
								IEnumerable<DateiPublizierungsInfos> enumerable = from toDo in list
								where toDoDateinamen.Contains(toDo.Dateiname)
								select toDo;
								this.Protkolliere(string.Format(ResReader.Reader.GetString("AnzahlDateienZuPublizieren"), enumerable.Count()));
								foreach (DateiPublizierungsInfos item2 in enumerable)
								{
									try
									{
										this.Protkolliere(string.Format(ResReader.Reader.GetString("PubliziereDatei"), item2.Dateiname));
										AIMLDateiUebertragung aimlDateiInhalt = new AIMLDateiUebertragung
										{
											CheckString = item2.GesamtCheckString,
											Dateiname = item2.Dateiname,
											Inhalt = ToolboxStrings.File2String(Path.Combine(this.ExportVerzeichnis, item2.Dateiname))
										};
										PublizierDienst.UebertrageAIMLDatei(text, aimlDateiInhalt);
									}
									catch (Exception ex3)
									{
										this.ZeigeFehler(string.Format(ResReader.Reader.GetString("FehlerBeimDateiPublizieren"), item2.Dateiname, ex3.Message), "Publizieren:transfering file");
										this.Ergebnis = ergebnisse.fehlerhaft;
									}
								}
								try
								{
									this.Protkolliere("Resete GaitoBot auf Server");
									PublizierDienst.ReseteGaitoBot(text);
								}
								catch (Exception ex4)
								{
									this.ZeigeFehler(string.Format(ResReader.Reader.GetString("FehlerBeimDateiPublizieren"), "", ex4.Message), "Publizieren:reseting gaitobot on server");
									this.Ergebnis = ergebnisse.fehlerhaft;
								}
							}
						}
					}
					else
					{
						this.ZeigeFehler(ResReader.Reader.GetString("GaitotbotIDExistiertNicht"), "Publizieren:Checking GaitoBotID");
						this.Ergebnis = ergebnisse.fehlerhaft;
					}
				}
			}
			goto IL_04ab;
			IL_04ab:
			this.TempverzeichnisWiederLoeschen();
			switch (this.Ergebnis)
			{
			case ergebnisse.erfolgreich:
				this.Protkolliere(ResReader.Reader.GetString("PublizierungErfolgreich"));
				break;
			case ergebnisse.fehlerhaft:
				this.Protkolliere(ResReader.Reader.GetString("PublizierungFehlerhaft"));
				break;
			default:
				throw new ApplicationException("Unbehandeltes Publizierungsergebnis: " + this.Ergebnis);
			}
			this.Ergebnis = ergebnisse.erfolgreich;
		}

		private void TempverzeichnisWiederLoeschen()
		{
			try
			{
				Directory.Delete(this.ExportVerzeichnis, true);
			}
			finally
			{
				this._arbeitsVerzeichnis = null;
			}
		}

		private void Protkolliere(string inhalt)
		{
			if (this.PublizerEvent != null)
			{
				this.PublizerEvent(this, new PublizierenEventArgs
				{
					Meldung = string.Format("{0}", inhalt)
				});
			}
			this._ergebnisProtokoll.AppendFormat(string.Format("{0}\r\n", inhalt));
			Debugger.GlobalDebugger.Protokolliere(inhalt, Debugger.ProtokollTypen.TechnischerVorgang);
		}

		private void ZeigeFehler(string inhalt, string kontext)
		{
			if (this.PublizerEvent != null)
			{
				this.PublizerEvent(this, new PublizierenEventArgs
				{
					Meldung = string.Format("{0}:{1}", ResReader.Reader.GetString("Fehler"), inhalt)
				});
			}
			this._ergebnisProtokoll.AppendFormat("{0}:{1} ({2})\r\n", ResReader.Reader.GetString("Fehler"), inhalt, kontext);
			Debugger.GlobalDebugger.FehlerZeigen(inhalt, "ArbeitsbereichPublizieren", kontext);
		}

		public void Dispose()
		{
			if (this._arbeitsVerzeichnis != null)
			{
				try
				{
					Directory.Delete(this._arbeitsVerzeichnis, true);
				}
				finally
				{
				}
			}
		}
	}
}
