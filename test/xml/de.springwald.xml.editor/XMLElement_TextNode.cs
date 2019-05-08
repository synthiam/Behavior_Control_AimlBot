using de.springwald.toolbox;
using de.springwald.xml.cursor;
using de.springwald.xml.editor.textnode;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

namespace de.springwald.xml.editor
{
	public class XMLElement_TextNode : XMLElement
	{
		private static Font _drawFont;

		private static float _breiteProBuchstabe;

		private static int _hoeheProBuchstabe;

		private static StringFormat _drawFormat;

		protected Color _farbeHintergrund_;

		protected Color _farbeHintergrundInvertiert_;

		protected Color _farbeHintergrundInvertiertOhneFokus_;

		protected SolidBrush _drawBrush_;

		protected SolidBrush _drawBrushInvertiert_;

		protected SolidBrush _drawBrushInvertiertOhneFokus_;

		private List<TextTeil> _textTeile;

		private string AktuellerInhalt
		{
			get
			{
				return ToolboxXML.TextAusTextNodeBereinigt(base.XMLNode);
			}
		}

		public char[] ZeichenZumUmbrechen
		{
			get;
			set;
		}

		public char[] ZeichenZumEinruecken
		{
			get;
			set;
		}

		public char[] ZeichenZumAusruecken
		{
			get;
			set;
		}

		private Color GetHintergrundFarbe(bool invertiert)
		{
			if (invertiert)
			{
				if (base._xmlEditor.HatFokus)
				{
					return this._farbeHintergrundInvertiert_;
				}
				return this._farbeHintergrundInvertiertOhneFokus_;
			}
			return this._farbeHintergrund_;
		}

		private SolidBrush GetZeichenFarbe(bool invertiert)
		{
			if (invertiert)
			{
				if (base._xmlEditor.HatFokus)
				{
					return this._drawBrushInvertiert_;
				}
				return this._drawBrushInvertiertOhneFokus_;
			}
			return this._drawBrush_;
		}

		public XMLElement_TextNode(XmlNode xmlNode, XMLEditor xmlEditor)
			: base(xmlNode, xmlEditor)
		{
			if (XMLElement_TextNode._drawFont == null)
			{
				XMLElement_TextNode._drawFormat = (StringFormat)StringFormat.GenericTypographic.Clone();
				XMLElement_TextNode._drawFormat.FormatFlags = (XMLElement_TextNode._drawFormat.FormatFlags | StringFormatFlags.MeasureTrailingSpaces);
				XMLElement_TextNode._drawFormat.Trimming = StringTrimming.None;
				XMLElement_TextNode._drawFont = new Font("Courier New", 10f, GraphicsUnit.Point);
				XMLElement_TextNode._breiteProBuchstabe = ToolboxUsercontrols.MeasureDisplayStringWidth(xmlEditor.ZeichnungsSteuerelement.CreateGraphics(), "W", XMLElement_TextNode._drawFont, XMLElement_TextNode._drawFormat);
				XMLElement_TextNode._hoeheProBuchstabe = XMLElement_TextNode._drawFont.Height;
			}
			this.FarbenSetzen();
		}

		protected override void Dispose(bool disposing)
		{
			if (this._drawBrush_ != null)
			{
				this._drawBrush_.Dispose();
			}
			if (this._drawBrushInvertiert_ != null)
			{
				this._drawBrushInvertiert_.Dispose();
			}
			if (this._drawBrushInvertiertOhneFokus_ != null)
			{
				this._drawBrushInvertiertOhneFokus_.Dispose();
			}
			base.Dispose(disposing);
		}

