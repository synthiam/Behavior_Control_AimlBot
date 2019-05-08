using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace de.springwald.gaitobot2
{
	public class StartupInfos
	{
		private List<DictionaryEntry> _ersetzungen;

		private List<string> _satztrenner;

		public GaitoBotEigenschaften BotEigenschaften
		{
			get;
			private set;
		}

		public List<DictionaryEntry> PersonAustauscher
		{
			get;
			private set;
		}

		public List<DictionaryEntry> Person2Austauscher
		{
			get;
			private set;
		}

		public List<DictionaryEntry> GeschlechtsAustauscher
		{
			get;
			private set;
		}

		public List<DictionaryEntry> Ersetzungen
		{
			get;
			private set;
		}

		public List<string> SatzTrenner
		{
			get;
			private set;
		}

		public StartupInfos()
		{
			this.Clear();
		}

		public void Clear()
		{
			this.BotEigenschaften = new GaitoBotEigenschaften();
			this.Ersetzungen = new List<DictionaryEntry>
			{
				new DictionaryEntry(" \\", " "),
				new DictionaryEntry("&QUOT;", ""),
				new DictionaryEntry("/", " "),
				new DictionaryEntry("&AMP;", " "),
				new DictionaryEntry("  ", " ")
			};
			this.GeschlechtsAustauscher = new List<DictionaryEntry>();
			this.PersonAustauscher = new List<DictionaryEntry>();
			this.Person2Austauscher = new List<DictionaryEntry>();
			this.SatzTrenner = new List<string>
			{
				";",
				".",
				"?",
				"!"
			};
		}

		public void FuegeEintraegeAusStartupDateiHinzu(XmlDocument startupDateiInhalt)
		{
			this.MerkeErsetzungen(startupDateiInhalt.SelectNodes("/gaitobot-startup/substitutions/input/substitute"), this.Ersetzungen);
			this.MerkeErsetzungen(startupDateiInhalt.SelectNodes("/gaitobot-startup/substitutions/gender/substitute"), this.GeschlechtsAustauscher);
			this.MerkeErsetzungen(startupDateiInhalt.SelectNodes("/gaitobot-startup/substitutions/person/substitute"), this.PersonAustauscher);
			this.MerkeErsetzungen(startupDateiInhalt.SelectNodes("/gaitobot-startup/substitutions/person2/substitute"), this.Person2Austauscher);
			XmlNodeList xmlNodeList = startupDateiInhalt.SelectNodes("/gaitobot-startup/bot/property");
			foreach (XmlElement item in xmlNodeList)
			{
				XmlAttribute xmlAttribute = item.Attributes["name"];
				XmlAttribute xmlAttribute2 = item.Attributes["value"];
				if (xmlAttribute2 != null && !string.IsNullOrEmpty(xmlAttribute2.Value) && xmlAttribute != null && !string.IsNullOrEmpty(xmlAttribute.Value))
				{
					this.BotEigenschaften.Setzen(xmlAttribute.Value, xmlAttribute2.Value);
				}
			}
			XmlNodeList xmlNodeList2 = startupDateiInhalt.SelectNodes("/gaitobot-startup/sentence-splitters/splitter");
			foreach (XmlElement item2 in xmlNodeList2)
			{
				XmlAttribute xmlAttribute3 = item2.Attributes["value"];
				if (xmlAttribute3 != null && !string.IsNullOrEmpty(xmlAttribute3.Value))
				{
					this.SatzTrenner.Add(xmlAttribute3.Value);
				}
			}
		}

		public void FuegeEintraegeAusStartupDateienInVerzeichnisHinzu(string verzeichnis)
		{
			string[] files = Directory.GetFiles(verzeichnis, "*.startup");
			foreach (string startupDateiname in files)
			{
				this.StartUpDateiVerarbeiten(startupDateiname);
			}
		}

		private void StartUpDateiVerarbeiten(string startupDateiname)
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.PreserveWhitespace = false;
			xmlDocument.Load(startupDateiname);
			this.FuegeEintraegeAusStartupDateiHinzu(xmlDocument);
		}

		private void MerkeErsetzungen(XmlNodeList ersetzungenXmlNodes, List<DictionaryEntry> zielListe)
		{
			foreach (XmlElement ersetzungenXmlNode in ersetzungenXmlNodes)
			{
				XmlAttribute xmlAttribute = ersetzungenXmlNode.Attributes["find"];
				XmlAttribute xmlAttribute2 = ersetzungenXmlNode.Attributes["replace"];
				if (xmlAttribute != null && !string.IsNullOrEmpty(xmlAttribute.Value) && xmlAttribute2 != null && !string.IsNullOrEmpty(xmlAttribute2.Value))
				{
					zielListe.Add(new DictionaryEntry(xmlAttribute.Value, xmlAttribute2.Value));
				}
			}
		}
	}
}
