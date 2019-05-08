using System;
using System.Xml;

namespace de.springwald.xml.editor
{
	public class XMLUndoSchrittNodeRemoved : XMLUndoSchritt
	{
		private XmlNode _geloeschterNode;

		private XmlNode _parentNode;

		private XmlNode _previousSibling;

		private XmlNode _nextSibling;

		public XMLUndoSchrittNodeRemoved(XmlNode nodeVorDemLoeschen)
		{
			this._geloeschterNode = nodeVorDemLoeschen;
			this._parentNode = nodeVorDemLoeschen.ParentNode;
			this._previousSibling = nodeVorDemLoeschen.PreviousSibling;
			this._nextSibling = nodeVorDemLoeschen.NextSibling;
			if (this._parentNode != null || this._previousSibling != null || this._nextSibling != null)
			{
				return;
			}
			throw new ApplicationException("Löschen des Nodes kann nicht für Undo vermerkt werden, da er keinen Bezug hat '" + nodeVorDemLoeschen.OuterXml + "'");
		}

		public override void UnDo()
		{
			if (this._previousSibling != null)
			{
				this._previousSibling.ParentNode.InsertAfter(this._geloeschterNode, this._previousSibling);
			}
			else if (this._nextSibling != null)
			{
				this._nextSibling.ParentNode.InsertBefore(this._geloeschterNode, this._nextSibling);
			}
			else
			{
				this._parentNode.AppendChild(this._geloeschterNode);
			}
		}
	}
}
