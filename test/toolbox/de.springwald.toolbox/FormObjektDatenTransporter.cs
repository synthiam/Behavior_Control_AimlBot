using System.Web.UI.WebControls;

namespace de.springwald.toolbox
{
	public abstract class FormObjektDatenTransporter
	{
		public static string ReadWriteWebTextBox(string wert, ref TextBox formObjekt, TransportRichtungen inControlSchreiben)
		{
			if (inControlSchreiben == TransportRichtungen.InDasControl)
			{
				formObjekt.Text = wert;
				return wert;
			}
			return formObjekt.Text;
		}

		public static int ReadWriteWebTextBox(int wert, ref TextBox formObjekt, TransportRichtungen inControlSchreiben)
		{
			if (inControlSchreiben == TransportRichtungen.InDasControl)
			{
				formObjekt.Text = wert.ToString();
				return wert;
			}
			return int.Parse(formObjekt.Text);
		}

		public static bool ReadWriteWebCheckBox(bool wert, ref CheckBox formObjekt, TransportRichtungen inControlSchreiben)
		{
			if (inControlSchreiben == TransportRichtungen.InDasControl)
			{
				formObjekt.Checked = wert;
				return wert;
			}
			return formObjekt.Checked;
		}
	}
}
