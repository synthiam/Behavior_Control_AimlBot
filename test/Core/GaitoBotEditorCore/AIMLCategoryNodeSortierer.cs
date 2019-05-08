using System;
using System.Collections;
using System.Xml;

namespace GaitoBotEditorCore
{
	public class AIMLCategoryNodeSortierer : IComparer
	{
		public int Compare(object x, object y)
		{
			try
			{
				return ((XmlNode)x).FirstChild.InnerText.CompareTo(((XmlNode)y).FirstChild.InnerText);
			}
			catch (Exception)
			{
				return 0;
			}
		}
	}
}
