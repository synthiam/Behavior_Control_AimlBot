using System.Xml;

namespace de.springwald.gaitobot2
{
	public class WissensCategory
	{
		private readonly MatchString _pattern;

		private readonly MatchString _that;

		private readonly XmlNode _categoryNode;

		private readonly string _themaName;

		private readonly string _aimlDateiName;

		public bool IstSrai
		{
			get;
			private set;
		}

		public XmlNode CategoryNode
		{
			get
			{
				return this._categoryNode;
			}
		}

		public MatchString Pattern
		{
			get
			{
				return this._pattern;
			}
		}

		public MatchString That
		{
			get
			{
				return this._that;
			}
		}

		public string ThemaName
		{
			get
			{
				return this._themaName;
			}
		}

		public string AIMLDateiname
		{
			get
			{
				return this._aimlDateiName;
			}
		}

		public WissensCategory(Normalisierung normalisierung, XmlNode categoryNode, string themaName, string aimlDateiName, GaitoBotEigenschaften botEigenschaften)
		{
			this._categoryNode = categoryNode;
			this._aimlDateiName = aimlDateiName;
			this._themaName = themaName;
			XmlNode xmlNode = this._categoryNode.SelectSingleNode("pattern");
			if (xmlNode != null)
			{
				this._pattern = new MatchString(MatchString.GetInhaltFromXmlNode(xmlNode, normalisierung, botEigenschaften));
			}
			else
			{
				this._pattern = new MatchString("*");
			}
			XmlNode xmlNode2 = this._categoryNode.SelectSingleNode("that");
			if (xmlNode2 != null)
			{
				this._that = new MatchString(MatchString.GetInhaltFromXmlNode(xmlNode2, normalisierung, botEigenschaften));
			}
			else
			{
				this._that = new MatchString("*");
			}
			this.ErmittleObSRAICategory();
		}

		private void ErmittleObSRAICategory()
		{
			this.IstSrai = (this._categoryNode.OuterXml.IndexOf("<srai>") != -1);
		}
	}
}
