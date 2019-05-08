using de.springwald.xml.dtd;
using System.IO;
using System.Reflection;

namespace GaitoBotEditorCore
{
	internal class StartupDateiDtd
	{
		private static string _dtdInhalt;

		private static string DTDInhalt
		{
			get
			{
				if (StartupDateiDtd._dtdInhalt == null)
				{
					Assembly executingAssembly = Assembly.GetExecutingAssembly();
					Stream manifestResourceStream = executingAssembly.GetManifestResourceStream("GaitoBotEditorCore.Resources.startup.dtd");
					StreamReader streamReader = new StreamReader(manifestResourceStream);
					StartupDateiDtd._dtdInhalt = streamReader.ReadToEnd();
					streamReader.Close();
				}
				return StartupDateiDtd._dtdInhalt;
			}
		}

		public static DTD GetStartupDtd()
		{
			DTDReaderDTD dTDReaderDTD = new DTDReaderDTD();
			return dTDReaderDTD.GetDTDFromString(StartupDateiDtd.DTDInhalt);
		}
	}
}
