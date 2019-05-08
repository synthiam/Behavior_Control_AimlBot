using System.Xml;

namespace GaitoBotEditorCore
{
	internal class AIMLDateiKommentare
	{
		private XmlDocument _xmlDoc;

		public AIMLDateiKommentare(XmlDocument xmlDoc)
		{
			this._xmlDoc = xmlDoc;
		}

		public void SchreibeKommentarEintrag(string name, string wert)
		{
			XmlComment kommentarNode = this.GetKommentarNode(name, true);
			kommentarNode.Value = string.Format("{0}: {1}", name, wert);
		}

		private XmlComment GetKommentarNode(string name, bool erzeugenWennNichtVorhanden)
		{
			foreach (XmlNode childNode in this._xmlDoc.DocumentElement.ChildNodes)
			{
				if (childNode is XmlComment && childNode.Value.Contains(string.Format("{0}: ", name)))
				{
					return (XmlComment)childNode;
				}
			}
			if (erzeugenWennNichtVorhanden)
			{
				XmlComment xmlComment = this._xmlDoc.CreateComment(string.Format("{0}: ", name));
				if (this._xmlDoc.DocumentElement.FirstChild == null)
				{
					this._xmlDoc.DocumentElement.AppendChild(xmlComment);
				}
				else
				{
					this._xmlDoc.DocumentElement.InsertBefore(xmlComment, this._xmlDoc.DocumentElement.FirstChild);
				}
				return xmlComment;
			}
			return null;
		}
	}
}
