using de.springwald.toolbox;
using System;
using System.Text;
using System.Xml;

namespace de.springwald.xml.cursor
{
	public class XMLCursor
	{
		private XMLCursorPos _startPos;

		private XMLCursorPos _endPos;

		private bool _cursorWirdGeradeGesetzt = false;

		public XMLCursorPos StartPos
		{
			get
			{
				return this._startPos;
			}
		}

		public XMLCursorPos EndPos
		{
			get
			{
				return this._endPos;
			}
		}

		public bool IstEtwasSelektiert
		{
			get
			{
				if (this._startPos.AktNode == null)
				{
					return false;
				}
				if (this._startPos.PosAmNode == XMLCursorPositionen.CursorAufNodeSelbstVorderesTag || this._startPos.PosAmNode == XMLCursorPositionen.CursorAufNodeSelbstHinteresTag)
				{
					return true;
				}
				if (this._startPos.Equals(this._endPos))
				{
					return false;
				}
				return true;
			}
		}

		public string SelektionAlsString
		{
			get
			{
				if (this.IstEtwasSelektiert)
				{
					StringBuilder stringBuilder = new StringBuilder();
					XMLCursor xMLCursor = this.Clone();
					xMLCursor.SelektionOptimieren();
					XmlNode xmlNode = xMLCursor.StartPos.AktNode;
					switch (xMLCursor.StartPos.PosAmNode)
					{
					case XMLCursorPositionen.CursorVorDemNode:
					case XMLCursorPositionen.CursorAufNodeSelbstVorderesTag:
					case XMLCursorPositionen.CursorAufNodeSelbstHinteresTag:
						stringBuilder.Append(xmlNode.OuterXml);
						break;
					case XMLCursorPositionen.CursorInnerhalbDesTextNodes:
					{
						string innerText = xmlNode.InnerText;
						int posImTextnode = xMLCursor.StartPos.PosImTextnode;
						int num = innerText.Length - posImTextnode;
						if (xmlNode == xMLCursor.EndPos.AktNode)
						{
							switch (xMLCursor.EndPos.PosAmNode)
							{
							case XMLCursorPositionen.CursorVorDemNode:
							case XMLCursorPositionen.CursorAufNodeSelbstVorderesTag:
							case XMLCursorPositionen.CursorInDemLeeremNode:
								throw new ApplicationException("XMLCursor.SelektionAlsString: unwahrscheinliche EndPos.PosAmNode '" + xMLCursor.EndPos.PosAmNode + "' für StartPos.CursorInnerhalbDesTextNodes");
							case XMLCursorPositionen.CursorInnerhalbDesTextNodes:
								if (xMLCursor.StartPos.PosImTextnode > xMLCursor.EndPos.PosImTextnode)
								{
									throw new ApplicationException("XMLCursor.SelektionAlsString: optimiert.StartPos.PosImTextnode > optimiert.EndPos.PosImTextnode");
								}
								num -= innerText.Length - xMLCursor.EndPos.PosImTextnode;
								break;
							default:
								throw new ApplicationException("XMLCursor.SelektionAlsString: unbehandelte EndPos.PosAmNode'" + xMLCursor.EndPos.PosAmNode + "' für StartPos.CursorInnerhalbDesTextNodes");
							case XMLCursorPositionen.CursorAufNodeSelbstHinteresTag:
							case XMLCursorPositionen.CursorHinterDemNode:
								break;
							}
						}
						innerText = innerText.Substring(posImTextnode, num);
						stringBuilder.Append(innerText);
						break;
					}
					default:
						throw new ApplicationException("XMLCursor.SelektionAlsString: unbehandelte StartPos.PosAmNode'" + xMLCursor.StartPos.PosAmNode + "'");
					case XMLCursorPositionen.CursorInDemLeeremNode:
					case XMLCursorPositionen.CursorHinterDemNode:
						break;
					}
					if (xMLCursor.StartPos.AktNode != xMLCursor.EndPos.AktNode)
					{
						do
						{
							xmlNode = xmlNode.NextSibling;
							if (xmlNode != null)
							{
								if (xmlNode == xMLCursor.EndPos.AktNode)
								{
									switch (xMLCursor.EndPos.PosAmNode)
									{
									case XMLCursorPositionen.CursorAufNodeSelbstVorderesTag:
									case XMLCursorPositionen.CursorAufNodeSelbstHinteresTag:
									case XMLCursorPositionen.CursorHinterDemNode:
										stringBuilder.Append(xmlNode.OuterXml);
										break;
									case XMLCursorPositionen.CursorInnerhalbDesTextNodes:
									{
										string innerText2 = xmlNode.InnerText;
										stringBuilder.Append(innerText2.Substring(0, xMLCursor.EndPos.PosImTextnode + 1));
										break;
									}
									case XMLCursorPositionen.CursorInDemLeeremNode:
										throw new ApplicationException("XMLCursor.SelektionAlsString: unwahrscheinliche EndPos.PosAmNode '" + xMLCursor.EndPos.PosAmNode + "' für StartPos.Node != EndPos.Node");
									default:
										throw new ApplicationException("XMLCursor.SelektionAlsString: unbehandelte EndPos.PosAmNode'" + xMLCursor.StartPos.PosAmNode + "' für StartPos.Node != EndPos.Node");
									}
								}
								else
								{
									stringBuilder.Append(xmlNode.OuterXml);
								}
							}
						}
						while (xmlNode != xMLCursor.EndPos.AktNode && xmlNode != null);
						if (xmlNode == null)
						{
							throw new ApplicationException("Endnode war nicht als NextSibling von Startnode erreichbar");
						}
					}
					return stringBuilder.ToString();
				}
				return "";
			}
		}

