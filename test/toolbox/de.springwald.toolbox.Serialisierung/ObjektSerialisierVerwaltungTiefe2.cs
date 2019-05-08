using System.Collections.Generic;
using System.IO;

namespace de.springwald.toolbox.Serialisierung
{
	public class ObjektSerialisierVerwaltungTiefe2<TObjektTyp> : ObjektSerialisiererVerwaltung<TObjektTyp>
	{
		protected const int AnzahlZeichenAusIdFuerVerzeichnis = 2;

		public override string[] AlleVorhandenenIDs
		{
			get
			{
				List<string> list = new List<string>();
				string[] directories = Directory.GetDirectories(base.ObjektXMLVerzeichnis);
				foreach (string path in directories)
				{
					string[] files = Directory.GetFiles(path, "*.xml");
					foreach (string fileName in files)
					{
						FileInfo fileInfo = new FileInfo(fileName);
						list.Add(fileInfo.Name.Replace(".xml", ""));
					}
				}
				return list.ToArray();
			}
		}

		public ObjektSerialisierVerwaltungTiefe2(string objektXmlVerzeichnis)
			: base(objektXmlVerzeichnis)
		{
		}

		public override void SpeichereObjektToFile(TObjektTyp objekt, bool backup, string backupKontextpostfix)
		{
			this.IdWennNoetigVergeben(objekt);
			string objektVerzeichnis = this.GetObjektVerzeichnis(((IObjektSerialisierVerwaltbar)(object)objekt).ID);
			if (!Directory.Exists(objektVerzeichnis))
			{
				Directory.CreateDirectory(objektVerzeichnis);
			}
			base.SpeichereObjektToFile(objekt, backup, backupKontextpostfix);
		}

		public virtual string GetObjektVerzeichnis(string id)
		{
			return Path.Combine(base.ObjektXMLVerzeichnis, string.Format("{0}\\", id.Substring(0, 2)));
		}

		public override string GetObjektDateiname(string id)
		{
			return Path.Combine(this.GetObjektVerzeichnis(id), string.Format("{0}.xml", id));
		}
	}
}
