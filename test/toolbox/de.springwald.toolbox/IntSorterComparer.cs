using System.Collections;

namespace de.springwald.toolbox
{
	public class IntSorterComparer : IComparer
	{
		public int Compare(object x, object y)
		{
			return ((IntSorterItem)x).SortWert - ((IntSorterItem)y).SortWert;
		}
	}
}
