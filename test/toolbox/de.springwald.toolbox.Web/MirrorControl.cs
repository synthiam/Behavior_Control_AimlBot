using System.Web.UI;
using System.Web.UI.WebControls;

namespace de.springwald.toolbox.Web
{
	[ToolboxData("<{0}:MirrorControl runat=server></{0}:MirrorControl>")]
	public class MirrorControl : WebControl
	{
		public string ControlID = null;

		protected override void Render(HtmlTextWriter writer)
		{
			if (this.ControlID != null)
			{
				Control control = this.Parent.FindControl(this.ControlID);
				if (control != null)
				{
					control.RenderControl(writer);
				}
			}
		}
	}
}
