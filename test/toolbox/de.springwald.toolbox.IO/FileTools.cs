using System;
using System.IO;
using System.Text;

namespace de.springwald.toolbox.IO
{
	public abstract class FileTools
	{
		public static void String2File(string inhalt, string dateiname)
		{
			if (File.Exists(dateiname))
			{
				try
				{
					File.Delete(dateiname);
				}
				catch (Exception arg)
				{
					throw new ApplicationException(string.Format("Konnte ZielDatei '{0}' nicht löschen: \n\n{1}", dateiname, arg));
				}
			}
			StreamWriter streamWriter;
			try
			{
				streamWriter = new StreamWriter(dateiname, false, Encoding.GetEncoding("ISO-8859-15"));
				streamWriter.AutoFlush = true;
			}
			catch (Exception arg2)
			{
				throw new ApplicationException(string.Format("Konnte ZielDatei '{0}' nicht anlegen: \n\n{1}", dateiname, arg2));
			}
			streamWriter.Write(inhalt);
			streamWriter.Close();
		}

		public static void AppendString2File_(string inhalt, string dateiname)
		{
			if (!File.Exists(dateiname))
			{
				FileTools.String2File(inhalt, dateiname);
			}
			else
			{
				try
				{
					StreamWriter streamWriter = new StreamWriter(dateiname, true, Encoding.GetEncoding("ISO-8859-15"));
					streamWriter.AutoFlush = true;
					streamWriter.Write(inhalt);
					streamWriter.Close();
				}
				catch (Exception arg)
				{
					throw new ApplicationException(string.Format("Konnte an ZielDatei '{0}' nicht anhängen: \n\n{1}", dateiname, arg));
				}
			}
		}

		public static string File2String(string dateiname)
		{
			string text = "";
			try
			{
				StreamReader streamReader = new StreamReader(dateiname, Encoding.GetEncoding("ISO-8859-15"));
				text = streamReader.ReadToEnd();
				streamReader.Close();
			}
			catch (FileNotFoundException ex)
			{
				throw new ApplicationException(string.Format("Konnte Datei '{0}' nicht einlesen:\n{1}", dateiname, ex.Message));
			}
			return text;
		}
	}
}
