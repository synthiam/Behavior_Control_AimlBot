using de.springwald.toolbox;
using de.springwald.xml;
using de.springwald.xml.editor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Schema;

namespace GaitoBotEditorCore
{
	public class AIMLDatei : IArbeitsbereichDatei
	{
		public enum PruefFehlerVerhalten
		{
			NiemalsFehlerZeigen,
			FehlerDirektZeigen,
			BeiFehlerFragenObAnzeigen
		}

		public class AIMLDateiNichtGeladenException : ApplicationException
		{
			public AIMLDateiNichtGeladenException(string message)
				: base(message)
			{
			}
		}

		private string _dateiname;

		private string _titel;

		private List<AIMLTopic> _topicListe;

		private int _anzahlCategories = -1;

		private static Regex _nameSpaceBereinigerOeffnendTag;

		private static Regex _nameSpaceBereinigerSchliessendTag;

		public const string FileExtension = "aiml";

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

		public string[] Unterverzeichnisse
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

		public bool HatFehler
		{
			get;
			private set;
		}

		public string Titel
		{
			get
			{
				if (this._titel == null)
				{
					this._titel = AIMLDatei.TitelAusDateinameHerleiten(this._dateiname);
				}
				return this._titel;
			}
			set
			{
				this._titel = value;
			}
		}

		public bool IsChanged
		{
			get;
			set;
		}

		public bool SeitLetztemIsChangedAufDTDFehlergeprueft
		{
			get;
			private set;
		}

		public AIMLTopic ZuletztInDieserDateiGewaehlesTopic
		{
			get;
			set;
		}

		public AIMLTopic RootTopic
		{
			get
			{
				foreach (AIMLTopic topic in this.getTopics())
				{
					if (topic.IstRootTopic)
					{
						return topic;
					}
				}
				return null;
			}
		}

