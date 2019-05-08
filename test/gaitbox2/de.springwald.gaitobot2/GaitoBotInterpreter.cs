using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Forms;

namespace de.springwald.gaitobot2
{
	public class GaitoBotInterpreter
	{
		public class AimlDateiWirdGeladenEventArgs : EventArgs
		{
			public string Dateiname;

			public AimlDateiWirdGeladenEventArgs(string dateiname)
			{
				this.Dateiname = dateiname;
			}
		}

		public delegate void AimlDateiWirdGeladenEventHandler(object sender, AimlDateiWirdGeladenEventArgs e);

		public const string SRAI_TARGET_BOTSTART = "TARGET BOTSTART";

		public const string SRAI_TARGET_EmptyInput = "TARGET EMPTYINPUT";

		public const string SRAI_TARGET_FirstBadAnswer = "TARGET FIRSTBADANSWER";

		public const string SRAI_TARGET_OnlyOneWord = "TARGET ONLYONEWORD";

		private bool _beiEinwortEingabenOhneMatchAufSraiOnlyOneWordUmleiten;

		private readonly Normalisierung _normalisierung;

		private readonly CultureInfo _loggingKultur;

		private readonly StringBuilder _ladeProtokoll;

		private GaitoBotEigenschaften _botEigenschaften;

		private readonly Wissen _wissen;

		public GaitoBotEigenschaften BotEigenschaften
		{
			get
			{
				if (this._botEigenschaften == null)
				{
					throw new ApplicationException("EsWurdenNochKeineBotEigenschaftenZugewiesen");
				}
				return this._botEigenschaften;
			}
			set
			{
				this._botEigenschaften = value;
			}
		}

		private CultureInfo LoggingKultur
		{
			get
			{
				return this._loggingKultur;
			}
		}

		public string LadeProtokoll
		{
			get
			{
				return this._ladeProtokoll.ToString();
			}
		}

		public int AnzahlCategories
		{
			get
			{
				return this._wissen.AnzahlCategories;
			}
		}

		public event AimlDateiWirdGeladenEventHandler AIMLDateiWirdGeladen;

		protected virtual void AIMLDateiWirdGeladenEvent(string dateiname)
		{
			if (this.AIMLDateiWirdGeladen != null)
			{
				this.AIMLDateiWirdGeladen(this, new AimlDateiWirdGeladenEventArgs(dateiname));
			}
		}

		public GaitoBotInterpreter(CultureInfo loggingKultur, StartupInfos startupInfos)
		{
			this._wissen = new Wissen();
			this._botEigenschaften = startupInfos.BotEigenschaften;
			this._loggingKultur = loggingKultur;
			this._normalisierung = new Normalisierung(startupInfos);
			this._ladeProtokoll = new StringBuilder();
		}

		public void LadenAusVerzeichnis(string verzeichnis)
		{
			this.PreInit();
			WissensLader wissensLader = new WissensLader(this._wissen, this._loggingKultur, this._normalisierung);
			wissensLader.AimlDateiWirdGeladen += this.loader_AIMLDateiWirdGeladen;
			wissensLader.LadeAimlDateienAusVerzeichnis(verzeichnis, this._botEigenschaften);
			wissensLader.AimlDateiWirdGeladen -= this.loader_AIMLDateiWirdGeladen;
			this._ladeProtokoll.Append(wissensLader.Protokoll);
			this.PostInit();
		}

		public void LadenAusDateinamen(string[] aimlDateinamen)
		{
			this.PreInit();
			WissensLader wissensLader = new WissensLader(this._wissen, this._loggingKultur, this._normalisierung);
			wissensLader.AimlDateiWirdGeladen += this.loader_AIMLDateiWirdGeladen;
			foreach (string aimlDateiname in aimlDateinamen)
			{
				Application.DoEvents();
				wissensLader.AIMLDateiVerarbeiten(aimlDateiname, this._botEigenschaften);
			}
			wissensLader.AimlDateiWirdGeladen -= this.loader_AIMLDateiWirdGeladen;
			this._ladeProtokoll.Append(wissensLader.Protokoll);
			this.PostInit();
		}

