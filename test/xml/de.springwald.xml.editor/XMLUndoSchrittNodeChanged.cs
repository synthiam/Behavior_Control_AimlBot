using System;
using System.Xml;

namespace de.springwald.xml.editor
{
	public class XMLUndoSchrittNodeChanged : XMLUndoSchritt
	{
		private XmlNode _geaenderterNode;

		private string _valueVorher;

		public XMLUndoSchrittNodeChanged(XmlNode geaenderterNode, string valueVorher)
		{
			this._geaenderterNode = geaenderterNode;
			this._valueVorher = valueVorher;
			if (geaenderterNode != null)
			{
				return;
			}
			throw new ApplicationException("Verändern des Nodes kann nicht für Undo vermerkt werden, da er NULL ist '" + this._geaenderterNode.OuterXml + "'");
		}

		public override void UnDo()
		{
			this._geaenderterNode.Value = this._valueVorher;
		}
	}
}
