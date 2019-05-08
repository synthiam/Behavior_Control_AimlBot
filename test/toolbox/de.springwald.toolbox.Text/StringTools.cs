using System;
using System.Globalization;
using System.Text;
using System.Web;

namespace de.springwald.toolbox.Text
{
	public class StringTools
	{
		public static void DoppelteLeerzeichenRaus_(StringBuilder eingabe)
		{
			long num = -1L;
			while (num != eingabe.Length)
			{
				num = eingabe.Length;
				eingabe.Replace("  ", " ");
			}
		}

		public static string StringMax(string eingabe, int maxlaenge, string abkuerzungszeichen)
		{
			return (string.IsNullOrEmpty(eingabe) || eingabe.Length <= maxlaenge) ? eingabe : (eingabe.Substring(0, maxlaenge) + abkuerzungszeichen);
		}

		public static string DoppelteLeerzeichenRaus_(string eingabe)
		{
			StringBuilder stringBuilder = new StringBuilder(eingabe);
			long num = -1L;
			while (num != stringBuilder.Length)
			{
				num = stringBuilder.Length;
				stringBuilder.Replace("  ", " ");
			}
			return stringBuilder.ToString();
		}

		public static bool IsInteger(string ausdruck)
		{
			return StringTools.IsNumeric(ausdruck, NumberStyles.Integer);
		}

		public static bool IsNumeric(string ausdruck, NumberStyles artDerZahl)
		{
			double num = default(double);
			return double.TryParse(ausdruck, artDerZahl, (IFormatProvider)NumberFormatInfo.InvariantInfo, out num);
		}

		[Obsolete("Verwende EncodingTools.UrlISO885915Encode")]
		public static string URLEncode(string rohstring)
		{
			return EncodingTools.UrlISO885915Encode(rohstring);
		}

		public static string HTMLEncode(string rohstring)
		{
			return HttpUtility.HtmlEncode(rohstring);
		}

		public static string HTMLDecode(string rohString)
		{
			return HttpUtility.HtmlDecode(rohString);
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
	}
}