		protected override void NodeZeichnenStart(XMLPaintArten paintArt, int offSetX, int offSetY, PaintEventArgs e)
		{
			int num = 2;
			int num2 = 0;
			if (paintArt == XMLPaintArten.Vorberechnen)
			{
				int invertiertStart = -1;
				int invertiertLaenge = 0;
				this.StartUndEndeDerSelektionBestimmen(ref invertiertStart, ref invertiertLaenge);
				int maxLaengeProZeile = (int)((float)(base._paintPos.ZeilenEndeX - base._paintPos.ZeilenStartX) / XMLElement_TextNode._breiteProBuchstabe);
				int bereitsLaengeDerZeile = (int)((float)base._paintPos.PosX / XMLElement_TextNode._breiteProBuchstabe);
				TextTeiler textTeiler = new TextTeiler(this.AktuellerInhalt, invertiertStart, invertiertLaenge, maxLaengeProZeile, bereitsLaengeDerZeile, this.ZeichenZumUmbrechen);
				this._textTeile = textTeiler.TextTeile;
			}
			else
			{
				foreach (TextTeil item in this._textTeile)
				{
					using (SolidBrush brush = new SolidBrush(this.GetHintergrundFarbe(item.Invertiert)))
					{
						e.Graphics.FillRectangle(brush, item.Rechteck);
					}
				}
			}
			foreach (TextTeil item2 in this._textTeile)
			{
				int num3 = (int)(XMLElement_TextNode._breiteProBuchstabe * (float)item2.Text.Length);
				if (item2.IstNeueZeile)
				{
					base._paintPos.PosY += base._xmlEditor.Regelwerk.AbstandYZwischenZeilen + base._paintPos.HoeheAktZeile;
					base._paintPos.PosX = base._paintPos.ZeilenStartX;
					base._paintPos.HoeheAktZeile = XMLElement_TextNode._hoeheProBuchstabe + num * 2;
				}
				if (paintArt == XMLPaintArten.Vorberechnen)
				{
					item2.Rechteck = new Rectangle(base._paintPos.PosX, base._paintPos.PosY, (int)(XMLElement_TextNode._breiteProBuchstabe * (float)item2.Text.Length), XMLElement_TextNode._hoeheProBuchstabe + num * 2);
					if (base._xmlNode == base._xmlEditor.CursorOptimiert.StartPos.AktNode && base._xmlEditor.CursorOptimiert.StartPos.PosAmNode == XMLCursorPositionen.CursorInnerhalbDesTextNodes && base._xmlEditor.CursorOptimiert.StartPos.PosImTextnode >= num2 && base._xmlEditor.CursorOptimiert.StartPos.PosImTextnode <= num2 + item2.Text.Length)
					{
						int val = base._paintPos.PosX + (int)((float)(base._xmlEditor.CursorOptimiert.StartPos.PosImTextnode - num2) * XMLElement_TextNode._breiteProBuchstabe);
						val = Math.Max(base._paintPos.PosX, val);
						base._cursorStrichPos = new Point(val, base._paintPos.PosY);
					}
					num2 += item2.Text.Length;
					base._klickBereiche.Add(item2.Rechteck);
				}
				else
				{
					e.Graphics.DrawString(item2.Text, XMLElement_TextNode._drawFont, this.GetZeichenFarbe(item2.Invertiert), (float)item2.Rechteck.X, (float)(item2.Rechteck.Y + num), XMLElement_TextNode._drawFormat);
				}
				base._paintPos.BisherMaxX = Math.Max(base._paintPos.BisherMaxX, item2.Rechteck.X + item2.Rechteck.Width);
				base._paintPos.HoeheAktZeile = Math.Max(base._paintPos.HoeheAktZeile, XMLElement_TextNode._hoeheProBuchstabe + num + num);
				base._paintPos.PosX += num3;
			}
			if (base._xmlNode == base._xmlEditor.CursorOptimiert.StartPos.AktNode && base._xmlEditor.CursorOptimiert.StartPos.PosAmNode == XMLCursorPositionen.CursorHinterDemNode)
			{
				base._cursorStrichPos = new Point(base._paintPos.PosX - 1, base._paintPos.PosY);
			}
		}

		protected override void WurdeAngeklickt(Point point, MausKlickAktionen aktion)
		{
			int num = 0;
			foreach (TextTeil item in this._textTeile)
			{
				if (item.Rechteck.Contains(point))
				{
					num += Math.Min(item.Text.Length - 1, (int)((double)((float)(point.X - item.Rechteck.X) / XMLElement_TextNode._breiteProBuchstabe) + 0.5));
					break;
				}
				num += item.Text.Length;
			}
			base._xmlEditor.CursorRoh.CursorPosSetzenDurchMausAktion(base._xmlNode, XMLCursorPositionen.CursorInnerhalbDesTextNodes, num, aktion);
		}

		protected virtual void FarbenSetzen()
		{
			this._farbeHintergrund_ = base._xmlEditor.ZeichnungsSteuerelement.BackColor;
			if (this._drawBrush_ != null)
			{
				this._drawBrush_.Dispose();
			}
			this._drawBrush_ = new SolidBrush(Color.Black);
			this._farbeHintergrundInvertiert_ = Color.DarkBlue;
			if (this._drawBrushInvertiert_ != null)
			{
				this._drawBrushInvertiert_.Dispose();
			}
			this._drawBrushInvertiert_ = new SolidBrush(Color.White);
			this._farbeHintergrundInvertiertOhneFokus_ = Color.Gray;
			if (this._drawBrushInvertiertOhneFokus_ != null)
			{
				this._drawBrushInvertiertOhneFokus_.Dispose();
			}
			this._drawBrushInvertiertOhneFokus_ = new SolidBrush(Color.White);
		}

		private int LiegtInWelchemTextTeil(Point point)
		{
			for (int i = 0; i < this._textTeile.Count; i++)
			{
				if (this._textTeile[i].Rechteck.Contains(point))
				{
					return i;
				}
			}
			throw new ApplicationException("Punkt liegt in keiner der bekannten Zeilen (XMLElement_Textnode.LiegtInWelcherZeile)");
		}

