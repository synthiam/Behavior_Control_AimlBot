using System.Collections;

namespace de.springwald.xml.editor
{
	public class XMLElementGruppenListe
	{
		private ArrayList _gruppen;

		public int Count
		{
			get
			{
				return this._gruppen.Count;
			}
		}

		public XMLElementGruppe this[int index]
		{
			get
			{
				return (XMLElementGruppe)this._gruppen[index];
			}
			set
			{
				this._gruppen[index] = value;
			}
		}

		public XMLElementGruppenListe()
		{
			this._gruppen = new ArrayList();
		}

		public void Dispose()
		{
			this._gruppen.Clear();
			this._gruppen = null;
		}

		public void Add(XMLElementGruppe gruppe)
		{
			this._gruppen.Add(gruppe);
		}

		public void Remove(XMLElementGruppe gruppe)
		{
			this._gruppen.Remove(gruppe);
		}
	}
}
