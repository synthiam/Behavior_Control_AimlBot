using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace de.springwald.gaitobot.content
{
	public class ContentManager
	{
		private ContentElementInfo[] _infos;

		public ContentElementInfo[] EnthalteneContentElementInfos
		{
			get
			{
				if (this._infos == null)
				{
					string value = "de.springwald.gaitobot.content.content.";
					string value2 = ".info";
					List<ContentElementInfo> list = new List<ContentElementInfo>();
					Assembly executingAssembly = Assembly.GetExecutingAssembly();
					string[] manifestResourceNames = executingAssembly.GetManifestResourceNames();
					foreach (string text in manifestResourceNames)
					{
						if (text.StartsWith(value) && text.EndsWith(value2))
						{
							Stream manifestResourceStream = executingAssembly.GetManifestResourceStream(text);
							StreamReader streamReader = new StreamReader(manifestResourceStream, Encoding.GetEncoding("ISO-8859-15"));
							string xml = streamReader.ReadToEnd();
							ContentElementInfo item = ContentElementInfo.ReadFromXmlString(xml);
							streamReader.Close();
							list.Add(item);
						}
					}
					this._infos = (from b in list
					orderby b.SortKey
					select b).ToArray();
				}
				return this._infos;
			}
		}

		public List<ContentDokument> GetDokumente(string uniqueId)
		{
			List<ContentDokument> list = new List<ContentDokument>();
			ContentElementInfo contentElementInfo = (from i in this.EnthalteneContentElementInfos
			where i.UniqueKey == uniqueId
			select i).FirstOrDefault();
			if (contentElementInfo == null)
			{
				throw new ApplicationException(string.Format("Contentdokument mit unique-ID '{0}' nicht vorhanden!", uniqueId));
			}
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			string[] manifestResourceNames = executingAssembly.GetManifestResourceNames();
			foreach (string text in manifestResourceNames)
			{
				if (!text.EndsWith(".info"))
				{
					bool flag = false;
					string[] array = text.Split(new char[1]
					{
						'.'
					}, StringSplitOptions.RemoveEmptyEntries);
					string titel = array[array.Length - 2];
					string dateiExtension = array[array.Length - 1];
					if (contentElementInfo.DateiPattern.EndsWith("*"))
					{
						string text2 = contentElementInfo.DateiPattern.Substring(0, contentElementInfo.DateiPattern.Length - 1).ToLower();
						flag = (text.ToLower().StartsWith(text2) && text.Length > text2.Length);
					}
					else
					{
						flag = (text.ToLower() == contentElementInfo.DateiPattern.ToLower());
					}
					if (flag)
					{
						Stream manifestResourceStream = executingAssembly.GetManifestResourceStream(text);
						StreamReader streamReader = new StreamReader(manifestResourceStream, Encoding.GetEncoding("ISO-8859-15"));
						string inhalt = streamReader.ReadToEnd();
						string[] unterverzeichnise = new string[1]
						{
							contentElementInfo.Name
						};
						list.Add(new ContentDokument
						{
							Inhalt = inhalt,
							Titel = titel,
							Unterverzeichnise = unterverzeichnise,
							DateiExtension = dateiExtension,
							ZusatzContentUniqueId = uniqueId
						});
						streamReader.Close();
					}
				}
			}
			if (list.Count == 0)
			{
				throw new ApplicationException(string.Format("Keine Dokumente für Content UniqueID '{0}' gefunden!", uniqueId));
			}
			return list;
		}
	}
}
