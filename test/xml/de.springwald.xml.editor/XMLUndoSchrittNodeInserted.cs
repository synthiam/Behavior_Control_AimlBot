using System;
using System.Xml;

namespace de.springwald.xml.editor
{
	public class XMLUndoSchrittNodeInserted : XMLUndoSchritt
	{
		private XmlNode _eingefuegterNode;

		private XmlNode _parentNode;

		public XMLUndoSchrittNodeInserted(XmlNode eingefuegterNode, XmlNode parentNode)
		{
			this._eingefuegterNode = eingefuegterNode;
			this._parentNode = parentNode;
			if (eingefuegterNode != null)
			{
				return;
			}
			throw new ApplicationException("Einfügen des Nodes kann nicht für Undo vermerkt werden, da er NULL ist '" + this._eingefuegterNode.OuterXml + "'");
		}

		public override void UnDo()
		{
			if (this._eingefuegterNode is XmlAttribute)
			{
				this._parentNode.Attributes.Remove((XmlAttribute)this._eingefuegterNode);
			}
			else
			{
				this._parentNode.RemoveChild(this._eingefuegterNode);
			}
		}
	}
}
