using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace de.springwald.gaitobot.publizierung.Gaitobot_de_publizieren
{
	[GeneratedCode("System.Web.Services", "4.0.30319.33440")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	public class AlteOderGeloeschteDateienLoeschenUndHochzuladendeZurueckGebenCompletedEventArgs : AsyncCompletedEventArgs
	{
		private object[] results;

		public string[] Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (string[])this.results[0];
			}
		}

		internal AlteOderGeloeschteDateienLoeschenUndHochzuladendeZurueckGebenCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState)
			: base(exception, cancelled, userState)
		{
			this.results = results;
		}
	}
}
