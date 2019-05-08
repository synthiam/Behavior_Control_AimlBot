namespace de.springwald.gaitobot2
{
	public class AntwortSatz
	{
		public string Satz
		{
			get;
			set;
		}

		public bool IstNotfallAntwort
		{
			get;
			set;
		}

		public AntwortSatz(string satz, bool istNotfallAntwort)
		{
			this.IstNotfallAntwort = istNotfallAntwort;
			this.Satz = satz;
		}
	}
}
