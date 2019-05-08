using de.springwald.gaitobot.publizierung.Properties;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Web.Services;
using System.Web.Services.Description;
using System.Web.Services.Protocols;

namespace de.springwald.gaitobot.publizierung.Gaitobot_de_publizieren
{
	[GeneratedCode("System.Web.Services", "4.0.30319.33440")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[WebServiceBinding(Name = "PublizierenSoap", Namespace = "de.gaitobot_de.webservices")]
	public class Publizieren : SoapHttpClientProtocol
	{
		private SendOrPostCallback UebertrageAIMLDateiOperationCompleted;

		private SendOrPostCallback ReseteGaitoBotOperationCompleted;

		private SendOrPostCallback ExistsGaitoBotIDOperationCompleted;

		private SendOrPostCallback MaxKBWissenOperationCompleted;

		private SendOrPostCallback AlteOderGeloeschteDateienLoeschenUndHochzuladendeZurueckGebenOperationCompleted;

		private bool useDefaultCredentialsSetExplicitly;

		public new string Url
		{
			get
			{
				return base.Url;
			}
			set
			{
				if (this.IsLocalFileSystemWebService(base.Url) && !this.useDefaultCredentialsSetExplicitly && !this.IsLocalFileSystemWebService(value))
				{
					base.UseDefaultCredentials = false;
				}
				base.Url = value;
			}
		}

		public new bool UseDefaultCredentials
		{
			get
			{
				return base.UseDefaultCredentials;
			}
			set
			{
				base.UseDefaultCredentials = value;
				this.useDefaultCredentialsSetExplicitly = true;
			}
		}

		public event UebertrageAIMLDateiCompletedEventHandler UebertrageAIMLDateiCompleted;

		public event ReseteGaitoBotCompletedEventHandler ReseteGaitoBotCompleted;

		public event ExistsGaitoBotIDCompletedEventHandler ExistsGaitoBotIDCompleted;

		public event MaxKBWissenCompletedEventHandler MaxKBWissenCompleted;

		public event AlteOderGeloeschteDateienLoeschenUndHochzuladendeZurueckGebenCompletedEventHandler AlteOderGeloeschteDateienLoeschenUndHochzuladendeZurueckGebenCompleted;

		public Publizieren()
		{
			this.Url = Settings.Default.de_springwald_gaitobot_publizierung_Gaitobot_de_publizieren_Publizieren;
			if (this.IsLocalFileSystemWebService(this.Url))
			{
				this.UseDefaultCredentials = true;
				this.useDefaultCredentialsSetExplicitly = false;
			}
			else
			{
				this.useDefaultCredentialsSetExplicitly = true;
			}
		}

		[SoapDocumentMethod("de.gaitobot_de.webservices/UebertrageAIMLDatei", RequestNamespace = "de.gaitobot_de.webservices", ResponseNamespace = "de.gaitobot_de.webservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public void UebertrageAIMLDatei(string gaitoBotEditorID, AIMLDateiUebertragung aimlDateiInhalt)
		{
			base.Invoke("UebertrageAIMLDatei", new object[2]
			{
				gaitoBotEditorID,
				aimlDateiInhalt
			});
		}

		public void UebertrageAIMLDateiAsync(string gaitoBotEditorID, AIMLDateiUebertragung aimlDateiInhalt)
		{
			this.UebertrageAIMLDateiAsync(gaitoBotEditorID, aimlDateiInhalt, null);
		}

		public void UebertrageAIMLDateiAsync(string gaitoBotEditorID, AIMLDateiUebertragung aimlDateiInhalt, object userState)
		{
			if (this.UebertrageAIMLDateiOperationCompleted == null)
			{
				this.UebertrageAIMLDateiOperationCompleted = this.OnUebertrageAIMLDateiOperationCompleted;
			}
			base.InvokeAsync("UebertrageAIMLDatei", new object[2]
			{
				gaitoBotEditorID,
				aimlDateiInhalt
			}, this.UebertrageAIMLDateiOperationCompleted, userState);
		}

		private void OnUebertrageAIMLDateiOperationCompleted(object arg)
		{
			if (this.UebertrageAIMLDateiCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.UebertrageAIMLDateiCompleted(this, new AsyncCompletedEventArgs(invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("de.gaitobot_de.webservices/ReseteGaitoBot", RequestNamespace = "de.gaitobot_de.webservices", ResponseNamespace = "de.gaitobot_de.webservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public void ReseteGaitoBot(string gaitoBotEditorID)
		{
			base.Invoke("ReseteGaitoBot", new object[1]
			{
				gaitoBotEditorID
			});
		}

		public void ReseteGaitoBotAsync(string gaitoBotEditorID)
		{
			this.ReseteGaitoBotAsync(gaitoBotEditorID, null);
		}

		public void ReseteGaitoBotAsync(string gaitoBotEditorID, object userState)
		{
			if (this.ReseteGaitoBotOperationCompleted == null)
			{
				this.ReseteGaitoBotOperationCompleted = this.OnReseteGaitoBotOperationCompleted;
			}
			base.InvokeAsync("ReseteGaitoBot", new object[1]
			{
				gaitoBotEditorID
			}, this.ReseteGaitoBotOperationCompleted, userState);
		}

		private void OnReseteGaitoBotOperationCompleted(object arg)
		{
			if (this.ReseteGaitoBotCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.ReseteGaitoBotCompleted(this, new AsyncCompletedEventArgs(invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("de.gaitobot_de.webservices/ExistsGaitoBotID", RequestNamespace = "de.gaitobot_de.webservices", ResponseNamespace = "de.gaitobot_de.webservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public bool ExistsGaitoBotID(string gaitoBotEditorID)
		{
			object[] array = base.Invoke("ExistsGaitoBotID", new object[1]
			{
				gaitoBotEditorID
			});
			return (bool)array[0];
		}

		public void ExistsGaitoBotIDAsync(string gaitoBotEditorID)
		{
			this.ExistsGaitoBotIDAsync(gaitoBotEditorID, null);
		}

		public void ExistsGaitoBotIDAsync(string gaitoBotEditorID, object userState)
		{
			if (this.ExistsGaitoBotIDOperationCompleted == null)
			{
				this.ExistsGaitoBotIDOperationCompleted = this.OnExistsGaitoBotIDOperationCompleted;
			}
			base.InvokeAsync("ExistsGaitoBotID", new object[1]
			{
				gaitoBotEditorID
			}, this.ExistsGaitoBotIDOperationCompleted, userState);
		}

		private void OnExistsGaitoBotIDOperationCompleted(object arg)
		{
			if (this.ExistsGaitoBotIDCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.ExistsGaitoBotIDCompleted(this, new ExistsGaitoBotIDCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("de.gaitobot_de.webservices/MaxKBWissen", RequestNamespace = "de.gaitobot_de.webservices", ResponseNamespace = "de.gaitobot_de.webservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public long MaxKBWissen(string gaitoBotEditorID)
		{
			object[] array = base.Invoke("MaxKBWissen", new object[1]
			{
				gaitoBotEditorID
			});
			return (long)array[0];
		}

		public void MaxKBWissenAsync(string gaitoBotEditorID)
		{
			this.MaxKBWissenAsync(gaitoBotEditorID, null);
		}

		public void MaxKBWissenAsync(string gaitoBotEditorID, object userState)
		{
			if (this.MaxKBWissenOperationCompleted == null)
			{
				this.MaxKBWissenOperationCompleted = this.OnMaxKBWissenOperationCompleted;
			}
			base.InvokeAsync("MaxKBWissen", new object[1]
			{
				gaitoBotEditorID
			}, this.MaxKBWissenOperationCompleted, userState);
		}

		private void OnMaxKBWissenOperationCompleted(object arg)
		{
			if (this.MaxKBWissenCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.MaxKBWissenCompleted(this, new MaxKBWissenCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("de.gaitobot_de.webservices/AlteOderGeloeschteDateienLoeschenUndHochzuladendeZurueckGeben", RequestNamespace = "de.gaitobot_de.webservices", ResponseNamespace = "de.gaitobot_de.webservices", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public string[] AlteOderGeloeschteDateienLoeschenUndHochzuladendeZurueckGeben(string gaitoBotEditorID, DateiPublizierungsInfos[] dateien)
		{
			object[] array = base.Invoke("AlteOderGeloeschteDateienLoeschenUndHochzuladendeZurueckGeben", new object[2]
			{
				gaitoBotEditorID,
				dateien
			});
			return (string[])array[0];
		}

		public void AlteOderGeloeschteDateienLoeschenUndHochzuladendeZurueckGebenAsync(string gaitoBotEditorID, DateiPublizierungsInfos[] dateien)
		{
			this.AlteOderGeloeschteDateienLoeschenUndHochzuladendeZurueckGebenAsync(gaitoBotEditorID, dateien, null);
		}

		public void AlteOderGeloeschteDateienLoeschenUndHochzuladendeZurueckGebenAsync(string gaitoBotEditorID, DateiPublizierungsInfos[] dateien, object userState)
		{
			if (this.AlteOderGeloeschteDateienLoeschenUndHochzuladendeZurueckGebenOperationCompleted == null)
			{
				this.AlteOderGeloeschteDateienLoeschenUndHochzuladendeZurueckGebenOperationCompleted = this.OnAlteOderGeloeschteDateienLoeschenUndHochzuladendeZurueckGebenOperationCompleted;
			}
			base.InvokeAsync("AlteOderGeloeschteDateienLoeschenUndHochzuladendeZurueckGeben", new object[2]
			{
				gaitoBotEditorID,
				dateien
			}, this.AlteOderGeloeschteDateienLoeschenUndHochzuladendeZurueckGebenOperationCompleted, userState);
		}

		private void OnAlteOderGeloeschteDateienLoeschenUndHochzuladendeZurueckGebenOperationCompleted(object arg)
		{
			if (this.AlteOderGeloeschteDateienLoeschenUndHochzuladendeZurueckGebenCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.AlteOderGeloeschteDateienLoeschenUndHochzuladendeZurueckGebenCompleted(this, new AlteOderGeloeschteDateienLoeschenUndHochzuladendeZurueckGebenCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		public new void CancelAsync(object userState)
		{
			base.CancelAsync(userState);
		}

		private bool IsLocalFileSystemWebService(string url)
		{
			if (url == null || url == string.Empty)
			{
				return false;
			}
			Uri uri = new Uri(url);
			if (uri.Port >= 1024 && string.Compare(uri.Host, "localHost", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return true;
			}
			return false;
		}
	}
}