		public event EventHandler ChangedEvent;

		protected virtual void Changed(EventArgs e)
		{
			if (this.ChangedEvent != null)
			{
				this.ChangedEvent(this, e);
			}
		}

		public XMLCursor()
		{
			this._endPos = new XMLCursorPos();
			this._startPos = new XMLCursorPos();
			this.UnterEventsAnmelden();
		}

		public XMLCursor Clone()
		{
			XMLCursor xMLCursor = new XMLCursor();
			xMLCursor.StartPos.CursorSetzen(this._startPos.AktNode, this._startPos.PosAmNode, this._startPos.PosImTextnode);
			xMLCursor.EndPos.CursorSetzen(this._endPos.AktNode, this._endPos.PosAmNode, this._endPos.PosImTextnode);
			return xMLCursor;
		}

		public void ErzwingeChanged()
		{
			this.Changed(EventArgs.Empty);
		}

		public void BeideCursorPosSetzen(XmlNode node, XMLCursorPositionen posAmNode, int posImTextnode)
		{
			bool flag = (byte)((node != this._startPos.AktNode) ? 1 : ((posAmNode != this._startPos.PosAmNode) ? 1 : ((posImTextnode != this._startPos.PosImTextnode) ? 1 : 0))) != 0;
			if (!flag)
			{
				flag = ((byte)((node != this._endPos.AktNode) ? 1 : ((posAmNode != this._endPos.PosAmNode) ? 1 : ((posImTextnode != this._endPos.PosImTextnode) ? 1 : 0))) != 0);
			}
			this._cursorWirdGeradeGesetzt = true;
			this._startPos.CursorSetzen(node, posAmNode, posImTextnode);
			this._endPos.CursorSetzen(node, posAmNode, posImTextnode);
			this._cursorWirdGeradeGesetzt = false;
			if (flag)
			{
				this.Changed(EventArgs.Empty);
			}
		}

		public void BeideCursorPosSetzen(XmlNode node, XMLCursorPositionen posAmNode)
		{
			this.BeideCursorPosSetzen(node, posAmNode, 0);
		}

		public void CursorPosSetzenDurchMausAktion(XmlNode xmlNode, XMLCursorPositionen cursorPos, int posInZeile, MausKlickAktionen aktion)
		{
			switch (aktion)
			{
			case MausKlickAktionen.MouseDown:
				this.BeideCursorPosSetzen(xmlNode, cursorPos, posInZeile);
				break;
			case MausKlickAktionen.MouseDownMove:
			case MausKlickAktionen.MouseUp:
				this.EndPos.CursorSetzen(xmlNode, cursorPos, posInZeile);
				break;
			}
		}

		public void CursorPosSetzenDurchMausAktion(XmlNode xmlNode, XMLCursorPositionen cursorPos, MausKlickAktionen aktion)
		{
			this.CursorPosSetzenDurchMausAktion(xmlNode, cursorPos, 0, aktion);
		}

