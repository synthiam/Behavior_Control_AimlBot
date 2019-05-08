using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace de.springwald.gaitobot.publizierung.Gaitobot_de_publizieren
{
	[GeneratedCode("System.Web.Services", "4.0.30319.33440")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	public class ExistsGaitoBotIDCompletedEventArgs : AsyncCompletedEventArgs
	{
		private object[] results;

		public bool Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (bool)this.results[0];
			}
		}

		internal ExistsGaitoBotIDCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState)
			: base(exception, cancelled, userState)
		{
			this.results = results;
		}
	}
}
