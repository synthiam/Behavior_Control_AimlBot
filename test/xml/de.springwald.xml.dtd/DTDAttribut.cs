using System.Collections.Specialized;

namespace de.springwald.xml.dtd
{
	public class DTDAttribut
	{
		public enum PflichtArten
		{
			Pflicht,
			Optional,
			Konstante
		}

		private string _name;

		private PflichtArten _pflicht;

		private string _typ;

		private string _standardWert;

		private StringCollection _erlaubteWerte;

		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				this._name = value;
			}
		}

		public PflichtArten Pflicht
		{
			get
			{
				return this._pflicht;
			}
			set
			{
				this._pflicht = value;
			}
		}

		public StringCollection ErlaubteWerte
		{
			get
			{
				return this._erlaubteWerte;
			}
			set
			{
				this._erlaubteWerte = value;
			}
		}

		public string StandardWert
		{
			get
			{
				return this._standardWert;
			}
			set
			{
				this._standardWert = value;
			}
		}

		public string Typ
		{
			get
			{
				return this._typ;
			}
			set
			{
				this._typ = value;
			}
		}

		public DTDAttribut()
		{
			this._name = "";
			this._pflicht = PflichtArten.Optional;
			this._standardWert = "";
			this._erlaubteWerte = new StringCollection();
		}
	}
}