		private void UnterEventsAnmelden()
		{
			this._endPos.PosChangedEvent += this.endPos_ChangedEvent;
			this._startPos.PosChangedEvent += this.startPos_ChangedEvent;
		}

		private void endPos_ChangedEvent(object sender, EventArgs e)
		{
			if (!this._cursorWirdGeradeGesetzt)
			{
				this.Changed(EventArgs.Empty);
			}
		}

		private void startPos_ChangedEvent(object sender, EventArgs e)
		{
			if (!this._cursorWirdGeradeGesetzt)
			{
				this.Changed(EventArgs.Empty);
			}
		}

		public void TextEinfuegen(string text, XMLRegelwerk regelwerk)
		{
			XMLCursor xMLCursor = this.Clone();
			xMLCursor.SelektionOptimieren();
			XMLCursorPos xMLCursorPos = default(XMLCursorPos);
			XMLCursorPos xMLCursorPos2 = (!xMLCursor.SelektionLoeschen(out xMLCursorPos)) ? this.StartPos.Clone() : xMLCursorPos;
			XmlNode xmlNode = null;
			xMLCursorPos2.TextEinfuegen(text, regelwerk, out xmlNode);
			if (xmlNode != null)
			{
				xMLCursorPos2.InsertXMLNode(xmlNode, regelwerk, false);
			}
			this.BeideCursorPosSetzen(xMLCursorPos2.AktNode, xMLCursorPos2.PosAmNode, xMLCursorPos2.PosImTextnode);
		}

		public void XMLNodeEinfuegen(XmlNode node, XMLRegelwerk regelwerk, bool neueCursorPosAufJedenFallHinterDenEingefuegtenNodeSetzen)
		{
			XMLCursor xMLCursor = this.Clone();
			xMLCursor.SelektionOptimieren();
			XMLCursorPos xMLCursorPos = default(XMLCursorPos);
			if (xMLCursor.SelektionLoeschen(out xMLCursorPos))
			{
				this.BeideCursorPosSetzen(xMLCursorPos.AktNode, xMLCursorPos.PosAmNode, xMLCursorPos.PosImTextnode);
			}
			if (this._startPos.InsertXMLNode(node, regelwerk, neueCursorPosAufJedenFallHinterDenEingefuegtenNodeSetzen))
			{
				this.EndPos.CursorSetzen(this.StartPos.AktNode, this.StartPos.PosAmNode, this.StartPos.PosImTextnode);
			}
		}

