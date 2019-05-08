using System;
using System.Text;
using System.Web;

namespace de.springwald.toolbox.Text
{
	public abstract class EncodingTools
	{
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
			stringBuilder = new StringBuilder(StringTools.DoppelteLeerzeichenRaus_(stringBuilder.ToString()));
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

		public static string EncodingSonderzeichenToAscii(string inhalt)
		{
			StringBuilder stringBuilder = new StringBuilder(inhalt);
			EncodingTools.EncodingSonderzeichenToMinimumAscii(stringBuilder);
			return stringBuilder.ToString();
		}

		public static void EncodingSonderzeichenToMinimumAscii(StringBuilder inhalt)
		{
			inhalt.Replace("„", "\"");
			inhalt.Replace("”", "\"");
			inhalt.Replace("&#8222;", "\"");
			inhalt.Replace("&#8221;", "\"");
			inhalt.Replace("&#8211;", "-");
			inhalt.Replace("&#8364;", "€");
		}

		public static byte[] GetBytesIso_8859_1(string umwandlungsText)
		{
			return Encoding.GetEncoding("iso-8859-1").GetBytes(umwandlungsText);
		}

		public static string GetStringIso_8859_1(byte[] umwandlungsFeld)
		{
			return Encoding.GetEncoding("iso-8859-1").GetString(umwandlungsFeld);
		}

		public static byte[] ConvertFromHHText(string text)
		{
			byte[] array = new byte[text.Length / 2];
			for (int i = 0; i < text.Length / 2; i++)
			{
				array[i] = Convert.ToByte(text.Substring(2 * i, 2), 16);
			}
			return array;
		}

		public static string ConvertToHHText(byte[] bytes)
		{
			string text = "";
			foreach (byte value in bytes)
			{
				text += Convert.ToString(value, 16).PadLeft(2, "0"[0]);
			}
			return text;
		}

		public static string UrlISO88591Encode(string wert)
		{
			return HttpUtility.UrlEncode(wert, Encoding.GetEncoding("iso-8859-1"));
		}

		public static string UrlISO885915Encode(string wert)
		{
			return HttpUtility.UrlEncode(wert, Encoding.GetEncoding("iso-8859-15"));
		}

		public static string UrlISO88591Decode(string wert)
		{
			return HttpUtility.UrlDecode(wert, Encoding.GetEncoding("iso-8859-1"));
		}

		public static string UrlISO885915Decode(string wert)
		{
			return HttpUtility.UrlDecode(wert, Encoding.GetEncoding("iso-8859-15"));
		}
	}
}
