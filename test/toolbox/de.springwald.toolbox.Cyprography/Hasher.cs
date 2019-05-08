using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace de.springwald.toolbox.Cyprography
{
	public class Hasher
	{
		public static string SHA1StringHash(string strString)
		{
			SHA1CryptoServiceProvider sHA1CryptoServiceProvider = new SHA1CryptoServiceProvider();
			string text = string.Empty;
			string empty = string.Empty;
			byte[] bytes = Encoding.ASCII.GetBytes(strString);
			byte[] array = sHA1CryptoServiceProvider.ComputeHash(bytes);
			for (int i = 0; i < array.Length; i++)
			{
				empty = Convert.ToString(array[i], 16);
				if (empty.Length == 1)
				{
					empty = "0" + empty;
				}
				text += empty;
			}
			return text;
		}

		private string SHA1FileHash(string dateiname)
		{
			SHA1CryptoServiceProvider sHA1CryptoServiceProvider = new SHA1CryptoServiceProvider();
			string text = string.Empty;
			string empty = string.Empty;
			FileStream fileStream = new FileStream(dateiname, FileMode.Open, FileAccess.Read, FileShare.Read, 8192);
			sHA1CryptoServiceProvider.ComputeHash(fileStream);
			fileStream.Close();
			byte[] hash = sHA1CryptoServiceProvider.Hash;
			for (int i = 0; i < hash.Length; i++)
			{
				empty = Convert.ToString(hash[i], 16);
				if (empty.Length == 1)
				{
					empty = "0" + empty;
				}
				text += empty;
			}
			return text;
		}
	}
}
