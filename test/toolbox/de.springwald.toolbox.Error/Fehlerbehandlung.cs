using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Web;
using System.Web.SessionState;

namespace de.springwald.toolbox.Error
{
	public abstract class Fehlerbehandlung
	{
		public static string FehlerEmailAbsender
		{
			get
			{
				return "info@springwald.de";
			}
		}

		public static string FehlerEmailEmpfaenger
		{
			get
			{
				return "info@springwald.de";
			}
		}

		public Fehlerbehandlung()
		{
		}

		public static string GenExceptionInfos(Exception exception, params string[] zusatzinformationszeilen)
		{
			return Fehlerbehandlung.GenExceptionInfos(exception, true, zusatzinformationszeilen);
		}

		public static string GenExceptionInfos(Exception exception, bool systemdetails, params string[] zusatzinformationszeilen)
		{
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = stringBuilder;
			DateTime dateTime = DateTime.Now;
			string arg = dateTime.ToString();
			dateTime = DateTime.Now;
			dateTime = dateTime.ToUniversalTime();
			stringBuilder2.AppendFormat("Es wurde um {0} ({1} UTC) ein Fehler aufgezeichnet.\r\n\r\n", arg, dateTime.ToString());
			Exception ex = (exception == null) ? null : exception.GetBaseException();
			if (ex != null)
			{
				stringBuilder.Append("Fehlermeldung: " + ex.Message + "\r\n");
			}
			try
			{
				stringBuilder.AppendFormat("NetBIOS-Name: {0}\r\n", Environment.MachineName);
			}
			catch (Exception ex2)
			{
				stringBuilder.AppendFormat("Der NetBIOS-Name des Computers kann nicht ermittelt werden: {0}\r\n", ex2.Message);
			}
			try
			{
				stringBuilder.AppendFormat("DNS-Hostnamen: {0}\r\n", Dns.GetHostName());
			}
			catch (Exception ex3)
			{
				stringBuilder.AppendFormat("Beim Auflösen des Hostnamens ist ein Fehler aufgetreten: {0}\r\n", ex3.Message);
			}
			HttpContext current = HttpContext.Current;
			if (current != null && current.Request != null)
			{
				stringBuilder.Append("Http Servername: " + current.Request.ServerVariables.Get("SERVER_NAME") + "\r\n");
				if (systemdetails)
				{
					stringBuilder.Append("Http ServerIP: " + current.Request.ServerVariables.Get("LOCAL_ADDR") + "\r\n");
				}
				if (current.Request.Url != (Uri)null)
				{
					stringBuilder.Append("Http Url: " + current.Request.Url.AbsoluteUri + "\r\n");
				}
			}
			if (systemdetails)
			{
				stringBuilder.Append("App UserName: " + Environment.UserName + "\r\n");
				try
				{
					stringBuilder.Append("App UserDomainName: " + Environment.UserDomainName + "\r\n");
				}
				catch
				{
				}
				stringBuilder.Append("UserInteractive: " + Environment.UserInteractive + "\r\n");
			}
			if (zusatzinformationszeilen != null && zusatzinformationszeilen.Length != 0)
			{
				stringBuilder.Append(Fehlerbehandlung.ErzeugeKopfzeile("Zusatzinformationen"));
				foreach (string str in zusatzinformationszeilen)
				{
					stringBuilder.Append(" >> " + str + "\r\n");
				}
			}
			if (systemdetails)
			{
				Exception ex4 = exception;
				int num = 0;
				while (ex4 != null && num > -20)
				{
					stringBuilder.Append(Fehlerbehandlung.ErzeugeExceptionDateils(string.Format("Exception Ebene: {0}", num.ToString()), ex4));
					num--;
					ex4 = ex4.InnerException;
				}
			}
			if (systemdetails)
			{
				try
				{
					AppDomain currentDomain = AppDomain.CurrentDomain;
					if (currentDomain != null)
					{
						stringBuilder.Append(Fehlerbehandlung.ErzeugeKopfzeile("AppDomain"));
						stringBuilder.Append("AppDomain.BaseDirectory: " + currentDomain.BaseDirectory + "\r\n");
						stringBuilder.Append("AppDomain.RelativeSearchPath: " + currentDomain.RelativeSearchPath + "\r\n");
						stringBuilder.Append("AppDomain.FriendlyName: " + currentDomain.FriendlyName + "\r\n");
						stringBuilder.Append("AppDomain.Id: " + currentDomain.Id + "\r\n");
						if (currentDomain.SetupInformation != null)
						{
							stringBuilder.Append(Fehlerbehandlung.ErzeugeKopfzeile("SetupInformation"));
							stringBuilder.Append("SetupInformation.ApplicationBase: " + currentDomain.SetupInformation.ApplicationBase + "\r\n");
							stringBuilder.Append("SetupInformation.ApplicationName: " + currentDomain.SetupInformation.ApplicationName + "\r\n");
							stringBuilder.Append("SetupInformation.ConfigurationFile: " + currentDomain.SetupInformation.ConfigurationFile + "\r\n");
							stringBuilder.Append("SetupInformation.PrivateBinPath: " + currentDomain.SetupInformation.PrivateBinPath + "\r\n");
						}
					}
				}
				catch (Exception ex5)
				{
					stringBuilder.AppendFormat("Fehler beim Zugriff auf Werte der AppDomain.CurrentDomain: {0}\r\n", ex5.Message);
				}
				stringBuilder.Append("Environment.CommandLine: " + Environment.CommandLine + "\r\n");
			}
			if (systemdetails)
			{
				try
				{
					HttpContext current2 = HttpContext.Current;
					if (current2 != null && current2.Request != null)
					{
						stringBuilder.Append(Fehlerbehandlung.ErzeugeKopfzeile("HTTPAufrufdetails"));
						stringBuilder.Append("RequestType: " + current2.Request.RequestType + "\r\n");
						stringBuilder.Append("Url: " + current2.Request.Url.AbsoluteUri + "\r\n");
						stringBuilder.Append("Path: " + current2.Request.Path + "\r\n");
						stringBuilder.Append("ApplicationPath: " + current2.Request.ApplicationPath + "\r\n");
						stringBuilder.Append("Handler: " + ((current2.Handler == null) ? "null" : current2.Handler.ToString()) + "\r\n");
						stringBuilder.Append(Fehlerbehandlung.ErzeugeKopfzeile("Session"));
						if (current2.Handler != null && (current2.Handler is IRequiresSessionState || current2.Handler is IReadOnlySessionState))
						{
							stringBuilder.Append("Session.IsNewSession: " + current2.Session.IsNewSession + "\r\n");
							stringBuilder.Append("Session.IsReadOnly: " + current2.Session.IsReadOnly + "\r\n");
							stringBuilder.Append("Session.SessionID: " + current2.Session.SessionID + "\r\n");
							stringBuilder.Append("Session.Count: " + current2.Session.Count + "\r\n");
						}
						stringBuilder.Append(Fehlerbehandlung.ErzeugeKopfzeile("QueryString"));
						if (current2.Request.QueryString != null && current2.Request.QueryString.Count > 0)
						{
							string[] allKeys = current2.Request.QueryString.AllKeys;
							foreach (string text in allKeys)
							{
								stringBuilder.Append(text + "=" + current2.Request.QueryString[text] + "\r\n");
							}
						}
						stringBuilder.Append(Fehlerbehandlung.ErzeugeKopfzeile("PostData"));
						if (current2.Request.Form != null && current2.Request.Form.Count > 0)
						{
							string[] allKeys2 = current2.Request.Form.AllKeys;
							foreach (string text2 in allKeys2)
							{
								stringBuilder.Append(text2 + "=" + current2.Request.Form[text2] + "\r\n");
							}
						}
						stringBuilder.Append(Fehlerbehandlung.ErzeugeKopfzeile("HTTP ServerVariables"));
						string[] allKeys3 = current2.Request.ServerVariables.AllKeys;
						foreach (string text3 in allKeys3)
						{
							stringBuilder.Append(text3 + "=" + current2.Request.ServerVariables[text3] + "\r\n");
						}
					}
				}
				catch (Exception ex6)
				{
					stringBuilder.Append("Fehler beim Zugriff auf Werte des HttpContext: " + ex6.Message);
				}
			}
			return stringBuilder.ToString();
		}