		private void StartUndEndeDerSelektionBestimmen(ref int selektionStart, ref int selektionLaenge)
		{
			XMLCursor cursorOptimiert = base._xmlEditor.CursorOptimiert;
			if (cursorOptimiert.StartPos.AktNode == base._xmlNode)
			{
				switch (cursorOptimiert.StartPos.PosAmNode)
				{
				case XMLCursorPositionen.CursorAufNodeSelbstVorderesTag:
				case XMLCursorPositionen.CursorAufNodeSelbstHinteresTag:
					selektionStart = 0;
					selektionLaenge = this.AktuellerInhalt.Length;
					break;
				case XMLCursorPositionen.CursorInDemLeeremNode:
				case XMLCursorPositionen.CursorHinterDemNode:
					selektionStart = -1;
					selektionLaenge = 0;
					break;
				case XMLCursorPositionen.CursorVorDemNode:
				case XMLCursorPositionen.CursorInnerhalbDesTextNodes:
					if (cursorOptimiert.StartPos.PosAmNode == XMLCursorPositionen.CursorInnerhalbDesTextNodes)
					{
						selektionStart = Math.Max(0, cursorOptimiert.StartPos.PosImTextnode);
					}
					else
					{
						selektionStart = 0;
					}
					if (cursorOptimiert.EndPos.AktNode == base._xmlNode)
					{
						switch (cursorOptimiert.EndPos.PosAmNode)
						{
						case XMLCursorPositionen.CursorAufNodeSelbstVorderesTag:
						case XMLCursorPositionen.CursorAufNodeSelbstHinteresTag:
						case XMLCursorPositionen.CursorHinterDemNode:
							selektionLaenge = Math.Max(0, this.AktuellerInhalt.Length - selektionStart);
							break;
						case XMLCursorPositionen.CursorInDemLeeremNode:
							selektionStart = -1;
							selektionLaenge = 0;
							break;
						case XMLCursorPositionen.CursorInnerhalbDesTextNodes:
							selektionLaenge = Math.Max(0, cursorOptimiert.EndPos.PosImTextnode - selektionStart);
							break;
						case XMLCursorPositionen.CursorVorDemNode:
							selektionLaenge = 0;
							break;
						default:
							throw new ApplicationException("Unbekannte XMLCursorPosition.EndPos.PosAmNode '" + cursorOptimiert.EndPos.PosAmNode + "'B");
						}
					}
					else if (cursorOptimiert.EndPos.AktNode.ParentNode == cursorOptimiert.StartPos.AktNode.ParentNode)
					{
						selektionLaenge = Math.Max(0, this.AktuellerInhalt.Length - selektionStart);
					}
					else
					{
						selektionStart = 0;
						selektionLaenge = this.AktuellerInhalt.Length;
					}
					break;
				default:
					throw new ApplicationException("Unbekannte XMLCursorPosition.StartPos.PosAmNode '" + cursorOptimiert.StartPos.PosAmNode + "'A");
				}
			}
			else if (cursorOptimiert.EndPos.AktNode == base._xmlNode)
			{
				switch (cursorOptimiert.EndPos.PosAmNode)
				{
				case XMLCursorPositionen.CursorAufNodeSelbstVorderesTag:
				case XMLCursorPositionen.CursorAufNodeSelbstHinteresTag:
				case XMLCursorPositionen.CursorHinterDemNode:
					selektionStart = 0;
					selektionLaenge = this.AktuellerInhalt.Length;
					break;
				case XMLCursorPositionen.CursorInDemLeeremNode:
					selektionStart = -1;
					selektionLaenge = 0;
					break;
				case XMLCursorPositionen.CursorInnerhalbDesTextNodes:
					if (cursorOptimiert.EndPos.AktNode.ParentNode == cursorOptimiert.StartPos.AktNode.ParentNode)
					{
						selektionStart = 0;
						selektionLaenge = Math.Max(0, cursorOptimiert.EndPos.PosImTextnode);
					}
					else
					{
						selektionStart = 0;
						selektionLaenge = this.AktuellerInhalt.Length;
					}
					break;
				case XMLCursorPositionen.CursorVorDemNode:
					selektionStart = -1;
					selektionLaenge = 0;
					break;
				default:
					throw new ApplicationException("Unbekannte XMLCursorPosition.EndPos.PosAmNode '" + cursorOptimiert.EndPos.PosAmNode + "'X");
				}
			}
			else if (base._xmlEditor.CursorOptimiert.IstNodeInnerhalbDerSelektion(base._xmlNode))
			{
				selektionStart = 0;
				selektionLaenge = this.AktuellerInhalt.Length;
			}
		}
	}
}
