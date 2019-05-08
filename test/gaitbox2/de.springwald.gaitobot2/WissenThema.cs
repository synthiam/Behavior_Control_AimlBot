using System.Collections;

namespace de.springwald.gaitobot2
{
	public class WissenThema
	{
		private readonly string _name;

		private readonly ArrayList _categories;

		private readonly ArrayList _starCategories;

		private bool _categoriesSortiert = false;

		private bool _starCategoriesSortiert = false;

		public string Name
		{
			get
			{
				return this._name;
			}
		}

		public ArrayList Categories
		{
			get
			{
				if (!this._categoriesSortiert)
				{
					this._categories.Sort(new CategorySortierer());
					this._categoriesSortiert = true;
				}
				return this._categories;
			}
		}

		public ArrayList StarCategories
		{
			get
			{
				if (!this._starCategoriesSortiert)
				{
					this._starCategories.Sort(new CategorySortierer());
					this._starCategoriesSortiert = true;
				}
				return this._starCategories;
			}
		}

		public WissenThema(string themaName)
		{
			this._name = themaName;
			this._categories = new ArrayList();
			this._starCategories = new ArrayList();
		}

		public void CategoryAufnehmen(WissensCategory category)
		{
			this._categories.Add(category);
			if (category.Pattern.Inhalt == "*")
			{
				this._starCategories.Add(category);
			}
			this.FlushSortierungen();
		}

		private void FlushSortierungen()
		{
			this._starCategoriesSortiert = false;
			this._categoriesSortiert = false;
		}
	}
}