		private static string ErzeugeKopfzeile(string kopfzeile)
		{
			if (kopfzeile == null)
			{
				kopfzeile = string.Empty;
			}
			else if (kopfzeile.Length < 52)
			{
				kopfzeile = kopfzeile.PadLeft(26 + kopfzeile.Length / 2).PadRight(52);
			}
			return string.Format("\r\n\r\n{0}{0}\r\n--- {1} ---\r\n{0}{0}\r\n\r\n", "------------------------------", kopfzeile);
		}

		private static string ErzeugeExceptionDateils(string kopfzeile, Exception exception)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(Fehlerbehandlung.ErzeugeKopfzeile(kopfzeile));
			if (exception != null)
			{
				stringBuilder.Append("Message: " + exception.Message + "\r\n");
				stringBuilder.Append("Source: " + exception.Source + "\r\n");
				stringBuilder.Append("TargetSite: " + ((exception.TargetSite == null) ? "" : exception.TargetSite.ToString()) + "\r\n");
				try
				{
					stringBuilder.Append("Assembly: " + ((exception.TargetSite != null && exception.TargetSite.Module != null && exception.TargetSite.Module.Assembly != null) ? exception.TargetSite.Module.Assembly.FullName : "") + "\r\n");
				}
				catch
				{
				}
				stringBuilder.Append("StackTrace:\r\n" + exception.StackTrace + "\r\n");
				if (exception.Data != null && exception.Data.Count > 0)
				{
					stringBuilder.Append("UserData:\r\n");
					foreach (object key in exception.Data.Keys)
					{
						stringBuilder.Append(">" + key + ": " + exception.Data[key] + "\r\n");
					}
				}
			}
			return stringBuilder.ToString();
		}

		public static string fehlermeldungErzeugen(string ortsBeschreibung, Exception oE, string benutzerBeschreibung, HttpContext context)
		{
			StringBuilder stringBuilder = new StringBuilder();
			List<string> list = new List<string>();
			if (ortsBeschreibung != null)
			{
				list.Add("Ort: " + ortsBeschreibung);
			}
			if (context != null && context.Session != null && context.Session["UserSession.LogSessionIDKey"] != null)
			{
				list.Add("SessionLog-ID: " + context.Session["UserSession.LogSessionIDKey"]);
			}
			if (!string.IsNullOrEmpty(benutzerBeschreibung))
			{
				list.Add("Angemeldeter Benutzer: " + benutzerBeschreibung);
			}
			return Fehlerbehandlung.GenExceptionInfos(oE, list.ToArray());
		}
	}
}
