using de.springwald.toolbox;
using de.springwald.xml.cursor;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

namespace de.springwald.xml.editor
{
	public class XMLEditor : IDisposable
	{
		public enum UndoSnapshotSetzenOptionen
		{
			ja,
			nein
		}

		private XMLCursor _cursor;

		private XmlNode _rootNode;

		private int _wunschUmbruchXBuffer;

		private Timer _timerCursorBlink;

		private bool _cursorBlinkOn = true;

		private bool _disposed;

		private bool _mausIstGedrueckt = false;

		private int _virtuelleBreite;

		private int _virtuelleHoehe;

		private Point _aktScrollingCursorPos_;

		private VScrollBar _vScrollBar;

		private HScrollBar _hScrollBar;

		private bool _naechsteTasteBeiKeyPressAlsTextAufnehmen = true;

		private bool _naechstesLostFokusVerhindern = false;

		private XMLUndoHandler _undoHandler;

		private Control _zeichnungsSteuerelement;

		private XMLElement _rootElement;

		private bool AktionenMoeglich
		{
			get
			{
				if (this.ReadOnly)
				{
					return false;
				}
				return this._cursor.StartPos.AktNode != null;
			}
		}

		public virtual bool IstEtwasInZwischenablage
		{
			get
			{
				return Clipboard.ContainsText(TextDataFormat.Text);
			}
		}

		public virtual bool IstEtwasSelektiert
		{
			get
			{
				return this.CursorOptimiert.IstEtwasSelektiert;
			}
		}

		public bool ReadOnly
		{
			get;
			set;
		}

		public int WunschUmbruchX_
		{
			get
			{
				return this._zeichnungsSteuerelement.Width - 100;
			}
		}

		public XMLRegelwerk Regelwerk
		{
			get;
			set;
		}

		public XMLCursor CursorOptimiert
		{
			get
			{
				XMLCursor xMLCursor = this._cursor.Clone();
				xMLCursor.SelektionOptimieren();
				return xMLCursor;
			}
		}

		public XMLCursor CursorRoh
		{
			get
			{
				return this._cursor;
			}
		}

		public XmlNode RootNode
		{
			get
			{
				return this._rootNode;
			}
			set
			{
				this._rootNode = value;
				if (this._rootNode == null)
				{
					if (this._rootElement != null)
					{
						this._rootElement.Dispose();
						this._rootElement = null;
					}
					this._zeichnungsSteuerelement.Enabled = false;
				}
				else
				{
					if (this._rootElement != null && this._rootElement.XMLNode != this._rootNode)
					{
						this._rootElement.Dispose();
						this._rootElement = null;
					}
					if (this._rootElement == null)
					{
						this._rootElement = this.createElement(this._rootNode);
					}
					if (this._undoHandler != null && this._undoHandler.RootNode != this._rootNode)
					{
						this._undoHandler.Dispose();
						this._undoHandler = null;
					}
					if (this._undoHandler == null)
					{
						this._undoHandler = new XMLUndoHandler(this._rootNode);
					}
					this._zeichnungsSteuerelement.Enabled = true;
				}
				this.ContentChanged();
			}
		}

		public bool IstRootNodeSelektiert
		{
			get
			{
				if (this.IstEtwasSelektiert)
				{
					XMLCursorPos startPos = this.CursorOptimiert.StartPos;
					if (startPos.AktNode == this.RootNode)
					{
						XMLCursorPositionen posAmNode = startPos.PosAmNode;
						if (posAmNode != XMLCursorPositionen.CursorAufNodeSelbstVorderesTag && posAmNode != XMLCursorPositionen.CursorAufNodeSelbstHinteresTag)
						{
							goto IL_0045;
						}
						return true;
					}
				}
				goto IL_0045;
				IL_0045:
				return false;
			}
		}

		public bool HatFokus
		{
			get
			{
				if (this._zeichnungsSteuerelement == null)
				{
					return false;
				}
				return this._zeichnungsSteuerelement.Focused;
			}
		}

		public bool CursorBlinkOn
		{
			get
			{
				return this._cursorBlinkOn;
			}
			set
			{
				if (this._zeichnungsSteuerelement != null)
				{
					if (this._zeichnungsSteuerelement.Focused)
					{
						this._cursorBlinkOn = value;
					}
					else
					{
						this._cursorBlinkOn = false;
					}
				}
				else
				{
					this._cursorBlinkOn = value;
				}
				this._timerCursorBlink.Enabled = false;
				this._timerCursorBlink.Enabled = true;
			}
		}

		private int ZeichnungsOffsetY
		{
			get
			{
				if (this._vScrollBar.Visible)
				{
					return -this._vScrollBar.Value;
				}
				return 0;
			}
		}

