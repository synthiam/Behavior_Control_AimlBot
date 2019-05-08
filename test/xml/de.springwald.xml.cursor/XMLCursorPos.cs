using de.springwald.toolbox;
using System;
using System.Xml;

namespace de.springwald.xml.cursor
{
	public class XMLCursorPos
	{
		private XmlNode _aktNode;

		private XMLCursorPositionen _posAmNode;

		private int _posImTextnode;

		public XmlNode AktNode
		{
			get
			{
				return this._aktNode;
			}
		}

		public int PosImTextnode
		{
			get
			{
				if (this._posAmNode == XMLCursorPositionen.CursorInnerhalbDesTextNodes)
				{
					goto IL_0013;
				}
				goto IL_0013;
				IL_0013:
				return this._posImTextnode;
			}
		}

		public XMLCursorPositionen PosAmNode
		{
			get
			{
				return this._posAmNode;
			}
		}

		public event EventHandler PosChangedEvent;

		protected virtual void PosChanged(EventArgs e)
		{
			if (this.PosChangedEvent != null)
			{
				this.PosChangedEvent(this, e);
			}
		}

		public XMLCursorPos()
		{
			this._aktNode = null;
			this._posAmNode = XMLCursorPositionen.CursorAufNodeSelbstVorderesTag;
			this._posImTextnode = 0;
		}

		public bool Equals(XMLCursorPos zweitePos)
		{
			if (this.AktNode != zweitePos.AktNode)
			{
				return false;
			}
			if (this.PosAmNode != zweitePos.PosAmNode)
			{
				return false;
			}
			if (this._posImTextnode != zweitePos._posImTextnode)
			{
				return false;
			}
			return true;
		}

		public XMLCursorPos Clone()
		{
			XMLCursorPos xMLCursorPos = new XMLCursorPos();
			xMLCursorPos.CursorSetzen(this._aktNode, this._posAmNode, this._posImTextnode);
			return xMLCursorPos;
		}

		public bool LiegtNodeHinterDieserPos(XmlNode node)
		{
			return ToolboxXML.Node1LiegtVorNode2(this._aktNode, node);
		}

		public bool LiegtNodeVorDieserPos(XmlNode node)
		{
			return ToolboxXML.Node1LiegtVorNode2(node, this._aktNode);
		}

		public void ErzwingeChanged()
		{
			this.PosChanged(EventArgs.Empty);
		}

		public void TextEinfuegen(string rohText, XMLRegelwerk regelwerk, out XmlNode ersatzNode)
		{
			string text = regelwerk.EinfuegeTextPreProcessing(rohText, this, out ersatzNode);
			if (ersatzNode == null)
			{
				switch (this.PosAmNode)
				{
				case XMLCursorPositionen.CursorAufNodeSelbstVorderesTag:
				case XMLCursorPositionen.CursorAufNodeSelbstHinteresTag:
					if (regelwerk.IstDiesesTagAnDieserStelleErlaubt("#PCDATA", this))
					{
						XmlText xmlText2 = this.AktNode.OwnerDocument.CreateTextNode(text);
						this.AktNode.ParentNode.ReplaceChild(this.AktNode, xmlText2);
						this.CursorSetzen(xmlText2, XMLCursorPositionen.CursorHinterDemNode);
					}
					throw new ApplicationException(string.Format("TextEinfuegen: unbehandelte CursorPos {0}", this.PosAmNode));
				case XMLCursorPositionen.CursorHinterDemNode:
					this.TextZwischenZweiNodesEinfuegen(this.AktNode, this.AktNode.NextSibling, text, regelwerk);
					break;
				case XMLCursorPositionen.CursorVorDemNode:
					this.TextZwischenZweiNodesEinfuegen(this.AktNode.PreviousSibling, this.AktNode, text, regelwerk);
					break;
				case XMLCursorPositionen.CursorInDemLeeremNode:
					if (regelwerk.IstDiesesTagAnDieserStelleErlaubt("#PCDATA", this))
					{
						XmlText xmlText = this.AktNode.OwnerDocument.CreateTextNode(text);
						this.AktNode.AppendChild(xmlText);
						this.CursorSetzen(xmlText, XMLCursorPositionen.CursorHinterDemNode);
					}
					break;
				case XMLCursorPositionen.CursorInnerhalbDesTextNodes:
				{
					string str = this.AktNode.InnerText.Substring(0, this.PosImTextnode);
					string str2 = this.AktNode.InnerText.Substring(this.PosImTextnode, this.AktNode.InnerText.Length - this.PosImTextnode);
					this.AktNode.InnerText = str + text + str2;
					this.CursorSetzen(this.AktNode, this.PosAmNode, this.PosImTextnode + text.Length);
					break;
				}
				default:
					throw new ApplicationException(string.Format("TextEinfuegen: Unbekannte CursorPos {0}", this.PosAmNode));
				}
			}
		}

