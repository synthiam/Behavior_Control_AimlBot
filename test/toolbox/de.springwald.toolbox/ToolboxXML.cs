using System;
using System.Collections;
using System.Xml;
using System.Xml.XPath;

namespace de.springwald.toolbox
{
	public class ToolboxXML
	{
		public static bool Node1LiegtVorNode2(XmlNode node1, XmlNode node2)
		{
			if (node1 == null || node2 == null)
			{
				throw new ApplicationException("Keiner der beiden zu vergleichenden Nodes darf NULL sein (Node1LiegtVorNode2)");
			}
			if (node1.OwnerDocument != node2.OwnerDocument)
			{
				return false;
			}
			if (node1 == node2)
			{
				return false;
			}
			XPathNavigator xPathNavigator = node1.CreateNavigator();
			XPathNavigator nav = node2.CreateNavigator();
			return xPathNavigator.ComparePosition(nav) == XmlNodeOrder.Before;
		}

		public static bool IstChild(XmlNode child, XmlNode parent)
		{
			if (child.ParentNode == null)
			{
				return false;
			}
			if (child.ParentNode == parent)
			{
				return true;
			}
			return ToolboxXML.IstChild(child.ParentNode, parent);
		}

		public static string TextAusTextNodeBereinigt(XmlNode textNode)
		{
			if (!(textNode is XmlText) && !(textNode is XmlComment) && !(textNode is XmlWhitespace))
			{
				throw new ApplicationException(string.Format("Received node is not a textnode  ({0})", textNode.OuterXml));
			}
			string text = textNode.Value.ToString();
			text = text.Replace(Environment.NewLine, "");
			return text.Trim('\n', '\t', '\r', '\v');
		}

		public static bool IstTextOderKommentarNode(XmlNode node)
		{
			return node is XmlText || node is XmlComment;
		}

		public static void WhitespacesBereinigen(XmlNode node)
		{
			if (node != null)
			{
				ArrayList arrayList = new ArrayList();
				ArrayList arrayList2 = new ArrayList();
				foreach (XmlNode childNode in node.ChildNodes)
				{
					if (childNode is XmlWhitespace)
					{
						arrayList.Add(childNode);
					}
					else if (childNode is XmlElement)
					{
						arrayList2.Add(childNode);
					}
				}
				foreach (XmlWhitespace item in arrayList)
				{
					if (item.Data.IndexOf(" ") != -1)
					{
						XmlText newChild = item.OwnerDocument.CreateTextNode(" ");
						item.ParentNode.ReplaceChild(newChild, item);
					}
					else
					{
						item.ParentNode.RemoveChild(item);
					}
				}
				foreach (XmlNode item2 in arrayList2)
				{
					ToolboxXML.WhitespacesBereinigen(item2);
				}
			}
		}
	}
}
