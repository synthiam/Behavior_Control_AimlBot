using System;
using System.Collections.Generic;
using System.Threading;

namespace de.springwald.toolbox.caching
{
	public class ObjectCacheContainer<TObjektTyp, TKeyTyp> : IDisposable
	{
		private volatile Dictionary<TKeyTyp, TObjektTyp> _cacheInhalte;

		public long VerfallsZeitpunktTicks
		{
			get;
			private set;
		}

		public long AnzahlObjekte
		{
			get
			{
				return this._cacheInhalte.Count;
			}
		}

		public ObjectCacheContainer(DateTime verfallszeitpunkt)
		{
			this.VerfallsZeitpunktTicks = verfallszeitpunkt.Ticks;
			this._cacheInhalte = new Dictionary<TKeyTyp, TObjektTyp>();
		}

		public void AddObjekt(TKeyTyp key, TObjektTyp objekt)
		{
			try
			{
				this._cacheInhalte.Add(key, objekt);
			}
			catch (Exception)
			{
			}
		}

		public void RemoveObjekt(TKeyTyp key)
		{
			try
			{
				if (this._cacheInhalte.ContainsKey(key))
				{
					Dictionary<TKeyTyp, TObjektTyp> cacheInhalte = this._cacheInhalte;
					Monitor.Enter(cacheInhalte);
					try
					{
						this._cacheInhalte.Remove(key);
					}
					finally
					{
						Monitor.Exit(cacheInhalte);
					}
				}
			}
			catch (Exception)
			{
			}
		}

		public TObjektTyp GetObjekt(TKeyTyp key)
		{
			try
			{
				if (this._cacheInhalte.ContainsKey(key))
				{
					return this._cacheInhalte[key];
				}
			}
			catch (Exception)
			{
			}
			return default(TObjektTyp);
		}

		public void Dispose()
		{
			if (this._cacheInhalte != null)
			{
				this._cacheInhalte.Clear();
			}
			this._cacheInhalte = null;
		}
	}
}
