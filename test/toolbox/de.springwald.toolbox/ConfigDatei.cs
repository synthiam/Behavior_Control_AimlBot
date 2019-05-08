using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace de.springwald.toolbox
{
	public abstract class ConfigDatei
	{
		private string _dateiname;

		private XmlDocument _xml;

		private XmlNode _werteRoot;

		private XmlNode _appRoot;

		public ConfigDatei(string dateiname)
		{
			this._dateiname = dateiname;
			this.Laden();
		}

		protected string VerzeichnisNameAufbereitet(string rohVerzeichnisName)
		{
			StringBuilder stringBuilder = new StringBuilder(rohVerzeichnisName);
			stringBuilder.Replace("[MyDocuments]", Environment.GetFolderPath(Environment.SpecialFolder.Personal));
			stringBuilder.Replace("[DesktopDirectory]", Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));
			stringBuilder.Replace("[LocalApplicationData]", Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
			stringBuilder.Replace("[MyComputer]", Environment.GetFolderPath(Environment.SpecialFolder.MyComputer));
			stringBuilder.Replace("[ProgramFiles]", Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles));
			stringBuilder.Replace("[Programs]", Environment.GetFolderPath(Environment.SpecialFolder.Programs));
			stringBuilder.Replace("[Templates]", Environment.GetFolderPath(Environment.SpecialFolder.Templates));
			stringBuilder.Replace("[StartupPath]", Application.StartupPath);
			return stringBuilder.ToString();
		}

		protected string getWert(string name)
		{
			XmlNode xmlNode = this._werteRoot.SelectSingleNode(name);
			if (xmlNode == null)
			{
				return null;
			}
			return xmlNode.InnerText;
		}

		protected string getWert(string name, string defaultWert)
		{
			XmlNode xmlNode = this._werteRoot.SelectSingleNode(name);
			if (xmlNode == null)
			{
				this.setWert(name, defaultWert);
				return defaultWert;
			}
			return xmlNode.InnerText;
		}

		protected bool getWert(string name, bool defaultWert)
		{
			XmlNode xmlNode = this._werteRoot.SelectSingleNode(name);
			if (xmlNode == null)
			{
				this.setWert(name, defaultWert);
				return defaultWert;
			}
			return xmlNode.InnerText == "true";
		}

		protected string getAppWert(string name)
		{
			XmlNode xmlNode = this._appRoot.SelectSingleNode(name);
			if (xmlNode == null)
			{
				return null;
			}
			return xmlNode.InnerText;
		}

		protected void setWert(string name, string wert)
		{
			XmlNode xmlNode = this._werteRoot.SelectSingleNode(name);
			if (xmlNode == null)
			{
				xmlNode = this._xml.CreateElement(name);
				this._werteRoot.AppendChild(xmlNode);
			}
			xmlNode.InnerText = wert;
			this.Speichern();
		}

		protected void setWert(string name, bool wert)
		{
			XmlNode xmlNode = this._werteRoot.SelectSingleNode(name);
			if (xmlNode == null)
			{
				xmlNode = this._xml.CreateElement(name);
				this._werteRoot.AppendChild(xmlNode);
			}
			if (wert)
			{
				xmlNode.InnerText = "true";
			}
			else
			{
				xmlNode.InnerText = "false";
			}
			this.Speichern();
		}

		private void setAppWert(string name, string wert)
		{
			XmlNode xmlNode = this._appRoot.SelectSingleNode(name);
			if (xmlNode == null)
			{
				xmlNode = this._xml.CreateElement(name);
				this._appRoot.AppendChild(xmlNode);
			}
			xmlNode.InnerText = wert;
		}

		private void Laden()
		{
			this._xml = new XmlDocument();
			if (File.Exists(this._dateiname))
			{
				this._xml.Load(this._dateiname);
				this._werteRoot = this._xml.DocumentElement.SelectSingleNode("content");
				this._appRoot = this._xml.DocumentElement.SelectSingleNode("file");
			}
			else
			{
				XmlNode xmlNode = this._xml.CreateElement("config");
				this._xml.AppendChild(xmlNode);
				this._appRoot = this._xml.CreateElement("file");
				xmlNode.AppendChild(this._appRoot);
				this._werteRoot = this._xml.CreateElement("content");
				xmlNode.AppendChild(this._werteRoot);
			}
		}

		private void Speichern()
		{
			this.setAppWert("app_name", Application.ProductName);
			this.setAppWert("app_version", Application.ProductVersion.ToString());
			int num = new Version(Application.ProductVersion).Major;
			this.setAppWert("app_major", num.ToString());
			num = new Version(Application.ProductVersion).Minor;
			this.setAppWert("app_minor", num.ToString());
			num = new Version(Application.ProductVersion).Revision;
			this.setAppWert("app_revision", num.ToString());
			num = new Version(Application.ProductVersion).Build;
			this.setAppWert("app_build", num.ToString());
			DateTime now;
			if (this.getAppWert("file_created") == null)
			{
				now = DateTime.Now;
				this.setAppWert("file_created", now.ToString());
			}
			now = DateTime.Now;
			this.setAppWert("file_lastsaved", now.ToString());
			this.setAppWert("system_os", Environment.OSVersion.VersionString);
			this.setAppWert("system_user", Environment.UserName);
			this._xml.Save(this._dateiname);
		}
	}
}
