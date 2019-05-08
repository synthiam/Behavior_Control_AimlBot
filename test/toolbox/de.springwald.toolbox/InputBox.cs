using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace de.springwald.toolbox
{
	public abstract class InputBox
	{
		public static string Show(string prompt, string header, string defaultResponse, out bool abgebrochen)
		{
			frmInput frmInput = new frmInput(prompt, header, defaultResponse);
			if (frmInput.ShowDialog() == DialogResult.OK)
			{
				abgebrochen = false;
				return frmInput.Response;
			}
			abgebrochen = true;
			return defaultResponse;
		}

		public static string Show(string prompt, string header, string defaultResponse)
		{
			frmInput frmInput = new frmInput(prompt, header, defaultResponse);
			DialogResult dialogResult = frmInput.ShowDialog();
			if (dialogResult == DialogResult.OK)
			{
				return frmInput.Response;
			}
			return defaultResponse;
		}

		public static string Show(string prompt, string header)
		{
			return InputBox.Show(prompt, header, string.Empty);
		}

		public static string Show(string prompt)
		{
			return InputBox.Show(prompt, Path.GetFileName(Assembly.GetCallingAssembly().Location), string.Empty);
		}
	}
}
