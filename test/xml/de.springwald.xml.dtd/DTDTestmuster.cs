using System.Text;

namespace de.springwald.xml.dtd
{
	public class DTDTestmuster
	{
		private string _elementName;

		private string _parentElementName;

		private bool _erfolgreich;

		private string _vergleichsStringFuerRegEx;

		private StringBuilder _elementNamenListe;

		public string ElementName
		{
			get
			{
				return this._elementName;
			}
		}

		public string VergleichStringFuerRegEx
		{
			get
			{
				if (this._vergleichsStringFuerRegEx == null)
				{
					this._elementNamenListe.Append("<");
					this._vergleichsStringFuerRegEx = this._elementNamenListe.ToString();
				}
				return this._vergleichsStringFuerRegEx;
			}
		}

		public string Zusammenfassung
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				if (this._erfolgreich)
				{
					stringBuilder.Append("+ ");
				}
				else
				{
					stringBuilder.Append("- ");
				}
				stringBuilder.Append(this._parentElementName);
				stringBuilder.Append(" (");
				stringBuilder.Append(this.VergleichStringFuerRegEx);
				stringBuilder.Append(")");
				if (this._elementName == null)
				{
					stringBuilder.Append(" [getestet: löschen]");
				}
				else
				{
					stringBuilder.AppendFormat("[getestet: {0}]", this._elementName);
				}
				return stringBuilder.ToString();
			}
		}

		public bool Erfolgreich
		{
			get
			{
				return this._erfolgreich;
			}
			set
			{
				this._erfolgreich = value;
			}
		}

		public DTDTestmuster(string elementName, string parentElementName)
		{
			this._elementNamenListe = new StringBuilder();
			this._elementNamenListe.Append(">");
			this._elementName = elementName;
			this._parentElementName = parentElementName;
			this._erfolgreich = false;
		}

		public void AddElement(string elementName)
		{
			this._elementNamenListe.AppendFormat("-{0}", elementName);
		}
	}
}