		public bool InsertXMLNode(XmlNode node, XMLRegelwerk regelwerk, bool neueCursorPosAufJedenFallHinterDenEingefuegtenNodeSetzen)
		{
			XmlNode parentNode = this.AktNode.ParentNode;
			switch (this.PosAmNode)
			{
			case XMLCursorPositionen.CursorAufNodeSelbstVorderesTag:
			case XMLCursorPositionen.CursorAufNodeSelbstHinteresTag:
				parentNode.ReplaceChild(node, this.AktNode);
				break;
			case XMLCursorPositionen.CursorVorDemNode:
				parentNode.InsertBefore(node, this.AktNode);
				break;
			case XMLCursorPositionen.CursorHinterDemNode:
				parentNode.InsertAfter(node, this.AktNode);
				break;
			case XMLCursorPositionen.CursorInDemLeeremNode:
				this.AktNode.AppendChild(node);
				break;
			case XMLCursorPositionen.CursorInnerhalbDesTextNodes:
			{
				string text = this.AktNode.InnerText.Substring(0, this.PosImTextnode);
				XmlNode xmlNode = parentNode.OwnerDocument.CreateTextNode(text);
				string text2 = this.AktNode.InnerText.Substring(this.PosImTextnode, this.AktNode.InnerText.Length - this.PosImTextnode);
				XmlNode newChild = parentNode.OwnerDocument.CreateTextNode(text2);
				parentNode.ReplaceChild(xmlNode, this.AktNode);
				parentNode.InsertAfter(node, xmlNode);
				parentNode.InsertAfter(newChild, node);
				break;
			}
			default:
				throw new ApplicationException(string.Format("InsertElementAnCursorPos: Unbekannte PosAmNode {0}", this.PosAmNode));
			}
			if (neueCursorPosAufJedenFallHinterDenEingefuegtenNodeSetzen)
			{
				this.CursorSetzen(node, XMLCursorPositionen.CursorHinterDemNode);
			}
			else if (regelwerk.IstSchliessendesTagSichtbar(node))
			{
				this.CursorSetzen(node, XMLCursorPositionen.CursorInDemLeeremNode);
			}
			else
			{
				this.CursorSetzen(node, XMLCursorPositionen.CursorHinterDemNode);
			}
			return true;
		}

		private void TextZwischenZweiNodesEinfuegen(XmlNode nodeVorher, XmlNode nodeNachher, string text, XMLRegelwerk regelwerk)
		{
			if (ToolboxXML.IstTextOderKommentarNode(nodeVorher))
			{
				nodeVorher.InnerText += text;
				this.CursorSetzen(nodeVorher, XMLCursorPositionen.CursorInnerhalbDesTextNodes, nodeVorher.InnerText.Length);
			}
			else if (ToolboxXML.IstTextOderKommentarNode(nodeNachher))
			{
				nodeNachher.InnerText = text + nodeNachher.InnerText;
				this.CursorSetzen(nodeNachher, XMLCursorPositionen.CursorInnerhalbDesTextNodes, text.Length);
			}
			else if (regelwerk.IstDiesesTagAnDieserStelleErlaubt("#PCDATA", this))
			{
				XmlText node = this.AktNode.OwnerDocument.CreateTextNode(text);
				this.InsertXMLNode(node, regelwerk, false);
			}
		}

