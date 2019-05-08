using de.springwald.toolbox;
using de.springwald.xml.cursor;
using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Xml;

namespace de.springwald.xml.editor
{
	public class XMLElement : IDisposable
	{
		private bool _disposed = false;

		protected XMLEditorPaintPos _paintPos;

		protected XMLEditorPaintPos _merkeStartPaintPos;

		protected Point _cursorStrichPos;

		protected int _startX = 0;

		protected int _startY = 0;

		protected XmlNode _xmlNode;

		protected XMLEditor _xmlEditor;

		protected ArrayList _childElemente = new ArrayList();

		protected bool _wirdGeradeGezeichnet;

		protected RectangleCollection _klickBereiche = new RectangleCollection();

		public XmlNode XMLNode
		{
			get
			{
				return this._xmlNode;
			}
		}

		public XMLEditorPaintPos PaintPos
		{
			get
			{
				return this._paintPos;
			}
			set
			{
				this._paintPos = value;
			}
		}

		protected virtual Point AnkerPos
		{
			get
			{
				return new Point(this._startX, this._startY);
			}
		}

		public XMLElement(XmlNode xmlNode, XMLEditor xmlEditor)
		{
			this._xmlNode = xmlNode;
			this._xmlEditor = xmlEditor;
			this._xmlEditor.CursorRoh.ChangedEvent += this.Cursor_ChangedEvent;
			this._xmlEditor.MouseDownEvent += this._xmlEditor_MouseDownEvent;
			this._xmlEditor.MouseUpEvent += this._xmlEditor_MouseUpEvent;
			this._xmlEditor.MouseDownMoveEvent += this._xmlEditor_MouseDownMoveEvent;
			this._xmlEditor.xmlElementeAufraeumenEvent += this._xmlEditor_xmlElementeAufraeumenEvent;
		}

		public virtual void Paint(XMLPaintArten paintArt, int offSetX, int offSetY, PaintEventArgs e)
		{
			if (!this._disposed && this._xmlNode != null && this._xmlEditor != null)
			{
				if (paintArt == XMLPaintArten.Vorberechnen)
				{
					this._merkeStartPaintPos = this._paintPos.Clone();
				}
				else
				{
					this._paintPos = this._merkeStartPaintPos.Clone();
				}
				this._startX = this._paintPos.PosX;
				this._startY = this._paintPos.PosY;
				if (paintArt == XMLPaintArten.Vorberechnen)
				{
					this.MausklickBereicheBufferLeeren();
					this._cursorStrichPos = new Point(this._startX, this._startY);
				}
				this._wirdGeradeGezeichnet = true;
				this.NodeZeichnenStart(paintArt, offSetX, offSetY, e);
				this.UnternodesZeichnen(paintArt, offSetX, offSetY, e);
				this.NodeZeichnenAbschluss(paintArt, offSetX, offSetY, e);
				this._wirdGeradeGezeichnet = false;
			}
		}

		protected virtual void NodeZeichnenStart(XMLPaintArten paintArt, int offSetX, int offSetY, PaintEventArgs e)
		{
		}

