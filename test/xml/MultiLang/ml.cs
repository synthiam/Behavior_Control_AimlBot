using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Threading;

namespace MultiLang
{
	internal class ml
	{
		private static string RootNamespace;

		private static ResourceManager ResMgr;

		public static string[] SupportedCultures;

		static ml()
		{
			ml.RootNamespace = "de.springwald.xml";
			ml.SupportedCultures = new string[1]
			{
				"de"
			};
			ml.ResMgr = new ResourceManager(ml.RootNamespace + ".MultiLang", Assembly.GetExecutingAssembly());
		}

		public static void ml_UseCulture(CultureInfo ci)
		{
			Thread.CurrentThread.CurrentUICulture = ci;
		}

		public static string ml_string(int StringID, string Text)
		{
			return ml.ml_resource(StringID);
		}

		public static string ml_resource(int StringID)
		{
			return ml.ResMgr.GetString("_" + StringID.ToString());
		}
	}
}
