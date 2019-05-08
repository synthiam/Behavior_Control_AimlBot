namespace de.springwald.gaitobot2
{
	public class BotDenkProtokollSchritt
	{
		public enum SchrittArten
		{
			Eingabe,
			Info,
			Warnung,
			PassendeKategorieGefunden
		}

		private readonly string _meldung;

		private readonly SchrittArten _art;

		private readonly WissensCategory _category;

		public string Meldung
		{
			get
			{
				return this._meldung;
			}
		}

		public SchrittArten Art
		{
			get
			{
				return this._art;
			}
		}

		public WissensCategory Category
		{
			get
			{
				return this._category;
			}
		}

		public BotDenkProtokollSchritt(string meldung, SchrittArten art)
		{
			this._meldung = meldung;
			this._art = art;
		}

		public BotDenkProtokollSchritt(string meldung, SchrittArten art, WissensCategory category)
		{
			this._meldung = meldung;
			this._art = art;
			this._category = category;
		}
	}
}
