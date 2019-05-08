using de.springwald.xml;
using de.springwald.xml.cursor;
using de.springwald.xml.dtd;
using de.springwald.xml.editor;
using System.Drawing;
using System.Xml;

namespace GaitoBotEditorCore
{
	public class StartupDatei_XmlRegelwerk : XMLRegelwerk
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
					xMLElementGruppe.AddElementName("property");
					xMLElementGruppe.AddElementName("predicates");
					xMLElementGruppe.AddElementName("substitutions");
					xMLElementGruppe.AddElementName("sentence-splitters");
					xMLElementGruppe.AddElementName("input, gender, person, person");
					xMLElementGruppe.AddElementName("gender");
					xMLElementGruppe.AddElementName("person");
					xMLElementGruppe.AddElementName("person2");
					xMLElementGruppe.AddElementName("substitute");
					xMLElementGruppe.AddElementName("splitter");
					base._elementGruppen.Add(xMLElementGruppe);
				}
				return base._elementGruppen;
			}
		}

		public StartupDatei_XmlRegelwerk(DTD dtd)
			: base(dtd)
		{
		}

		public override Color NodeFarbe(XmlNode node, bool selektiert)
		{
			return base.NodeFarbe(node, selektiert);
		}

		public override bool IstSchliessendesTagSichtbar(XmlNode xmlNode)
		{
			return base.IstSchliessendesTagSichtbar(xmlNode);
		}

		public override DarstellungsArten DarstellungsArt(XmlNode xmlNode)
		{
			if (xmlNode is XmlElement)
			{
				switch (xmlNode.Name)
				{
				case "input":
				case "substitute":
				case "splitter":
				case "bot":
				case "property":
				case "predicates":
				case "substitutions":
				case "sentence-splitters":
				case "predicate":
					return DarstellungsArten.EigeneZeile;
				}
			}
			return base.DarstellungsArt(xmlNode);
		}

		public override string EinfuegeTextPreProcessing(string einfuegeText, XMLCursorPos woEinfuegen, out XmlNode ersatzNode)
		{
			return base.EinfuegeTextPreProcessing(einfuegeText, woEinfuegen, out ersatzNode);
		}

		public override string[] ErlaubteEinfuegeElemente_(XMLCursorPos zielPunkt, bool pcDATAMitAuflisten, bool kommentareMitAuflisten)
		{
			return base.ErlaubteEinfuegeElemente_(zielPunkt, pcDATAMitAuflisten, kommentareMitAuflisten);
		}

		public override XMLElement createPaintElementForNode(XmlNode xmlNode, XMLEditor xmlEditor)
		{
			return base.createPaintElementForNode(xmlNode, xmlEditor);
		}
	}
}
