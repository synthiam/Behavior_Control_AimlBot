using de.springwald.toolbox;
using MultiLang;
using System;
using System.IO;
using System.Windows.Forms;

namespace GaitoBotEditorCore
{
	public class Config : ConfigDatei
	{
		private static Config _globalConfig;

		private string SpeicherVerzeichnisRoh
		{
			get
			{
				string text = base.getWert("SaveDirectory");
				if (text != null && !File.Exists(text))
				{
					text = null;
				}
				if (text == null)
				{
					text = ((!ToolboxSonstiges.IstEntwicklungsmodus) ? ("[MyDocuments]\\" + Application.ProductName) : "[StartupPath]\\..\\..\\_Speichern_");
					base.setWert("SaveDirectory", text);
				}
				return base.getWert("SaveDirectory");
			}
		}

		private string ArbeitsbereicheSpeicherVerzeichnisRoh
		{
			get
			{
				if (base.getWert("WorkspaceDirectory") == null)
				{
					string wert = this.SpeicherVerzeichnisRoh + "\\workspaces";
					base.setWert("WorkspaceDirectory", wert);
				}
				return base.getWert("WorkspaceDirectory");
			}
		}

		public bool ErsteSchritteLaden
		{
			get
			{
				return base.getWert("ErsteSchritteLaden", true);
			}
			set
			{
				base.setWert("ErsteSchritteLaden", value);
			}
		}

		public bool AktuelleInfosLaden
		{
			get
			{
				return base.getWert("AktuelleInfosLaden", false);
			}
			set
			{
				base.setWert("AktuelleInfosLaden", value);
			}
		}

		public string ProgrammSprache
		{
			get
			{
				return base.getWert("language", "");
			}
			set
			{
				base.setWert("language", value);
			}
		}

		public string EULABestaetigtFuer
		{
			get
			{
				return base.getWert("EulaAccepted");
			}
			set
			{
				base.setWert("EulaAccepted", value);
			}
		}

		public string SpeicherVerzeichnis
		{
			get
			{
				string text = base.VerzeichnisNameAufbereitet(this.SpeicherVerzeichnisRoh);
				if (!Directory.Exists(text))
				{
					try
					{
						Directory.CreateDirectory(text);
					}
					catch (Exception arg)
					{
						throw new ApplicationException(string.Format(ResReader.Reader.GetString("VerzeichnisErstellungFehlgeschlagen"), text, arg));
					}
					finally
					{
					}
				}
				return text;
			}
		}

		public string ArbeitsbereicheSpeicherVerzeichnis
		{
			get
			{
				string text = base.VerzeichnisNameAufbereitet(this.ArbeitsbereicheSpeicherVerzeichnisRoh);
				if (!Directory.Exists(text))
				{
					try
					{
						Directory.CreateDirectory(text);
					}
					catch (Exception arg)
					{
						throw new ApplicationException(string.Format(ResReader.Reader.GetString("VerzeichnisErstellungFehlgeschlagen"), text, arg));
					}
					finally
					{
					}
				}
				return text;
			}
		}

		public string SupportEmailAdresse
		{
			get
			{
				return "support@springwald.de";
			}
		}

		public string SupportWebScriptURL
		{
			get
			{
				return "http://www.springwald.de/web/support/error.aspx";
			}
		}

		public string FeedbackEmailAdresse
		{
			get
			{
				return "support@springwald.de";
			}
		}

		public string FeedbackWebScriptURL
		{
			get
			{
				return "http://www.springwald.de/web/support/feedback.aspx";
			}
		}

		private static string ConfigDateiname
		{
			get
			{
				string text = (!ToolboxSonstiges.IstEntwicklungsmodus) ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), Application.ProductName) : Path.Combine(Application.StartupPath, "..\\..\\_Speichern_");
				if (!Directory.Exists(text))
				{
					try
					{
						Directory.CreateDirectory(text);
					}
					catch (Exception arg)
					{
						throw new ApplicationException(string.Format(ResReader.Reader.GetString("VerzeichnisErstellungFehlgeschlagen"), text, arg));
					}
					finally
					{
					}
				}
				return text + "\\config.xml";
			}
		}

		public static Config GlobalConfig
		{
			get
			{
				if (Config._globalConfig == null)
				{
					Config._globalConfig = new Config(Config.ConfigDateiname);
				}
				return Config._globalConfig;
			}
		}

		public string URLAktuelleInformationen
		{
			get
			{
				return this.NewsWebAdresse();
			}
		}

		public string URLErsteSchritte
		{
			get
			{
				return this.WebDokumentAdresse("firststeps");
			}
		}

		public string URLHandbuch
		{
			get
			{
				return this.WebDokumentAdresse("manual");
			}
		}

		public string URLNochKeineGaitoBotID
		{
			get
			{
				return this.WebGoAdresse("keineGaitobotID");
			}
		}

		public Config(string dateiname)
			: base(dateiname)
		{
		}

		private string NewsWebAdresse()
		{
			return global::MultiLang.ml.ml_string(77, "http://www.gaitobot.de/gaitobot/EditorDocuments/news_de.aspx");
		}

		private string WebGoAdresse(string goName)
		{
			Version version = new Version(Application.ProductVersion);
			return string.Format(ResReader.Reader.GetString("WebGoAdresse"), version.Major, version.Minor, version.Revision, LizenzManager.ProgrammLizenzName, goName);
		}

		private string WebDokumentAdresse(string dokumentKennung)
		{
			Version version = new Version(Application.ProductVersion);
			return string.Format(ResReader.Reader.GetString("WebDokumentAdresse"), version.Major, version.Minor, version.Revision, LizenzManager.ProgrammLizenzName, dokumentKennung);
		}
	}
}
