using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace de.springwald.toolbox.Web
{
	public abstract class WebPlaceholderTools
	{
		public static bool IstPlaceholderLeer(ContentPlaceHolder placeHolder)
		{
			foreach (Control control in placeHolder.Controls)
			{
				if (control is LiteralControl)
				{
					if (((LiteralControl)placeHolder.Controls[0]).Text.Trim(' ', '\r', '\n') != "")
					{
						return false;
					}
					continue;
				}
				if (!(control is ContentPlaceHolder))
				{
					return false;
				}
				if (!WebPlaceholderTools.IstPlaceholderLeer((ContentPlaceHolder)control))
				{
					return false;
				}
			}
			return true;
		}

		public static string GetPlaceholderText(ContentPlaceHolder placeHolder)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (object control in placeHolder.Controls)
			{
				if (control is LiteralControl && !(((LiteralControl)control).Text.Trim(' ', '\r', '\n') == ""))
				{
					stringBuilder.Append(((LiteralControl)control).Text);
				}
				if (control is Literal && !(((Literal)control).Text.Trim(' ', '\r', '\n') == ""))
				{
					stringBuilder.Append(((Literal)control).Text);
				}
				if (control is ContentPlaceHolder)
				{
					stringBuilder.Append(WebPlaceholderTools.GetPlaceholderText((ContentPlaceHolder)control));
				}
			}
			return stringBuilder.ToString();
		}
	}
}