		private int ZeichnungsOffsetX
		{
			get
			{
				if (this._hScrollBar.Visible)
				{
					return -this._hScrollBar.Value;
				}
				return 0;
			}
		}

		public Point AktScrollingCursorPos
		{
			get
			{
				return this._aktScrollingCursorPos_;
			}
			set
			{
				this._aktScrollingCursorPos_ = value;
			}
		}

		public string UndoSchrittName
		{
			get
			{
				if (this.UndoMoeglich)
				{
					return this._undoHandler.NextUndoSnapshotName;
				}
				return ResReader.Reader.GetString("KeinUndoSchrittVerfuegbar");
			}
		}

		public bool UndoMoeglich
		{
			get
			{
				if (this._undoHandler == null)
				{
					return false;
				}
				return this._undoHandler.UndoMoeglich;
			}
		}

		public Control ZeichnungsSteuerelement
		{
			get
			{
				return this._zeichnungsSteuerelement;
			}
		}

		public event EventHandler ContentChangedEvent;

		public event EventHandler xmlElementeAufraeumenEvent;

		public event MouseEventHandler MouseDownEvent;

		public event MouseEventHandler MouseUpEvent;

		public event MouseEventHandler MouseDownMoveEvent;

		public event KeyEventHandler KeyDownEvent;

		public event KeyPressEventHandler KeyPressEvent;

		public virtual bool AktionPasteFromClipboard(UndoSnapshotSetzenOptionen undoSnapshotSetzen)
		{
			if (!this.AktionenMoeglich)
			{
				return false;
			}
			string text = "";
			try
			{
				XMLCursorPos startPos;
				if (Clipboard.ContainsText(TextDataFormat.Text))
				{
					if (this.IstRootNodeSelektiert)
					{
						return this.AktionRootNodeDurchClipboardInhaltErsetzen(undoSnapshotSetzen);
					}
					if (this.IstEtwasSelektiert)
					{
						if (this.AktionDelete(UndoSnapshotSetzenOptionen.nein))
						{
							startPos = this._cursor.StartPos;
							goto IL_008a;
						}
						return false;
					}
					startPos = this.CursorOptimiert.StartPos;
					goto IL_008a;
				}
				return false;
				IL_008a:
				if (undoSnapshotSetzen == UndoSnapshotSetzenOptionen.ja)
				{
					this._undoHandler.SnapshotSetzen(ResReader.Reader.GetString("AktionEinfuegen"), this._cursor);
				}
				text = Clipboard.GetText(TextDataFormat.Text);
				text = text.Replace("\r\n", " ");
				text = text.Replace("\n\r", " ");
				text = text.Replace("\r", " ");
				text = text.Replace("\n", " ");
				text = text.Replace("\t", " ");
				string xmlFragment = string.Format("<paste>{0}</paste>", text);
				XmlTextReader xmlTextReader = new XmlTextReader(xmlFragment, XmlNodeType.Element, null);
				xmlTextReader.MoveToContent();
				XmlNode xmlNode = this._rootNode.OwnerDocument.ReadNode(xmlTextReader);
				XMLCursorPos xMLCursorPos = startPos.Clone();
				foreach (XmlNode childNode in xmlNode.ChildNodes)
				{
					if (childNode is XmlText)
					{
						XmlNode xmlNode3 = null;
						xMLCursorPos.TextEinfuegen(childNode.Clone().Value, this.Regelwerk, out xmlNode3);
						if (xmlNode3 != null)
						{
							xMLCursorPos.InsertXMLNode(xmlNode3.Clone(), this.Regelwerk, true);
						}
					}
					else
					{
						xMLCursorPos.InsertXMLNode(childNode.Clone(), this.Regelwerk, true);
					}
				}
				XMLCursorPositionen posAmNode = this._cursor.EndPos.PosAmNode;
				if (posAmNode == XMLCursorPositionen.CursorVorDemNode || posAmNode == XMLCursorPositionen.CursorInnerhalbDesTextNodes)
				{
					this._cursor.BeideCursorPosSetzen(xMLCursorPos.AktNode, xMLCursorPos.PosAmNode, xMLCursorPos.PosImTextnode);
				}
				else
				{
					this._cursor.BeideCursorPosSetzen(xMLCursorPos.AktNode, XMLCursorPositionen.CursorHinterDemNode);
				}
				return true;
			}
			catch (Exception ex)
			{
				Debugger.GlobalDebugger.Protokolliere(string.Format("AktionPasteFromClipboard:Fehler für Einfügetext '{0}':{1}", text, ex.Message), Debugger.ProtokollTypen.Fehlermeldung);
				return false;
			}
		}

		private void AktionenEnterGedrueckt()
		{
		}

