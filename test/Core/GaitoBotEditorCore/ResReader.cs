using de.springwald.toolbox;
using System.Reflection;

namespace GaitoBotEditorCore
{
	public class ResReader
	{
		private static string _ressourcenDatei = "GaitoBotEditorCore.Resources.GaitoBotEditorCore";

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
