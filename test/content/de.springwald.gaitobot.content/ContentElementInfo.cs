using de.springwald.toolbox.Serialisierung;
using System;
using System.Xml.Serialization;

namespace de.springwald.gaitobot.content
{
	[Serializable]
	public class ContentElementInfo
	{
		[XmlElement]
		public bool ReadOnly
		{
			get;
			set;
		}

		[XmlElement]
		public bool NurFuerGaitoBotExportieren
		{
			get;
			set;
		}

		[XmlElement]
		public string UniqueKey
		{
			get;
			set;
		}

		[XmlElement]
		public string SortKey
		{
			get;
			set;
		}

		[XmlElement]
		public string Beschreibung
		{
			get;
			set;
		}

		[XmlElement]
		public string Name
		{
			get;
			set;
		}

		[XmlElement]
		public string DateiPattern
		{
			get;
			set;
		}

		[XmlArray]
		public string[] AbhaengigkeitenUniqueIds
		{
			get;
			set;
		}

		public ContentElementInfo()
		{
			this.UniqueKey = Guid.NewGuid().ToString();
			this.AbhaengigkeitenUniqueIds = new string[0];
		}

		public static ContentElementInfo ReadFromXmlString(string xml)
		{
			return GenericSerializationHelperClass<ContentElementInfo>.FromXml(xml, false);
		}

		public static ContentElementInfo ReadFromFile(string dateiname)
		{
			return GenericSerializationHelperClass<ContentElementInfo>.FromXmlFile(dateiname, false);
		}

		public void WriteToFile(string dateiname)
		{
			GenericSerializationHelperClass<ContentElementInfo>.ToXmlFile(this, dateiname, false);
		}
	}
}
