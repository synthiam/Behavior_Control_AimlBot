using de.springwald.xml.cursor;
using System.Text;
using System.Xml;

namespace de.springwald.xml.dtd.pruefer
{
	public class DTDPruefer
	{
		private DTD _dtd;

		private DTDNodeEditCheck _nodeCheckerintern;

		private StringBuilder _fehlermeldungen;

		private DTDNodeEditCheck NodeChecker
		{
			get
			{
				if (this._nodeCheckerintern == null)
				{
					this._nodeCheckerintern = new DTDNodeEditCheck(this._dtd);
				}
				return this._nodeCheckerintern;
			}
		}

		public string Fehlermeldungen
		{
			get
			{
				return this._fehlermeldungen.ToString();
			}
		}

		public DTDPruefer(DTD dtd)
		{
			this._dtd = dtd;
			this.Reset();
		}

		public bool IstXmlAttributOk(XmlAttribute xmlAttribut)
		{
			this.Reset();
			return this.PruefeAttribut(xmlAttribut);
		}

		public bool IstXmlNodeOk(XmlNode xmlNode, bool posBereitsAlsOKBestaetigt)
		{
			this.Reset();
			if (posBereitsAlsOKBestaetigt)
			{
				return true;
			}
			return this.PruefeNodePos(xmlNode);
		}

		private bool PruefeNodePos(XmlNode node)
		{
			if (node is XmlWhitespace)
			{
				return true;
			}
			if (this._dtd.IstDTDElementBekannt(DTD.GetElementNameFromNode(node)))
			{
				try
				{
					if (this.NodeChecker.IstDerNodeAnDieserStelleErlaubt(node))
					{
						return true;
					}
					this._fehlermeldungen.AppendFormat(ResReader.Reader.GetString("TagHierNichtErlaubt"), node.Name);
					XMLCursorPos xMLCursorPos = new XMLCursorPos();
					xMLCursorPos.CursorSetzen(node, XMLCursorPositionen.CursorAufNodeSelbstVorderesTag);
					string[] array = this.NodeChecker.AnDieserStelleErlaubteTags_(xMLCursorPos, false, false);
					if (array.Length != 0)
					{
						this._fehlermeldungen.Append(ResReader.Reader.GetString("ErlaubteTags"));
						string[] array2 = array;
						foreach (string arg in array2)
						{
							this._fehlermeldungen.AppendFormat("{0} ", arg);
						}
					}
					else
					{
						this._fehlermeldungen.Append(ResReader.Reader.GetString("AnDieserStelleKeineTagsErlaubt"));
					}
					return false;
				}
				catch (DTD.XMLUnknownElementException ex)
				{
					this._fehlermeldungen.AppendFormat(ResReader.Reader.GetString("UnbekanntesElement"), ex.ElementName);
					return false;
				}
			}
			this._fehlermeldungen.AppendFormat(ResReader.Reader.GetString("UnbekanntesElement"), DTD.GetElementNameFromNode(node));
			return false;
		}

		private bool PruefeAttribut(XmlAttribute attribut)
		{
			return false;
		}

		private void Reset()
		{
			this._fehlermeldungen = new StringBuilder();
		}
	}
}
