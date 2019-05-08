using de.springwald.toolbox;
using de.springwald.xml;
using de.springwald.xml.editor;
using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;

namespace GaitoBotEditorCore
{
	public class StartupDatei : IArbeitsbereichDatei
	{
		public enum PruefFehlerVerhalten
		{
			NiemalsFehlerZeigen,
			FehlerDirektZeigen,
			BeiFehlerFragenObAnzeigen
		}

		public class StartUpDateiNichtGeladenException : ApplicationException
		{
			public StartUpDateiNichtGeladenException(string message)
				: base(message)
			{
			}
		}

		private Arbeitsbereich _arbeitsbereich;

		private string _dateiname;

		private string _titel;

		private bool _seitLetztemIsChangedAufDTDFehlergeprueft;

		private bool _hatDTDFehler;

		private static Regex _nameSpaceBereinigerOeffnendTag;

		private static Regex _nameSpaceBereinigerSchliessendTag;

		public const string FileExtension = "startup";

		public string ZusatzContentUniqueId
		{
			get;
			set;
		}

		public bool ReadOnly
		{
			get;
			set;
		}

		public bool NurFuerGaitoBotExportieren
		{
			get;
			set;
		}

		public Arbeitsbereich Arbeitsbereich
		{
			get;
			private set;
		}

		public XmlDocument XML
		{
			get;
			private set;
		}

		public string Dateiname
		{
			get
			{
				return this._dateiname;
			}
			set
			{
				this._dateiname = value;
				this._titel = null;
			}
		}

		public string Titel
		{
			get
			{
				if (this._titel == null)
				{
					this._titel = StartupDatei.TitelAusDateinameHerleiten(this._dateiname);
				}
				return this._titel;
			}
			set
			{
				this._titel = value;
			}
		}

		public bool HatFehler
		{
			get
			{
				return this._hatDTDFehler;
			}
		}

		public bool IsChanged
		{
			get;
			set;
		}

		public bool SeitLetztemIsChangedAufDTDFehlergeprueft
		{
			get
			{
				return this._seitLetztemIsChangedAufDTDFehlergeprueft;
			}
		}

		public string SortKey
		{
			get
			{
				return this.Dateiname;
			}
		}

		public string[] Unterverzeichnisse
		{
			get;
			set;
		}

		public event EventHandler OnChanged;

		public event EventHandler NodeWirdGeprueftEvent;

		protected virtual void OnChangedEvent(EventArgs e)
		{
			if (this.OnChanged != null)
			{
				this.OnChanged(this, e);
			}
		}

		protected virtual void ActivateNodeWirdGeprueft(EventArgs e)
		{
			if (this.NodeWirdGeprueftEvent != null)
			{
				this.NodeWirdGeprueftEvent(this, e);
			}
		}

		public StartupDatei(Arbeitsbereich arbeitsbereich)
		{
			this.Unterverzeichnisse = new string[0];
			this._arbeitsbereich = arbeitsbereich;
			this.XML = new XmlDocument();
			this.XML.PreserveWhitespace = false;
			this.XML.NodeChanged += this.XMLChangedEvent;
			this.XML.NodeInserted += this.XMLChangedEvent;
			this.XML.NodeRemoved += this.XMLChangedEvent;
		}

		public void LeerFuellen()
		{
			this.LiesAusString("<gaitobot-startup></gaitobot-startup>");
		}

