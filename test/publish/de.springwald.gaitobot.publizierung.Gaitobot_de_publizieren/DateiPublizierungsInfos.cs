using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace de.springwald.gaitobot.publizierung.Gaitobot_de_publizieren
{
	[Serializable]
	[GeneratedCode("System.Xml", "4.0.30319.33440")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "de.gaitobot_de.webservices")]
	public class DateiPublizierungsInfos
	{
		private string dateinameField;

		private ulong cRC32ChecksummeField;

		private long groesseField;

		private string gesamtCheckStringField;

		[XmlAttribute]
		public string Dateiname
		{
			get
			{
				return this.dateinameField;
			}
			set
			{
				this.dateinameField = value;
			}
		}

		[XmlAttribute]
		public ulong CRC32Checksumme
		{
			get
			{
				return this.cRC32ChecksummeField;
			}
			set
			{
				this.cRC32ChecksummeField = value;
			}
		}

		[XmlAttribute]
		public long Groesse
		{
			get
			{
				return this.groesseField;
			}
			set
			{
				this.groesseField = value;
			}
		}

		[XmlAttribute]
		public string GesamtCheckString
		{
			get
			{
				return this.gesamtCheckStringField;
			}
			set
			{
				this.gesamtCheckStringField = value;
			}
		}
	}
}
