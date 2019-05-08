namespace de.springwald.gaitobot2
{
	public class GaitoBotSessionSchritt
	{
		public string UserEingabe
		{
			get;
			set;
		}

		public string BotAusgabe
		{
			get;
			set;
		}

		public string That
		{
			get;
			set;
		}

		public string Topic
		{
			get;
			set;
		}

		public GaitoBotSessionSchritt()
		{
			this.That = "*";
			this.Topic = "*";
			this.UserEingabe = "";
			this.BotAusgabe = "";
		}

		public static GaitoBotSessionSchritt CreateBegruessungsSchritt(string botBegruessung)
		{
			GaitoBotSessionSchritt gaitoBotSessionSchritt = new GaitoBotSessionSchritt();
			gaitoBotSessionSchritt.BotAusgabe = botBegruessung;
			gaitoBotSessionSchritt.That = "*";
			gaitoBotSessionSchritt.Topic = "*";
			gaitoBotSessionSchritt.UserEingabe = "";
			return gaitoBotSessionSchritt;
		}
	}
}
