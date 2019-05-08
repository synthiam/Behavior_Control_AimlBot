using System;

namespace de.springwald.gaitobot.publizierung
{
	[Serializable]
	public class AIMLDateiUebertragung
	{
		public string Inhalt
		{
			get;
			set;
		}

		public string CheckString
		{
			get;
			set;
		}

		public string Dateiname
		{
			get;
			set;
		}
	}
}