		public void LadenAusXMLDoms(List<DomDocLadePaket> xmlDoms)
		{
			this.PreInit();
			WissensLader wissensLader = new WissensLader(this._wissen, this._loggingKultur, this._normalisierung);
			wissensLader.AimlDateiWirdGeladen += this.loader_AIMLDateiWirdGeladen;
			foreach (DomDocLadePaket xmlDom in xmlDoms)
			{
				Application.DoEvents();
				wissensLader.AimldomDokumentVerarbeiten(xmlDom.XmlDocument, xmlDom.Dateiname, this._botEigenschaften);
			}
			wissensLader.AimlDateiWirdGeladen -= this.loader_AIMLDateiWirdGeladen;
			this._ladeProtokoll.Append(wissensLader.Protokoll);
			this._ladeProtokoll.AppendFormat(ResReader.Reader(this._loggingKultur).GetString("DOMDokumenteEingelesen", this._loggingKultur), xmlDoms.Count, this._wissen.AnzahlCategories);
			this._ladeProtokoll.Append("\n");
			this.PostInit();
		}

		public string GetAntwort(string eingabe, GaitoBotSession session)
		{
			if (eingabe == null || eingabe.Trim() == "")
			{
				eingabe = "TARGET EMPTYINPUT";
			}
			session.Denkprotokoll.Add(new BotDenkProtokollSchritt(eingabe, BotDenkProtokollSchritt.SchrittArten.Eingabe));
			AntwortFinder antwortFinder = new AntwortFinder(this._normalisierung.StartupInfos.SatzTrenner.ToArray(), this._normalisierung, this._wissen, session, this._botEigenschaften, this._beiEinwortEingabenOhneMatchAufSraiOnlyOneWordUmleiten);
			List<AntwortSatz> list = antwortFinder.GetAntwortSaetze(eingabe);
			StringBuilder stringBuilder = new StringBuilder();
			if (list == null)
			{
				session.Denkprotokoll.Add(new BotDenkProtokollSchritt(ResReader.Reader(session.DenkprotokollKultur).GetString("KonnteKeineGueltigeAntwortEreugen", session.DenkprotokollKultur), BotDenkProtokollSchritt.SchrittArten.Warnung));
				list = new List<AntwortSatz>();
				list.Add(new AntwortSatz(ResReader.Reader(this.LoggingKultur).GetString("NotfallAntwort", this.LoggingKultur), true));
			}
			foreach (AntwortSatz item in list)
			{
				string[] array = item.Satz.Split('|');
				foreach (string text in array)
				{
					GaitoBotSessionSchritt gaitoBotSessionSchritt = new GaitoBotSessionSchritt();
					gaitoBotSessionSchritt.BotAusgabe = text;
					if (session.LetzterSchritt != null)
					{
						gaitoBotSessionSchritt.That = session.LetzterSchritt.BotAusgabe;
					}
					else
					{
						gaitoBotSessionSchritt.That = string.Empty;
					}
					gaitoBotSessionSchritt.Topic = session.AktuellesThema;
					gaitoBotSessionSchritt.UserEingabe = eingabe;
					session.AddSchritt(gaitoBotSessionSchritt);
					if (stringBuilder.Length != 0)
					{
						stringBuilder.Append(" ");
					}
					stringBuilder.Append(text);
				}
			}
			return stringBuilder.ToString();
		}

		private void loader_AIMLDateiWirdGeladen(object sender, WissensLader.AimlDateiWirdGeladenEventArgs e)
		{
			this.AIMLDateiWirdGeladenEvent(e.Dateiname);
		}

		private void PostInit()
		{
			this.trennerLaden();
		}

		private void PreInit()
		{
		}

		private void trennerLaden()
		{
		}
	}
}
