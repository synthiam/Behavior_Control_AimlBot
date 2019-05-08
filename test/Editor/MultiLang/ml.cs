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
			MultiLang.ml.RootNamespace = "GaitoBotEditor";
			MultiLang.ml.SupportedCultures = new string[2]
			{
				"de",
				"en"
			};
			MultiLang.ml.ResMgr = new ResourceManager(MultiLang.ml.RootNamespace + ".MultiLang", Assembly.GetExecutingAssembly());
		}

		public static void ml_UseCulture(CultureInfo ci)
		{
			Thread.CurrentThread.CurrentUICulture = ci;
		}

		public static string ml_string(int StringID, string Text)
		{
			return MultiLang.ml.ml_resource(StringID);
		}

		public static string ml_resource(int StringID)
		{
			return MultiLang.ml.ResMgr.GetString("_" + StringID.ToString());
		}
	}
}
