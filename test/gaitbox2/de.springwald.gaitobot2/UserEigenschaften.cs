using System;
using System.Collections;
using System.Text;

namespace de.springwald.gaitobot2
{
	public class UserEigenschaften
	{
		private Hashtable _eigenschaften;

		public UserEigenschaften()
		{
			this._eigenschaften = new Hashtable();
		}

		public void Leeren()
		{
			this._eigenschaften = new Hashtable();
		}

		public string GetSerialString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			IDictionaryEnumerator enumerator = this._eigenschaften.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					DictionaryEntry dictionaryEntry = (DictionaryEntry)enumerator.Current;
					if (!this.IsEmpty(dictionaryEntry.Key.ToString()))
					{
						if (stringBuilder.Length == 0)
						{
							stringBuilder.AppendFormat("{0}={1}", dictionaryEntry.Key, dictionaryEntry.Value);
						}
						else
						{
							stringBuilder.AppendFormat("|{0}={1}", dictionaryEntry.Key, dictionaryEntry.Value);
						}
					}
				}
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			return stringBuilder.ToString();
		}

		public void FillFromSerialString(string inhalte)
		{
			string[] array = inhalte.Split('|');
			string[] array2 = array;
			foreach (string text in array2)
			{
				string[] array3 = text.Split('=');
				if (array3.Length == 2)
				{
					string text2 = array3[0].Trim();
					if (text2.Length != 0)
					{
						this._eigenschaften[text2] = array3[1];
					}
				}
			}
		}

		public bool IsEmpty(string name)
		{
			name = name.ToLower();
			if (this._eigenschaften.Contains(name))
			{
				if (this._eigenschaften[name] == null)
				{
					return true;
				}
				if (((string)this._eigenschaften[name]).Trim() == "")
				{
					return true;
				}
				return false;
			}
			return true;
		}

		public string Lesen(string name)
		{
			name = name.ToLower();
			if (!this.IsEmpty(name))
			{
				return (string)this._eigenschaften[name];
			}
			return ResReader.Reader(null).GetString("unbekannteEigenschaft");
		}

		public void Setzen(string name, string inhalt)
		{
			name = name.ToLower();
			this._eigenschaften[name] = inhalt;
		}
	}
}