		public bool SelektionLoeschen(out XMLCursorPos neueCursorPosNachLoeschen)
		{
			if (!this.IstEtwasSelektiert)
			{
				neueCursorPosNachLoeschen = this.StartPos.Clone();
				return false;
			}
			if (this.StartPos.AktNode == this.EndPos.AktNode)
			{
				switch (this.StartPos.PosAmNode)
				{
				case XMLCursorPositionen.CursorAufNodeSelbstVorderesTag:
				case XMLCursorPositionen.CursorAufNodeSelbstHinteresTag:
				{
					XmlNode aktNode = this.StartPos.AktNode;
					XmlNode previousSibling = aktNode.PreviousSibling;
					XmlNode nextSibling = aktNode.NextSibling;
					neueCursorPosNachLoeschen = new XMLCursorPos();
					if (previousSibling != null && nextSibling != null && previousSibling is XmlText && nextSibling is XmlText)
					{
						neueCursorPosNachLoeschen.CursorSetzen(previousSibling, XMLCursorPositionen.CursorInnerhalbDesTextNodes, previousSibling.InnerText.Length);
						XmlNode xmlNode = previousSibling;
						xmlNode.InnerText += nextSibling.InnerText;
						aktNode.ParentNode.RemoveChild(aktNode);
						nextSibling.ParentNode.RemoveChild(nextSibling);
						return true;
					}
					if (previousSibling != null)
					{
						neueCursorPosNachLoeschen.CursorSetzen(previousSibling, XMLCursorPositionen.CursorHinterDemNode);
					}
					else if (nextSibling != null)
					{
						neueCursorPosNachLoeschen.CursorSetzen(nextSibling, XMLCursorPositionen.CursorVorDemNode);
					}
					else
					{
						neueCursorPosNachLoeschen.CursorSetzen(aktNode.ParentNode, XMLCursorPositionen.CursorInDemLeeremNode);
					}
					aktNode.ParentNode.RemoveChild(aktNode);
					return true;
				}
				case XMLCursorPositionen.CursorVorDemNode:
					if (ToolboxXML.IstTextOderKommentarNode(this.StartPos.AktNode))
					{
						this.StartPos.CursorSetzen(this.StartPos.AktNode, XMLCursorPositionen.CursorInnerhalbDesTextNodes, 0);
						return this.SelektionLoeschen(out neueCursorPosNachLoeschen);
					}
					this.BeideCursorPosSetzen(this.StartPos.AktNode, XMLCursorPositionen.CursorAufNodeSelbstVorderesTag);
					return this.SelektionLoeschen(out neueCursorPosNachLoeschen);
				case XMLCursorPositionen.CursorHinterDemNode:
					if (ToolboxXML.IstTextOderKommentarNode(this.StartPos.AktNode))
					{
						this.StartPos.CursorSetzen(this.StartPos.AktNode, XMLCursorPositionen.CursorInnerhalbDesTextNodes, this.StartPos.AktNode.InnerText.Length);
						return this.SelektionLoeschen(out neueCursorPosNachLoeschen);
					}
					this.BeideCursorPosSetzen(this.StartPos.AktNode, XMLCursorPositionen.CursorAufNodeSelbstVorderesTag);
					return this.SelektionLoeschen(out neueCursorPosNachLoeschen);
				case XMLCursorPositionen.CursorInnerhalbDesTextNodes:
				{
					int posImTextnode = this.StartPos.PosImTextnode;
					int num = this.EndPos.PosImTextnode;
					if (this.EndPos.PosAmNode == XMLCursorPositionen.CursorHinterDemNode)
					{
						num = this.StartPos.AktNode.InnerText.Length;
					}
					if (posImTextnode == 0 && num >= this.StartPos.AktNode.InnerText.Length)
					{
						XMLCursor xMLCursor2 = new XMLCursor();
						xMLCursor2.BeideCursorPosSetzen(this.StartPos.AktNode, XMLCursorPositionen.CursorAufNodeSelbstVorderesTag);
						return xMLCursor2.SelektionLoeschen(out neueCursorPosNachLoeschen);
					}
					string innerText = this.StartPos.AktNode.InnerText;
					innerText = innerText.Remove(posImTextnode, num - posImTextnode);
					this.StartPos.AktNode.InnerText = innerText;
					neueCursorPosNachLoeschen = new XMLCursorPos();
					if (posImTextnode == 0)
					{
						neueCursorPosNachLoeschen.CursorSetzen(this.StartPos.AktNode, XMLCursorPositionen.CursorVorDemNode);
					}
					else
					{
						neueCursorPosNachLoeschen.CursorSetzen(this.StartPos.AktNode, XMLCursorPositionen.CursorInnerhalbDesTextNodes, posImTextnode);
					}
					return true;
				}
				case XMLCursorPositionen.CursorInDemLeeremNode:
					if (this.EndPos.PosAmNode == XMLCursorPositionen.CursorHinterDemNode || this.EndPos.PosAmNode == XMLCursorPositionen.CursorVorDemNode)
					{
						XMLCursor xMLCursor = new XMLCursor();
						xMLCursor.BeideCursorPosSetzen(this.StartPos.AktNode, XMLCursorPositionen.CursorAufNodeSelbstVorderesTag, 0);
						return xMLCursor.SelektionLoeschen(out neueCursorPosNachLoeschen);
					}
					throw new ApplicationException("AuswahlLoeschen:#6363S undefined Endpos " + this.EndPos.PosAmNode + "!");
				default:
					throw new ApplicationException("AuswahlLoeschen:#63346 StartPos.PosAmNode " + this.StartPos.PosAmNode + " not allowed!");
				}
			}
			while (this.StartPos.AktNode.NextSibling != this.EndPos.AktNode)
			{
				this.StartPos.AktNode.ParentNode.RemoveChild(this.StartPos.AktNode.NextSibling);
			}
			XMLCursor xMLCursor3 = this.Clone();
			xMLCursor3.StartPos.CursorSetzen(this.EndPos.AktNode, XMLCursorPositionen.CursorVorDemNode);
			XMLCursorPos xMLCursorPos = default(XMLCursorPos);
			xMLCursor3.SelektionLoeschen(out xMLCursorPos);
			this.EndPos.CursorSetzen(this.StartPos.AktNode, XMLCursorPositionen.CursorHinterDemNode);
			return this.SelektionLoeschen(out neueCursorPosNachLoeschen);
		}

