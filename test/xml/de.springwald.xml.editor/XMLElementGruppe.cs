using System.Collections;

namespace de.springwald.xml.editor
{
	public class XMLElementGruppe
	{
		private Hashtable _elemente;

		private string _titel;

		private bool _standardMaessigZusammengeklappt;

		public string Titel
		{
			get
			{
				return this._titel;
			}
		}

		public bool StandardMaessigZusammengeklappt
		{
			get
			{
				return this._standardMaessigZusammengeklappt;
			}
		}

		public XMLElementGruppe(string titel, bool standardMaessigZusammengeklappt)
		{
			this._titel = titel;
			this._elemente = new Hashtable();
			this._standardMaessigZusammengeklappt = standardMaessigZusammengeklappt;
		}

		public void AddElementName(string name)
		{
			this._elemente.Add(name.ToLower(), null);
		}

		public bool ContainsElement(string name)
		{
			return this._elemente.ContainsKey(name.ToLower());
		}
	}
}
