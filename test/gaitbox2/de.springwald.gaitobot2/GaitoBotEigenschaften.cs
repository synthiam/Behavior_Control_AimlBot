using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace de.springwald.gaitobot2
{
	public class GaitoBotEigenschaften
	{
		private readonly Dictionary<string, string> _eigenschaften;

		public string[] Namen
		{
			get
			{
				List<string> list = new List<string>();
				foreach (string key in this._eigenschaften.Keys)
				{
					if (!this.IsEmpty(key))
					{
						list.Add(key);
					}
				}
				return list.ToArray();
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

		public GaitoBotEigenschaften()
		{
			this._eigenschaften = new Dictionary<string, string>();
		}

		private bool IsEmpty(string name)
		{
			name = name.ToLower();
			if (this._eigenschaften.ContainsKey(name))
			{
				if (this._eigenschaften[name] == null)
				{
					return true;
				}
				if (this._eigenschaften[name].Trim() == "")
				{
					return true;
				}
				return false;
			}
			return true;
		}

		public string Lesen(string name)
		{
			name = name.ToLower();
			if (!this.IsEmpty(name))
			{
				return this._eigenschaften[name];
			}
			return ResReader.Reader(null).GetString("unbekannteBotEigenschaft");
		}

		public void Setzen(string name, string inhalt)
		{
			name = name.ToLower();
			this._eigenschaften[name] = inhalt;
			this.ChangedEvent();
		}

		public static void SerialisiereListeInXMLDatei(string dateiname, GaitoBotEigenschaften eigenschaften)
		{
			List<GaitoBotEigenschaft> list = new List<GaitoBotEigenschaft>();
			foreach (string key in eigenschaften._eigenschaften.Keys)
			{
				if (!eigenschaften.IsEmpty(key))
				{
					list.Add(new GaitoBotEigenschaft
					{
						Name = key,
						Inhalt = eigenschaften.Lesen(key)
					});
				}
			}
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<GaitoBotEigenschaft>));
			StreamWriter streamWriter = File.CreateText(dateiname);
			xmlSerializer.Serialize(streamWriter, list);
			streamWriter.Close();
		}

		public static GaitoBotEigenschaften DeSerialisiereListeAusXMLDatei(string dateiname)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<GaitoBotEigenschaft>));
			if (!File.Exists(dateiname))
			{
				throw new ApplicationException("GaitoBotEigenschaften-Datei '" + dateiname + "' nicht vorhanden!");
			}
			StreamReader streamReader = File.OpenText(dateiname);
			List<GaitoBotEigenschaft> list = (List<GaitoBotEigenschaft>)xmlSerializer.Deserialize(streamReader);
			streamReader.Close();
			GaitoBotEigenschaften gaitoBotEigenschaften = new GaitoBotEigenschaften();
			foreach (GaitoBotEigenschaft item in list)
			{
				gaitoBotEigenschaften.Setzen(item.Name, item.Inhalt);
			}
			return gaitoBotEigenschaften;
		}
	}
}
