using de.springwald.xml;
using de.springwald.xml.cursor;
using de.springwald.xml.dtd;
using de.springwald.xml.editor;
using System.Collections;
using System.Drawing;
using System.Text;
using System.Xml;

namespace GaitoBotEditorCore
{
	public class AimlXmlRegelwerk : XMLRegelwerk
	{
		public override XMLElementGruppenListe ElementGruppen
		{
			get
			{
				if (base._elementGruppen == null)
				{
					base._elementGruppen = new XMLElementGruppenListe();
					XMLElementGruppe xMLElementGruppe = new XMLElementGruppe(ResReader.Reader.GetString("GruppeStandard"), false);
					xMLElementGruppe.AddElementName("bot");
					xMLElementGruppe.AddElementName("get");
					xMLElementGruppe.AddElementName("li");
					xMLElementGruppe.AddElementName("pattern");
					xMLElementGruppe.AddElementName("random");
					xMLElementGruppe.AddElementName("set");
					xMLElementGruppe.AddElementName("srai");
					xMLElementGruppe.AddElementName("sr");
					xMLElementGruppe.AddElementName("star");
					xMLElementGruppe.AddElementName("template");
					xMLElementGruppe.AddElementName("that");
					xMLElementGruppe.AddElementName("thatstar");
					xMLElementGruppe.AddElementName("think");
					base._elementGruppen.Add(xMLElementGruppe);
					XMLElementGruppe xMLElementGruppe2 = new XMLElementGruppe(ResReader.Reader.GetString("GruppeFortgeschritten"), true);
					xMLElementGruppe2.AddElementName("condition");
					xMLElementGruppe2.AddElementName("formal");
					xMLElementGruppe2.AddElementName("gender");
					xMLElementGruppe2.AddElementName("input");
					xMLElementGruppe2.AddElementName("person");
					xMLElementGruppe2.AddElementName("person2");
					xMLElementGruppe2.AddElementName("sentence");
					base._elementGruppen.Add(xMLElementGruppe2);
					XMLElementGruppe xMLElementGruppe3 = new XMLElementGruppe(ResReader.Reader.GetString("GruppeHTML"), true);
					xMLElementGruppe3.AddElementName("a");
					xMLElementGruppe3.AddElementName("applet");
					xMLElementGruppe3.AddElementName("br");
					xMLElementGruppe3.AddElementName("em");
					xMLElementGruppe3.AddElementName("img");
					xMLElementGruppe3.AddElementName("p");
					xMLElementGruppe3.AddElementName("table");
					xMLElementGruppe3.AddElementName("ul");
					base._elementGruppen.Add(xMLElementGruppe3);
					XMLElementGruppe xMLElementGruppe4 = new XMLElementGruppe("GaitoBot", true);
					xMLElementGruppe4.AddElementName("script");
					base._elementGruppen.Add(xMLElementGruppe4);
				}
				return base._elementGruppen;
			}
		}

		public AimlXmlRegelwerk(DTD dtd)
			: base(dtd)
		{
		}

		public override Color NodeFarbe(XmlNode node, bool selektiert)
		{
			if (!selektiert)
			{
				switch (node.Name)
				{
				case "condition":
					return Color.FromArgb(150, 221, 220);
				case "li":
				{
					string name = node.ParentNode.Name;
					if (!(name == "random"))
					{
						if (name == "condition")
						{
							return Color.FromArgb(200, 250, 250);
						}
						break;
					}
					return Color.FromArgb(255, 243, 187);
				}
				case "random":
					return Color.FromArgb(255, 211, 80);
				case "think":
					return Color.FromArgb(200, 200, 200);
				}
			}
			return base.NodeFarbe(node, selektiert);
		}

		public override bool IstSchliessendesTagSichtbar(XmlNode xmlNode)
		{
			string name = xmlNode.Name;
			if (name == "that")
			{
				if (xmlNode.ParentNode.Name == "template")
				{
					return false;
				}
				return true;
			}
			return base.IstSchliessendesTagSichtbar(xmlNode);
		}