		public void LiesAusString(string inhalt)
		{
			inhalt = this.StartUpInhaltBereinigen(inhalt);
			try
			{
				this.XML.LoadXml(inhalt);
				ToolboxXML.WhitespacesBereinigen(this.XML.DocumentElement);
			}
			catch (XmlException ex)
			{
				throw new StartUpDateiNichtGeladenException(string.Format(ResReader.Reader.GetString("StartupDateiInhaltlichFehlerhaft"), ex.Message));
			}
			catch (Exception ex2)
			{
				throw new StartUpDateiNichtGeladenException(string.Format(ResReader.Reader.GetString("StartupDateiDefekt"), ex2.Message));
			}
			if (!this.XML.DocumentElement.Name.Equals("gaitobot-startup"))
			{
				throw new StartUpDateiNichtGeladenException(string.Format(ResReader.Reader.GetString("StartupDateiDefekt"), ResReader.Reader.GetString("DokumentElementNichtGaitoBotStartup")));
			}
			if (this.XML.SelectNodes("/gaitobot-startup").Count != 1)
			{
				throw new StartUpDateiNichtGeladenException(string.Format(ResReader.Reader.GetString("StartupDateiDefekt"), ResReader.Reader.GetString("DokumentElementStartupNurEinmal")));
			}
			if (ToolboxSonstiges.IstEntwicklungsmodus)
			{
				this.GegenStartupDTDPruefen(PruefFehlerVerhalten.NiemalsFehlerZeigen);
			}
			else
			{
				this.GegenStartupDTDPruefen(PruefFehlerVerhalten.BeiFehlerFragenObAnzeigen);
			}
			this.IsChanged = false;
		}

		public void LiesAusDatei(string dateiname, string arbeitsbereichRootPfad_)
		{
			string text = "";
			try
			{
				StreamReader streamReader = new StreamReader(dateiname, Encoding.GetEncoding("ISO-8859-15"));
				text = streamReader.ReadToEnd();
				streamReader.Close();
			}
			catch (FileNotFoundException ex)
			{
				throw new ApplicationException(string.Format("Unable to read file '{0}'\n{1}", dateiname, ex.Message));
			}
			this._dateiname = dateiname;
			this.LiesAusString(text);
		}

		public void GegenStartupDTDPruefen(PruefFehlerVerhalten pruefVerhalten)
		{
			this._seitLetztemIsChangedAufDTDFehlergeprueft = true;
			XMLQuellcodeAlsRTF xMLQuellcodeAlsRTF = new XMLQuellcodeAlsRTF();
			xMLQuellcodeAlsRTF.Regelwerk = new XMLRegelwerk(StartUpDTD.GetStartUpDTD());
			xMLQuellcodeAlsRTF.Rootnode = this.XML.DocumentElement;
			xMLQuellcodeAlsRTF.NodeWirdGeprueftEvent += this.toRtf_NodeWirdGeprueftEvent;
			xMLQuellcodeAlsRTF.Rendern();
			xMLQuellcodeAlsRTF.NodeWirdGeprueftEvent -= this.toRtf_NodeWirdGeprueftEvent;
			if (xMLQuellcodeAlsRTF.FehlerProtokollAlsText == "")
			{
				this._hatDTDFehler = false;
			}
			else
			{
				switch (pruefVerhalten)
				{
				case PruefFehlerVerhalten.BeiFehlerFragenObAnzeigen:
					if (MessageBox.Show(ResReader.Reader.GetString("FehlerImXMLDokJetztAnzeigen"), ResReader.Reader.GetString("DTDFehler"), MessageBoxButtons.YesNo) == DialogResult.Yes)
					{
						xMLQuellcodeAlsRTF.QuellCodeUndFehlerInNeuemFormZeigen();
					}
					break;
				case PruefFehlerVerhalten.FehlerDirektZeigen:
					xMLQuellcodeAlsRTF.QuellCodeUndFehlerInNeuemFormZeigen();
					break;
				}
				this._hatDTDFehler = true;
			}
		}