		public bool MoveLeft(XmlNode rootnode, XMLRegelwerk regelwerk)
		{
			XmlNode aktNode = this.AktNode;
			switch (this.PosAmNode)
			{
			case XMLCursorPositionen.CursorAufNodeSelbstVorderesTag:
			case XMLCursorPositionen.CursorAufNodeSelbstHinteresTag:
				this.CursorSetzen(this.AktNode, XMLCursorPositionen.CursorVorDemNode);
				goto IL_01b9;
			case XMLCursorPositionen.CursorVorDemNode:
				if (aktNode != rootnode)
				{
					if (aktNode.PreviousSibling != null)
					{
						this.CursorSetzen(aktNode.PreviousSibling, XMLCursorPositionen.CursorHinterDemNode);
						this.MoveLeft(rootnode, regelwerk);
					}
					else
					{
						this.CursorSetzen(aktNode.ParentNode, XMLCursorPositionen.CursorVorDemNode);
					}
					goto IL_01b9;
				}
				return false;
			case XMLCursorPositionen.CursorHinterDemNode:
				if (ToolboxXML.IstTextOderKommentarNode(aktNode))
				{
					this.CursorSetzen(aktNode, XMLCursorPositionen.CursorInnerhalbDesTextNodes, Math.Max(0, ToolboxXML.TextAusTextNodeBereinigt(aktNode).Length - 1));
				}
				else if (aktNode.ChildNodes.Count < 1)
				{
					if (regelwerk.IstSchliessendesTagSichtbar(aktNode))
					{
						this.CursorSetzen(this.AktNode, XMLCursorPositionen.CursorInDemLeeremNode);
					}
					else
					{
						this.CursorSetzen(this.AktNode, XMLCursorPositionen.CursorVorDemNode);
					}
				}
				else
				{
					this.CursorSetzen(aktNode.LastChild, XMLCursorPositionen.CursorHinterDemNode);
				}
				goto IL_01b9;
			case XMLCursorPositionen.CursorInDemLeeremNode:
				this.CursorSetzen(this.AktNode, XMLCursorPositionen.CursorVorDemNode);
				goto IL_01b9;
			case XMLCursorPositionen.CursorInnerhalbDesTextNodes:
				if (ToolboxXML.IstTextOderKommentarNode(aktNode))
				{
					if (this.PosImTextnode > 1)
					{
						this.CursorSetzen(this.AktNode, this.PosAmNode, this.PosImTextnode - 1);
					}
					else
					{
						this.CursorSetzen(this.AktNode, XMLCursorPositionen.CursorVorDemNode);
					}
					goto IL_01b9;
				}
				throw new ApplicationException(string.Format("XMLCursorPos.MoveLeft: CursorPos ist XMLCursorPositionen.CursorInnerhalbDesTextNodes, es ist aber kein Textnode gewählt, sondern der Node {0}", aktNode.OuterXml));
			default:
				{
					throw new ApplicationException(string.Format("XMLCursorPos.MoveLeft: Unbekannte CursorPos {0}", this.PosAmNode));
				}
				IL_01b9:
				return true;
			}
		}

