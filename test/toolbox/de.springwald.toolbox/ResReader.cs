using System.Reflection;

namespace de.springwald.toolbox
{
	public abstract class ResReader
	{
		private static string _ressourcenDatei = "de.springwald.toolbox.Resources.xml";

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
