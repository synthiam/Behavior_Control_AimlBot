using System.Xml;

namespace de.springwald.gaitobot2
{
	public class DomDocLadePaket
	{
		private readonly XmlDocument _doc;

		private readonly string _dateiname;

		public XmlDocument XmlDocument
		{
			get
			{
				return this._doc;
			}
		}

		public string Dateiname
		{
			get
			{
				return this._dateiname;
			}
		}

		public DomDocLadePaket(XmlDocument doc, string dateiname)
		{
			this._doc = doc;
			this._dateiname = dateiname;
		}
	}
}
