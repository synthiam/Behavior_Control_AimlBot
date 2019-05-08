using de.springwald.xml.dtd;
using System.IO;
using System.Reflection;

namespace GaitoBotEditorCore
{
	public abstract class AIMLDTD
	{
		private static string _dtdInhalt;

		private static string DTDInhalt
		{
			get
			{
				if (AIMLDTD._dtdInhalt == null)
				{
					Assembly executingAssembly = Assembly.GetExecutingAssembly();
					Stream manifestResourceStream = executingAssembly.GetManifestResourceStream("GaitoBotEditorCore.Resources.aiml.dtd");
					StreamReader streamReader = new StreamReader(manifestResourceStream);
					AIMLDTD._dtdInhalt = streamReader.ReadToEnd();
					streamReader.Close();
				}
				return AIMLDTD._dtdInhalt;
			}
		}

		public static DTD GetAIMLDTD()
		{
			DTDReaderDTD dTDReaderDTD = new DTDReaderDTD();
			return dTDReaderDTD.GetDTDFromString(AIMLDTD.DTDInhalt);
		}
	}
}
