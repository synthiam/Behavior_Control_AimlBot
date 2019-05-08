using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace de.springwald.gaitobot2
{
	public class MatchString
	{
		private readonly Regex _regExObjekt;

		private readonly string _inhalt;

		public string Inhalt
		{
			get
			{
				return this._inhalt;
			}
		}

		public Regex RegExObjekt
		{
			get
			{
				return this._regExObjekt;
			}
		}

		public MatchString(string inhalt)
		{
			this._inhalt = inhalt;
			this._regExObjekt = this.GetRegExObjekt(inhalt);
		}

		private Regex GetRegExObjekt(string inhalt)
		{
			string text;
			StringBuilder stringBuilder;
			if (inhalt == "*")
			{
				text = "\\A(?<star1>.+?)\\z";
			}
			else
			{
				int num = 0;
				text = inhalt;
				stringBuilder = new StringBuilder();
				stringBuilder.Append("\\A");
				while (text.IndexOf("*") != -1)
				{
					num++;
					int startIndex = text.IndexOf("*");
					text = text.Remove(startIndex, 1);
					text = text.Insert(startIndex, string.Format("(?<star{0}>.+?)", num));
				}
				stringBuilder.Append(text);
				stringBuilder.Append("\\z");
				text = stringBuilder.ToString();
			}
			int num2 = 0;
			stringBuilder = new StringBuilder();
			string[] array = text.Split('_');
			foreach (string text2 in array)
			{
				if (stringBuilder.Length == 0)
				{
					stringBuilder.Append(text2);
				}
				else
				{
					num2++;
					stringBuilder.AppendFormat("(?<slash{0}>.+?){1}", num2, text2);
				}
			}
			return new Regex(stringBuilder.ToString(), RegexOptions.IgnoreCase);
		}

		public static string GetInhaltFromXmlNode(XmlNode inhaltNode, Normalisierung normalisierung, GaitoBotEigenschaften botEigenschaften)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat(" ");
			if (!inhaltNode.HasChildNodes)
			{
				stringBuilder.Append(inhaltNode.Value);
			}
			else
			{
				foreach (XmlNode childNode in inhaltNode.ChildNodes)
				{
					if (childNode is XmlText)
					{
						stringBuilder.Append(childNode.InnerText);
						continue;
					}
					if (childNode is XmlElement)
					{
						if (childNode.Name == "bot")
						{
							if (childNode.Attributes["name"] != null)
							{
								string value = childNode.Attributes["name"].Value;
								stringBuilder.Append(botEigenschaften.Lesen(value));
							}
							else
							{
								stringBuilder.Append("BOT_UNKNOWN");
							}
						}
						else
						{
							stringBuilder.AppendFormat("[[Unbekanntes Tag '{0}' in Pattern '{1}']]", childNode.OuterXml, inhaltNode.OuterXml);
						}
						continue;
					}
					throw new ApplicationException(string.Format("Unbekannter XMLTyp '{0}' in Pattern '{1}'", childNode.OuterXml, inhaltNode.OuterXml));
				}
			}
			stringBuilder.AppendFormat(" ");
			string eingabe = stringBuilder.ToString();
			eingabe = normalisierung.StandardErsetzungenDurchfuehren(eingabe);
			string[] array = eingabe.Split(" \r\n\t".ToCharArray());
			stringBuilder = new StringBuilder();
			string[] array2 = array;
			foreach (string text in array2)
			{
				string text2 = (!(text == "*") && !(text == "_")) ? Normalisierung.EingabePatternOptimieren(text, false) : text;
				if (stringBuilder.Length == 0)
				{
					stringBuilder.Append(text2);
				}
				else
				{
					stringBuilder.AppendFormat(" {0}", text2);
				}
			}
			return stringBuilder.ToString();
		}
	}
}
