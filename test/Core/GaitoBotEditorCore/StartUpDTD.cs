using de.springwald.xml.dtd;
using System.IO;
using System.Reflection;

namespace GaitoBotEditorCore
{
	public abstract class StartUpDTD
	{
		private static string _dtdInhalt;

		private static string DTDInhalt
		{
			get
			{
				if (StartUpDTD._dtdInhalt == null)
				{
					Assembly executingAssembly = Assembly.GetExecutingAssembly();
					Stream manifestResourceStream = executingAssembly.GetManifestResourceStream("GaitoBotEditorCore.Resources.startup.dtd");
					StreamReader streamReader = new StreamReader(manifestResourceStream);
					StartUpDTD._dtdInhalt = streamReader.ReadToEnd();
					streamReader.Close();
				}
				return StartUpDTD._dtdInhalt;
			}
		}

		public static DTD GetStartUpDTD()
		{
			DTDReaderDTD dTDReaderDTD = new DTDReaderDTD();
			return dTDReaderDTD.GetDTDFromString(StartUpDTD.DTDInhalt);
		}
	}
}
