using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace de.springwald.gaitobot2
{
	internal class AntwortFinder
	{
		private readonly Wissen _wissen;

		private readonly Normalisierung _normalisierer;

		private readonly GaitoBotSession _session;

		private readonly GaitoBotEigenschaften _botEigenschaften;

		private readonly bool _beiEinwortEingabenOhneMatchAufSraiOnlyOneWordUmleiten;

		private readonly string[] _trenner;

		public AntwortFinder(string[] trenner, Normalisierung normalisierer, Wissen wissen, GaitoBotSession session, GaitoBotEigenschaften botEigenschaften, bool beiEinwortEingabenOhneMatchAufSraiOnlyOneWordUmleiten)
		{
			this._beiEinwortEingabenOhneMatchAufSraiOnlyOneWordUmleiten = beiEinwortEingabenOhneMatchAufSraiOnlyOneWordUmleiten;
			this._trenner = trenner;
			this._botEigenschaften = botEigenschaften;
			this._wissen = wissen;
			this._session = session;
			this._normalisierer = normalisierer;
		}

		public List<AntwortSatz> GetAntwortSaetze(string eingabe)
		{
			if (this._session == null)
			{
				throw new ApplicationException("_session = null!");
			}
			string empty = string.Empty;
			string aktuellesThema = this._session.AktuellesThema;
			empty = ((this._session.LetzterSchritt == null) ? "*" : this._session.LetzterSchritt.BotAusgabe);
			this._session.Denkprotokoll.Add(new BotDenkProtokollSchritt(string.Format(ResReader.Reader(this._session.DenkprotokollKultur).GetString("ThatTopicImDenkprotokoll", this._session.DenkprotokollKultur), aktuellesThema, empty), BotDenkProtokollSchritt.SchrittArten.Info));
			eingabe = this._normalisierer.StandardErsetzungenDurchfuehren(eingabe);
			string[] array = eingabe.Split(this._trenner, StringSplitOptions.RemoveEmptyEntries);
			List<AntwortSatz> list = new List<AntwortSatz>();
			bool flag = true;
			string[] array2 = array;
			foreach (string text in array2)
			{
				if (text != null && text.Trim() != "")
				{
					AntwortSatz antwortSatz = this.GetQualifizierteAntwort_(text, empty, new ArrayList());
					if (antwortSatz != null)
					{
						if (this._beiEinwortEingabenOhneMatchAufSraiOnlyOneWordUmleiten && antwortSatz.IstNotfallAntwort && eingabe.Trim().IndexOf(" ") == -1)
						{
							antwortSatz = new AntwortSatz("TARGET ONLYONEWORD", true);
							antwortSatz.IstNotfallAntwort = true;
						}
						if (antwortSatz.IstNotfallAntwort)
						{
							if (flag)
							{
								list = new List<AntwortSatz>();
								list.Add(antwortSatz);
							}
						}
						else
						{
							if (flag)
							{
								list = new List<AntwortSatz>();
							}
							list.Add(antwortSatz);
							flag = false;
						}
					}
				}
			}
			if (flag && !this._session.BereitsEineNotfallAntwortGezeigt)
			{
				this._session.BereitsEineNotfallAntwortGezeigt = true;
				return this.GetAntwortSaetze("TARGET FIRSTBADANSWER");
			}
			foreach (AntwortSatz item in list)
			{
				if (item.IstNotfallAntwort && item.Satz == "TARGET ONLYONEWORD")
				{
					AntwortSatz qualifizierteAntwort_ = this.GetQualifizierteAntwort_("TARGET ONLYONEWORD", "EMPTYTHAT", new ArrayList());
					if (qualifizierteAntwort_ != null)
					{
						item.Satz = qualifizierteAntwort_.Satz;
					}
				}
			}
			if (list.Count == 0)
			{
				list = null;
			}
			return list;
		}

		private AntwortSatz GetQualifizierteAntwort_(string einSatz, string that, ArrayList vorherigeSRAIs)
		{
			this._session.Denkprotokoll.Add(new BotDenkProtokollSchritt(einSatz, BotDenkProtokollSchritt.SchrittArten.Eingabe));
			if (vorherigeSRAIs.Count > 50)
			{
				return new AntwortSatz(string.Format("SRAI-recursion at '{0}'", einSatz), false);
			}
			einSatz = this._normalisierer.StandardErsetzungenDurchfuehren(einSatz);
			einSatz = Normalisierung.EingabePatternOptimieren(einSatz, false).Trim();
			if (einSatz == "")
			{
				this._session.Denkprotokoll.Add(new BotDenkProtokollSchritt("Die Eingabe ist nach Normalisierung und Patternoptimierung leer", BotDenkProtokollSchritt.SchrittArten.Info));
				return null;
			}
			string text = that;
			that = this._normalisierer.StandardErsetzungenDurchfuehren(that);
			if (that == "")
			{
				that = text;
			}
			else
			{
				text = that;
			}
			that = Normalisierung.EingabePatternOptimieren(that.Trim(), false);
			if (that == "")
			{
				that = text;
			}
			WissenThema thema = this._wissen.GetThema(this._session.AktuellesThema);
			AntwortSatz result = default(AntwortSatz);
			if (thema != null)
			{
				foreach (WissensCategory category in thema.Categories)
				{
					if (category.Pattern.Inhalt != "*" && category.That.Inhalt != "*" && this.PasstCategory(category, einSatz, that, vorherigeSRAIs, out result))
					{
						return result;
					}
				}
			}
			thema = this._wissen.GetThema(this._session.AktuellesThema);
			if (thema != null)
			{
				foreach (WissensCategory category2 in thema.Categories)
				{
					if (category2.Pattern.Inhalt != "*" && category2.That.Inhalt == "*" && this.PasstCategory(category2, einSatz, that, vorherigeSRAIs, out result))
					{
						return result;
					}
				}
			}
			for (int i = 1; i < this._session.LetzteThemen.Count; i++)
			{
				string themaName = this._session.LetzteThemen[this._session.LetzteThemen.Count - 1 - i];
				thema = this._wissen.GetThema(themaName);
				if (thema != null)
				{
					foreach (WissensCategory category3 in thema.Categories)
					{
						if (category3.IstSrai && category3.Pattern.Inhalt != "*" && category3.That.Inhalt != "*" && this.PasstCategory(category3, einSatz, that, vorherigeSRAIs, out result))
						{
							return result;
						}
					}
				}
			}
			for (int i = 1; i < this._session.LetzteThemen.Count; i++)
			{
				string themaName = this._session.LetzteThemen[this._session.LetzteThemen.Count - 1 - i];
				thema = this._wissen.GetThema(themaName);
				if (thema != null)
				{
					foreach (WissensCategory category4 in thema.Categories)
					{
						if (category4.IstSrai && category4.Pattern.Inhalt != "*" && category4.That.Inhalt == "*" && this.PasstCategory(category4, einSatz, that, vorherigeSRAIs, out result))
						{
							return result;
						}
					}
				}
			}
			thema = this._wissen.GetThema(this._session.AktuellesThema);
			if (thema != null)
			{
				foreach (WissensCategory starCategory in thema.StarCategories)
				{
					if (starCategory.That.Inhalt != "*" && this.PasstCategory(starCategory, einSatz, that, vorherigeSRAIs, out result))
					{
						return result;
					}
				}
			}
			for (int i = 1; i < this._session.LetzteThemen.Count; i++)
			{
				string themaName = this._session.LetzteThemen[this._session.LetzteThemen.Count - 1 - i];
				thema = this._wissen.GetThema(themaName);
				if (thema != null)
				{
					foreach (WissensCategory category5 in thema.Categories)
					{
						if (category5.That.Inhalt != "*" && this.PasstCategory(category5, einSatz, that, vorherigeSRAIs, out result))
						{
							return result;
						}
					}
				}
			}
			thema = this._wissen.GetThema(this._session.AktuellesThema);
			if (thema != null)
			{
				foreach (WissensCategory starCategory2 in thema.StarCategories)
				{
					if (starCategory2.That.Inhalt == "*" && this.PasstCategory(starCategory2, einSatz, that, vorherigeSRAIs, out result))
					{
						return result;
					}
				}
			}
			for (int i = 1; i < this._session.LetzteThemen.Count; i++)
			{
				string themaName = this._session.LetzteThemen[this._session.LetzteThemen.Count - 1 - i];
				thema = this._wissen.GetThema(themaName);
				if (thema != null)
				{
					foreach (WissensCategory category6 in thema.Categories)
					{
						if (category6.That.Inhalt == "*" && this.PasstCategory(category6, einSatz, that, vorherigeSRAIs, out result))
						{
							return result;
						}
					}
				}
			}
			return null;
		}

		private bool PasstCategory(WissensCategory category, string eingabe, string that, ArrayList vorherigeSRAIs, out AntwortSatz antwort)
		{
			PatternMatcher patternMatcher = new PatternMatcher(category.That.RegExObjekt, that);
			if (patternMatcher.Erfolgreich)
			{
				PatternMatcher patternMatcher2 = new PatternMatcher(category.Pattern.RegExObjekt, eingabe);
				if (patternMatcher2.Erfolgreich)
				{
					this._session.Denkprotokoll.Add(new BotDenkProtokollSchritt(string.Format(ResReader.Reader(this._session.DenkprotokollKultur).GetString("PassendeKategorieGefunden", this._session.DenkprotokollKultur)), BotDenkProtokollSchritt.SchrittArten.PassendeKategorieGefunden, category));
					AntwortSatz antwortSatz = this.InterpretiereCategory_(category, patternMatcher2, patternMatcher, that, vorherigeSRAIs);
					if (antwortSatz.IstNotfallAntwort)
					{
						antwort = antwortSatz;
					}
					else if (category.Pattern.Inhalt == "*" && category.That.Inhalt == "*")
					{
						antwort = new AntwortSatz(antwortSatz.Satz, true);
					}
					else
					{
						antwort = new AntwortSatz(antwortSatz.Satz, false);
					}
					return true;
				}
			}
			antwort = null;
			return false;
		}

		private AntwortSatz InterpretiereCategory_(WissensCategory category, PatternMatcher patternMatch, PatternMatcher thatMatch, string that, ArrayList vorherigeSRAIs)
		{
			StringBuilder stringBuilder = new StringBuilder();
			bool istNotfallAntwort = true;
			XmlNode xmlNode = category.CategoryNode.SelectSingleNode("template");
			if (xmlNode != null)
			{
				foreach (XmlNode childNode in xmlNode.ChildNodes)
				{
					if (childNode is XmlText)
					{
						stringBuilder.Append(childNode.InnerText);
						istNotfallAntwort = false;
					}
					else
					{
						bool flag = default(bool);
						string ausgabeDiesesTags = this.GetAusgabeDiesesTags((long)stringBuilder.Length, childNode, patternMatch, thatMatch, that, out flag, vorherigeSRAIs);
						if (!flag)
						{
							istNotfallAntwort = false;
						}
						stringBuilder.Append(ausgabeDiesesTags);
					}
				}
				return new AntwortSatz(stringBuilder.ToString(), istNotfallAntwort);
			}
			return new AntwortSatz("Error: NO TEMPLATE-NODE FOUND", true);
		}

		private string GetAusgabeDiesesTags(long laengeBisherigeAntwort, XmlNode node, PatternMatcher patternMatch, PatternMatcher thatMatch, string that, out bool enthaeltAusschliesslichNotfallAntwort, ArrayList vorherigeSRAIs)
		{
			enthaeltAusschliesslichNotfallAntwort = false;
			if (node is XmlText)
			{
				return node.InnerText;
			}
			XmlNode xmlNode2;
			switch (node.Name)
			{
			case "bot":
				if (node.Attributes["name"] == null)
				{
					enthaeltAusschliesslichNotfallAntwort = true;
					return "NAMELESS-BOT-PROPERTY";
				}
				return this._botEigenschaften.Lesen(node.Attributes["name"].Value);
			case "condition":
			{
				ConditionStatus conditionStatus = new ConditionStatus();
				conditionStatus.AttributeAusNodeHinzufuegen(node);
				if (!conditionStatus.KannSchonSchliessen || conditionStatus.Erfuellt(this._session))
				{
					if (node.SelectNodes("li").Count == 0)
					{
						StringBuilder stringBuilder = new StringBuilder();
						enthaeltAusschliesslichNotfallAntwort = true;
						foreach (XmlNode childNode in node.ChildNodes)
						{
							bool flag = default(bool);
							stringBuilder.Append(this.GetAusgabeDiesesTags(laengeBisherigeAntwort + stringBuilder.Length, childNode, patternMatch, thatMatch, that, out flag, vorherigeSRAIs));
							if (!flag)
							{
								enthaeltAusschliesslichNotfallAntwort = false;
							}
						}
						return stringBuilder.ToString();
					}
					foreach (XmlNode item in node.SelectNodes("li"))
					{
						bool flag2 = false;
						if (item.Attributes["name"] == null && item.Attributes["value"] == null && item.Attributes["contains"] == null && item.Attributes["exists"] == null)
						{
							flag2 = true;
						}
						else
						{
							ConditionStatus conditionStatus2 = conditionStatus.Clone();
							conditionStatus2.AttributeAusNodeHinzufuegen(item);
							flag2 = (conditionStatus2.KannSchonSchliessen && conditionStatus2.Erfuellt(this._session));
						}
						if (flag2)
						{
							bool flag3 = default(bool);
							string ausgabeDiesesTags = this.GetAusgabeDiesesTags(laengeBisherigeAntwort, item, patternMatch, thatMatch, that, out flag3, vorherigeSRAIs);
							if (flag3)
							{
								enthaeltAusschliesslichNotfallAntwort = true;
							}
							return ausgabeDiesesTags;
						}
					}
				}
				return "";
			}
			case "formal":
			{
				StringBuilder stringBuilder = new StringBuilder();
				enthaeltAusschliesslichNotfallAntwort = true;
				foreach (XmlNode childNode2 in node.ChildNodes)
				{
					bool flag17 = default(bool);
					stringBuilder.Append(this.GetAusgabeDiesesTags(laengeBisherigeAntwort + stringBuilder.Length, childNode2, patternMatch, thatMatch, that, out flag17, vorherigeSRAIs));
					if (!flag17)
					{
						enthaeltAusschliesslichNotfallAntwort = false;
					}
				}
				char c2 = ' ';
				StringBuilder stringBuilder5 = new StringBuilder();
				string text3 = stringBuilder.ToString();
				for (int j = 0; j < text3.Length; j++)
				{
					char c3 = text3[j];
					if (c2 == ' ')
					{
						stringBuilder5.Append(c3.ToString().ToUpper());
					}
					else
					{
						stringBuilder5.Append(c3.ToString().ToLower());
					}
					c2 = c3;
				}
				return stringBuilder5.ToString();
			}
			case "gender":
			{
				StringBuilder stringBuilder = new StringBuilder();
				enthaeltAusschliesslichNotfallAntwort = true;
				foreach (XmlNode childNode3 in node.ChildNodes)
				{
					bool flag9 = default(bool);
					stringBuilder.Append(this.GetAusgabeDiesesTags(laengeBisherigeAntwort + stringBuilder.Length, childNode3, patternMatch, thatMatch, that, out flag9, vorherigeSRAIs));
					if (!flag9)
					{
						enthaeltAusschliesslichNotfallAntwort = false;
					}
				}
				return this._normalisierer.GeschlechtAustauschen(stringBuilder.ToString());
			}
			case "get":
				if (node.Attributes["name"] != null)
				{
					string text = StandardGlobaleEigenschaften.GetStandardConditionContent(node.Attributes["name"].Value);
					if (text == null)
					{
						text = this._session.UserEigenschaften.Lesen(node.Attributes["name"].Value);
					}
					return text;
				}
				return ResReader.Reader(null).GetString("unbekannteEigenschaft");
			case "input":
			{
				int num = 1;
				if (node.Attributes["index"] != null)
				{
					string value2 = node.Attributes["index"].Value;
					if (!string.IsNullOrEmpty(value2))
					{
						value2 = value2.Split(new char[1]
						{
							','
						}, StringSplitOptions.RemoveEmptyEntries)[0];
						if (!int.TryParse(value2, out num))
						{
							num = 1;
						}
					}
				}
				if (num < 0)
				{
					num = 1;
				}
				if (num > this._session.LetzteSchritte.Count - 1)
				{
					return "";
				}
				string userEingabe = this._session.LetzteSchritte[this._session.LetzteSchritte.Count - num].UserEingabe;
				if (userEingabe == "TARGET BOTSTART")
				{
					return "";
				}
				return userEingabe;
			}
			case "li":
			{
				StringBuilder stringBuilder = new StringBuilder();
				enthaeltAusschliesslichNotfallAntwort = true;
				foreach (XmlNode childNode4 in node.ChildNodes)
				{
					bool flag14 = default(bool);
					stringBuilder.Append(this.GetAusgabeDiesesTags(laengeBisherigeAntwort + stringBuilder.Length, childNode4, patternMatch, thatMatch, that, out flag14, vorherigeSRAIs));
					if (!flag14)
					{
						enthaeltAusschliesslichNotfallAntwort = false;
					}
				}
				return stringBuilder.ToString();
			}
			case "lowercase":
			{
				StringBuilder stringBuilder = new StringBuilder();
				enthaeltAusschliesslichNotfallAntwort = true;
				foreach (XmlNode childNode5 in node.ChildNodes)
				{
					bool flag10 = default(bool);
					stringBuilder.Append(this.GetAusgabeDiesesTags(laengeBisherigeAntwort + stringBuilder.Length, childNode5, patternMatch, thatMatch, that, out flag10, vorherigeSRAIs));
					if (!flag10)
					{
						enthaeltAusschliesslichNotfallAntwort = false;
					}
				}
				return stringBuilder.ToString().ToLower();
			}
			case "person2":
			{
				StringBuilder stringBuilder = new StringBuilder();
				enthaeltAusschliesslichNotfallAntwort = true;
				foreach (XmlNode childNode6 in node.ChildNodes)
				{
					bool flag5 = default(bool);
					stringBuilder.Append(this.GetAusgabeDiesesTags(laengeBisherigeAntwort + stringBuilder.Length, childNode6, patternMatch, thatMatch, that, out flag5, vorherigeSRAIs));
					if (!flag5)
					{
						enthaeltAusschliesslichNotfallAntwort = false;
					}
				}
				return this._normalisierer.Person2Austauschen(stringBuilder.ToString());
			}
			case "person":
			{
				StringBuilder stringBuilder = new StringBuilder();
				enthaeltAusschliesslichNotfallAntwort = true;
				foreach (XmlNode childNode7 in node.ChildNodes)
				{
					bool flag8 = default(bool);
					stringBuilder.Append(this.GetAusgabeDiesesTags(laengeBisherigeAntwort + stringBuilder.Length, childNode7, patternMatch, thatMatch, that, out flag8, vorherigeSRAIs));
					if (!flag8)
					{
						enthaeltAusschliesslichNotfallAntwort = false;
					}
				}
				return this._normalisierer.PersonAustauschen(stringBuilder.ToString());
			}
			case "random":
			{
				if (node.SelectNodes("li").Count == 0)
				{
					enthaeltAusschliesslichNotfallAntwort = true;
					return "NO LI-TAGS IN RANDOM-TAG";
				}
				XmlNode node11 = this.WaehleZufaelligenNode(node.SelectNodes("li"));
				bool flag13 = default(bool);
				string ausgabeDiesesTags3 = this.GetAusgabeDiesesTags(laengeBisherigeAntwort, node11, patternMatch, thatMatch, that, out flag13, vorherigeSRAIs);
				if (flag13)
				{
					enthaeltAusschliesslichNotfallAntwort = true;
				}
				return ausgabeDiesesTags3;
			}
			case "script":
			{
				if (node.Attributes["language"] == null)
				{
					return node.OuterXml;
				}
				string value4 = node.Attributes["language"].Value;
				if (!(value4 == "gaitoscript"))
				{
					if (value4 == "javascript")
					{
						goto IL_0c19;
					}
					goto IL_0c19;
				}
				GaitoScriptInterpreter gaitoScriptInterpreter = new GaitoScriptInterpreter(this._session);
				gaitoScriptInterpreter.Execute(node.InnerText);
				if (gaitoScriptInterpreter.Fehler == null)
				{
					if (gaitoScriptInterpreter.Ausgabe == null)
					{
						return "";
					}
					return gaitoScriptInterpreter.Ausgabe;
				}
				return string.Format("GaitoScript-Error: {0}", gaitoScriptInterpreter.Fehler);
			}
			case "sentence":
			{
				StringBuilder stringBuilder = new StringBuilder();
				enthaeltAusschliesslichNotfallAntwort = true;
				foreach (XmlNode childNode8 in node.ChildNodes)
				{
					bool flag15 = default(bool);
					stringBuilder.Append(this.GetAusgabeDiesesTags(laengeBisherigeAntwort + stringBuilder.Length, childNode8, patternMatch, thatMatch, that, out flag15, vorherigeSRAIs));
					if (!flag15)
					{
						enthaeltAusschliesslichNotfallAntwort = false;
					}
				}
				bool flag16 = true;
				StringBuilder stringBuilder4 = new StringBuilder();
				string text2 = stringBuilder.ToString();
				for (int i = 0; i < text2.Length; i++)
				{
					char c = text2[i];
					if (flag16)
					{
						stringBuilder4.Append(c.ToString().ToUpper());
						if (c != ' ')
						{
							flag16 = false;
						}
					}
					else
					{
						stringBuilder4.Append(c);
					}
					char c2 = c;
				}
				return stringBuilder4.ToString();
			}
			case "set":
			{
				StringBuilder stringBuilder = new StringBuilder();
				enthaeltAusschliesslichNotfallAntwort = true;
				foreach (XmlNode childNode9 in node.ChildNodes)
				{
					bool flag6 = default(bool);
					stringBuilder.Append(this.GetAusgabeDiesesTags(laengeBisherigeAntwort + stringBuilder.Length, childNode9, patternMatch, thatMatch, that, out flag6, vorherigeSRAIs));
					if (!flag6)
					{
						enthaeltAusschliesslichNotfallAntwort = false;
					}
				}
				if (node.Attributes["name"] != null)
				{
					string value = node.Attributes["name"].Value;
					string a = value.Trim().ToLower();
					if (a == "topic")
					{
						this._session.SetzeAktuellesThema(stringBuilder.ToString());
					}
					this._session.UserEigenschaften.Setzen(value, stringBuilder.ToString());
				}
				return stringBuilder.ToString();
			}
			case "srai":
			{
				vorherigeSRAIs.Add(node);
				StringBuilder stringBuilder2 = new StringBuilder();
				foreach (XmlNode childNode10 in node.ChildNodes)
				{
					bool flag12 = default(bool);
					stringBuilder2.Append(this.GetAusgabeDiesesTags(0L, childNode10, patternMatch, thatMatch, that, out flag12, vorherigeSRAIs));
				}
				StringBuilder stringBuilder3 = new StringBuilder();
				if (laengeBisherigeAntwort > 0)
				{
					stringBuilder3.Append("|");
					that = "EMPTYTHAT";
				}
				AntwortSatz qualifizierteAntwort_ = this.GetQualifizierteAntwort_(stringBuilder2.ToString(), that, vorherigeSRAIs);
				if (qualifizierteAntwort_ == null)
				{
					enthaeltAusschliesslichNotfallAntwort = true;
				}
				else
				{
					stringBuilder3.Append(qualifizierteAntwort_.Satz);
					if (qualifizierteAntwort_.IstNotfallAntwort)
					{
						enthaeltAusschliesslichNotfallAntwort = true;
					}
					else
					{
						enthaeltAusschliesslichNotfallAntwort = false;
					}
				}
				return stringBuilder3.ToString();
			}
			case "star":
			{
				int starNr2 = 1;
				if (node.Attributes["index"] != null && !int.TryParse(node.Attributes["index"].Value, out starNr2))
				{
					starNr2 = 1;
				}
				return patternMatch.GetStarInhalt(starNr2);
			}
			case "that":
			{
				int num2 = 1;
				if (node.Attributes["index"] != null)
				{
					string value3 = node.Attributes["index"].Value;
					if (!string.IsNullOrEmpty(value3))
					{
						value3 = value3.Split(new char[1]
						{
							','
						}, StringSplitOptions.RemoveEmptyEntries)[0];
						if (!int.TryParse(value3, out num2))
						{
							num2 = 1;
						}
					}
				}
				if (num2 < 0)
				{
					num2 = 1;
				}
				if (num2 == 1)
				{
					return that;
				}
				if (num2 > this._session.LetzteSchritte.Count - 1)
				{
					return "";
				}
				return this._session.LetzteSchritte[this._session.LetzteSchritte.Count - num2].BotAusgabe;
			}
			case "thatstar":
			{
				int starNr = 1;
				if (node.Attributes["index"] != null && !int.TryParse(node.Attributes["index"].Value, out starNr))
				{
					starNr = 1;
				}
				return thatMatch.GetStarInhalt(starNr);
			}
			case "think":
				foreach (XmlNode childNode11 in node.ChildNodes)
				{
					bool flag7 = default(bool);
					this.GetAusgabeDiesesTags(laengeBisherigeAntwort, childNode11, patternMatch, thatMatch, that, out flag7, vorherigeSRAIs);
				}
				return "";
			case "uppercase":
			{
				StringBuilder stringBuilder = new StringBuilder();
				enthaeltAusschliesslichNotfallAntwort = true;
				foreach (XmlNode childNode12 in node.ChildNodes)
				{
					bool flag4 = default(bool);
					stringBuilder.Append(this.GetAusgabeDiesesTags(laengeBisherigeAntwort + stringBuilder.Length, childNode12, patternMatch, thatMatch, that, out flag4, vorherigeSRAIs));
					if (!flag4)
					{
						enthaeltAusschliesslichNotfallAntwort = false;
					}
				}
				return stringBuilder.ToString().ToUpper();
			}
			default:
				{
					return node.OuterXml;
				}
				IL_0c19:
				xmlNode2 = node.Clone();
				enthaeltAusschliesslichNotfallAntwort = true;
				foreach (XmlNode childNode13 in xmlNode2.ChildNodes)
				{
					if (childNode13.Name == "get")
					{
						bool flag11 = default(bool);
						string ausgabeDiesesTags2 = this.GetAusgabeDiesesTags(laengeBisherigeAntwort, childNode13, patternMatch, thatMatch, that, out flag11, vorherigeSRAIs);
						if (!flag11)
						{
							enthaeltAusschliesslichNotfallAntwort = false;
						}
						XmlText newChild = childNode13.OwnerDocument.CreateTextNode(ausgabeDiesesTags2);
						xmlNode2.InsertBefore(newChild, childNode13);
						xmlNode2.RemoveChild(childNode13);
					}
				}
				return xmlNode2.OuterXml;
			}
		}

		private XmlNode WaehleZufaelligenNode(XmlNodeList LiTags)
		{
			ArrayList arrayList = new ArrayList();
			int num = 0;
			int num2 = -1;
			foreach (XmlNode LiTag in LiTags)
			{
				num = (this._session.RandomHistory.Contains(LiTag) ? (this._session.RandomHistory.IndexOf(LiTag) + 1) : 0);
				if (num2 == -1 || num < num2)
				{
					arrayList = new ArrayList();
					num2 = num;
				}
				if (num == num2)
				{
					arrayList.Add(LiTag);
				}
			}
			Random random = new Random(this._session.ZufallsSeed);
			int count = arrayList.Count;
			int index = random.Next(0, count);
			XmlNode xmlNode2 = (XmlNode)arrayList[index];
			this._session.RandomHistory.Remove(xmlNode2);
			this._session.RandomHistory.Add(xmlNode2);
			return xmlNode2;
		}
	}
}
