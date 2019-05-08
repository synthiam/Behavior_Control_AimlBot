using System.Web;

namespace de.springwald.toolbox.Web
{
	public class WebAnwendungsKontext
	{
		public static bool IstLocalHost(HttpContext context)
		{
			return false;
		}
	}
}
