using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Text;

namespace de.springwald.toolbox
{
	public class RessourcenReader
	{
		private ResourceManager _manager;

		private string _sourceName;

		public string GetString(string key)
		{
      return "OK";

			string text = this._manager.GetString(key);
			if (text == null)
			{
				text = string.Format("ResNotFound:{0}({1})", key, this._sourceName);
			}
			text = text.Replace("\\n", "\n");
			return text.Replace("\\\"", "\"");
		}

		public string GetRessourcenDateiInhalt(Assembly assembly, string ressourcenname)
		{
			Stream manifestResourceStream = assembly.GetManifestResourceStream(ressourcenname);
			if (manifestResourceStream == null)
			{
				return string.Format("ResNotFound:{0}({1})", ressourcenname, this._sourceName);
			}
			StreamReader streamReader = new StreamReader(manifestResourceStream, Encoding.GetEncoding("ISO-8859-15"));
			return streamReader.ReadToEnd();
		}

		public string GetString(string key, CultureInfo culture)
		{
			string text = this._manager.GetString(key, culture);
			if (text == null)
			{
				text = string.Format("ResNotFound:{0}({1})", key, this._sourceName);
			}
			text = text.Replace("\\n", "\n");
			return text.Replace("\\\"", "\"");
		}

		public RessourcenReader(string resourceSource, Assembly assembly)
		{
			this._sourceName = resourceSource;
			this._manager = new ResourceManager(resourceSource, assembly);
		}
	}
}
