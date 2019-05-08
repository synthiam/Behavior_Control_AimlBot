using de.springwald.toolbox.Text;
using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.UI;

namespace de.springwald.toolbox.Web
{
	public class UrlBuilder : UriBuilder
	{
		private StringDictionary _queryString = null;

		public StringDictionary QueryString
		{
			get
			{
				if (this._queryString == null)
				{
					this._queryString = new StringDictionary();
				}
				return this._queryString;
			}
		}

		public string PageName
		{
			get
			{
				string path = base.Path;
				return path.Substring(path.LastIndexOf("/") + 1);
			}
			set
			{
				string path = base.Path;
				path = path.Substring(0, path.LastIndexOf("/"));
				base.Path = path + "/" + value;
			}
		}

		public UrlBuilder()
		{
		}

		public UrlBuilder(string uri)
			: base(uri)
		{
			this.PopulateQueryString();
		}

		public UrlBuilder(Uri uri)
			: base(uri)
		{
			this.PopulateQueryString();
		}

		public UrlBuilder(string schemeName, string hostName)
			: base(schemeName, hostName)
		{
		}

		public UrlBuilder(string scheme, string host, int portNumber)
			: base(scheme, host, portNumber)
		{
		}

		public UrlBuilder(string scheme, string host, int port, string pathValue)
			: base(scheme, host, port, pathValue)
		{
		}

		public UrlBuilder(string scheme, string host, int port, string path, string extraValue)
			: base(scheme, host, port, path, extraValue)
		{
		}

		public UrlBuilder(Page page)
			: base(page.Request.Url.AbsoluteUri)
		{
			this.PopulateQueryString();
		}

		public void SetzeParameterEncodedEin(string name, string inhalt)
		{
			if (string.IsNullOrEmpty(inhalt))
			{
				this.QueryString.Remove(name);
			}
			else
			{
				this.QueryString[name] = EncodingTools.UrlISO885915Encode(inhalt);
			}
		}

		public new string ToString()
		{
			this.GetQueryString();
			return base.Uri.AbsoluteUri;
		}

		public void Navigate()
		{
			this._Navigate(true);
		}

		public void Navigate(bool endResponse)
		{
			this._Navigate(endResponse);
		}

		private void _Navigate(bool endResponse)
		{
			string url = this.ToString();
			HttpContext.Current.Response.Redirect(url, endResponse);
		}

		private void PopulateQueryString()
		{
			string query = base.Query;
			if (!string.IsNullOrEmpty(query))
			{
				if (this._queryString == null)
				{
					this._queryString = new StringDictionary();
				}
				this._queryString.Clear();
				query = query.Substring(1);
				string[] array = query.Split('&');
				string[] array2 = array;
				foreach (string text in array2)
				{
					string[] array3 = text.Split('=');
					this._queryString[array3[0]] = ((array3.Length > 1) ? array3[1] : string.Empty);
				}
			}
		}

		private void GetQueryString()
		{
			int count = this.QueryString.Count;
			if (count == 0)
			{
				base.Query = string.Empty;
			}
			else
			{
				string[] array = new string[count];
				string[] array2 = new string[count];
				string[] array3 = new string[count];
				this._queryString.Keys.CopyTo(array, 0);
				this._queryString.Values.CopyTo(array2, 0);
				for (int i = 0; i < count; i++)
				{
					array3[i] = array[i] + "=" + array2[i];
				}
				base.Query = string.Join("&", array3);
			}
		}
	}
}