		private bool AktionRootNodeDurchClipboardInhaltErsetzen(UndoSnapshotSetzenOptionen undoSnapshotSetzen)
		{
			if (!this.AktionenMoeglich)
			{
				return false;
			}
			string text = "";
			try
			{
				text = Clipboard.GetText(TextDataFormat.Text);
				XmlTextReader xmlTextReader = new XmlTextReader(text, XmlNodeType.Element, null);
				xmlTextReader.MoveToContent();
				XmlNode xmlNode = this._rootNode.OwnerDocument.ReadNode(xmlTextReader);
				if (xmlNode.Name != this._rootNode.Name)
				{
					return false;
				}
				if (undoSnapshotSetzen == UndoSnapshotSetzenOptionen.ja)
				{
					this._undoHandler.SnapshotSetzen(ResReader.Reader.GetString("RootNodedurchZwischenablageersetzen"), this._cursor);
				}
				this._rootNode.RemoveAll();
				while (xmlNode.Attributes.Count > 0)
				{
					XmlAttribute node = xmlNode.Attributes.Remove(xmlNode.Attributes[0]);
					this._rootNode.Attributes.Append(node);
				}
				XMLCursorPos xMLCursorPos = new XMLCursorPos();
				xMLCursorPos.CursorSetzen(this._rootNode, XMLCursorPositionen.CursorInDemLeeremNode);
				XMLCursorPos xMLCursorPos2 = xMLCursorPos.Clone();
				while (xmlNode.ChildNodes.Count > 0)
				{
					XmlNode newChild = xmlNode.RemoveChild(xmlNode.FirstChild);
					this._rootNode.AppendChild(newChild);
				}
				this.ContentChanged();
				this._cursor.BeideCursorPosSetzen(this._rootNode, XMLCursorPositionen.CursorAufNodeSelbstVorderesTag);
				this._cursor.ErzwingeChanged();
				return true;
			}
			catch (Exception ex)
			{
				Debugger.GlobalDebugger.Protokolliere(string.Format("AktionRootNodeDurchClipboardInhaltErsetzen:Fehler für Einfügetext '{0}':{1}", text, ex.Message), Debugger.ProtokollTypen.Fehlermeldung);
				return false;
			}
		}

		public virtual bool AktionCopyToClipboard()
		{
			if (!this.AktionenMoeglich)
			{
				return false;
			}
			string selektionAlsString = this._cursor.SelektionAlsString;
			if (string.IsNullOrEmpty(selektionAlsString))
			{
				return false;
			}
			try
			{
				Clipboard.Clear();
				Clipboard.SetText(selektionAlsString, TextDataFormat.Text);
			}
			catch (Exception)
			{
				return false;
			}
			return true;
		}

		public virtual bool AktionCursorAufPos1()
		{
			if (this._rootNode == null)
			{
				return false;
			}
			if (this._rootNode.FirstChild != null)
			{
				this._cursor.BeideCursorPosSetzen(this._rootNode.FirstChild, XMLCursorPositionen.CursorVorDemNode);
			}
			else
			{
				this._cursor.BeideCursorPosSetzen(this._rootNode, XMLCursorPositionen.CursorInDemLeeremNode);
			}
			return true;
		}

		public virtual bool AktionAllesMarkieren()
		{
			this._cursor.BeideCursorPosSetzen(this._rootNode, XMLCursorPositionen.CursorAufNodeSelbstVorderesTag);
			return true;
		}

		public virtual bool AktionCutToClipboard(UndoSnapshotSetzenOptionen undoSnapshotSetzen)
		{
			if (!this.AktionenMoeglich)
			{
				return false;
			}
			if (this._cursor != this._cursor)
			{
				goto IL_0026;
			}
			goto IL_0026;
			IL_0026:
			if (this.CursorOptimiert.StartPos.AktNode == this._rootNode)
			{
				return false;
			}
			if (this.AktionCopyToClipboard())
			{
				if (this.AktionDelete(UndoSnapshotSetzenOptionen.ja))
				{
					return true;
				}
				return false;
			}
			return false;
		}

		public virtual bool AktionDelete(UndoSnapshotSetzenOptionen undoSnapshotSetzen)
		{
			if (!this.AktionenMoeglich)
			{
				return false;
			}
			if (this.IstRootNodeSelektiert)
			{
				return false;
			}
			if (undoSnapshotSetzen == UndoSnapshotSetzenOptionen.ja)
			{
				this._undoHandler.SnapshotSetzen(ResReader.Reader.GetString("AktionLoeschen"), this._cursor);
			}
			XMLCursor cursor = this._cursor;
			cursor.SelektionOptimieren();
			XMLCursorPos xMLCursorPos = default(XMLCursorPos);
			if (cursor.SelektionLoeschen(out xMLCursorPos))
			{
				this._cursor.BeideCursorPosSetzen(xMLCursorPos.AktNode, xMLCursorPos.PosAmNode, xMLCursorPos.PosImTextnode);
				this.ContentChanged();
				return true;
			}
			return false;
		}

