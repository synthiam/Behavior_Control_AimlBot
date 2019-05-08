using Microsoft.Win32;
using System;

namespace de.springwald.toolbox
{
	public class RebootRegistry
	{
		private static DateTime _letzteStarterMeldung;

		public static void RegelmaessigMelden()
		{
			if ((DateTime.UtcNow - RebootRegistry._letzteStarterMeldung).TotalSeconds > 20.0)
			{
				RebootRegistry._letzteStarterMeldung = DateTime.UtcNow;
				RebootRegistry.SetzeRegKey("Lebenszeichen", DateTime.UtcNow.ToBinary().ToString());
			}
		}

		public static RegistryKey ConfigRegistryKey()
		{
			return Registry.CurrentUser.CreateSubKey("Software\\Springwald\\Game");
		}

		public static string ErmittleRegKey(string keyname, string vorgabeWennLeer)
		{
			RegistryKey registryKey = RebootRegistry.ConfigRegistryKey();
			string result = null;
			try
			{
				result = (string)registryKey.GetValue(keyname, vorgabeWennLeer);
			}
			finally
			{
				if (registryKey != null)
				{
					registryKey.Close();
				}
			}
			return result;
		}

		public static void SetzeRegKey(string keyname, string inhalt)
		{
			RegistryKey registryKey = RebootRegistry.ConfigRegistryKey();
			try
			{
				registryKey.SetValue(keyname, inhalt);
			}
			finally
			{
				if (registryKey != null)
				{
					registryKey.Close();
				}
			}
		}
	}
}
