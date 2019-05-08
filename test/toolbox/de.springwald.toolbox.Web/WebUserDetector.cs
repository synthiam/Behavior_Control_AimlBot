using System.Web;

namespace de.springwald.toolbox.Web
{
	public abstract class WebUserDetector
	{
		public static bool IstGoogleBot(HttpRequest request)
		{
			if (request == null)
			{
				return false;
			}
			string text = request.ServerVariables["HTTP_USER_AGENT"];
			if (!string.IsNullOrEmpty(text) && text.Contains("Googlebot"))
			{
				return true;
			}
			return false;
		}
	}
}