		public override DarstellungsArten DarstellungsArt(XmlNode xmlNode)
		{
			if (xmlNode is XmlElement)
			{
				switch (xmlNode.Name)
				{
				case "a":
				case "set":
				case "bot":
				case "formal":
				case "gender":
				case "person":
				case "person2":
					return DarstellungsArten.Fliesselement;
				case "think":
					if (xmlNode.ParentNode.Name == "template")
					{
						if (xmlNode.PreviousSibling != null && !(xmlNode.PreviousSibling.Name == "think") && this.DarstellungsArt(xmlNode.PreviousSibling) == DarstellungsArten.Fliesselement)
						{
							return DarstellungsArten.Fliesselement;
						}
						if (xmlNode.NextSibling != null && this.DarstellungsArt(xmlNode.NextSibling) == DarstellungsArten.Fliesselement)
						{
							return DarstellungsArten.Fliesselement;
						}
						return DarstellungsArten.EigeneZeile;
					}
					return DarstellungsArten.Fliesselement;
				case "that":
					if (xmlNode.ParentNode.Name == "template")
					{
						return DarstellungsArten.Fliesselement;
					}
					return DarstellungsArten.EigeneZeile;
				default:
					return base.DarstellungsArt(xmlNode);
				}
			}
			return base.DarstellungsArt(xmlNode);
		}

		public override string EinfuegeTextPreProcessing(string einfuegeText, XMLCursorPos woEinfuegen, out XmlNode ersatzNode)
		{
			XmlNode xmlNode = (!(woEinfuegen.AktNode is XmlText)) ? woEinfuegen.AktNode : woEinfuegen.AktNode.ParentNode;
			if (einfuegeText == "*")
			{
				switch (xmlNode.Name)
				{
				default:
					ersatzNode = woEinfuegen.AktNode.OwnerDocument.CreateElement("star");
					return "";
				case "pattern":
				case "that":
				case "script":
					break;
				}
			}
			string name = xmlNode.Name;
			string result;
			if (!(name == "srai"))
			{
				if (name == "pattern")
				{
					StringBuilder stringBuilder = new StringBuilder(einfuegeText.ToUpper());
					stringBuilder.Replace("Ä", "AE");
					stringBuilder.Replace("Ö", "OE");
					stringBuilder.Replace("Ü", "UE");
					stringBuilder.Replace("ß", "SS");
					char[] array = stringBuilder.ToString().ToCharArray();
					ArrayList arrayList = new ArrayList();
					for (int i = 0; i < array.Length; i++)
					{
						if ((array[i] == '*' || array[i] == '_') && xmlNode.Name == "pattern")
						{
							arrayList.Add(array[i]);
						}
						else if ((array[i] > '@' & array[i] < '[') || (array[i] > '`' & array[i] < '{') || (array[i] > '/' & array[i] < ':') || array[i] == ' ')
						{
							arrayList.Add(array[i]);
						}
					}
					char[] array2 = new char[arrayList.Count];
					for (int j = 0; j < arrayList.Count; j++)
					{
						array2[j] = (char)arrayList[j];
					}
					result = new string(array2);
					ersatzNode = null;
					return result;
				}
				return base.EinfuegeTextPreProcessing(einfuegeText, woEinfuegen, out ersatzNode);
			}
			result = einfuegeText.Replace("*", "");
			result = result.Replace("_", "");
			ersatzNode = null;
			return result;
		}

		public override string[] ErlaubteEinfuegeElemente_(XMLCursorPos zielPunkt, bool pcDATAMitAuflisten, bool kommentareMitAuflisten)
		{
			if (zielPunkt.AktNode != null && zielPunkt.AktNode.Name.ToLower() == "category")
			{
				return new string[0];
			}
			return base.ErlaubteEinfuegeElemente_(zielPunkt, pcDATAMitAuflisten, kommentareMitAuflisten);
		}

		public override XMLElement createPaintElementForNode(XmlNode xmlNode, XMLEditor xmlEditor)
		{
			if (xmlNode is XmlText && xmlNode.ParentNode != null && xmlNode.ParentNode.Name.ToLower() == "script")
			{
				XMLElement_TextNode xMLElement_TextNode = new XMLElement_TextNode(xmlNode, xmlEditor);
				xMLElement_TextNode.ZeichenZumUmbrechen = new char[3]
				{
					'}',
					'{',
					';'
				};
				return xMLElement_TextNode;
			}
			return base.createPaintElementForNode(xmlNode, xmlEditor);
		}
	}
}
