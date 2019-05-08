using Jint;
using System;

namespace de.springwald.gaitobot2
{
	public class GaitoScriptInterpreter : IDisposable
	{
		private Jint.Engine _engine;

		private GaitoBotSession _gaitoBotSession;

		private string _fehler = null;

		private object _ausgabe;

		public string Ausgabe
		{
			get
			{
				return (this._ausgabe == null) ? null : this._ausgabe.ToString();
			}
		}

		public string Fehler
		{
			get
			{
				return this._fehler;
			}
		}

		private static void Hu(double was)
		{
			Console.WriteLine(was);
		}

		private static void Ha(bool was)
		{
			Console.WriteLine(was);
		}

		public GaitoScriptInterpreter(GaitoBotSession gaitoBotSession)
		{
			this._gaitoBotSession = gaitoBotSession;
			this.Initialisieren();
		}

		private void Test()
		{
			this._gaitoBotSession.UserEigenschaften.Lesen("test");
			this._gaitoBotSession.UserEigenschaften.Setzen("test", "wert");
		}

		public void Execute(string scriptInhalt)
		{
			this._ausgabe = null;
			try
			{
				this._ausgabe = this._engine.Execute(scriptInhalt);
			}
			catch (Exception ex)
			{
				this._fehler = ex.Message;
			}
		}

		private void Initialisieren()
		{
			this._engine = new Jint.Engine();
			this._engine.SetValue("SetMaxRecursions", 10);
		}

		public void Dispose()
		{
			this._engine = null;
		}
	}
}
