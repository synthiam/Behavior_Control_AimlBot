using System;
using System.Xml;

namespace de.springwald.xml.editor
{
	public class XMLUndoSchrittAttributRemoved : XMLUndoSchritt
	{
		private XmlAttribute _geloeschtesAttribut;

		private XmlNode _ownerElement;

		public XMLUndoSchrittAttributRemoved(XmlAttribute attributVorDemLoeschen)
		{
			this._geloeschtesAttribut = attributVorDemLoeschen;
			this._ownerElement = attributVorDemLoeschen.OwnerElement;
			if (this._ownerElement != null)
			{
				return;
			}
			throw new ApplicationException("Löschen des Attributes kann nicht für Undo vermerkt werden, da es keinen Bezug hat '" + attributVorDemLoeschen.OuterXml + "'");
		}

		public override void UnDo()
		{
			this._ownerElement.Attributes.Append(this._geloeschtesAttribut);
		}
	}
}
