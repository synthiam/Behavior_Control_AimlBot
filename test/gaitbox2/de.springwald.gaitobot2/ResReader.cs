using de.springwald.toolbox;
using System.Globalization;
using System.Reflection;

namespace de.springwald.gaitobot2
{
	public static class ResReader
	{
		private const string RessourcenDateiDe = "de.springwald.gaitobot2.Resources.gaitobot_de";

		private const string RessourcenDatei = "de.springwald.gaitobot2.Resources.gaitobot";

		private static readonly RessourcenReader ReaderDe = new RessourcenReader("de.springwald.gaitobot2.Resources.gaitobot_de", Assembly.GetExecutingAssembly());

		private static readonly RessourcenReader ReaderNeutral = new RessourcenReader("de.springwald.gaitobot2.Resources.gaitobot", Assembly.GetExecutingAssembly());

		public static RessourcenReader Reader(CultureInfo culture)
		{
			if (culture != null)
			{
				string a = culture.ToString().ToLower();
				if (a == "de")
				{
					return ResReader.ReaderDe;
				}
			}
			return ResReader.ReaderNeutral;
		}
	}
}