		protected virtual void UnternodesZeichnen(XMLPaintArten paintArt, int offSetX, int offSetY, PaintEventArgs e)
		{
			if (!(this._xmlNode is XmlText))
			{
				if (this._xmlNode == null)
				{
					throw new ApplicationException("UnternodesZeichnen:XMLNode ist leer");
				}
				this._paintPos.PosX += this._xmlEditor.Regelwerk.AbstandFliessElementeX;
				switch (this._xmlEditor.Regelwerk.DarstellungsArt(this._xmlNode))
				{
				case DarstellungsArten.EigeneZeile:
					this._paintPos.ZeilenStartX = this._paintPos.PosX;
					break;
				}
				for (int i = 0; i < this._xmlNode.ChildNodes.Count; i++)
				{
					XMLElement xMLElement;
					if (i >= this._childElemente.Count)
					{
						xMLElement = this._xmlEditor.createElement(this._xmlNode.ChildNodes[i]);
						this._childElemente.Add(xMLElement);
					}
					else
					{
						xMLElement = (XMLElement)this._childElemente[i];
						if (xMLElement == null)
						{
							throw new ApplicationException(string.Format("UnternodesZeichnen:childElement ist leer: PaintArt:{0} outerxml:{1} >> innerxml {2}", paintArt, this._xmlNode.OuterXml, this._xmlNode.InnerXml));
						}
						if (xMLElement.XMLNode != this._xmlNode.ChildNodes[i] && paintArt == XMLPaintArten.Vorberechnen)
						{
							xMLElement.Dispose();
							xMLElement = this._xmlEditor.createElement(this._xmlNode.ChildNodes[i]);
							this._childElemente[i] = xMLElement;
						}
					}
					switch (this._xmlEditor.Regelwerk.DarstellungsArt(xMLElement.XMLNode))
					{
					case DarstellungsArten.Fliesselement:
					{
						if (this._paintPos.PosX > this._paintPos.ZeilenEndeX)
						{
							this._paintPos.PosY += this._paintPos.HoeheAktZeile + this._xmlEditor.Regelwerk.AbstandYZwischenZeilen;
							this._paintPos.HoeheAktZeile = 0;
							this._paintPos.PosX = this._paintPos.ZeilenStartX;
						}
						XMLEditorPaintPos obj = new XMLEditorPaintPos
						{
							ZeilenStartX = this._paintPos.ZeilenStartX,
							ZeilenEndeX = this._paintPos.ZeilenEndeX,
							PosX = this._paintPos.PosX,
							PosY = this._paintPos.PosY,
							HoeheAktZeile = this._paintPos.HoeheAktZeile
						};
						XMLEditorPaintPos paintPos = xMLElement.PaintPos = obj;
						xMLElement.Paint(paintArt, offSetX, offSetY, e);
						break;
					}
					case DarstellungsArten.EigeneZeile:
					{
						this._paintPos.PosY += this._xmlEditor.Regelwerk.AbstandYZwischenZeilen + this._paintPos.HoeheAktZeile;
						this._paintPos.HoeheAktZeile = 0;
						this._paintPos.PosX = this._startX + this._xmlEditor.Regelwerk.ChildEinrueckungX;
						XMLEditorPaintPos paintPos = new XMLEditorPaintPos
						{
							ZeilenStartX = this._paintPos.ZeilenStartX,
							ZeilenEndeX = this._paintPos.ZeilenEndeX,
							PosX = this._paintPos.PosX,
							PosY = this._paintPos.PosY,
							HoeheAktZeile = this._paintPos.HoeheAktZeile
						};
						if (paintArt != 0)
						{
							using (Pen pen = new Pen(Color.Gray, 1f))
							{
								pen.DashStyle = DashStyle.Dash;
								pen.StartCap = LineCap.SquareAnchor;
								pen.EndCap = LineCap.NoAnchor;
								Graphics graphics = e.Graphics;
								Pen pen2 = pen;
								Point ankerPos = this.AnkerPos;
								int x = ankerPos.X;
								ankerPos = this.AnkerPos;
								int y = ankerPos.Y;
								ankerPos = this.AnkerPos;
								int x2 = ankerPos.X;
								ankerPos = xMLElement.AnkerPos;
								graphics.DrawLine(pen2, x, y, x2, ankerPos.Y);
								pen.StartCap = LineCap.NoAnchor;
								pen.EndCap = LineCap.SquareAnchor;
								Graphics graphics2 = e.Graphics;
								Pen pen3 = pen;
								ankerPos = this.AnkerPos;
								int x3 = ankerPos.X;
								ankerPos = xMLElement.AnkerPos;
								int y2 = ankerPos.Y;
								ankerPos = xMLElement.AnkerPos;
								int x4 = ankerPos.X;
								ankerPos = xMLElement.AnkerPos;
								graphics2.DrawLine(pen3, x3, y2, x4, ankerPos.Y);
							}
						}
						xMLElement.PaintPos = paintPos;
						xMLElement.Paint(paintArt, offSetX, offSetY, e);
						break;
					}
					default:
						MessageBox.Show("undefiniert");
						break;
					}
					this._paintPos.PosX = xMLElement.PaintPos.PosX;
					this._paintPos.PosY = xMLElement.PaintPos.PosY;
					this._paintPos.HoeheAktZeile = xMLElement.PaintPos.HoeheAktZeile;
					this._paintPos.BisherMaxX = Math.Max(this._paintPos.BisherMaxX, xMLElement.PaintPos.BisherMaxX);
				}
				while (this._xmlNode.ChildNodes.Count < this._childElemente.Count)
				{
					XMLElement xMLElement = (XMLElement)this._childElemente[this._childElemente.Count - 1];
					this._childElemente.Remove(this._childElemente[this._childElemente.Count - 1]);
					xMLElement.Dispose();
					this._childElemente.TrimToSize();
				}
			}
		}

