using System.Drawing;
using System.Xml;

namespace de.springwald.xml.editor
{
	internal class XMLElement_Kommentar : XMLElement_TextNode
	{
		public XMLElement_Kommentar(XmlNode xmlNode, XMLEditor xmlEditor)
			: base(xmlNode, xmlEditor)
		{
		}

		protected override void FarbenSetzen()
		{
			base._farbeHintergrund_ = Color.LightGray;
			if (base._drawBrush_ != null)
			{
				base._drawBrush_.Dispose();
			}
			base._drawBrush_ = new SolidBrush(Color.Black);
			base._farbeHintergrundInvertiert_ = Color.Black;
			if (base._drawBrushInvertiert_ != null)
			{
				base._drawBrushInvertiert_.Dispose();
			}
			base._drawBrushInvertiert_ = new SolidBrush(Color.Gray);
			base._farbeHintergrundInvertiertOhneFokus_ = Color.Gray;
			if (base._drawBrushInvertiertOhneFokus_ != null)
			{
				base._drawBrushInvertiertOhneFokus_.Dispose();
			}
			base._drawBrushInvertiertOhneFokus_ = new SolidBrush(Color.LightGray);
		}
	}
}
