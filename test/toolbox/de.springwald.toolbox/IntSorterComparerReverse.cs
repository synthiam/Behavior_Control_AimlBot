using System.Collections;

namespace de.springwald.toolbox
{
	public class IntSorterComparerReverse : IComparer
	{
		public int Compare(object x, object y)
		{
			return ((IntSorterItem)y).SortWert - ((IntSorterItem)x).SortWert;
		}
	}
}
