using System.Collections;

namespace de.springwald.gaitobot2
{
	public class CategorySortierer : IComparer
	{
		public int Compare(object x, object y)
		{
			int length = ((WissensCategory)x).Pattern.Inhalt.Length;
			int length2 = ((WissensCategory)y).Pattern.Inhalt.Length;
			if (length == length2)
			{
				return -(((WissensCategory)x).That.Inhalt.Length - ((WissensCategory)y).That.Inhalt.Length);
			}
			return -(length - length2);
		}

		public int Compare2(object x, object y)
		{
			int length = ((WissensCategory)x).That.Inhalt.Length;
			int length2 = ((WissensCategory)y).That.Inhalt.Length;
			if (length == length2)
			{
				return -(((WissensCategory)x).Pattern.Inhalt.Length - ((WissensCategory)y).Pattern.Inhalt.Length);
			}
			return -(length - length2);
		}
	}
}
