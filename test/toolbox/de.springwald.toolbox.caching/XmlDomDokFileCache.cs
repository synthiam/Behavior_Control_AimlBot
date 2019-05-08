using de.springwald.toolbox.Serialisierung;
using System;
using System.IO;
using System.Xml;

namespace de.springwald.toolbox.caching
{
	public class XmlDomDokFileCache : ICacheParameter
	{
		private ObjektCache<string, XmlDocument> _domCache;

		public long MaxObjekte
		{
			get
			{
				return this._domCache.MaxObjekte;
			}
			set
			{
				this._domCache.MaxObjekte = value;
			}
		}

		public TimeSpan MaxObjektLebensdauer
		{
			get
			{
				return this._domCache.MaxObjektLebensdauer;
			}
			set
			{
				this._domCache.MaxObjektLebensdauer = value;
			}
		}

		public XmlDomDokFileCache()
		{
			this._domCache = new ObjektCache<string, XmlDocument>();
			this._domCache.OnNeuesObjektWirdBenoetigt += this._domCache_OnNeuesObjektWirdBenoetigt;
		}

		public string GetStatus(string zeilenUmbruch)
		{
			return this._domCache.GetStatus(zeilenUmbruch);
		}

		public XmlDocument GetDokument(string filename, ObjektSerialisiererVerwaltung<XmlDocument>.WasTunWennObjektNichtVorhanden wasTunWennDateiNichtVorhanden)
		{
			filename = filename.Replace("/", "\\");
			filename = filename.Trim().ToLower();
			return this._domCache.GetCacheObjekt(filename, null, wasTunWennDateiNichtVorhanden);
		}

		private void _domCache_OnNeuesObjektWirdBenoetigt(object sender, ObjektCache<string, XmlDocument>.NeuesObjektWirdBenoetigtEventArgs eventArgs)
		{
			string key = eventArgs.Key;
			if (File.Exists(key))
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.Load(key);
				eventArgs.Objekt = xmlDocument;
			}
		}
	}
}
