using de.springwald.toolbox;
using System.Reflection;

namespace GaitoBotEditor
{
	public class ResReader
	{
		private static string _ressourcenDatei = "GaitoBotEditor.Resources.GaitoBotEditor";

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
