using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace de.springwald.toolbox
{
	public abstract class ToolboxStrings
	{
		public static bool IsEmail(string text)
		{
			return Regex.IsMatch(text, "^[a-z0-9|ä|ü|ö|\\-|\\.|_]+@[a-z0-9|ä|ü|ö|\\-|\\.|_]+\\.[a-z]{2,4}$", RegexOptions.IgnoreCase);
		}

		public static bool IsInteger(string ausdruck)
		{
			return ToolboxStrings.IsNumeric(ausdruck, NumberStyles.Integer);
		}

		public static bool IsNumeric(string ausdruck, NumberStyles artDerZahl)
		{
			double num = default(double);
			return double.TryParse(ausdruck, artDerZahl, (IFormatProvider)NumberFormatInfo.InvariantInfo, out num);
		}

		public static string ReplaceEx(string original, string pattern, string replacement)
		{
			int num2;
			int num;
			int num3 = num2 = (num = 0);
			string text = original.ToUpper();
			string value = pattern.ToUpper();
			int val = original.Length / pattern.Length * (replacement.Length - pattern.Length);
			char[] array = new char[original.Length + Math.Max(0, val)];
			while ((num = text.IndexOf(value, num2)) != -1)
			{
				for (int i = num2; i < num; i++)
				{
					array[num3++] = original[i];
				}
				for (int j = 0; j < replacement.Length; j++)
				{
					array[num3++] = replacement[j];
				}
				num2 = num + pattern.Length;
			}
			if (num2 == 0)
			{
				return original;
			}
			for (int k = num2; k < original.Length; k++)
			{
				array[num3++] = original[k];
			}
			return new string(array, 0, num3);
		}

		public static void String2File(string inhalt, string dateiname)
		{
			if (File.Exists(dateiname))
			{
				try
				{
					File.Delete(dateiname);
				}
				catch (Exception arg)
				{
					throw new ApplicationException(string.Format("Konnte ZielDatei '{0}' nicht löschen: \n\n{1}", dateiname, arg));
				}
			}
			StreamWriter streamWriter;
			try
			{
				streamWriter = new StreamWriter(dateiname, false, Encoding.GetEncoding("ISO-8859-15"));
				streamWriter.AutoFlush = true;
			}
			catch (Exception arg2)
			{
				throw new ApplicationException(string.Format("Konnte ZielDatei '{0}' nicht anlegen: \n\n{1}", dateiname, arg2));
			}
			streamWriter.Write(inhalt);
			streamWriter.Close();
		}

		public static string File2String(string dateiname)
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
				throw new ApplicationException(string.Format("Konnte Datei '{0}' nicht einlesen:\n{1}", dateiname, ex.Message));
			}
			return text;
		}

		public static string DoppelteLeerzeichenRaus(string eingabe)
		{
			string a = "";
			string text = eingabe;
			while (a != text)
			{
				a = text;
				text = text.Replace("  ", " ");
			}
			return text;
		}

		public static string VerlaengereStringLinks(string eingabeString, int size, char fill)
		{
			while (eingabeString.Length < size)
			{
				eingabeString = fill.ToString() + eingabeString;
			}
			return eingabeString;
		}

		public static string UmlauteAussschreiben(string eingabe)
		{
			StringBuilder stringBuilder = new StringBuilder(eingabe);
			stringBuilder.Replace("Ä", "Ae");
			stringBuilder.Replace("Ü", "Ue");
			stringBuilder.Replace("Ö", "Oe");
			stringBuilder.Replace("ä", "ae");
			stringBuilder.Replace("ö", "oe");
			stringBuilder.Replace("ü", "ue");
			stringBuilder.Replace("ß", "ss");
			return stringBuilder.ToString();
		}

		public static string AsciiUmlaute2HTML(string eingabe)
		{
			StringBuilder stringBuilder = new StringBuilder(eingabe);
			stringBuilder.Replace("ä", "&auml;");
			stringBuilder.Replace("ö", "&ouml;");
			stringBuilder.Replace("ü", "&uuml;");
			stringBuilder.Replace("Ä", "&Auml;");
			stringBuilder.Replace("Ö", "&Ouml;");
			stringBuilder.Replace("Ü", "&Uuml;");
			stringBuilder.Replace("ß", "&szlig;");
			return stringBuilder.ToString();
		}

		public static string HTML2ASCII(string eingabe)
		{
			StringBuilder stringBuilder = new StringBuilder(eingabe);
			stringBuilder.Replace("/t", " ");
			stringBuilder.Replace("/r/n", " ");
			stringBuilder.Replace("/n", " ");
			stringBuilder = new StringBuilder(ToolboxStrings.DoppelteLeerzeichenRaus(stringBuilder.ToString()));
			stringBuilder.Replace("&auml;", "ä");
			stringBuilder.Replace("&ouml;", "ö");
			stringBuilder.Replace("&uuml;", "ü");
			stringBuilder.Replace("&Auml;", "Ä");
			stringBuilder.Replace("&Ouml;", "Ö");
			stringBuilder.Replace("&Uuml;", "Ü");
			stringBuilder.Replace("&szlig;", "ß");
			stringBuilder.Replace("&amp;", "&");
			stringBuilder.Replace("&quot;", "\"");
			stringBuilder.Replace("&#128;", "€");
			stringBuilder.Replace("&#132;", "\"");
			stringBuilder.Replace("&#147;", "\"");
			stringBuilder.Replace("&nbsp;", " ");
			return stringBuilder.ToString();
		}
	}
}
