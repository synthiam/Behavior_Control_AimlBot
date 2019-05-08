using System.Collections;
using System.Linq;

namespace de.springwald.gaitobot2
{
	public class Wissen
	{
		private readonly Hashtable _themen;

		public int AnzahlCategories
		{
			get
			{
				return this._themen.Values.Cast<WissenThema>().Sum((WissenThema thema) => thema.Categories.Count);
			}
		}

		public Wissen()
		{
			this._themen = new Hashtable();
		}

		public WissenThema GetThema(string themaName)
		{
			if (themaName == null)
			{
				themaName = "*";
			}
			return (WissenThema)this._themen[themaName];
		}

		public void CategoryAufnehmen(WissensCategory category)
		{
			WissenThema wissenThema = (WissenThema)this._themen[category.ThemaName];
			if (wissenThema == null)
			{
				wissenThema = new WissenThema(category.ThemaName);
				this._themen.Add(category.ThemaName, wissenThema);
			}
			wissenThema.CategoryAufnehmen(category);
		}
	}
}
