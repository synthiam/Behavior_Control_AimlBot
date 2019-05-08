using de.springwald.toolbox.Serialisierung;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace de.springwald.toolbox.caching
{
	[Serializable]
	public class ObjektCache<KeyTyp, ObjektTyp> : ICacheParameter
	{
		public class NeuesObjektWirdBenoetigtEventArgs : EventArgs
		{
			public readonly KeyTyp Key;

			public ObjektTyp Objekt;

			public object HilfsdatenLieferungsObjekt;

			public ObjektSerialisiererVerwaltung<ObjektTyp>.WasTunWennObjektNichtVorhanden WasTunWennNichtVorhanden;

			public NeuesObjektWirdBenoetigtEventArgs(KeyTyp key, object hilfsdatenLieferungsObjekt, ObjektTyp objekt, ObjektSerialisiererVerwaltung<ObjektTyp>.WasTunWennObjektNichtVorhanden wasTunWennNichtVorhanden)
			{
				this.Key = key;
				this.Objekt = objekt;
				this.HilfsdatenLieferungsObjekt = hilfsdatenLieferungsObjekt;
				this.WasTunWennNichtVorhanden = wasTunWennNichtVorhanden;
			}
		}

		public delegate void NeuesObjektWirdBenoetigtEventHandler(object sender, NeuesObjektWirdBenoetigtEventArgs eventArgs);

		private readonly object _cacheLock = new object();

		private volatile List<ObjectCacheContainer<ObjektTyp, KeyTyp>> _container;

		private long _maxObjekteProContainer;

		private long _maxContainer = 20L;

		public long MaxObjekte
		{
			get
			{
				return this._maxContainer * this._maxObjekteProContainer;
			}
			set
			{
				this._maxObjekteProContainer = value / this._maxContainer;
			}
		}

		public TimeSpan MaxObjektLebensdauer
		{
			get;
			set;
		}

		public bool NichtCachen
		{
			get;
			set;
		}

		public event NeuesObjektWirdBenoetigtEventHandler OnNeuesObjektWirdBenoetigt;

		private void NeuesObjektWirdBenoetigtEventWerfen(NeuesObjektWirdBenoetigtEventArgs eventArgs)
		{
			if (this.OnNeuesObjektWirdBenoetigt != null)
			{
				this.OnNeuesObjektWirdBenoetigt(this, eventArgs);
				return;
			}
			throw new ApplicationException("Es wurde kein Listener auf das 'NeuesObjektWirdBenötigt' Event registriert. Daher könnte *nie* ein Objekt aus dem ObjektCache geliefert werden.");
		}

		public string GetStatus(string zeilenUmbruch)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int num = 0;
			foreach (ObjectCacheContainer<ObjektTyp, KeyTyp> item in this._container)
			{
				num++;
				stringBuilder.AppendFormat("Container {0}:{1}{2}", item.VerfallsZeitpunktTicks, item.AnzahlObjekte, zeilenUmbruch);
				stringBuilder.Append(zeilenUmbruch);
			}
			return stringBuilder.ToString();
		}

		public ObjektCache()
		{
			this._container = new List<ObjectCacheContainer<ObjektTyp, KeyTyp>>();
			this.MaxObjektLebensdauer = new TimeSpan(2, 0, 0);
			this.MaxObjekte = 10000L;
			this.NichtCachen = false;
		}

		public void VerwerfeObjekt(KeyTyp key)
		{
			try
			{
				for (int num = this._container.Count - 1; num >= 0; num--)
				{
					this._container[num].RemoveObjekt(key);
				}
			}
			catch (Exception)
			{
			}
		}

		public bool IstObjektImCache(KeyTyp key)
		{
			foreach (ObjectCacheContainer<ObjektTyp, KeyTyp> item in this._container)
			{
				ObjektTyp objekt = item.GetObjekt(key);
				if (objekt != null)
				{
					return true;
				}
			}
			return false;
		}

		public void LeereCache()
		{
			try
			{
				object cacheLock = this._cacheLock;
				Monitor.Enter(cacheLock);
				try
				{
					while (this._container.Count > 0)
					{
						ObjectCacheContainer<ObjektTyp, KeyTyp> objectCacheContainer = this._container[0];
						this._container.Remove(objectCacheContainer);
						objectCacheContainer.Dispose();
					}
				}
				finally
				{
					Monitor.Exit(cacheLock);
				}
			}
			catch (Exception)
			{
			}
		}

		public ObjektTyp GetCacheObjekt(KeyTyp key, object referenzObjekt, ObjektSerialisiererVerwaltung<ObjektTyp>.WasTunWennObjektNichtVorhanden wasTunWennNull)
		{
			ObjektTyp objekt = default(ObjektTyp);
			if (!this.NichtCachen)
			{
				try
				{
					for (int num = this._container.Count - 1; num >= 0; num--)
					{
						if (DateTime.UtcNow.Ticks > this._container[num].VerfallsZeitpunktTicks)
						{
							object cacheLock = this._cacheLock;
							Monitor.Enter(cacheLock);
							try
							{
								ObjectCacheContainer<ObjektTyp, KeyTyp> objectCacheContainer = this._container[num];
								this._container.Remove(objectCacheContainer);
								objectCacheContainer.Dispose();
							}
							finally
							{
								Monitor.Exit(cacheLock);
							}
						}
					}
				}
				catch (Exception)
				{
					string text = "";
				}
				foreach (ObjectCacheContainer<ObjektTyp, KeyTyp> item in this._container)
				{
					ObjektTyp objekt2 = item.GetObjekt(key);
					if (objekt2 != null)
					{
						return objekt2;
					}
				}
			}
			NeuesObjektWirdBenoetigtEventArgs neuesObjektWirdBenoetigtEventArgs = new NeuesObjektWirdBenoetigtEventArgs(key, referenzObjekt, objekt, wasTunWennNull);
			this.NeuesObjektWirdBenoetigtEventWerfen(neuesObjektWirdBenoetigtEventArgs);
			objekt = neuesObjektWirdBenoetigtEventArgs.Objekt;
			if (!this.NichtCachen && objekt != null)
			{
				ObjectCacheContainer<ObjektTyp, KeyTyp> objectCacheContainer2 = null;
				if (this._container.Count == 0)
				{
					objectCacheContainer2 = new ObjectCacheContainer<ObjektTyp, KeyTyp>(DateTime.UtcNow + this.MaxObjektLebensdauer);
					this._container.Add(objectCacheContainer2);
				}
				else
				{
					int num2 = 0;
					try
					{
						while (objectCacheContainer2 == null && num2 < this._container.Count)
						{
							objectCacheContainer2 = this._container[num2];
							if (objectCacheContainer2.AnzahlObjekte >= this._maxObjekteProContainer)
							{
								objectCacheContainer2 = null;
							}
							num2++;
						}
					}
					catch (Exception)
					{
						string text2 = "";
					}
					if (objectCacheContainer2 == null)
					{
						object cacheLock2 = this._cacheLock;
						Monitor.Enter(cacheLock2);
						try
						{
							objectCacheContainer2 = new ObjectCacheContainer<ObjektTyp, KeyTyp>(DateTime.UtcNow + this.MaxObjektLebensdauer);
							this._container.Add(objectCacheContainer2);
							while (this._container.Count > this._maxContainer)
							{
								ObjectCacheContainer<ObjektTyp, KeyTyp> objectCacheContainer3 = this._container[0];
								this._container.Remove(objectCacheContainer3);
								objectCacheContainer3.Dispose();
							}
						}
						finally
						{
							Monitor.Exit(cacheLock2);
						}
					}
				}
				objectCacheContainer2.AddObjekt(key, objekt);
			}
			if (objekt == null)
			{
				switch (wasTunWennNull)
				{
				case ObjektSerialisiererVerwaltung<ObjektTyp>.WasTunWennObjektNichtVorhanden.FehlerWerfen:
					throw new ApplicationException("Es konnte kein Objekt für den key '" + key + "' bereitgestellt werden!");
				default:
					throw new ApplicationException("Unbehandeltes ObjektCacheFehlerBeiNullObjekt '" + wasTunWennNull + "' bei wasTunWennNull für key '" + key + "'!");
				case ObjektSerialisiererVerwaltung<ObjektTyp>.WasTunWennObjektNichtVorhanden.NullZurueckgeben:
					break;
				}
			}
			return objekt;
		}
	}
}