		public virtual bool AktionTextAnCursorPosEinfuegen(string einfuegeText, UndoSnapshotSetzenOptionen undoSnapshotSetzen)
		{
			if (!this.AktionenMoeglich)
			{
				return false;
			}
			if (undoSnapshotSetzen == UndoSnapshotSetzenOptionen.ja)
			{
				this._undoHandler.SnapshotSetzen(string.Format(ResReader.Reader.GetString("AktionSchreiben"), einfuegeText), this._cursor);
			}
			this._cursor.TextEinfuegen(einfuegeText, this.Regelwerk);
			this.ContentChanged();
			return true;
		}

		public virtual bool AktionAttributWertInNodeSetzen(XmlNode node, string attributName, string wert, UndoSnapshotSetzenOptionen undoSnapshotSetzen)
		{
			if (!this.AktionenMoeglich)
			{
				return false;
			}
			XmlAttribute xmlAttribute = node.Attributes[attributName];
			if (wert == "")
			{
				if (xmlAttribute != null)
				{
					if (undoSnapshotSetzen == UndoSnapshotSetzenOptionen.ja)
					{
						this._undoHandler.SnapshotSetzen(string.Format(ResReader.Reader.GetString("AktionAttributGeloescht"), attributName, node.Name), this._cursor);
					}
					node.Attributes.Remove(xmlAttribute);
				}
			}
			else if (xmlAttribute == null)
			{
				if (undoSnapshotSetzen == UndoSnapshotSetzenOptionen.ja)
				{
					this._undoHandler.SnapshotSetzen(string.Format(ResReader.Reader.GetString("AktionAttributValueGeaendert"), attributName, node.Name, wert), this._cursor);
				}
				xmlAttribute = node.OwnerDocument.CreateAttribute(attributName);
				node.Attributes.Append(xmlAttribute);
				xmlAttribute.Value = wert;
			}
			else if (xmlAttribute.Value != wert)
			{
				if (undoSnapshotSetzen == UndoSnapshotSetzenOptionen.ja)
				{
					this._undoHandler.SnapshotSetzen(string.Format(ResReader.Reader.GetString("AktionAttributValueGeaendert"), attributName, node.Name, wert), this._cursor);
				}
				xmlAttribute.Value = wert;
			}
			this.ContentChanged();
			return true;
		}

		public bool AktionNodeOderZeichenVorDerCursorPosLoeschen(XMLCursorPos position, UndoSnapshotSetzenOptionen undoSnapshotSetzen)
		{
			if (!this.AktionenMoeglich)
			{
				return false;
			}
			XMLCursor xMLCursor = new XMLCursor();
			xMLCursor.StartPos.CursorSetzen(position.AktNode, position.PosAmNode, position.PosImTextnode);
			XMLCursorPos xMLCursorPos = xMLCursor.StartPos.Clone();
			xMLCursorPos.MoveLeft(this._rootNode, this.Regelwerk);
			xMLCursor.EndPos.CursorSetzen(xMLCursorPos.AktNode, xMLCursorPos.PosAmNode, xMLCursorPos.PosImTextnode);
			xMLCursor.SelektionOptimieren();
			if (xMLCursor.StartPos.AktNode == this._rootNode)
			{
				return false;
			}
			if (undoSnapshotSetzen == UndoSnapshotSetzenOptionen.ja)
			{
				this._undoHandler.SnapshotSetzen(ResReader.Reader.GetString("AktionLoeschen"), this._cursor);
			}
			XMLCursorPos xMLCursorPos2 = default(XMLCursorPos);
			if (xMLCursor.SelektionLoeschen(out xMLCursorPos2))
			{
				this._cursor.BeideCursorPosSetzen(xMLCursorPos2.AktNode, xMLCursorPos2.PosAmNode, xMLCursorPos2.PosImTextnode);
				this.ContentChanged();
				return true;
			}
			return false;
		}

