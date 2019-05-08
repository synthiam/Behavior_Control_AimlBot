using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace de.springwald.toolbox.Serialisierung
{
	public class ObjektSerialisiererVerwaltung<objektTyp>
	{
		public enum WasTunWennObjektNichtVorhanden
		{
			FehlerWerfen,
			NullZurueckgeben
		}

		protected Random _rnd;

		protected string ObjektXMLVerzeichnis
		{
			get;
			set;
		}

		public virtual string[] AlleVorhandenenIDs
		{
			get
			{
				List<string> list = new List<string>();
				string[] files = Directory.GetFiles(this.ObjektXMLVerzeichnis, "chatbot.xml", SearchOption.AllDirectories);
				foreach (string text in files)
				{
					string[] array = text.Split('\\');
					list.Add(array[array.Length - 2]);
				}
				return list.ToArray();
			}
		}

		public ObjektSerialisiererVerwaltung(string objektXMLVerzeichnis)
		{
			this.ObjektXMLVerzeichnis = objektXMLVerzeichnis;
			this.ZufallInitialisieren();
		}

		public bool ExistsID(string id)
		{
			if (string.IsNullOrEmpty(id))
			{
				return false;
			}
			return File.Exists(this.GetObjektDateiname(id));
		}

		public virtual string GetObjektDateiname(string id)
		{
			return Path.Combine(this.ObjektXMLVerzeichnis, string.Format("{0}.xml", id));
		}

		public virtual string GetObjektBackupDateiname(string id, string kontextpostfix)
		{
			return Path.Combine(this.ObjektXMLVerzeichnis, string.Format("backup\\{0}_{1:s}_{2}.xml", id, DateTime.UtcNow, kontextpostfix).Replace(":", "-"));
		}

		public virtual void SpeichereObjektToFile(objektTyp objekt, bool backup, string backupKontextpostfix)
		{
			this.IdWennNoetigVergeben(objekt);
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(objektTyp));
			string path = (!backup) ? this.GetObjektDateiname(((IObjektSerialisierVerwaltbar)(object)objekt).ID) : this.GetObjektBackupDateiname(((IObjektSerialisierVerwaltbar)(object)objekt).ID, backupKontextpostfix);
			StreamWriter streamWriter = File.CreateText(path);
			xmlSerializer.Serialize(streamWriter, objekt);
			streamWriter.Close();
		}

		public virtual void IdWennNoetigVergeben(objektTyp objekt)
		{
			if (((IObjektSerialisierVerwaltbar)(object)objekt).ID == null)
			{
				((IObjektSerialisierVerwaltbar)(object)objekt).ID = this.NaechsteFreieID();
			}
		}

		public virtual objektTyp LadeObjektFromFile(string id, WasTunWennObjektNichtVorhanden wasTunWennObjektNichtVorhanden)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(objektTyp));
			string objektDateiname = this.GetObjektDateiname(id);
			if (!File.Exists(objektDateiname))
			{
				switch (wasTunWennObjektNichtVorhanden)
				{
				case WasTunWennObjektNichtVorhanden.FehlerWerfen:
					throw new ApplicationException("Objekt-Datei '" + objektDateiname + "' nicht vorhanden!");
				case WasTunWennObjektNichtVorhanden.NullZurueckgeben:
					return default(objektTyp);
				default:
					throw new ApplicationException("Unbehandelter WasTunWennObjektNichtVorhanden-Zustand '" + wasTunWennObjektNichtVorhanden + "' nicht vorhanden!");
				}
			}
			StreamReader streamReader = File.OpenText(objektDateiname);
			objektTyp result = (objektTyp)xmlSerializer.Deserialize(streamReader);
			streamReader.Close();
			return result;
		}

		protected virtual string NaechsteFreieID()
		{
			string neueID = this.GetNeueID();
			while (this.ExistsID(neueID))
			{
				neueID = this.GetNeueID();
			}
			return neueID.ToString();
		}

		protected virtual void ZufallInitialisieren()
		{
			DateTime utcNow = DateTime.UtcNow;
			this._rnd = new Random(utcNow.Millisecond);
			int num = 0;
			while (true)
			{
				int num2 = num;
				utcNow = DateTime.UtcNow;
				if (num2 < utcNow.Minute)
				{
					Random rnd = this._rnd;
					utcNow = DateTime.UtcNow;
					rnd.Next(utcNow.Second);
					num++;
					continue;
				}
				break;
			}
		}

		protected virtual string GetNeueID()
		{
			char[] array = new char[27]
			{
				'A',
				'C',
				'D',
				'E',
				'F',
				'G',
				'H',
				'K',
				'L',
				'P',
				'Q',
				'R',
				'S',
				'T',
				'U',
				'V',
				'W',
				'X',
				'Y',
				'Z',
				'2',
				'3',
				'4',
				'5',
				'6',
				'7',
				'9'
			};
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < 10; i++)
			{
				char value = array[this._rnd.Next(0, array.Length - 1)];
				stringBuilder.Append(value);
			}
			return stringBuilder.ToString();
		}
	}
}
