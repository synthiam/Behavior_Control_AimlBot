using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace de.springwald.xml.dtd
{
	public class DTD
	{
		public class XMLUnknownElementException : Exception
		{
			private string _elementname;

			public string ElementName
			{
				get
				{
					return this._elementname;
				}
			}

			public XMLUnknownElementException(string elementname)
			{
				this._elementname = elementname;
			}
		}

		private List<DTDElement> _elemente;

		private Hashtable _elementeNachNamen;

		private List<DTDEntity> _entities;

		public List<DTDElement> Elemente
		{
			get
			{
				return this._elemente;
			}
		}

		public List<DTDEntity> Entities
		{
			get
			{
				return this._entities;
			}
		}

		public DTD(List<DTDElement> elemente, List<DTDEntity> entities)
		{
			this._elemente = elemente;
			this._entities = entities;
			this._elementeNachNamen = new Hashtable();
		}

		public DTD()
		{
		}

		public bool IstDTDElementBekannt(string elementName)
		{
			return this.DTDElementByNameIntern_(elementName, false) != null;
		}

		public DTDElement DTDElementByNode_(XmlNode node, bool fehlerWennNichtVorhanden)
		{
			return this.DTDElementByNameIntern_(DTD.GetElementNameFromNode(node), fehlerWennNichtVorhanden);
		}

		public DTDElement DTDElementByName(string elementName, bool fehlerWennNichtVorhanden)
		{
			return this.DTDElementByNameIntern_(elementName, fehlerWennNichtVorhanden);
		}

		public static string GetElementNameFromNode(XmlNode node)
		{
			if (node == null)
			{
				return "";
			}
			if (node is XmlText)
			{
				return "#PCDATA";
			}
			if (node is XmlComment)
			{
				return "#COMMENT";
			}
			if (node is XmlWhitespace)
			{
				return "#WHITESPACE";
			}
			return node.Name;
		}

		public DTDElement DTDElementByNameIntern_(string elementName, bool fehlerWennNichtVorhanden)
		{
			DTDElement dTDElement = (DTDElement)this._elementeNachNamen[elementName];
			if (dTDElement != null)
			{
				return dTDElement;
			}
			foreach (DTDElement item in this._elemente)
			{
				if (elementName == item.Name)
				{
					this._elementeNachNamen.Add(elementName, item);
					return item;
				}
			}
			if (fehlerWennNichtVorhanden)
			{
				throw new XMLUnknownElementException(elementName);
			}
			return null;
		}
	}
}