		public bool AktionNodeOderZeichenHinterCursorPosLoeschen(XMLCursorPos position, UndoSnapshotSetzenOptionen undoSnapshotSetzen)
		{
			if (!this.AktionenMoeglich)
			{
				return false;
			}
			if (undoSnapshotSetzen == UndoSnapshotSetzenOptionen.ja)
			{
				this._undoHandler.SnapshotSetzen(ResReader.Reader.GetString("AktionLoeschen"), this._cursor);
			}
			XMLCursor xMLCursor = new XMLCursor();
			xMLCursor.StartPos.CursorSetzen(position.AktNode, position.PosAmNode, position.PosImTextnode);
			XMLCursorPos xMLCursorPos = xMLCursor.StartPos.Clone();
			xMLCursorPos.MoveRight(this._rootNode, this.Regelwerk);
			xMLCursor.EndPos.CursorSetzen(xMLCursorPos.AktNode, xMLCursorPos.PosAmNode, xMLCursorPos.PosImTextnode);
			xMLCursor.SelektionOptimieren();
			if (xMLCursor.StartPos.AktNode == this._rootNode)
			{
				return false;
			}
			XMLCursorPos xMLCursorPos2 = default(XMLCursorPos);
			if (xMLCursor.SelektionLoeschen(out xMLCursorPos2))
			{
				this._cursor.BeideCursorPosSetzen(xMLCursorPos2.AktNode, xMLCursorPos2.PosAmNode, xMLCursorPos2.PosImTextnode);
				this._cursor.ErzwingeChanged();
				this.ContentChanged();
				return true;
			}
			return false;
		}

		public virtual XmlNode AktionNeuesElementAnAktCursorPosEinfuegen(string nodeName, UndoSnapshotSetzenOptionen undoSnapshotSetzen, bool neueCursorPosAufJedenFallHinterDenEingefuegtenNodeSetzen)
		{
			if (!this.AktionenMoeglich)
			{
				return null;
			}
			if (nodeName == "")
			{
				throw new ApplicationException(ResReader.Reader.GetString("KeinNodeNameAngegeben"));
			}
			if (undoSnapshotSetzen == UndoSnapshotSetzenOptionen.ja)
			{
				this._undoHandler.SnapshotSetzen(string.Format(ResReader.Reader.GetString("AktionInsertNode"), nodeName), this._cursor);
			}
			XmlNode xmlNode = (!(nodeName == "#COMMENT")) ? this._rootNode.OwnerDocument.CreateNode(XmlNodeType.Element, nodeName, null) : this._rootNode.OwnerDocument.CreateComment("NEW COMMENT");
			this._cursor.XMLNodeEinfuegen(xmlNode, this.Regelwerk, neueCursorPosAufJedenFallHinterDenEingefuegtenNodeSetzen);
			this.ContentChanged();
			return xmlNode;
		}

		protected virtual void ContentChanged()
		{
			if (this.ContentChangedEvent != null)
			{
				this.ContentChangedEvent(this, EventArgs.Empty);
			}
			if (this._zeichnungsSteuerelement != null)
			{
				this._zeichnungsSteuerelement.Invalidate();
			}
			this.CursorBlinkOn = true;
			this.xmlElementeAufraeumen();
		}

		public XMLEditor(XMLRegelwerk regelwerk, Control zeichnungsSteuerelement)
		{
			this.Regelwerk = regelwerk;
			this._zeichnungsSteuerelement = zeichnungsSteuerelement;
			this._zeichnungsSteuerelement.Enabled = false;
			this._cursor = new XMLCursor();
			this._cursor.ChangedEvent += this._cursor_ChangedEvent;
			this.MausEventsAnmelden();
			this.TastaturEventsAnmelden(zeichnungsSteuerelement);
			this.InitCursorBlink();
			this.InitScrolling();
		}

		public virtual XMLElement createElement(XmlNode xmlNode)
		{
			return this.Regelwerk.createPaintElementForNode(xmlNode, this);
		}

		private void _cursor_ChangedEvent(object sender, EventArgs e)
		{
			this.ScrollingNotwendig();
			if (this._zeichnungsSteuerelement != null)
			{
				this._zeichnungsSteuerelement.Invalidate();
			}
			this.CursorBlinkOn = true;
		}

		private void InitCursorBlink()
		{
			this._timerCursorBlink = new Timer();
			this._timerCursorBlink.Enabled = this._cursorBlinkOn;
			this._timerCursorBlink.Interval = 600;
			this._timerCursorBlink.Tick += this._timerCursorBlink_Tick;
		}

		private void _timerCursorBlink_Tick(object sender, EventArgs e)
		{
			if (this._zeichnungsSteuerelement != null)
			{
				if (this.HatFokus)
				{
					this._cursorBlinkOn = !this._cursorBlinkOn;
					this._zeichnungsSteuerelement.Invalidate();
				}
				else if (this._cursorBlinkOn)
				{
					this._cursorBlinkOn = false;
					this._zeichnungsSteuerelement.Invalidate();
				}
			}
		}

		protected virtual void xmlElementeAufraeumen()
		{
			if (this.xmlElementeAufraeumenEvent != null)
			{
				this.xmlElementeAufraeumenEvent(this, EventArgs.Empty);
			}
		}

		public void Dispose()
		{
			if (!this._disposed)
			{
				this.xmlElementeAufraeumen();
				this._disposed = true;
			}
		}

