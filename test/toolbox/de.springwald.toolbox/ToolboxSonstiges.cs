using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace de.springwald.toolbox
{
	public abstract class ToolboxSonstiges
	{
		private static bool _istEntwicklungsmodus;

		private static bool _istEntwicklungsmodusGecheckt = false;

		public static bool IstEntwicklungsmodus
		{
			get
			{
				return System.Diagnostics.Debugger.IsAttached;
			}
		}

		public static int Jahr4stelligMachen(int jahr)
		{
			if (jahr < 20)
			{
				return jahr + 2000;
			}
			if (jahr <= 99)
			{
				return 1900 + jahr;
			}
			return jahr;
		}

		public static DateTime Int2Datum(int datum)
		{
			if (datum < 1)
			{
				return default(DateTime);
			}
			int num = datum / 10000;
			datum -= num * 10000;
			int num2 = datum / 100;
			datum -= num2 * 100;
			int day = datum;
			return new DateTime(num, num2, day);
		}

		public static int Datum2Int(DateTime datum)
		{
			return datum.Year * 10000 + datum.Month * 100 + datum.Day;
		}

		public static void HTMLSeiteAufrufen(string url)
		{
			try
			{
				Process.Start(url);
			}
			catch (Exception ex)
			{
				MessageBox.Show(string.Format(ResReader.Reader.GetString("KannHTMLSeiteNichtAufrufen"), ex.Message));
			}
		}
	}
}