		protected virtual void NodeZeichnenAbschluss(XMLPaintArten paintArt, int offSetX, int offSetY, PaintEventArgs e)
		{
			this.ZeichneCursorStrich(e);
		}

		protected virtual void ZeichneCursorStrich(PaintEventArgs e)
		{
			if (!this._xmlEditor.CursorRoh.IstEtwasSelektiert && this._xmlNode == this._xmlEditor.CursorOptimiert.StartPos.AktNode && this._xmlEditor.CursorOptimiert.StartPos.PosAmNode != XMLCursorPositionen.CursorAufNodeSelbstVorderesTag && this._xmlEditor.CursorOptimiert.StartPos.PosAmNode != XMLCursorPositionen.CursorAufNodeSelbstHinteresTag)
			{
				if (this._xmlEditor.CursorBlinkOn)
				{
					using (Pen pen = new Pen(Color.Black, 2f))
					{
						e.Graphics.DrawLine(pen, this._cursorStrichPos.X, this._cursorStrichPos.Y + 1, this._cursorStrichPos.X, this._cursorStrichPos.Y + 20);
					}
				}
				this._xmlEditor.AktScrollingCursorPos = this._cursorStrichPos;
			}
		}

		private void KlickbereicheAnzeigen(PaintEventArgs e)
		{
			using (Pen pen = new Pen(Color.Red, 1f))
			{
				foreach (Rectangle item in this._klickBereiche)
				{
					e.Graphics.DrawRectangle(pen, item);
				}
			}
		}

		private void MausklickBereicheBufferLeeren()
		{
			this._klickBereiche.Clear();
		}

		private void _xmlEditor_xmlElementeAufraeumenEvent(object sender, EventArgs e)
		{
			if (this._xmlNode == null)
			{
				this.Dispose();
			}
			else if (this._xmlNode.ParentNode == null)
			{
				this.Dispose();
			}
		}

		protected virtual void WurdeAngeklickt(Point point, MausKlickAktionen aktion)
		{
			this._xmlEditor.CursorRoh.CursorPosSetzenDurchMausAktion(this._xmlNode, XMLCursorPositionen.CursorAufNodeSelbstVorderesTag, aktion);
		}

		private void _xmlEditor_MouseDownEvent(object sender, MouseEventArgs e)
		{
			Point point = new Point(e.X, e.Y);
			foreach (Rectangle item in this._klickBereiche)
			{
				if (!item.Contains(point))
				{
					continue;
				}
				this.WurdeAngeklickt(point, MausKlickAktionen.MouseDown);
				break;
			}
		}

		private void _xmlEditor_MouseUpEvent(object sender, MouseEventArgs e)
		{
			Point point = new Point(e.X, e.Y);
			foreach (Rectangle item in this._klickBereiche)
			{
				if (!item.Contains(point))
				{
					continue;
				}
				this.WurdeAngeklickt(point, MausKlickAktionen.MouseUp);
				break;
			}
		}

		private void _xmlEditor_MouseDownMoveEvent(object sender, MouseEventArgs e)
		{
			Point point = new Point(e.X, e.Y);
			foreach (Rectangle item in this._klickBereiche)
			{
				if (!item.Contains(point))
				{
					continue;
				}
				this.WurdeAngeklickt(point, MausKlickAktionen.MouseDownMove);
				break;
			}
		}

		private void Cursor_ChangedEvent(object sender, EventArgs e)
		{
			if (this._xmlNode.ParentNode == null)
			{
				this.Dispose();
			}
			else if (((XMLCursor)sender).StartPos.AktNode == this._xmlNode)
			{
				return;
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this._disposed && disposing)
			{
				this._xmlEditor.CursorRoh.ChangedEvent -= this.Cursor_ChangedEvent;
				this._xmlEditor.MouseDownEvent -= this._xmlEditor_MouseDownEvent;
				this._xmlEditor.MouseUpEvent -= this._xmlEditor_MouseUpEvent;
				this._xmlEditor.MouseDownMoveEvent -= this._xmlEditor_MouseDownMoveEvent;
				this._xmlEditor.xmlElementeAufraeumenEvent -= this._xmlEditor_xmlElementeAufraeumenEvent;
				foreach (XMLElement item in this._childElemente)
				{
					if (item != null)
					{
						item.Dispose();
					}
				}
				this._paintPos = null;
				this._xmlEditor = null;
			}
			this._disposed = true;
		}
	}
}