		protected virtual void MouseDown(MouseEventArgs e)
		{
			if (this.MouseDownEvent != null)
			{
				this.MouseDownEvent(this, e);
			}
		}

		protected virtual void MouseUp(MouseEventArgs e)
		{
			if (this.MouseUpEvent != null)
			{
				this.MouseUpEvent(this, e);
			}
		}

		protected virtual void MouseDownMove(MouseEventArgs e)
		{
			if (this.MouseDownMoveEvent != null)
			{
				this.MouseDownMoveEvent(this, e);
			}
		}

		private void MausEventsAnmelden()
		{
			this._zeichnungsSteuerelement.MouseDown += this._zeichnungsSteuerelement_MouseDown;
			this._zeichnungsSteuerelement.MouseMove += this._zeichnungsSteuerelement_MouseMove;
			this._zeichnungsSteuerelement.MouseUp += this._zeichnungsSteuerelement_MouseUp;
		}

		private void _zeichnungsSteuerelement_MouseDown(object sender, MouseEventArgs e)
		{
			this._mausIstGedrueckt = true;
			this.MouseDown(e);
		}

		private void _zeichnungsSteuerelement_MouseUp(object sender, MouseEventArgs e)
		{
			this._mausIstGedrueckt = false;
			this.MouseUp(e);
		}

		private void _zeichnungsSteuerelement_MouseMove(object sender, MouseEventArgs e)
		{
			if (this._mausIstGedrueckt)
			{
				this.MouseDownMove(e);
			}
		}

		private void InitScrolling()
		{
			if (this._zeichnungsSteuerelement != null)
			{
				this._vScrollBar = new VScrollBar();
				this._hScrollBar = new HScrollBar();
				this._zeichnungsSteuerelement.Controls.Add(this._vScrollBar);
				this._zeichnungsSteuerelement.Controls.Add(this._hScrollBar);
				this._vScrollBar.ValueChanged += this._vScrollBar_ValueChanged;
				this._hScrollBar.ValueChanged += this._hScrollBar_ValueChanged;
				this._zeichnungsSteuerelement.Resize += this._zeichnungsSteuerelement_Resize;
				this._zeichnungsSteuerelement_Resize(null, null);
				this._zeichnungsSteuerelement.MouseWheel += this._zeichnungsSteuerelement_MouseWheel;
			}
		}

		private void ScrollingNotwendig()
		{
			this.DoTheScrollIntern();
		}

		private void DoTheScrollIntern()
		{
			this.ScrollbarsAnzeigen();
			this.CursorInSichtbarenBereichScrollen();
		}

		private void _hScrollBar_ValueChanged(object sender, EventArgs e)
		{
			this._zeichnungsSteuerelement.Invalidate();
		}

		private void _vScrollBar_ValueChanged(object sender, EventArgs e)
		{
			this._zeichnungsSteuerelement.Invalidate();
		}

		private void _zeichnungsSteuerelement_MouseWheel(object sender, MouseEventArgs e)
		{
			if (this._vScrollBar.Visible)
			{
				this._vScrollBar.Value = Math.Max(0, Math.Min(this._vScrollBar.Maximum - this._vScrollBar.LargeChange, Math.Max(0, this._vScrollBar.Value - e.Delta)));
			}
		}

		private void ScrollbarsAnzeigen()
		{
			if (this._vScrollBar.Height > this._virtuelleHoehe + 20)
			{
				this._vScrollBar.Value = 0;
				this._vScrollBar.Visible = false;
			}
			else
			{
				this._vScrollBar.Visible = true;
				this._vScrollBar.Maximum = this._virtuelleHoehe;
				this._vScrollBar.LargeChange = this._zeichnungsSteuerelement.Height;
			}
			if (this._hScrollBar.Width > this._virtuelleBreite + 20)
			{
				this._hScrollBar.Value = 0;
				this._hScrollBar.Visible = false;
			}
			else
			{
				this._hScrollBar.Visible = true;
				this._hScrollBar.Maximum = this._virtuelleBreite;
				this._hScrollBar.LargeChange = this._zeichnungsSteuerelement.Width;
			}
			this._vScrollBar.Top = 0;
			this._vScrollBar.Left = this._zeichnungsSteuerelement.Width - this._vScrollBar.Width;
			if (this._hScrollBar.Visible)
			{
				this._vScrollBar.Height = this._zeichnungsSteuerelement.Height - this._hScrollBar.Height;
			}
			else
			{
				this._vScrollBar.Height = this._zeichnungsSteuerelement.Height;
			}
			this._hScrollBar.Left = 0;
			this._hScrollBar.Top = this._zeichnungsSteuerelement.Height - this._hScrollBar.Height;
			if (this._vScrollBar.Visible)
			{
				this._hScrollBar.Width = this._zeichnungsSteuerelement.Width - this._vScrollBar.Width;
			}
			else
			{
				this._hScrollBar.Width = this._zeichnungsSteuerelement.Width;
			}
		}

