using de.springwald.xml.cursor;
using de.springwald.xml.dtd;
using de.springwald.xml.dtd.pruefer;
using de.springwald.xml.editor;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Xml;

namespace de.springwald.xml
{
	public class XMLRegelwerk
	{
		private DTD _dtd;

		private DTDPruefer _dtdPruefer;

		private DTDNodeEditCheck _checker;

		protected XMLElementGruppenListe _elementGruppen;

		public DTDPruefer DTDPruefer
		{
			get
			{
				if (this._dtdPruefer == null)
				{
					if (this._dtd == null)
					{
						throw new ApplicationException("No DTD attached!");
					}
					this._dtdPruefer = new DTDPruefer(this._dtd);
				}
				return this._dtdPruefer;
			}
		}

		public DTD DTD
		{
			get
			{
				return this._dtd;
			}
		}

		public virtual int ChildEinrueckungX
		{
			get
			{
				return 20;
			}
		}

		public virtual int AbstandYZwischenZeilen
		{
			get
			{
				return 5;
			}
		}

		public virtual int AbstandFliessElementeX
		{
			get
			{
				return 0;
			}
		}

		public virtual XMLElementGruppenListe ElementGruppen
		{
			get
			{
				if (this._elementGruppen == null)
				{
					this._elementGruppen = new XMLElementGruppenListe();
				}
				return this._elementGruppen;
			}
		}

		public XMLRegelwerk(DTD dtd)
		{
			this._dtd = dtd;
		}

		public XMLRegelwerk()
		{
			this._dtd = null;
		}

		public virtual Color NodeFarbe(XmlNode node, bool selektiert)
		{
			if (selektiert)
			{
				return Color.DarkBlue;
			}
			return Color.FromArgb(245, 245, 255);
		}

		public virtual XMLElement createPaintElementForNode(XmlNode xmlNode, XMLEditor xmlEditor)
		{
			if (xmlNode is XmlElement)
			{
				return new XMLElement_StandardNode(xmlNode, xmlEditor);
			}
			if (xmlNode is XmlText)
			{
				return new XMLElement_TextNode(xmlNode, xmlEditor);
			}
			if (xmlNode is XmlComment)
			{
				return new XMLElement_Kommentar(xmlNode, xmlEditor);
			}
			return new XMLElement_StandardNode(xmlNode, xmlEditor);
		}

		public virtual DarstellungsArten DarstellungsArt(XmlNode xmlNode)
		{
			if (xmlNode is XmlText)
			{
				return DarstellungsArten.Fliesselement;
			}
			if (xmlNode is XmlWhitespace)
			{
				return DarstellungsArten.Fliesselement;
			}
			if (xmlNode is XmlComment)
			{
				return DarstellungsArten.EigeneZeile;
			}
			if (this.IstSchliessendesTagSichtbar(xmlNode))
			{
				return DarstellungsArten.EigeneZeile;
			}
			return DarstellungsArten.Fliesselement;
		}

		public virtual bool IstSchliessendesTagSichtbar(XmlNode xmlNode)
		{
			if (xmlNode is XmlText)
			{
				return false;
			}
			DTDElement dTDElement = this._dtd.DTDElementByNode_(xmlNode, false);
			if (dTDElement != null)
			{
				if (dTDElement.AlleElementNamenWelcheAlsDirektesChildZulaessigSind.Count > 1)
				{
					return true;
				}
				return false;
			}
			return true;
		}

		public bool IstDiesesTagAnDieserStelleErlaubt(string tagname, XMLCursorPos zielPunkt)
		{
			return (from e in this.ErlaubteEinfuegeElemente_(zielPunkt, true, true)
			where e == tagname
			select e).Count() > 0;
		}

		public virtual string[] ErlaubteEinfuegeElemente_(XMLCursorPos zielPunkt, bool pcDATAMitAuflisten, bool kommentareMitAuflisten)
		{
			if (zielPunkt.AktNode == null)
			{
				return new string[0];
			}
			if (this._dtd == null)
			{
				return new string[1]
				{
					""
				};
			}
			if (this._checker == null)
			{
				this._checker = new DTDNodeEditCheck(this._dtd);
			}
			return this._checker.AnDieserStelleErlaubteTags_(zielPunkt, pcDATAMitAuflisten, kommentareMitAuflisten);
		}

		public virtual bool PreviewKeyDown(PreviewKeyDownEventArgs e, out bool naechsteTasteBeiKeyPressAlsTextAufnehmen, XMLEditor editor)
		{
			naechsteTasteBeiKeyPressAlsTextAufnehmen = false;
			Keys keyData = e.KeyData;
			if (keyData == (Keys)131155)
			{
				editor.AktionNeuesElementAnAktCursorPosEinfuegen("srai", XMLEditor.UndoSnapshotSetzenOptionen.ja, false);
				naechsteTasteBeiKeyPressAlsTextAufnehmen = false;
				return true;
			}
			naechsteTasteBeiKeyPressAlsTextAufnehmen = true;
			return false;
		}

		public virtual string EinfuegeTextPreProcessing(string einfuegeText, XMLCursorPos woEinfuegen, out XmlNode ersatzNode)
		{
			ersatzNode = null;
			return einfuegeText;
		}
	}
}
