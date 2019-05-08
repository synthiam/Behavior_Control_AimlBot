using de.springwald.toolbox;
using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace de.springwald.gaitobot2
{
	internal class WissensLader
	{
		public class AimlDateiWirdGeladenEventArgs : EventArgs
		{
			public readonly string Dateiname;

			public AimlDateiWirdGeladenEventArgs(string dateiname)
			{
				this.Dateiname = dateiname;
			}
		}

		public delegate void AimlDateiWirdGeladenEventHandler(object sender, AimlDateiWirdGeladenEventArgs e);

		private readonly Wissen _wissen;

		private readonly Normalisierung _normalisierung;

		private readonly StringBuilder _protokoll;

		private readonly CultureInfo _protokollKultur;

		public string Protokoll
		{
			get
			{
				return this._protokoll.ToString();
			}
		}

		public event AimlDateiWirdGeladenEventHandler AimlDateiWirdGeladen;

		protected virtual void AimlDateiWirdGeladenEvent(string dateiname)
		{
			if (this.AimlDateiWirdGeladen != null)
			{
				this.AimlDateiWirdGeladen(this, new AimlDateiWirdGeladenEventArgs(dateiname));
			}
		}

		public WissensLader(Wissen wissen, CultureInfo protokollKultur, Normalisierung normalisierung)
		{
			this._wissen = wissen;
			this._protokollKultur = protokollKultur;
			this._normalisierung = normalisierung;
			this._protokoll = new StringBuilder();
			this._protokoll.AppendFormat(ResReader.Reader(this._protokollKultur).GetString("WissensLadenGestartetUm", this._protokollKultur), DateTime.Now.ToString());
			this._protokoll.Append("\n");
		}

		public void LadeAimlDateienAusVerzeichnis(string verzeichnis, GaitoBotEigenschaften botEigenschaften)
		{
			if (Directory.Exists(verzeichnis))
			{
				string[] files = Directory.GetFiles(verzeichnis, "*.aiml");
				if (files.Length == 0)
				{
					this._protokoll.AppendFormat(ResReader.Reader(this._protokollKultur).GetString("KeineAIMLDateienImVerzeichnisGefunden", this._protokollKultur), verzeichnis);
					this._protokoll.Append("\n");
				}
				string[] array = files;
				foreach (string aimlDateiname in array)
				{
					this.AIMLDateiVerarbeiten(aimlDateiname, botEigenschaften);
				}
				this._protokoll.AppendFormat(ResReader.Reader(this._protokollKultur).GetString("VerzeichnisEingelesen", this._protokollKultur), verzeichnis, this._wissen.AnzahlCategories);
				this._protokoll.Append("\n");
			}
			else
			{
				this._protokoll.AppendFormat(ResReader.Reader(this._protokollKultur).GetString("VerzeichnisNichtGefunden", this._protokollKultur), verzeichnis);
				this._protokoll.Append("\n");
			}
		}

		public void AIMLDateiVerarbeiten(string aimlDateiname, GaitoBotEigenschaften botEigenschaften)
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.PreserveWhitespace = true;
			xmlDocument.Load(aimlDateiname);
			this.AimldomDokumentVerarbeiten(xmlDocument, aimlDateiname, botEigenschaften);
		}

		public void AimldomDokumentVerarbeiten(XmlDocument doc, string aimlDateiname, GaitoBotEigenschaften botEigenschaften)
		{
			this.AimlDateiWirdGeladenEvent(aimlDateiname);
			ToolboxXML.WhitespacesBereinigen(doc.OwnerDocument);
			XmlNodeList childNodes = doc.DocumentElement.ChildNodes;
			foreach (XmlNode item in childNodes)
			{
				string name = item.Name;
				if (!(name == "topic"))
				{
					if (name == "category")
					{
						this.CategoryVerarbeiten(item, aimlDateiname, botEigenschaften);
					}
				}
				else
				{
					this.TopicVerarbeiten(item, aimlDateiname, botEigenschaften);
				}
			}
		}

		private void TopicVerarbeiten(XmlNode topicNode, string aimlDateiname, GaitoBotEigenschaften botEigenschaften)
		{
			string text = topicNode.Attributes[0].Value;
			if (string.IsNullOrEmpty(text))
			{
				text = "*";
			}
			foreach (XmlNode item in topicNode.SelectNodes("category"))
			{
				if (item.Name == "category")
				{
					this.CategoryVerarbeiten(item, text, aimlDateiname, botEigenschaften);
				}
			}
		}

		private void CategoryVerarbeiten(XmlNode categoryNode, string aimlDateiname, GaitoBotEigenschaften botEigenschaften)
		{
			this.CategoryVerarbeiten(categoryNode, "*", aimlDateiname, botEigenschaften);
		}

		private void CategoryVerarbeiten(XmlNode categoryNode, string themaName, string aimlDateiname, GaitoBotEigenschaften botEigenschaften)
		{
			WissensCategory category = new WissensCategory(this._normalisierung, categoryNode, themaName, aimlDateiname, botEigenschaften);
			this._wissen.CategoryAufnehmen(category);
		}
	}
}