		public void Save(out bool cancel)
		{
			if (this._dateiname == null)
			{
				throw new ApplicationException(ResReader.Reader.GetString("NochKeinDateinameVergeben"));
			}
			if (this._seitLetztemIsChangedAufDTDFehlergeprueft)
			{
				this.GegenStartupDTDPruefen(PruefFehlerVerhalten.NiemalsFehlerZeigen);
			}
			bool flag = true;
			if (this.HatFehler)
			{
				DialogResult dialogResult = MessageBox.Show(ResReader.Reader.GetString("SollDieDateiDefektGespeichertWerden"), string.Format(ResReader.Reader.GetString("DateiIstDefektUndSollteVorSpeichernKorrigiertWerden"), this.Dateiname), MessageBoxButtons.YesNoCancel);
				switch (dialogResult)
				{
				case DialogResult.Yes:
					cancel = false;
					flag = true;
					break;
				case DialogResult.No:
					cancel = false;
					flag = false;
					break;
				case DialogResult.Cancel:
					cancel = true;
					flag = false;
					break;
				default:
					throw new ApplicationException("Unbekanntes Dialogergebnis '" + dialogResult + "'");
				}
			}
			else
			{
				cancel = false;
			}
			if (flag)
			{
				ArrayList arrayList = new ArrayList();
				foreach (XmlNode childNode in this.XML.DocumentElement.ChildNodes)
				{
					if (childNode.Name == "meta")
					{
						arrayList.Add(childNode);
					}
				}
				foreach (XmlNode item in arrayList)
				{
					item.ParentNode.RemoveChild(item);
				}
				for (int num = 10; num >= 0; num--)
				{
					try
					{
						File.Delete(string.Format("{0}.bak_{1}", this.Dateiname, num.ToString()));
						File.Move(string.Format("{0}.bak_{1}", this.Dateiname, num - 1), string.Format("{0}.bak_{1}", this.Dateiname, num));
					}
					catch (Exception)
					{
					}
				}
				if (File.Exists(this.Dateiname))
				{
					File.Move(this.Dateiname, string.Format("{0}.bak_0", this.Dateiname));
				}
				FileStream fileStream = File.Create(this.Dateiname);
				string input = this.XML.InnerXml.ToString();
				input = Regex.Replace(input, "<\\?xml .*? ?>", "");
				input = "<?xml version=\"1.0\" encoding=\"ISO-8859-15\"?>" + input;
				if (true)
				{
					StringBuilder stringBuilder = new StringBuilder(input);
					stringBuilder.Replace("<bot>", "\r\n<bot>");
					stringBuilder.Replace("<predicates>", "\r\n\t<predicates>");
					stringBuilder.Replace("<substitutions>", "\r\n\t<substitutions>");
					stringBuilder.Replace("<sentence-splitters>", "\r\n\t\t<sentence-splitters>");
					stringBuilder.Replace("<input>", "\r\n<input>");
					stringBuilder.Replace("<gender>", "\r\n<gender>");
					stringBuilder.Replace("<person>", "\r\n<person>");
					stringBuilder.Replace("<person2>", "\r\n<person2>");
					stringBuilder.Replace("<property ", "\r\n\t\t<property ");
					stringBuilder.Replace("<substitute ", "\r\n\t\t<substitute ");
					stringBuilder.Replace("<splitter ", "\r\n\t\t\t<splitter ");
					stringBuilder.Replace("<!--", "\r\n<!--");
					input = stringBuilder.ToString();
				}
				byte[] bytes = Encoding.GetEncoding("ISO-8859-15").GetBytes(input);
				fileStream.Write(bytes, 0, bytes.Length);
				fileStream.Close();
				this.IsChanged = false;
			}
		}

		private void XMLChangedEvent(object src, XmlNodeChangedEventArgs args)
		{
			this.IsChanged = true;
			this._seitLetztemIsChangedAufDTDFehlergeprueft = false;
			this.OnChangedEvent(EventArgs.Empty);
		}

		private string StartUpInhaltBereinigen(string inhalt)
		{
			string pattern = "<[?]xml[\\t\\r\\n ]+[^>]+>";
			inhalt = Regex.Replace(inhalt, pattern, "<?xml version=\"1.0\" encoding=\"ISO-8859-15\"?>");
			pattern = "</[a-zA-Z]*-?startup>";
			inhalt = Regex.Replace(inhalt, pattern, "</gaitobot-startup>");
			pattern = "<[a-zA-Z]*-?startup>";
			inhalt = Regex.Replace(inhalt, pattern, "<gaitobot-startup>");
			return inhalt;
		}

		public static string TitelAusDateinameHerleiten(string dateiname)
		{
			string text = dateiname;
			int num = text.LastIndexOf("\\");
			if (num != -1)
			{
				text = text.Remove(0, text.LastIndexOf("\\") + 1);
			}
			if (text.LastIndexOf(".startup") > 0)
			{
				text = text.Remove(text.LastIndexOf(".startup"), 8);
			}
			return text;
		}

		private void toRtf_NodeWirdGeprueftEvent(object sender, EventArgs e)
		{
			this.ActivateNodeWirdGeprueft(EventArgs.Empty);
		}
	}
}
