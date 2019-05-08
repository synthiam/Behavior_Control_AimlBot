using de.springwald.gaitobot.content;
using System;
using System.IO;
using System.Xml.Serialization;

namespace GaitoBotEditorCore
{
	[Serializable]
	public class ArbeitsbereichMetaInfos
	{
		private string[] _contentElementUniqueIds;

		private string[] _nichtExportierenDateinamen;

		private string _name;

		private string _gaitoBotID;

		private string _exportVerzeichnis;

		private bool _exportVerzeichnisVorExportLeeren;

		private bool _alleStarTagsInExtraDateiExportieren;

		[XmlArray]
		public string[] ContentElementUniqueIds
		{
			get
			{
				return this._contentElementUniqueIds;
			}
			set
			{
				this._contentElementUniqueIds = value;
				this.ChangedEvent();
			}
		}

		[XmlArray]
		public string[] NichtExportierenDateinamen
		{
			get
			{
				return this._nichtExportierenDateinamen;
			}
			set
			{
				this._nichtExportierenDateinamen = value;
				this.ChangedEvent();
			}
		}

		[XmlElement]
		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				this._name = value;
				this.ChangedEvent();
			}
		}

		[XmlElement]
		public string GaitoBotID
		{
			get
			{
				return this._gaitoBotID;
			}
			set
			{
				this._gaitoBotID = value;
				this.ChangedEvent();
			}
		}

		[XmlElement]
		public string Exportverzeichnis
		{
			get
			{
				return this._exportVerzeichnis;
			}
			set
			{
				this._exportVerzeichnis = value;
				this.ChangedEvent();
			}
		}

		[XmlElement]
		public bool ExportverzeichnisVorExportLeeren
		{
			get
			{
				return this._exportVerzeichnisVorExportLeeren;
			}
			set
			{
				this._exportVerzeichnisVorExportLeeren = value;
				this.ChangedEvent();
			}
		}

		[XmlElement]
		public bool AlleStarTagsInExtraDateiExportieren
		{
			get
			{
				return this._alleStarTagsInExtraDateiExportieren;
			}
			set
			{
				this._alleStarTagsInExtraDateiExportieren = value;
				this.ChangedEvent();
			}
		}

		public event EventHandler Changed;

		protected virtual void ChangedEvent()
		{
			if (this.Changed != null)
			{
				this.Changed(this, EventArgs.Empty);
			}
		}

		public ArbeitsbereichMetaInfos()
		{
			ContentManager contentManager = new ContentManager();
			this.GaitoBotID = string.Empty;
			this.NichtExportierenDateinamen = new string[0];
			this.ContentElementUniqueIds = new string[0];
			this.ExportverzeichnisVorExportLeeren = false;
			this.AlleStarTagsInExtraDateiExportieren = true;
			this.Name = ResReader.Reader.GetString("unbenannt");
			this.Exportverzeichnis = "C:\\AIML_EXPORT";
		}

		public static void SerialisiereInXMLDatei(string dateiname, ArbeitsbereichMetaInfos infos)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(ArbeitsbereichMetaInfos));
			StreamWriter streamWriter = File.CreateText(dateiname);
			xmlSerializer.Serialize(streamWriter, infos);
			streamWriter.Close();
		}

		public static ArbeitsbereichMetaInfos DeSerialisiereAusXMLDatei(string dateiname)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(ArbeitsbereichMetaInfos));
			if (!File.Exists(dateiname))
			{
				throw new ApplicationException("ArbeitsbereichMetaInfos-Datei '" + dateiname + "' nicht vorhanden!");
			}
			StreamReader streamReader = File.OpenText(dateiname);
			ArbeitsbereichMetaInfos result = (ArbeitsbereichMetaInfos)xmlSerializer.Deserialize(streamReader);
			streamReader.Close();
			return result;
		}
	}
}