		public bool MoveRight(XmlNode rootnode, XMLRegelwerk regelwerk)
		{
			XmlNode aktNode = this.AktNode;
			switch (this.PosAmNode)
			{
			case XMLCursorPositionen.CursorAufNodeSelbstVorderesTag:
			case XMLCursorPositionen.CursorAufNodeSelbstHinteresTag:
				this.CursorSetzen(this.AktNode, XMLCursorPositionen.CursorHinterDemNode);
				goto IL_0208;
			case XMLCursorPositionen.CursorHinterDemNode:
				if (aktNode.NextSibling != null)
				{
					this.CursorSetzen(aktNode.NextSibling, XMLCursorPositionen.CursorVorDemNode);
					this.MoveRight(rootnode, regelwerk);
					goto IL_0208;
				}
				if (aktNode.ParentNode != rootnode)
				{
					this.CursorSetzen(aktNode.ParentNode, XMLCursorPositionen.CursorHinterDemNode);
					if (!regelwerk.IstSchliessendesTagSichtbar(aktNode.ParentNode))
					{
						this.MoveRight(rootnode, regelwerk);
					}
					goto IL_0208;
				}
				return false;
			case XMLCursorPositionen.CursorInDemLeeremNode:
				this.CursorSetzen(this.AktNode, XMLCursorPositionen.CursorHinterDemNode);
				goto IL_0208;
			case XMLCursorPositionen.CursorVorDemNode:
				if (ToolboxXML.IstTextOderKommentarNode(aktNode))
				{
					if (ToolboxXML.TextAusTextNodeBereinigt(aktNode).Length > 1)
					{
						this.CursorSetzen(this.AktNode, XMLCursorPositionen.CursorInnerhalbDesTextNodes, 1);
					}
					else
					{
						this.CursorSetzen(this.AktNode, XMLCursorPositionen.CursorHinterDemNode);
					}
				}
				else if (aktNode.ChildNodes.Count < 1)
				{
					if (!regelwerk.IstSchliessendesTagSichtbar(aktNode))
					{
						this.CursorSetzen(this.AktNode, XMLCursorPositionen.CursorHinterDemNode);
					}
					else
					{
						this.CursorSetzen(this.AktNode, XMLCursorPositionen.CursorInDemLeeremNode);
					}
				}
				else
				{
					this.CursorSetzen(aktNode.FirstChild, XMLCursorPositionen.CursorVorDemNode);
				}
				goto IL_0208;
			case XMLCursorPositionen.CursorInnerhalbDesTextNodes:
				if (ToolboxXML.IstTextOderKommentarNode(aktNode))
				{
					if (ToolboxXML.TextAusTextNodeBereinigt(aktNode).Length > this.PosImTextnode + 1)
					{
						this.CursorSetzen(this.AktNode, this.PosAmNode, this.PosImTextnode + 1);
					}
					else
					{
						this.CursorSetzen(this.AktNode, XMLCursorPositionen.CursorHinterDemNode);
					}
					goto IL_0208;
				}
				throw new ApplicationException(string.Format("XMLCurorPos.MoveRight: CursorPos ist XMLCursorPositionen.CursorInnerhalbDesTextNodes, es ist aber kein Textnode gewählt, sondern der Node {0}", aktNode.OuterXml));
			default:
				{
					throw new ApplicationException(string.Format("XMLCurorPos.MoveRight: Unbekannte CursorPos {0}", this.PosAmNode));
				}
				IL_0208:
				return true;
			}
		}

		public void CursorSetzen(XmlNode aktNode, XMLCursorPositionen posAmNode, int posImTextnode)
		{
			bool flag = (byte)((aktNode != this._aktNode) ? 1 : ((posAmNode != this._posAmNode) ? 1 : ((posImTextnode != this._posImTextnode) ? 1 : 0))) != 0;
			this._aktNode = aktNode;
			this._posAmNode = posAmNode;
			this._posImTextnode = posImTextnode;
			if (flag)
			{
				this.PosChanged(EventArgs.Empty);
			}
		}

		public void CursorSetzen(XmlNode aktNode, XMLCursorPositionen posAmNode)
		{
			this.CursorSetzen(aktNode, posAmNode, 0);
		}

		public bool IstNodeInnerhalbDerSelektion(XmlNode node)
		{
			if (this._aktNode == null)
			{
				return false;
			}
			if (node == null)
			{
				return false;
			}
			if (node == this._aktNode)
			{
				return this._posAmNode == XMLCursorPositionen.CursorAufNodeSelbstVorderesTag || this._posAmNode == XMLCursorPositionen.CursorAufNodeSelbstHinteresTag;
			}
			return this.IstNodeInnerhalbDerSelektion(node.ParentNode);
		}
	}
}