		public void CursorInSichtbarenBereichScrollen()
		{
			Point aktScrollingCursorPos;
			if (this._hScrollBar.Visible)
			{
				aktScrollingCursorPos = this.AktScrollingCursorPos;
				int x = aktScrollingCursorPos.X;
				if (x < 0)
				{
					this._hScrollBar.Value = Math.Max(0, this._hScrollBar.Value + x);
				}
				else
				{
					aktScrollingCursorPos = this.AktScrollingCursorPos;
					int num = aktScrollingCursorPos.X + -this._hScrollBar.Width;
					if (num > 0)
					{
						this._hScrollBar.Value = Math.Max(0, Math.Min(this._virtuelleBreite - this._hScrollBar.LargeChange, this._hScrollBar.Value + num));
					}
				}
			}
			if (this._vScrollBar.Visible)
			{
				aktScrollingCursorPos = this.AktScrollingCursorPos;
				int y = aktScrollingCursorPos.Y;
				if (y < 0)
				{
					this._vScrollBar.Value = Math.Max(0, this._vScrollBar.Value + y);
				}
				else
				{
					aktScrollingCursorPos = this.AktScrollingCursorPos;
					int num2 = aktScrollingCursorPos.Y + 20 + -this._vScrollBar.Height;
					if (num2 > 0)
					{
						this._vScrollBar.Value = Math.Max(0, Math.Min(this._virtuelleHoehe - this._vScrollBar.LargeChange, this._vScrollBar.Value + num2));
					}
				}
			}
		}

		private void _zeichnungsSteuerelement_Resize(object sender, EventArgs e)
		{
			this.ScrollbarsAnzeigen();
		}

		protected virtual void KeyDown(KeyEventArgs e)
		{
			if (this.KeyDownEvent != null)
			{
				this.KeyDownEvent(this, e);
			}
		}

		protected virtual void KeyPress(KeyPressEventArgs e)
		{
			if (this.KeyPressEvent != null)
			{
				this.KeyPressEvent(this, e);
			}
		}

		private void TastaturEventsAnmelden(Control zeichnungsSteuerelement)
		{
			zeichnungsSteuerelement.Leave += this.zeichnungsSteuerelement_Leave;
			zeichnungsSteuerelement.PreviewKeyDown += this.zeichnungsSteuerelement_PreviewKeyDown;
			zeichnungsSteuerelement.KeyPress += this.zeichnungsSteuerelement_KeyPress;
		}

		private void zeichnungsSteuerelement_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			if (!this.Regelwerk.PreviewKeyDown(e, out this._naechsteTasteBeiKeyPressAlsTextAufnehmen, this))
			{
				this._naechsteTasteBeiKeyPressAlsTextAufnehmen = false;
				switch (e.KeyData)
				{
				case Keys.Escape:
				case Keys.Control:
					break;
				case Keys.Return:
				case Keys.LButton | Keys.MButton | Keys.Back | Keys.Shift:
					this.AktionenEnterGedrueckt();
					break;
				case Keys.Tab:
				{
					XmlNode xmlNode = this._cursor.StartPos.AktNode;
					bool flag = false;
					if (xmlNode.FirstChild != null)
					{
						xmlNode = xmlNode.FirstChild;
					}
					else if (xmlNode.NextSibling != null)
					{
						xmlNode = xmlNode.NextSibling;
					}
					else if (xmlNode.ParentNode.NextSibling != null)
					{
						xmlNode = xmlNode.ParentNode.NextSibling;
					}
					else
					{
						flag = true;
					}
					if (!flag)
					{
						this._cursor.BeideCursorPosSetzen(xmlNode, XMLCursorPositionen.CursorInDemLeeremNode);
					}
					this._naechstesLostFokusVerhindern = true;
					break;
				}
				case Keys.LButton | Keys.MButton | Keys.Space | Keys.Shift:
					this._cursor.EndPos.MoveLeft(this._rootNode, this.Regelwerk);
					break;
				case Keys.LButton | Keys.RButton | Keys.MButton | Keys.Space | Keys.Shift:
					this._cursor.EndPos.MoveRight(this._rootNode, this.Regelwerk);
					break;
				case (Keys)131137:
					this.AktionAllesMarkieren();
					break;
				case (Keys)131160:
					this.AktionCutToClipboard(UndoSnapshotSetzenOptionen.ja);
					break;
				case (Keys)131139:
					this.AktionCopyToClipboard();
					break;
				case (Keys)131158:
					this.AktionPasteFromClipboard(UndoSnapshotSetzenOptionen.ja);
					break;
				case Keys.Home:
					this.AktionCursorAufPos1();
					break;
				case (Keys)131162:
					this.UnDo();
					break;
				case Keys.Left:
				{
					XMLCursorPos xMLCursorPos = this._cursor.StartPos.Clone();
					xMLCursorPos.MoveLeft(this._rootNode, this.Regelwerk);
					this._cursor.BeideCursorPosSetzen(xMLCursorPos.AktNode, xMLCursorPos.PosAmNode, xMLCursorPos.PosImTextnode);
					break;
				}
				case Keys.Right:
				{
					XMLCursorPos xMLCursorPos = this._cursor.StartPos.Clone();
					xMLCursorPos.MoveRight(this._rootNode, this.Regelwerk);
					this._cursor.BeideCursorPosSetzen(xMLCursorPos.AktNode, xMLCursorPos.PosAmNode, xMLCursorPos.PosImTextnode);
					break;
				}
				case Keys.Back:
				case Keys.Back | Keys.Shift:
					if (this._cursor.IstEtwasSelektiert)
					{
						this.AktionDelete(UndoSnapshotSetzenOptionen.ja);
					}
					else
					{
						this.AktionNodeOderZeichenVorDerCursorPosLoeschen(this._cursor.StartPos, UndoSnapshotSetzenOptionen.ja);
					}
					break;
				case Keys.Delete:
				case Keys.RButton | Keys.MButton | Keys.Back | Keys.Space | Keys.Shift:
					if (this._cursor.IstEtwasSelektiert)
					{
						this.AktionDelete(UndoSnapshotSetzenOptionen.ja);
					}
					else
					{
						this.AktionNodeOderZeichenHinterCursorPosLoeschen(this._cursor.StartPos, UndoSnapshotSetzenOptionen.ja);
					}
					break;
				default:
					this._naechsteTasteBeiKeyPressAlsTextAufnehmen = true;
					break;
				}
			}
		}

