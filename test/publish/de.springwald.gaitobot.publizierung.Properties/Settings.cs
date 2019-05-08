using System.CodeDom.Compiler;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace de.springwald.gaitobot.publizierung.Properties
{
	[CompilerGenerated]
	[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "10.0.0.0")]
	internal sealed class Settings : ApplicationSettingsBase
	{
		private static Settings defaultInstance = (Settings)SettingsBase.Synchronized(new Settings());

		public static Settings Default
		{
			get
			{
				return Settings.defaultInstance;
			}
		}

		[ApplicationScopedSetting]
		[DebuggerNonUserCode]
		[SpecialSetting(SpecialSetting.WebServiceUrl)]
		[DefaultSettingValue("http://localhost:55387/gaitobot/Webservices/Publizieren.asmx")]
		public string de_springwald_gaitobot_publizierung_Gaitobot_de_publizieren_Publizieren
		{
			get
			{
				return (string)((SettingsBase)this)["de_springwald_gaitobot_publizierung_Gaitobot_de_publizieren_Publizieren"];
			}
		}
	}
}
