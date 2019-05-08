using de.springwald.toolbox;
using GaitoBotEditorCore;
using MultiLang;
using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace GaitoBotEditor
{
	internal class programm
	{
		[STAThread]
		private static void Main()
		{
			int num = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length;
			if (num > 1)
			{
				MessageBox.Show(global::MultiLang.ml.ml_string(51, "Das Programm wird bereits ausgeführt."));
			}
			else
			{
				Application.ThreadException += programm.Application_ThreadException;
				if (Config.GlobalConfig.ProgrammSprache == "")
				{
					SpracheAuswaehlen spracheAuswaehlen = new SpracheAuswaehlen();
					Application.DoEvents();
					spracheAuswaehlen.Show();
					try
					{
						while (!spracheAuswaehlen.Visible)
						{
							Application.DoEvents();
						}
					}
					catch (Exception)
					{
					}
					Application.DoEvents();
					try
					{
						while (spracheAuswaehlen != null && spracheAuswaehlen.Visible)
						{
							Application.DoEvents();
						}
					}
					catch (Exception)
					{
					}
				}
				try
				{
					Application.Run(new frmMain());
				}
				catch (Exception e)
				{
					programm.ZeigeFehler(e);
				}
			}
		}

		private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
		{
			programm.ZeigeFehler(e.Exception);
			Application.Exit();
		}

		private static void ZeigeFehler(Exception e)
		{
			frmFehlerReport frmFehlerReport = new frmFehlerReport();
			frmFehlerReport.ZeigeFehler(e, "", Config.GlobalConfig.SupportEmailAdresse, Config.GlobalConfig.SupportWebScriptURL);
			Application.DoEvents();
			while (frmFehlerReport.Visible)
			{
				Application.DoEvents();
			}
			frmFehlerReport.Dispose();
		}
	}
}