		public void SelektionOptimieren()
		{
			if (this._startPos.AktNode != null)
			{
				if (this._startPos.AktNode == this._endPos.AktNode)
				{
					if (this._startPos.PosAmNode > this._endPos.PosAmNode)
					{
						XMLCursorPositionen posAmNode = this._startPos.PosAmNode;
						int posImTextnode = this._startPos.PosImTextnode;
						this._startPos.CursorSetzen(this._endPos.AktNode, this._endPos.PosAmNode, this._endPos.PosImTextnode);
						this._endPos.CursorSetzen(this._endPos.AktNode, posAmNode, posImTextnode);
					}
					else if (this._startPos.PosAmNode == XMLCursorPositionen.CursorInnerhalbDesTextNodes && this._endPos.PosAmNode == XMLCursorPositionen.CursorInnerhalbDesTextNodes && this._startPos.PosImTextnode > this._endPos.PosImTextnode)
					{
						int posImTextnode = this._startPos.PosImTextnode;
						this._startPos.CursorSetzen(this._startPos.AktNode, XMLCursorPositionen.CursorInnerhalbDesTextNodes, this._endPos.PosImTextnode);
						this._endPos.CursorSetzen(this._startPos.AktNode, XMLCursorPositionen.CursorInnerhalbDesTextNodes, posImTextnode);
					}
				}
				else
				{
					if (ToolboxXML.Node1LiegtVorNode2(this._endPos.AktNode, this._startPos.AktNode))
					{
						XMLCursorPos startPos = this._startPos;
						this._startPos = this._endPos;
						this._endPos = startPos;
					}
					if (ToolboxXML.IstChild(this._endPos.AktNode, this._startPos.AktNode))
					{
						this.BeideCursorPosSetzen(this._startPos.AktNode, XMLCursorPositionen.CursorAufNodeSelbstVorderesTag);
					}
					if (this._startPos.AktNode.ParentNode != this._endPos.AktNode.ParentNode)
					{
						XmlNode xmlNode = this.TiefsterGemeinsamerParent(this._startPos.AktNode, this._endPos.AktNode);
						XmlNode xmlNode2 = this._startPos.AktNode;
						while (xmlNode2.ParentNode != xmlNode)
						{
							xmlNode2 = xmlNode2.ParentNode;
						}
						XmlNode xmlNode3 = this._endPos.AktNode;
						while (xmlNode3.ParentNode != xmlNode)
						{
							xmlNode3 = xmlNode3.ParentNode;
						}
						this._startPos.CursorSetzen(xmlNode2, XMLCursorPositionen.CursorAufNodeSelbstVorderesTag);
						this._endPos.CursorSetzen(xmlNode3, XMLCursorPositionen.CursorAufNodeSelbstVorderesTag);
					}
				}
			}
		}

		public XmlNode TiefsterGemeinsamerParent(XmlNode node1, XmlNode node2)
		{
			for (XmlNode parentNode = node1.ParentNode; parentNode != null; parentNode = parentNode.ParentNode)
			{
				for (XmlNode parentNode2 = node2.ParentNode; parentNode2 != null; parentNode2 = parentNode2.ParentNode)
				{
					if (parentNode == parentNode2)
					{
						return parentNode;
					}
				}
			}
			return null;
		}

		public bool IstNodeInnerhalbDerSelektion(XmlNode node)
		{
			if (this._startPos.IstNodeInnerhalbDerSelektion(node))
			{
				return true;
			}
			if (this._endPos.IstNodeInnerhalbDerSelektion(node))
			{
				return true;
			}
			if (this._startPos.Equals(this._endPos))
			{
				return this._startPos.IstNodeInnerhalbDerSelektion(node);
			}
			if (this._startPos.AktNode == node || this._endPos.AktNode == node)
			{
				if (node is XmlText)
				{
					return true;
				}
				return false;
			}
			if (this._startPos.LiegtNodeHinterDieserPos(node))
			{
				if (this._endPos.LiegtNodeVorDieserPos(node))
				{
					return true;
				}
				return false;
			}
			return false;
		}
	}
}
