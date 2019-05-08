using de.springwald.xml.dtd.pruefer;
using System;
using System.Text;
using System.Xml;

namespace de.springwald.xml.editor
{
	public class XMLQuellcodeAlsRTF
	{
		private enum RtfFarben
		{
			schwarz,
			rot,
			grau
		}

		private XMLRegelwerk _regelwerk;

		private bool _zeilenNummernAnzeigen = true;

		private int _zeilenNummer;

		private XmlNode _rootnode;

		private StringBuilder _fehlerProtokollAlsText;

		private StringBuilder _quellcodeAlsRTF;

		private string _rtf_Header = "{\\rtf1\\ansi\\deff0\r\n{\\colortbl;\\red0\\green0\\blue0;\\red255\\green0\\blue0;\\red200\\green200\\blue200;}";

		private string _rtf_Footer = "\r\n}";

		private string _rtf_Umbruch = "\\line\r\n";

		private string _rtf_FarbeSchwarz = "\\cf1\r\n";

		private string _rtf_FarbeRot = "\\cf2\r\n";

		private string _rtf_FarbeGrau = "\\cf3\r\n";

		private bool _nochNichtGerendert = true;

		public XMLRegelwerk Regelwerk
		{
			set
			{
				this._regelwerk = value;
			}
		}

		public XmlNode Rootnode
		{
			set
			{
				this._rootnode = value;
				this._nochNichtGerendert = true;
			}
		}

		public string FehlerProtokollAlsText
		{
			get
			{
				if (this._nochNichtGerendert)
				{
					this.RenderIntern();
				}
				return this._fehlerProtokollAlsText.ToString();
			}
		}

		public string QuellCodeAlsRTF
		{
			get
			{
				if (this._nochNichtGerendert)
				{
					this.RenderIntern();
				}
				return this._quellcodeAlsRTF.ToString();
			}
		}

		public event EventHandler NodeWirdGeprueftEvent;

		protected virtual void ActivateNodeWirdGeprueft(EventArgs e)
		{
			if (this.NodeWirdGeprueftEvent != null)
			{
				this.NodeWirdGeprueftEvent(this, e);
			}
		}

		public void SetChanged()
		{
			this._nochNichtGerendert = true;
		}

		public void Rendern()
		{
			this.RenderIntern();
		}

		public void QuellCodeUndFehlerInNeuemFormZeigen()
		{
			using (frmDokFehlerZeigen frmDokFehlerZeigen = new frmDokFehlerZeigen())
			{
				frmDokFehlerZeigen.Anzeigen(this.QuellCodeAlsRTF, this.FehlerProtokollAlsText);
				frmDokFehlerZeigen.ShowDialog();
			}
		}

		private void RenderIntern()
		{
			this._quellcodeAlsRTF = new StringBuilder();
			this._fehlerProtokollAlsText = new StringBuilder();
			if (this._regelwerk == null)
			{
				this._fehlerProtokollAlsText.Append("Noch kein Regelwerk-Objekt zugewiesen");
			}
			else if (this._rootnode == null)
			{
				this._fehlerProtokollAlsText.Append(ResReader.Reader.GetString("NochKeinRootNodeZugewiesen"));
			}
			else
			{
				bool flag = false;
				this._zeilenNummer = 0;
				this._quellcodeAlsRTF.Append(this._rtf_Header);
				this._quellcodeAlsRTF.AppendFormat("{0}\r\n", this.GetNodeAlsQuellText(this._rootnode, "", false, false, false, ref flag));
				this._quellcodeAlsRTF.Append(this._rtf_Footer);
				this._nochNichtGerendert = false;
			}
		}