		private void zeichnungsSteuerelement_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (this._naechsteTasteBeiKeyPressAlsTextAufnehmen)
			{
				this.AktionTextAnCursorPosEinfuegen(e.KeyChar.ToString(), UndoSnapshotSetzenOptionen.ja);
			}
			this._naechsteTasteBeiKeyPressAlsTextAufnehmen = false;
		}

		private void zeichnungsSteuerelement_Leave(object sender, EventArgs e)
		{
			if (this._naechstesLostFokusVerhindern)
			{
				this._naechstesLostFokusVerhindern = false;
				this.ZeichnungsSteuerelement.Focus();
			}
		}

		public void UnDo()
		{
			if (this._undoHandler == null)
			{
				throw new ApplicationException("No Undo-Handler attached, but Undo invoked!");
			}
			XMLCursor xMLCursor = this._undoHandler.Undo();
			if (xMLCursor != null)
			{
				this._cursor.StartPos.CursorSetzen(xMLCursor.StartPos.AktNode, xMLCursor.StartPos.PosAmNode, xMLCursor.StartPos.PosImTextnode);
				this._cursor.EndPos.CursorSetzen(xMLCursor.EndPos.AktNode, xMLCursor.EndPos.PosAmNode, xMLCursor.EndPos.PosImTextnode);
			}
			this.ContentChanged();
		}

		public void Paint(PaintEventArgs e)
		{
			if (this._rootElement != null)
			{
				XMLEditorPaintPos xMLEditorPaintPos = new XMLEditorPaintPos();
				xMLEditorPaintPos.PosX = 10 + this.ZeichnungsOffsetX;
				xMLEditorPaintPos.PosY = 10 + this.ZeichnungsOffsetY;
				xMLEditorPaintPos.ZeilenStartX = 10 + this.ZeichnungsOffsetX;
				xMLEditorPaintPos.ZeilenEndeX = this.WunschUmbruchX_ - this.ZeichnungsOffsetX;
				this._rootElement.PaintPos = xMLEditorPaintPos.Clone();
				this._rootElement.Paint(XMLPaintArten.Vorberechnen, this.ZeichnungsOffsetX, this.ZeichnungsOffsetY, e);
				this._virtuelleBreite = this._rootElement.PaintPos.BisherMaxX + 50 - this.ZeichnungsOffsetX;
				this._virtuelleHoehe = this._rootElement.PaintPos.PosY + 50 - this.ZeichnungsOffsetY;
				this._rootElement.PaintPos = xMLEditorPaintPos;
				this._rootElement.Paint(XMLPaintArten.AllesNeuZeichnenMitFehlerHighlighting, this.ZeichnungsOffsetX, this.ZeichnungsOffsetY, e);
			}
		}

		public void FokusAufEingabeFormularSetzen()
		{
			if (this._zeichnungsSteuerelement != null)
			{
				this._zeichnungsSteuerelement.Focus();
			}
		}
	}
}
