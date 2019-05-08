namespace de.springwald.toolbox.Impressum
{
	public static class Impressum
	{
		public static string ImpressumTextAscii
		{
			get
			{
				return "Springwald Software GmbH\r\nAlter Eistreff 36\r\n44789 Bochum\r\nDeutschland / Germany\r\nwww.springwald.de\r\n\r\nVertretungsberechtigter Geschäftsführer: Daniel Springwald (Anschrift wie oben)\r\nAmtsgericht Bochum, Handelsregisternummer: HRB 12756\r\nUSt-IdNr: DE272058075\r\n\r\nEmail: info@springwald.de\r\nTelefon: 0700 / 777 464 925*\r\n* max. 12,4 Cent pro Minute aus dem Festnetz der Deutschen Telekom";
			}
		}

		public static string ImpressumTextHtml
		{
			get
			{
				return Impressum.ImpressumTextAscii.Replace("\r\n", "<br/>");
			}
		}
	}
}
