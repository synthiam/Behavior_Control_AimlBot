using System;
using System.Xml.Serialization;

namespace de.springwald.gaitobot2
{
	[Serializable]
	public class GaitoBotEigenschaft
	{
		[XmlAttribute("name")]
		public string Name
		{
			get;
			set;
		}

		[XmlAttribute("value")]
		public string Inhalt
		{
			get;
			set;
		}
	}
}
