using de.springwald.toolbox.Serialisierung;

namespace de.springwald.toolbox.caching
{
	public interface IFileNameBasedCache<ObjektTyp>
	{
		ObjektTyp GetObjekt(string dateiname, ObjektSerialisiererVerwaltung<ObjektTyp>.WasTunWennObjektNichtVorhanden wasTunWennObjektNichtVorhanden);
	}
}
