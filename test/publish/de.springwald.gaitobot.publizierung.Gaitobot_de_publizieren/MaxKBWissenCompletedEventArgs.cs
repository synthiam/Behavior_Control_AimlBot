using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace de.springwald.gaitobot.publizierung.Gaitobot_de_publizieren
{
	[GeneratedCode("System.Web.Services", "4.0.30319.33440")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	public class MaxKBWissenCompletedEventArgs : AsyncCompletedEventArgs
	{
		private object[] results;

		public long Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (long)this.results[0];
			}
		}

		internal MaxKBWissenCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState)
			: base(exception, cancelled, userState)
		{
			this.results = results;
		}
	}
}
