using System;

namespace de.springwald.toolbox.caching
{
	public interface ICacheParameter
	{
		TimeSpan MaxObjektLebensdauer
		{
			get;
			set;
		}

		long MaxObjekte
		{
			get;
			set;
		}

		string GetStatus(string zeilenUmbruch);
	}
}
