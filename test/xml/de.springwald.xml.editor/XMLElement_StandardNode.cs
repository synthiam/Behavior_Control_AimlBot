using de.springwald.toolbox;
using de.springwald.xml.cursor;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace de.springwald.xml.editor
{
	public class XMLElement_StandardNode : XMLElement
	{
		private Color _farbeRahmenHintergrund;

		private Color _farbeRahmenRand;

		private Color _farbeNodeNameSchrift;

		private Color _farbePfeil;

		private Color _farbeAttributeHintergrund;

		private Color _farbeAttributeRand;

		private Color _farbeAttributeSchrift;

		private const int _randX = 4;

		private const int _randY = 2;

		private const int _rundung = 3;

		private const int _pfeilLaenge = 7;

		private const int _pfeilDicke = 7;

		private static StringFormat _drawFormat;

		private static Font _drawFontAttribute;

		private static float _breiteProBuchstabeAttribute;

		private static int _hoeheProBuchstabeAttribut;

		private static Font _drawFontNodeName;

		private static int _hoeheProBuchstabeNodeName;

		private Rectangle _pfeilBereichLinks;

		private Rectangle _pfeilBereichRechts;

		private Rectangle _tagBereichLinks;

		private Rectangle _tagBereichRechts;

		private int _ankerEinzugY = 0;

		private int _rahmenBreite;

		private int _rahmenHoehe;

		protected override Point AnkerPos
		{
			get
			{
				return new Point(base._startX - 4, base._startY + this._ankerEinzugY);
			}
		}

		public XMLElement_StandardNode(XmlNode xmlNode, XMLEditor xmlEditor)
			: base(xmlNode, xmlEditor)
		{
			if (XMLElement_StandardNode._drawFontNodeName == null)
			{
				XMLElement_StandardNode._drawFormat = (StringFormat)StringFormat.GenericTypographic.Clone();
				XMLElement_StandardNode._drawFormat.FormatFlags = (XMLElement_StandardNode._drawFormat.FormatFlags | StringFormatFlags.MeasureTrailingSpaces);
				XMLElement_StandardNode._drawFormat.Trimming = StringTrimming.None;
				XMLElement_StandardNode._drawFontNodeName = new Font("Arial", 10f, GraphicsUnit.Point);
				XMLElement_StandardNode._hoeheProBuchstabeNodeName = XMLElement_StandardNode._drawFontNodeName.Height;
				XMLElement_StandardNode._drawFontAttribute = new Font("Courier New", 8f, GraphicsUnit.Point);
				XMLElement_StandardNode._breiteProBuchstabeAttribute = ToolboxUsercontrols.MeasureDisplayStringWidth(xmlEditor.ZeichnungsSteuerelement.CreateGraphics(), "W", XMLElement_StandardNode._drawFontAttribute, XMLElement_StandardNode._drawFormat);
				XMLElement_StandardNode._hoeheProBuchstabeAttribut = XMLElement_StandardNode._drawFontAttribute.Height;
			}
			this._ankerEinzugY = XMLElement_StandardNode._hoeheProBuchstabeNodeName / 2 + 2;
		}

		protected override void NodeZeichnenStart(XMLPaintArten paintArt, int offSetX, int offSetY, PaintEventArgs e)
		{
			if (paintArt == XMLPaintArten.Vorberechnen && base._xmlEditor.CursorOptimiert.StartPos.AktNode == base._xmlNode && base._xmlEditor.CursorOptimiert.StartPos.PosAmNode == XMLCursorPositionen.CursorVorDemNode)
			{
				base._cursorStrichPos = new Point(base._paintPos.PosX + 1, base._paintPos.PosY);
			}
			int num = (int)ToolboxUsercontrols.MeasureDisplayStringWidth(e.Graphics, base._xmlNode.Name, XMLElement_StandardNode._drawFontNodeName, XMLElement_StandardNode._drawFormat);
			if (paintArt != 0)
			{
				this.FarbenSetzen(paintArt);
				this.zeichneRahmenNachGroesse(base._paintPos.PosX, base._paintPos.PosY, this._rahmenBreite, this._rahmenHoehe, 3, this._farbeRahmenHintergrund, this._farbeRahmenRand, e);
			}
			using (SolidBrush brush = new SolidBrush(this._farbeNodeNameSchrift))
			{
				base._paintPos.PosX += 4;
				if (paintArt != 0)
				{
					e.Graphics.DrawString(base._xmlNode.Name, XMLElement_StandardNode._drawFontNodeName, brush, (float)base._paintPos.PosX, (float)(base._paintPos.PosY + 2), XMLElement_StandardNode._drawFormat);
				}
				base._paintPos.PosX += num + 4;
				this.AttributeZeichnen(paintArt, e);
				this._rahmenBreite = base._paintPos.PosX - base._startX;
				this._rahmenHoehe = XMLElement_StandardNode._hoeheProBuchstabeNodeName + 2 + 2;
				base._paintPos.PosX++;
				if (base._xmlEditor.Regelwerk.IstSchliessendesTagSichtbar(base._xmlNode))
				{
					if (paintArt != 0)
					{
						using (SolidBrush brush2 = new SolidBrush(this._farbePfeil))
						{
							int posX = base._paintPos.PosX;
							int num2 = base._paintPos.PosY + this._ankerEinzugY;
							Point point = new Point(posX, num2 - 7);
							Point point2 = new Point(posX + 7, num2);
							Point point3 = new Point(posX, num2 + 7);
							Point[] points = new Point[3]
							{
								point,
								point2,
								point3
							};
							e.Graphics.FillPolygon(brush2, points);
							this._pfeilBereichLinks = new Rectangle(posX, num2 - 7, 7, 14);
						}
					}
					base._paintPos.PosX += 7;
				}
				else
				{
					this._pfeilBereichLinks = new Rectangle(0, 0, 0, 0);
				}
				if (paintArt == XMLPaintArten.Vorberechnen && base._xmlEditor.CursorOptimiert.StartPos.AktNode == base._xmlNode && base._xmlEditor.CursorOptimiert.StartPos.PosAmNode == XMLCursorPositionen.CursorInDemLeeremNode)
				{
					base._cursorStrichPos = new Point(base._paintPos.PosX - 1, base._paintPos.PosY);
				}
				base._paintPos.HoeheAktZeile = Math.Max(base._paintPos.HoeheAktZeile, this._rahmenHoehe);
				if (paintArt == XMLPaintArten.Vorberechnen)
				{
					this._tagBereichLinks = new Rectangle(base._startX, base._startY, base._paintPos.PosX - base._startX, this._rahmenHoehe);
					base._klickBereiche.Add(this._tagBereichLinks);
					if (!base._xmlEditor.Regelwerk.IstSchliessendesTagSichtbar(base._xmlNode) && base._xmlEditor.CursorOptimiert.StartPos.AktNode == base._xmlNode && base._xmlEditor.CursorOptimiert.StartPos.PosAmNode == XMLCursorPositionen.CursorHinterDemNode)
					{
						base._cursorStrichPos = new Point(base._paintPos.PosX - 1, base._paintPos.PosY);
					}
				}
				base._paintPos.BisherMaxX = Math.Max(base._paintPos.BisherMaxX, base._paintPos.PosX);
			}
		}

		protected override void NodeZeichnenAbschluss(XMLPaintArten paintArt, int offSetX, int offSetY, PaintEventArgs e)
		{
			if (e != null)
			{
				if (base._xmlEditor.Regelwerk.IstSchliessendesTagSichtbar(base._xmlNode))
				{
					int num = 0;
					int posX = base._paintPos.PosX;
					int num2 = 0;
					if (paintArt != 0)
					{
						using (SolidBrush brush = new SolidBrush(this._farbePfeil))
						{
							int posX2 = base._paintPos.PosX;
							int num3 = base._paintPos.PosY + this._ankerEinzugY;
							Point point = new Point(posX2 + 7, num3 - 7);
							Point point2 = new Point(posX2, num3);
							Point point3 = new Point(posX2 + 7, num3 + 7);
							Point[] points = new Point[3]
							{
								point,
								point2,
								point3
							};
							e.Graphics.FillPolygon(brush, points);
							this._pfeilBereichRechts = new Rectangle(posX2, num3 - 7, 7, 14);
						}
					}
					base._paintPos.PosX += 7;
					num2 = XMLElement_StandardNode._hoeheProBuchstabeNodeName + 4;
					num = (int)e.Graphics.MeasureString(base._xmlNode.Name, XMLElement_StandardNode._drawFontNodeName, 64000, XMLElement_StandardNode._drawFormat).Width;
					if (paintArt != 0)
					{
						this.zeichneRahmenNachGroesse(base._paintPos.PosX, base._paintPos.PosY, num + 8, num2, 3, this._farbeRahmenHintergrund, this._farbeRahmenRand, e);
					}
					base._paintPos.PosX += 4;
					if (paintArt != 0)
					{
						using (SolidBrush brush2 = new SolidBrush(this._farbeNodeNameSchrift))
						{
							e.Graphics.DrawString(base._xmlNode.Name, XMLElement_StandardNode._drawFontNodeName, brush2, (float)base._paintPos.PosX, (float)(base._paintPos.PosY + 2), XMLElement_StandardNode._drawFormat);
						}
					}
					base._paintPos.PosX += num + 4;
					base._paintPos.PosX++;
					if (paintArt == XMLPaintArten.Vorberechnen)
					{
						this._tagBereichRechts = new Rectangle(posX, base._paintPos.PosY, base._paintPos.PosX - posX, num2);
						base._klickBereiche.Add(this._tagBereichRechts);
						if (base._xmlEditor.CursorOptimiert.StartPos.AktNode == base._xmlNode && base._xmlEditor.CursorOptimiert.StartPos.PosAmNode == XMLCursorPositionen.CursorHinterDemNode)
						{
							base._cursorStrichPos = new Point(base._paintPos.PosX - 1, base._paintPos.PosY);
						}
					}
					base._paintPos.BisherMaxX = Math.Max(base._paintPos.BisherMaxX, base._paintPos.PosX);
				}
				else
				{
					this._pfeilBereichRechts = new Rectangle(0, 0, 0, 0);
					this._tagBereichRechts = new Rectangle(0, 0, 0, 0);
				}
			}
			base.NodeZeichnenAbschluss(paintArt, offSetX, offSetY, e);
		}

		private void AttributeZeichnen(XMLPaintArten paintArt, PaintEventArgs e)
		{
			XmlAttributeCollection attributes = base.XMLNode.Attributes;
			if (attributes != null && attributes.Count != 0)
			{
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < attributes.Count; i++)
				{
					stringBuilder.AppendFormat(" {0}=\"{1}\"", attributes[i].Name, attributes[i].Value);
				}
				int num = (int)(XMLElement_StandardNode._breiteProBuchstabeAttribute * (float)stringBuilder.Length);
				int hoeheProBuchstabeNodeName = XMLElement_StandardNode._hoeheProBuchstabeNodeName;
				if (paintArt != 0)
				{
					this.zeichneRahmenNachGroesse(base._paintPos.PosX, base._paintPos.PosY + 2, num, hoeheProBuchstabeNodeName, 2, this._farbeAttributeHintergrund, this._farbeAttributeRand, e);
					using (SolidBrush brush = new SolidBrush(this._farbeAttributeSchrift))
					{
						e.Graphics.DrawString(stringBuilder.ToString(), XMLElement_StandardNode._drawFontAttribute, brush, (float)(base._paintPos.PosX + 1), (float)(base._paintPos.PosY + 2), XMLElement_StandardNode._drawFormat);
					}
				}
				base._paintPos.PosX += num + 4;
			}
		}

		private void zeichneRahmenNachKoordinaten(int x1, int y1, int x2, int y2, int rundung, Color fuellFarbe, Color rahmenFarbe, PaintEventArgs e)
		{
			using (GraphicsPath graphicsPath = new GraphicsPath())
			{
				graphicsPath.AddLine(x1 + rundung, y1, x2 - rundung, y1);
				graphicsPath.AddLine(x2, y1 + rundung, x2, y2 - rundung);
				graphicsPath.AddLine(x2 - rundung, y2, x1 + rundung, y2);
				graphicsPath.AddLine(x1, y2 - rundung, x1, y1 + rundung);
				graphicsPath.CloseFigure();
				if (fuellFarbe != Color.Transparent)
				{
					using (SolidBrush brush = new SolidBrush(fuellFarbe))
					{
						e.Graphics.FillPath(brush, graphicsPath);
					}
				}
				using (Pen pen = new Pen(rahmenFarbe, 1f))
				{
					if (rahmenFarbe != Color.Transparent)
					{
						e.Graphics.DrawPath(pen, graphicsPath);
					}
				}
			}
		}

		private void zeichneRahmenNachGroesse(int x, int y, int breite, int hoehe, int rundung, Color fuellFarbe, Color rahmenFarbe, PaintEventArgs e)
		{
			this.zeichneRahmenNachKoordinaten(x, y, x + breite, y + hoehe, rundung, fuellFarbe, rahmenFarbe, e);
		}

		protected override void WurdeAngeklickt(Point point, MausKlickAktionen aktion)
		{
			if (this._pfeilBereichLinks.Contains(point))
			{
				if (base._xmlNode.ChildNodes.Count > 0)
				{
					base._xmlEditor.CursorRoh.CursorPosSetzenDurchMausAktion(base._xmlNode.FirstChild, XMLCursorPositionen.CursorVorDemNode, aktion);
				}
				else
				{
					base._xmlEditor.CursorRoh.CursorPosSetzenDurchMausAktion(base._xmlNode, XMLCursorPositionen.CursorInDemLeeremNode, aktion);
				}
			}
			else if (this._pfeilBereichRechts.Contains(point))
			{
				if (base._xmlNode.ChildNodes.Count > 0)
				{
					base._xmlEditor.CursorRoh.CursorPosSetzenDurchMausAktion(base._xmlNode.LastChild, XMLCursorPositionen.CursorHinterDemNode, aktion);
				}
				else
				{
					base._xmlEditor.CursorRoh.CursorPosSetzenDurchMausAktion(base._xmlNode, XMLCursorPositionen.CursorInDemLeeremNode, aktion);
				}
			}
			else if (this._tagBereichLinks.Contains(point))
			{
				base._xmlEditor.CursorRoh.CursorPosSetzenDurchMausAktion(base._xmlNode, XMLCursorPositionen.CursorAufNodeSelbstVorderesTag, aktion);
			}
			else if (this._tagBereichRechts.Contains(point))
			{
				base._xmlEditor.CursorRoh.CursorPosSetzenDurchMausAktion(base._xmlNode, XMLCursorPositionen.CursorAufNodeSelbstHinteresTag, aktion);
			}
			else
			{
				base.WurdeAngeklickt(point, aktion);
			}
		}

		private void FarbenSetzen(XMLPaintArten paintArt)
		{
			if (base._xmlEditor.CursorOptimiert.IstNodeInnerhalbDerSelektion(base._xmlNode))
			{
				this._farbeRahmenHintergrund = base._xmlEditor.Regelwerk.NodeFarbe(base._xmlNode, true);
				this._farbeNodeNameSchrift = Color.White;
				this._farbePfeil = Color.Black;
				this._farbeAttributeHintergrund = Color.Transparent;
				this._farbeAttributeSchrift = Color.White;
			}
			else
			{
				this._farbeRahmenHintergrund = base._xmlEditor.Regelwerk.NodeFarbe(base._xmlNode, false);
				this._farbeNodeNameSchrift = Color.Black;
				this._farbePfeil = Color.LightGray;
				this._farbeAttributeHintergrund = Color.White;
				this._farbeAttributeSchrift = Color.Black;
			}
			this._farbeAttributeRand = Color.FromArgb(225, 225, 225);
			this._farbeRahmenRand = Color.FromArgb(100, 100, 150);
			if (paintArt == XMLPaintArten.AllesNeuZeichnenMitFehlerHighlighting && !base._xmlEditor.Regelwerk.DTDPruefer.IstXmlNodeOk(base._xmlNode, false))
			{
				this._farbeNodeNameSchrift = Color.Red;
				this._farbePfeil = Color.Red;
				this._farbeRahmenRand = Color.Red;
			}
		}
	}
}