		private string GetNodeAlsQuellText(XmlNode node, string einzug, bool neueZeileNotwendig, bool parentWarFehlerhaft, bool posBereitsAlsOKGeprueft, ref bool nodeFehlerhaft)
		{
			this.ActivateNodeWirdGeprueft(EventArgs.Empty);
			if (node is XmlWhitespace)
			{
				return "";
			}
			if (node is XmlComment)
			{
				return string.Format("<!--{0}-->", node.InnerText);
			}
			StringBuilder stringBuilder = new StringBuilder();
			string str = "    ";
			string text;
			string text2;
			if (parentWarFehlerhaft)
			{
				nodeFehlerhaft = true;
				text = null;
				text2 = "";
			}
			else
			{
				DTDPruefer dTDPruefer = this._regelwerk.DTDPruefer;
				if (dTDPruefer.IstXmlNodeOk(node, posBereitsAlsOKGeprueft))
				{
					nodeFehlerhaft = false;
					text = null;
					text2 = this.RTFFarbe(RtfFarben.schwarz);
				}
				else
				{
					nodeFehlerhaft = true;
					text = dTDPruefer.Fehlermeldungen;
					text2 = this.RTFFarbe(RtfFarben.rot);
				}
			}
			if (node is XmlText)
			{
				if (neueZeileNotwendig)
				{
					stringBuilder.Append(this.GetNeueZeile() + einzug);
				}
				stringBuilder.Append(text2);
				StringBuilder stringBuilder2 = new StringBuilder(node.Value);
				stringBuilder2.Replace("\t", " ");
				stringBuilder2.Replace("\r\n", " ");
				stringBuilder2.Replace("  ", " ");
				stringBuilder.Append(stringBuilder2.ToString());
				stringBuilder2 = null;
			}
			else if (this._regelwerk.IstSchliessendesTagSichtbar(node))
			{
				switch (this._regelwerk.DarstellungsArt(node))
				{
				case DarstellungsArten.EigeneZeile:
					stringBuilder.Append(this.GetNeueZeile());
					stringBuilder.Append(text2);
					stringBuilder.Append(einzug);
					stringBuilder.AppendFormat("<{0}{1}>", node.Name, this.GetAttributeAlsQuellText(node.Attributes));
					if (text != null)
					{
						this._fehlerProtokollAlsText.Append(this.GetZeilenNummerString(this._zeilenNummer) + ": " + text + "\r\n");
					}
					stringBuilder.Append(this.GetChildrenAlsQuellText(node.ChildNodes, einzug + str, true, nodeFehlerhaft, false));
					stringBuilder.Append(this.GetNeueZeile());
					stringBuilder.Append(text2);
					stringBuilder.Append(einzug);
					stringBuilder.AppendFormat("</{0}>", node.Name);
					break;
				case DarstellungsArten.Fliesselement:
					if (neueZeileNotwendig)
					{
						stringBuilder.Append(this.GetNeueZeile() + einzug);
					}
					stringBuilder.AppendFormat("{0}<{1}{2}>", text2, node.Name, this.GetAttributeAlsQuellText(node.Attributes));
					stringBuilder.Append(this.GetChildrenAlsQuellText(node.ChildNodes, einzug + str, true, nodeFehlerhaft, false));
					stringBuilder.AppendFormat("{0}</{1}>", text2, node.Name);
					break;
				default:
					throw new ApplicationException("Unbekannte Darstellungsart " + this._regelwerk.DarstellungsArt(node));
				}
			}
			else
			{
				if (neueZeileNotwendig)
				{
					stringBuilder.Append(this.GetNeueZeile() + einzug);
				}
				stringBuilder.Append(text2);
				stringBuilder.AppendFormat("<{0}{1}>", node.Name, this.GetAttributeAlsQuellText(node.Attributes));
			}
			return stringBuilder.ToString();
		}

		private string GetNeueZeile()
		{
			if (this._zeilenNummernAnzeigen)
			{
				this._zeilenNummer++;
				StringBuilder stringBuilder = new StringBuilder(this._rtf_Umbruch);
				stringBuilder.Append(this.RTFFarbe(RtfFarben.schwarz));
				stringBuilder.AppendFormat("{0}: ", this.GetZeilenNummerString(this._zeilenNummer));
				return stringBuilder.ToString();
			}
			return this._rtf_Umbruch;
		}

		private string GetZeilenNummerString(int nummer)
		{
			StringBuilder stringBuilder = new StringBuilder(nummer.ToString(), 6);
			while (stringBuilder.Length < 6)
			{
				stringBuilder.Insert(0, "0");
			}
			return stringBuilder.ToString();
		}

		private string RTFFarbe(RtfFarben farbe)
		{
			switch (farbe)
			{
			case RtfFarben.schwarz:
				return this._rtf_FarbeSchwarz;
			case RtfFarben.rot:
				return this._rtf_FarbeRot;
			case RtfFarben.grau:
				return this._rtf_FarbeGrau;
			default:
				throw new ApplicationException("Unbekannt Farbe '" + farbe + "'");
			}
		}

		private string GetChildrenAlsQuellText(XmlNodeList children, string einzug, bool neueZeileNotwendig, bool parentNodeBereitsFehlerhaft, bool posBereitsAlsOKGeprueft)
		{
			StringBuilder stringBuilder = new StringBuilder();
			bool parentWarFehlerhaft = parentNodeBereitsFehlerhaft;
			bool flag = false;
			foreach (XmlNode child in children)
			{
				stringBuilder.Append(this.GetNodeAlsQuellText(child, einzug, neueZeileNotwendig, parentWarFehlerhaft, posBereitsAlsOKGeprueft, ref flag));
				if (flag)
				{
					flag = true;
					parentWarFehlerhaft = true;
				}
				else
				{
					posBereitsAlsOKGeprueft = true;
				}
				neueZeileNotwendig = false;
			}
			return stringBuilder.ToString();
		}

		private string GetAttributeAlsQuellText(XmlAttributeCollection attribute)
		{
			if (attribute == null)
			{
				return "";
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (XmlAttribute item in attribute)
			{
				stringBuilder.AppendFormat(" {0}=\"{1}\"", item.Name, item.Value);
			}
			return stringBuilder.ToString();
		}
	}
}
