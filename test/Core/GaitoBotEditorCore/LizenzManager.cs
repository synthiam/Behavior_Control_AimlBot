using System;
using System.Windows.Forms;

namespace GaitoBotEditorCore
{
	public abstract class LizenzManager
	{
		public enum LizenzArten
		{
			Betaversion,
			Standard
		}

		private static LizenzArten _lizenzArt;

		private static bool _bereitsErmittelt = false;

		private static string _warumDarfProgrammNichtGestartetWerden = null;

		private static string _lizenziertAuf;

		public static string ProgrammName
		{
			get
			{
				return Application.ProductName;
			}
		}

		public static string ProgrammLizenzName
		{
			get
			{
				LizenzManager.LizenzErmitteln();
				return LizenzManager._lizenzArt.ToString();
			}
		}

		public static string ProgrammVersionNummerAnzeige
		{
			get
			{
				Version version = new Version(Application.ProductVersion);
				return string.Format("{0}.{1}.{2} B{3}", version.Major, version.Minor, version.Build, version.Revision);
			}
		}

		public static string LizenziertAuf
		{
			get
			{
				LizenzManager.LizenzErmitteln();
				return LizenzManager._lizenziertAuf;
			}
		}

		public static LizenzArten Lizenz
		{
			get
			{
				LizenzManager.LizenzErmitteln();
				return LizenzManager._lizenzArt;
			}
		}

		public static string WarumDarfProgrammNichtGestartetWerden
		{
			get
			{
				LizenzManager.LizenzErmitteln();
				return LizenzManager._warumDarfProgrammNichtGestartetWerden;
			}
		}

		public static bool DarfProgrammUeberhauptGestartetWerden
		{
			get
			{
				return LizenzManager.WarumDarfProgrammNichtGestartetWerden == null;
			}
		}

		public LizenzManager()
		{
		}

		private static void LizenzErmitteln()
		{
			if (!LizenzManager._bereitsErmittelt)
			{
				LizenzManager._warumDarfProgrammNichtGestartetWerden = null;
				LizenzManager._lizenziertAuf = "-";
				LizenzManager._lizenzArt = LizenzArten.Standard;
			}
		}
	}
}
