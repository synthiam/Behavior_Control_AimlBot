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
	public class AIMLDateiUebertragung
	{
		private string inhaltField;

		private string checkStringField;

		private string dateinameField;

		public string Inhalt
		{
			get
			{
				return this.inhaltField;
			}
			set
			{
				this.inhaltField = value;
			}
		}

		public string CheckString
		{
			get
			{
				return this.checkStringField;
			}
			set
			{
				this.checkStringField = value;
			}
		}

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
	}
}
