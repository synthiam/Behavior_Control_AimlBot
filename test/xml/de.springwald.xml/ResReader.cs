using de.springwald.toolbox;
using System.Reflection;

namespace de.springwald.xml
{
	public class ResReader
	{
		private static string _ressourcenDatei = "de.springwald.xml.Resources.xml";

		private static RessourcenReader _reader = new RessourcenReader(ResReader._ressourcenDatei, Assembly.GetExecutingAssembly());

		public static RessourcenReader Reader
		{
			get
			{
				return ResReader._reader;
			}
		}
	}
}
