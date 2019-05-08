using de.springwald.toolbox;
using System;
using System.Collections;
using System.Collections.Generic;

namespace de.springwald.gaitobot2
{
	public class Normalisierung
	{
		private const string Leer = "";

		public StartupInfos StartupInfos
		{
			get;
			private set;
		}

		public Normalisierung(StartupInfos startupInfos)
		{
			if (startupInfos == null)
			{
				throw new ArgumentNullException("startupInfos");
			}
			this.StartupInfos = startupInfos;
		}

		public string StandardErsetzungenDurchfuehren(string eingabe)
		{
			string text = eingabe.Replace("Ä", "Ae");
			text = text.Replace("ä", "ae");
			text = text.Replace("Ö", "Oe");
			text = text.Replace("ö", "oe");
			text = text.Replace("Ü", "Ue");
			text = text.Replace("ü", "ue");
			text = text.Replace("ß", "ss");
			return this.ErsetzungenDurchfuehren(this.StartupInfos.Ersetzungen, text, false);
		}

		public string PersonAustauschen(string eingabe)
		{
			return this.ErsetzungenDurchfuehren(this.StartupInfos.PersonAustauscher, eingabe, true);
		}

		public string Person2Austauschen(string eingabe)
		{
			return this.ErsetzungenDurchfuehren(this.StartupInfos.Person2Austauscher, eingabe, true);
		}

		public string GeschlechtAustauschen(string eingabe)
		{
			return this.ErsetzungenDurchfuehren(this.StartupInfos.GeschlechtsAustauscher, eingabe, true);
		}

		public static string EingabePatternOptimieren(string rohEingabe, bool sternErlaubt)
		{
			if (rohEingabe.Length == 0)
			{
				return "";
			}
			char[] array = rohEingabe.ToCharArray();
			ArrayList arrayList = new ArrayList();
			for (int i = 0; i < array.Length; i++)
			{
				if ((array[i] > '@' & array[i] < '[') || (array[i] > '`' & array[i] < '{') || (array[i] > '/' & array[i] < ':') || array[i] == ' ')
				{
					arrayList.Add(array[i]);
				}
				else if (sternErlaubt && array[i] == '*')
				{
					arrayList.Add(array[i]);
				}
			}
			char[] array2 = new char[arrayList.Count];
			for (int j = 0; j < arrayList.Count; j++)
			{
				array2[j] = (char)arrayList[j];
			}
			string eingabe = new string(array2);
			eingabe = ToolboxStrings.DoppelteLeerzeichenRaus(eingabe);
			return eingabe.Trim();
		}

		private string ErsetzungenDurchfuehren(List<DictionaryEntry> ersetzungen, string rohEingabe, bool kreisUbersetzungenVermeiden)
		{
			Hashtable hashtable = null;
			if (kreisUbersetzungenVermeiden)
			{
				hashtable = new Hashtable();
			}
			if (rohEingabe.Length < 1)
			{
				return "";
			}
			if (ersetzungen.Count < 1)
			{
				return rohEingabe;
			}
			string text = " " + rohEingabe + " ";
			foreach (DictionaryEntry item in ersetzungen)
			{
				string text2 = (string)item.Key;
				if (!kreisUbersetzungenVermeiden || !hashtable.Contains(text2.ToLower()))
				{
					string text3 = (string)item.Value;
					if (kreisUbersetzungenVermeiden)
					{
						string a = text;
						text = ToolboxStrings.ReplaceEx(text, text2, text3);
						if (a != text)
						{
							hashtable.Add(text3.ToLower(), null);
						}
					}
					else
					{
						text = ToolboxStrings.ReplaceEx(text, text2, text3);
					}
				}
			}
			return text.Trim();
		}
	}
}
