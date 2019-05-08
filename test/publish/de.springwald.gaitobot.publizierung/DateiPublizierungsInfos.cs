using de.springwald.toolbox.file;
using System;
using System.IO;
using System.Xml.Serialization;

namespace de.springwald.gaitobot.publizierung
{
	[Serializable]
	public class DateiPublizierungsInfos
	{
		[XmlAttribute]
		public string Dateiname
		{
			get;
			set;
		}

		[XmlAttribute]
		public ulong CRC32Checksumme
		{
			get;
			set;
		}

		[XmlAttribute]
		public long Groesse
		{
			get;
			set;
		}

		[XmlAttribute]
		public string GesamtCheckString
		{
			get;
			set;
		}

		public void SetzeWerte(string dateinameMitPfad)
		{
			DateiChecksumme dateiChecksumme = new DateiChecksumme();
			this.CRC32Checksumme = dateiChecksumme.GetCRC32(dateinameMitPfad);
			FileInfo fileInfo = new FileInfo(dateinameMitPfad);
			this.Groesse = fileInfo.Length;
			this.GesamtCheckString = string.Format("{0}-{1}", this.CRC32Checksumme, this.Groesse);
			FileInfo fileInfo2 = new FileInfo(dateinameMitPfad);
			this.Dateiname = fileInfo2.Name;
		}
	}
}
