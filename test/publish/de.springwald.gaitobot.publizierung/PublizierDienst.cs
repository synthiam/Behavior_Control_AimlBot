using de.springwald.gaitobot.publizierung.Gaitobot_de_publizieren;
using de.springwald.toolbox;

namespace de.springwald.gaitobot.publizierung
{
	public class PublizierDienst
	{
		private static Publizieren _webdienst_Buffer;

		private static Publizieren Webdienst
		{
			get
			{
				if (PublizierDienst._webdienst_Buffer == null)
				{
					PublizierDienst._webdienst_Buffer = new Publizieren();
					PublizierDienst._webdienst_Buffer.Url = "http://www.gaitobot.de/gaitobot/Webservices/Publizieren.asmx";
					PublizierDienst._webdienst_Buffer.Timeout = 50000;
				}
				return PublizierDienst._webdienst_Buffer;
			}
		}

		public static bool ExistsGaitoBotID(string gaitoBotEditorID)
		{
			return PublizierDienst.Webdienst.ExistsGaitoBotID(gaitoBotEditorID);
		}

		public static void ReseteGaitoBot(string gaitoBotEditorID)
		{
			PublizierDienst.Webdienst.ReseteGaitoBot(gaitoBotEditorID);
		}

		public static long MaxKBWissen(string gaitoBotEditorID)
		{
			return PublizierDienst.Webdienst.MaxKBWissen(gaitoBotEditorID);
		}

		public static string[] AlteOderGeloeschteDateienLoeschenUndHochzuladendeZurueckGeben(string gaitoBotEditorID, DateiPublizierungsInfos[] dateien)
		{
			de.springwald.gaitobot.publizierung.Gaitobot_de_publizieren.DateiPublizierungsInfos[] dateien2 = GenericConverter<DateiPublizierungsInfos[], de.springwald.gaitobot.publizierung.Gaitobot_de_publizieren.DateiPublizierungsInfos[]>.ConvertTo(dateien);
			return PublizierDienst.Webdienst.AlteOderGeloeschteDateienLoeschenUndHochzuladendeZurueckGeben(gaitoBotEditorID, dateien2);
		}

		public static void UebertrageAIMLDatei(string gaitoBotEditorID, AIMLDateiUebertragung aimlDateiInhalt)
		{
			de.springwald.gaitobot.publizierung.Gaitobot_de_publizieren.AIMLDateiUebertragung aimlDateiInhalt2 = new de.springwald.gaitobot.publizierung.Gaitobot_de_publizieren.AIMLDateiUebertragung
			{
				CheckString = aimlDateiInhalt.CheckString,
				Dateiname = aimlDateiInhalt.Dateiname,
				Inhalt = aimlDateiInhalt.Inhalt
			};
			PublizierDienst.Webdienst.UebertrageAIMLDatei(gaitoBotEditorID, aimlDateiInhalt2);
		}
	}
}
