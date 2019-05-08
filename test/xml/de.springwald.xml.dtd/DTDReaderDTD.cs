using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace de.springwald.xml.dtd
{
	public class DTDReaderDTD
	{
		private string _rohinhalt;

		private string _workingInhalt;

		private List<DTDElement> _elemente;

		private List<DTDEntity> _entities;

		public string RohInhalt
		{
			get
			{
				return this._rohinhalt;
			}
		}

		public string WorkingInhalt
		{
			get
			{
				return this._workingInhalt;
			}
		}

		public DTD GetDTDFromFile(string dateiname)
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
				throw new ApplicationException(string.Format(ResReader.Reader.GetString("KonnteDateiNichtEinlesen"), dateiname, ex.Message));
			}
			return this.GetDTDFromString(text);
		}

		public DTD GetDTDFromString(string inhalt)
		{
			inhalt = inhalt.Replace("\t", " ");
			this._rohinhalt = inhalt;
			this._workingInhalt = inhalt;
			this._elemente = new List<DTDElement>();
			this._entities = new List<DTDEntity>();
			this.InhaltAnalysieren();
			this._elemente.Add(this.CreateElementFromQuellcode("#PCDATA"));
			this._elemente.Add(this.CreateElementFromQuellcode("#COMMENT"));
			return new DTD(this._elemente, this._entities);
		}

		private void InhaltAnalysieren()
		{
			this.KommentareEntfernen();
			this.EntitiesAuslesen();
			this.EntitiesAustauschen();
			this.ElementeAuslesen();
		}

		private void KommentareEntfernen()
		{
			string pattern = "<!--((?!-->|<!--)([\\t\\r\\n]|.))*-->";
			this._workingInhalt = Regex.Replace(this._workingInhalt, pattern, "");
		}

		private void ElementeAuslesen()
		{
			string pattern = "(?<element><!ELEMENT[\\t\\r\\n ]+[^>]+>)";
			Regex regex = new Regex(pattern);
			Match match = regex.Match(this._workingInhalt);
			SortedList sortedList = new SortedList();
			while (match.Success)
			{
				string value = match.Groups["element"].Value;
				DTDElement dTDElement = this.CreateElementFromQuellcode(value);
				try
				{
					sortedList.Add(dTDElement.Name, dTDElement);
				}
				catch (ArgumentException ex)
				{
					throw new ApplicationException(string.Format(ResReader.Reader.GetString("FehlerBeimLesenDesDTDELementes"), dTDElement.Name, ex.Message));
				}
				match = match.NextMatch();
			}
			for (int i = 0; i < sortedList.Count; i++)
			{
				this._elemente.Add((DTDElement)sortedList[sortedList.GetKey(i)]);
			}
		}

		private DTDElement CreateElementFromQuellcode(string elementQuellcode)
		{
			if (elementQuellcode == "#PCDATA")
			{
				DTDElement dTDElement = new DTDElement();
				dTDElement.Name = "#PCDATA";
				dTDElement.ChildElemente = new DTDChildElemente("");
				return dTDElement;
			}
			if (elementQuellcode == "#COMMENT")
			{
				DTDElement dTDElement2 = new DTDElement();
				dTDElement2.Name = "#COMMENT";
				dTDElement2.ChildElemente = new DTDChildElemente("");
				return dTDElement2;
			}
			string pattern = "(?<element><!ELEMENT[\\t\\r\\n ]+(?<elementname>[\\w-_]+?)([\\t\\r\\n ]+(?<innerelements>[(]([\\t\\r\\n]|.)+?[)][*+]?)?)?(?<empty>[\\t\\r\\n ]+EMPTY)? *>)";
			Regex regex = new Regex(pattern);
			Match match = regex.Match(elementQuellcode);
			if (!match.Success)
			{
				throw new ApplicationException(string.Format(ResReader.Reader.GetString("NichtsImElementCodeGefunden"), elementQuellcode));
			}
			DTDElement dTDElement3 = new DTDElement();
			if (!match.Groups["elementname"].Success)
			{
				throw new ApplicationException(string.Format(ResReader.Reader.GetString("KeinNameInElementcodegefunden"), elementQuellcode));
			}
			dTDElement3.Name = match.Groups["elementname"].Value;
			this.CreateDTDAttributesForElement(dTDElement3);
			if (match.Groups["innerelements"].Success)
			{
				this.ChildElementeAuslesen(dTDElement3, match.Groups["innerelements"].Value);
			}
			else
			{
				this.ChildElementeAuslesen(dTDElement3, "");
			}
			match = match.NextMatch();
			if (match.Success)
			{
				throw new ApplicationException(string.Format(ResReader.Reader.GetString("MehrAlsEinsImElementCodeGefunden"), elementQuellcode));
			}
			return dTDElement3;
		}

		private void ChildElementeAuslesen(DTDElement element, string childElementeQuellcode)
		{
			element.ChildElemente = new DTDChildElemente(childElementeQuellcode);
		}

		private void EntitiesAustauschen()
		{
			string a = "";
			while (a != this._workingInhalt)
			{
				a = this._workingInhalt;
				foreach (DTDEntity entity in this._entities)
				{
					if (entity.IstErsetzungsEntity)
					{
						this._workingInhalt = this._workingInhalt.Replace("%" + entity.Name + ";", entity.Inhalt);
					}
				}
			}
		}

		private void EntitiesAuslesen()
		{
			string pattern = "(?<entity><!ENTITY[\\t\\r\\n ]+[^>]+>)";
			Regex regex = new Regex(pattern);
			Match match = regex.Match(this._workingInhalt);
			while (match.Success)
			{
				string value = match.Groups["entity"].Value;
				DTDEntity item = this.CreateEntityFromQuellcode(value);
				this._entities.Add(item);
				match = match.NextMatch();
			}
		}

		private DTDEntity CreateEntityFromQuellcode(string entityQuellcode)
		{
			string pattern = "(?<entity><!ENTITY[\\t\\r\\n ]+(?:(?<prozent>%)[\\t\\r\\n ]+)?(?<entityname>[\\w-_]+?)[\\t\\r\\n ]+\"(?<inhalt>[^>]+)\"[\\t\\r\\n ]?>)";
			Regex regex = new Regex(pattern);
			Match match = regex.Match(entityQuellcode);
			if (!match.Success)
			{
				throw new ApplicationException(string.Format(ResReader.Reader.GetString("NichtsImEntityCode"), entityQuellcode));
			}
			DTDEntity dTDEntity = new DTDEntity();
			dTDEntity.IstErsetzungsEntity = match.Groups["prozent"].Success;
			if (!match.Groups["entityname"].Success)
			{
				throw new ApplicationException(string.Format(ResReader.Reader.GetString("KeinNameImEntityCode"), entityQuellcode));
			}
			dTDEntity.Name = match.Groups["entityname"].Value;
			if (!match.Groups["inhalt"].Success)
			{
				throw new ApplicationException(string.Format(ResReader.Reader.GetString("KeinInhaltImEntityCode"), entityQuellcode));
			}
			dTDEntity.Inhalt = match.Groups["inhalt"].Value;
			match = match.NextMatch();
			if (match.Success)
			{
				throw new ApplicationException(string.Format(ResReader.Reader.GetString("MehrAlsEinsImEntityQuellCode"), entityQuellcode));
			}
			return dTDEntity;
		}

		private void CreateDTDAttributesForElement(DTDElement element)
		{
			element.Attribute = new List<DTDAttribut>();
			string pattern = "(?<attributliste><!ATTLIST " + element.Name + "[\\t\\r\\n ]+(?<attribute>[^>]+?)[\\t\\r\\n ]?>)";
			Regex regex = new Regex(pattern);
			Match match = regex.Match(this._workingInhalt);
			if (!match.Success)
			{
				return;
			}
			string value = match.Groups["attribute"].Value;
			string pattern2 = "[\\t\\r\\n ]?(?<name>[\\w-_]+)[\\t\\r\\n ]+(?<typ>CDATA|ID|IDREF|IDREFS|NMTOKEN|NMTOKENS|ENTITY|ENTITIES|NOTATION|xml:|[(][|\\w-_ \\t\\r\\n]+[)])[\\t\\r\\n ]+(?:(?<anzahl>#REQUIRED|#IMPLIED|#FIXED)[\\t\\r\\n ]+)?(?:\"(?<vorgabewert>[\\w-_]+)\")?[\\t\\r\\n ]?";
			Regex regex2 = new Regex(pattern2);
			match = regex2.Match(value);
			if (match.Success)
			{
				string text = "|";
				char[] separator = text.ToCharArray();
				while (match.Success)
				{
					DTDAttribut dTDAttribut = new DTDAttribut();
					dTDAttribut.Name = match.Groups["name"].Value;
					dTDAttribut.StandardWert = match.Groups["vorgabewert"].Value;
					string value2 = match.Groups["anzahl"].Value;
					switch (value2)
					{
					default:
						if (value2.Length != 0)
						{
							goto case null;
						}
						goto case "#IMPLIED";
					case null:
						if (value2 == "#FIXED")
						{
							dTDAttribut.Pflicht = DTDAttribut.PflichtArten.Konstante;
							break;
						}
						throw new ApplicationException(string.Format(ResReader.Reader.GetString("UnbekannteAttributAnzahl"), match.Groups["anzahl"].Value, match.Value, element.Name));
					case "#REQUIRED":
						dTDAttribut.Pflicht = DTDAttribut.PflichtArten.Pflicht;
						break;
					case "#IMPLIED":
						dTDAttribut.Pflicht = DTDAttribut.PflichtArten.Optional;
						break;
					}
					string value3 = match.Groups["typ"].Value;
					value3 = value3.Trim();
					if (value3.StartsWith("("))
					{
						dTDAttribut.Typ = "";
						value3 = value3.Replace("(", "");
						value3 = value3.Replace(")", "");
						value3 = value3.Replace(")", "");
						string[] array = value3.Split(separator);
						string[] array2 = array;
						foreach (string text2 in array2)
						{
							string text3 = text2.Replace("\n", " ");
							text3 = text3.Trim();
							dTDAttribut.ErlaubteWerte.Add(text3);
						}
					}
					else
					{
						dTDAttribut.Typ = value3;
					}
					element.Attribute.Add(dTDAttribut);
					match = match.NextMatch();
				}
				return;
			}
			throw new ApplicationException(string.Format(ResReader.Reader.GetString("KeineAttributeInAttributListe"), value));
		}
	}
}