		public string SortKey
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				if (this.Unterverzeichnisse != null)
				{
					string[] unterverzeichnisse = this.Unterverzeichnisse;
					foreach (string arg in unterverzeichnisse)
					{
						stringBuilder.AppendFormat("__{0} ", arg);
					}
				}
				stringBuilder.AppendFormat("_{0}", this.Titel);
				return stringBuilder.ToString();
			}
		}

		public int AnzahlCategories
		{
			get
			{
				if (this._anzahlCategories == -1)
				{
					this._anzahlCategories = 0;
					foreach (AIMLTopic topic in this.getTopics())
					{
						this._anzahlCategories += topic.Categories.Count;
					}
				}
				return this._anzahlCategories;
			}
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

		public AIMLDatei(Arbeitsbereich arbeitsbereich)
		{
			this.Arbeitsbereich = arbeitsbereich;
			this.Unterverzeichnisse = new string[0];
			this.XML = new XmlDocument();
			this.XML.PreserveWhitespace = true;
			this.XML.NodeChanged += this.XMLChangedEvent;
			this.XML.NodeInserted += this.XMLChangedEvent;
			this.XML.NodeRemoved += this.XMLChangedEvent;
		}

		public static string TitelAusDateinameHerleiten(string dateiname)
		{
			string text = dateiname;
			int num = text.LastIndexOf("\\");
			if (num != -1)
			{
				text = text.Remove(0, text.LastIndexOf("\\") + 1);
			}
			if (text.LastIndexOf(".xml") > 0)
			{
				text = text.Remove(text.LastIndexOf(".xml"), 4);
			}
			if (text.LastIndexOf(".aiml") > 0)
			{
				text = text.Remove(text.LastIndexOf(".aiml"), 5);
			}
			return text;
		}

		public static string[] UnterverzeichnisseAusDateinameHerleiten(string dateiname, string arbeitsbereichRootPfad)
		{
			if (dateiname.StartsWith(arbeitsbereichRootPfad, StringComparison.OrdinalIgnoreCase))
			{
				dateiname = dateiname.Remove(0, arbeitsbereichRootPfad.Length);
				string[] array = dateiname.Split(new char[1]
				{
					'\\'
				}, StringSplitOptions.RemoveEmptyEntries);
				ArrayList arrayList = new ArrayList();
				for (int i = 0; i < array.Length - 1; i++)
				{
					arrayList.Add(array[i]);
				}
				return (string[])arrayList.ToArray(typeof(string));
			}
			return null;
		}

		public bool LoescheTopic(AIMLTopic topic)
		{
			if (topic == null)
			{
				return false;
			}
			if (topic.IstRootTopic)
			{
				return false;
			}
			if (this._topicListe.Contains(topic))
			{
				this._topicListe.Remove(topic);
			}
			this.Arbeitsbereich.Verlauf.AlleVerweiseDiesesAIMLTopicEntfernen(topic);
			topic.Delete();
			topic.Dispose();
			return true;
		}

		public List<AIMLTopic> getTopics()
		{
			if (this._topicListe == null)
			{
				this.TopicListeBereitstellen();
			}
			return this._topicListe;
		}

		public AIMLTopic AddNewTopic()
		{
			AIMLTopic aIMLTopic = AIMLTopic.createNewTopic(this);
			this._topicListe.Add(aIMLTopic);
			return aIMLTopic;
		}

		public void LeerFuellen()
		{
			this.LiesAusString("<aiml></aiml>");
		}

		public void LiesAusString(string inhalt)
		{
			inhalt = this.AIMLInhaltBereinigen(inhalt);
			try
			{
				this.XML.LoadXml(inhalt);
				ToolboxXML.WhitespacesBereinigen(this.XML.DocumentElement);
			}
			catch (XmlException ex)
			{
				throw new AIMLDateiNichtGeladenException(string.Format(ResReader.Reader.GetString("AIMLDateiInhaltlichFehlerhaft"), ex.Message));
			}
			catch (Exception ex2)
			{
				throw new AIMLDateiNichtGeladenException(string.Format(ResReader.Reader.GetString("AIMLDateiDefekt"), ex2.Message));
			}
			if (!this.XML.DocumentElement.Name.Equals("aiml"))
			{
				throw new AIMLDateiNichtGeladenException(string.Format(ResReader.Reader.GetString("AIMLDateiDefekt"), ResReader.Reader.GetString("DokumentElementNichtAIML")));
			}
			if (this.XML.SelectNodes("/aiml").Count != 1)
			{
				throw new AIMLDateiNichtGeladenException(string.Format(ResReader.Reader.GetString("AIMLDateiDefekt"), ResReader.Reader.GetString("DokumentElementAIMLNurEinmal")));
			}
			if (ToolboxSonstiges.IstEntwicklungsmodus)
			{
				this.GegenAIMLDTDPruefen(PruefFehlerVerhalten.NiemalsFehlerZeigen);
			}
			else
			{
				this.GegenAIMLDTDPruefen(PruefFehlerVerhalten.BeiFehlerFragenObAnzeigen);
			}
			this.TopicListeBereitstellen();
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
			this.Unterverzeichnisse = AIMLDatei.UnterverzeichnisseAusDateinameHerleiten(dateiname, arbeitsbereichRootPfad_);
		}

		public void GegenAIMLDTDPruefen(PruefFehlerVerhalten pruefVerhalten)
		{
			this.SeitLetztemIsChangedAufDTDFehlergeprueft = true;
			XMLQuellcodeAlsRTF xMLQuellcodeAlsRTF = new XMLQuellcodeAlsRTF();
			xMLQuellcodeAlsRTF.Regelwerk = new XMLRegelwerk(AIMLDTD.GetAIMLDTD());
			xMLQuellcodeAlsRTF.Rootnode = this.XML.DocumentElement;
			xMLQuellcodeAlsRTF.NodeWirdGeprueftEvent += this.toRtf_NodeWirdGeprueftEvent;
			xMLQuellcodeAlsRTF.Rendern();
			xMLQuellcodeAlsRTF.NodeWirdGeprueftEvent -= this.toRtf_NodeWirdGeprueftEvent;
			if (xMLQuellcodeAlsRTF.FehlerProtokollAlsText == "")
			{
				this.HatFehler = false;
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
				this.HatFehler = true;
			}
		}

		public void Save(out bool cancel)
		{
			if (this._dateiname == null)
			{
				throw new ApplicationException(ResReader.Reader.GetString("NochKeinDateinameVergeben"));
			}
			if (!this.SeitLetztemIsChangedAufDTDFehlergeprueft)
			{
				this.GegenAIMLDTDPruefen(PruefFehlerVerhalten.NiemalsFehlerZeigen);
			}
			bool flag = true;
			if (this.HatFehler)
			{
				DialogResult dialogResult = MessageBox.Show(ResReader.Reader.GetString("SollDieDateiDefektGespeichertWerden"), string.Format(ResReader.Reader.GetString("DateiIstDefektUndSollteVorSpeichernKorrigiertWerden"), this.Titel), MessageBoxButtons.YesNoCancel);
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
				AIMLDateiKommentare aIMLDateiKommentare = new AIMLDateiKommentare(this.XML);
				aIMLDateiKommentare.SchreibeKommentarEintrag("created with", string.Format("{0} V{1}", Application.ProductName, Application.ProductVersion));
				aIMLDateiKommentare.SchreibeKommentarEintrag("licence", LizenzManager.ProgrammLizenzName);
				aIMLDateiKommentare.SchreibeKommentarEintrag("author", Environment.UserName);
				aIMLDateiKommentare = null;
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
				string text = "ISO-8859-15";
				string input = this.XML.InnerXml.ToString();
				input = Regex.Replace(input, "<\\?xml .*? ?>", "");
				input = "<?xml version=\"1.0\" encoding=\"" + text + "\"?>" + input;
				if (true)
				{
					StringBuilder stringBuilder = new StringBuilder(input);
					stringBuilder.Replace("<category>", "\r\n<category>");
					stringBuilder.Replace("</category>", "\r\n</category>");
					stringBuilder.Replace("<template>", "\r\n\t<template>");
					stringBuilder.Replace("<pattern>", "\r\n\t<pattern>");
					stringBuilder.Replace("<random>", "\r\n\t\t<random>");
					stringBuilder.Replace("</random>", "\r\n\t\t</random>");
					stringBuilder.Replace("<li>", "\r\n\t\t\t<li>");
					stringBuilder.Replace("</aiml>", "\r\n</aiml>");
					stringBuilder.Replace("<aiml", "\r\n<aiml");
					stringBuilder.Replace("<!--", "\r\n<!--");
					input = stringBuilder.ToString();
				}
				byte[] bytes = Encoding.GetEncoding(text).GetBytes(input);
				fileStream.Write(bytes, 0, bytes.Length);
				fileStream.Close();
				this.IsChanged = false;
			}
		}

		public void MitTargetBotStartFuellen()
		{
			this.LiesAusString("<aiml><category><pattern>TARGET BOTSTART</pattern><template>This is the greeting of a GaitoBot conversation.</template></category></aiml>");
		}

		private void XMLChangedEvent(object src, XmlNodeChangedEventArgs args)
		{
			this.IsChanged = true;
			this.SeitLetztemIsChangedAufDTDFehlergeprueft = false;
			this._anzahlCategories = -1;
			this.OnChangedEvent(EventArgs.Empty);
		}

		private string AIMLInhaltBereinigen(string inhalt)
		{
			string pattern = "<[?]xml[\\t\\r\\n ]+[^>]+>";
			inhalt = Regex.Replace(inhalt, pattern, "<?xml version=\"1.0\" encoding=\"ISO-8859-15\"?>");
			pattern = "<aiml[\\t\\r\\n ]+[^>]+>";
			inhalt = Regex.Replace(inhalt, pattern, "<aiml>");
			inhalt = inhalt.Replace("<person/>", "<person><star/></person>");
			inhalt = inhalt.Replace("<person2/>", "<person2><star/></person2>");
			inhalt = inhalt.Replace("<gender/>", "<gender><star/></gender>");
			inhalt = inhalt.Replace("<sr/>", "<srai><star/></srai>");
			inhalt = inhalt.Replace("<person />", "<person><star/></person>");
			inhalt = inhalt.Replace("<person2 />", "<person2><star/></person2>");
			inhalt = inhalt.Replace("<gender />", "<gender><star/></gender>");
			inhalt = inhalt.Replace("<sr />", "<srai><star/></srai>");
			inhalt = this.TagsUmNameSpaceBereinigen(inhalt);
			return inhalt;
		}

		private string TagsUmNameSpaceBereinigen(string inhalt)
		{
			if (AIMLDatei._nameSpaceBereinigerOeffnendTag == null)
			{
				string pattern = "<(?<namespace>[\\w-_]+):(?<inhalt>[^>]+)>";
				AIMLDatei._nameSpaceBereinigerOeffnendTag = new Regex(pattern, RegexOptions.Compiled);
			}
			Match match = AIMLDatei._nameSpaceBereinigerOeffnendTag.Match(inhalt);
			while (match.Success)
			{
				string value = match.Groups["namespace"].Value;
				string value2 = match.Groups["inhalt"].Value;
				inhalt = inhalt.Replace(string.Format("<{0}:{1}>", value, value2), string.Format("<{0}>", value2));
				match = AIMLDatei._nameSpaceBereinigerOeffnendTag.Match(inhalt);
			}
			if (AIMLDatei._nameSpaceBereinigerSchliessendTag == null)
			{
				string pattern = "</(?<namespace>[\\w-_]+):(?<inhalt>[^>]+)>";
				AIMLDatei._nameSpaceBereinigerSchliessendTag = new Regex(pattern, RegexOptions.Compiled);
			}
			match = AIMLDatei._nameSpaceBereinigerSchliessendTag.Match(inhalt);
			while (match.Success)
			{
				string value = match.Groups["namespace"].Value;
				string value2 = match.Groups["inhalt"].Value;
				inhalt = inhalt.Replace(string.Format("</{0}:{1}>", value, value2), string.Format("</{0}>", value2));
				match = AIMLDatei._nameSpaceBereinigerSchliessendTag.Match(inhalt);
			}
			return inhalt;
		}

		private void TopicListeBereitstellen()
		{
			this._topicListe = new List<AIMLTopic>();
			this._topicListe.Add(new AIMLTopic(this.XML.DocumentElement, this));
			XmlNodeList xmlNodeList = this.XML.SelectNodes("/aiml/topic");
			foreach (XmlNode item in xmlNodeList)
			{
				this._topicListe.Add(new AIMLTopic(item, this));
			}
		}

		private static void ValidationCallback(object sender, ValidationEventArgs args)
		{
			Console.WriteLine("Validation error loading: {0}", "");
			Console.WriteLine(args.Message);
		}

		private void toRtf_NodeWirdGeprueftEvent(object sender, EventArgs e)
		{
			this.ActivateNodeWirdGeprueft(EventArgs.Empty);
		}
	}
}
