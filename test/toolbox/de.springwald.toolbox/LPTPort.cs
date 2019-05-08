using System.Runtime.InteropServices;

namespace de.springwald.toolbox
{
	public class LPTPort
	{
		[DllImport("inpout32.dll", EntryPoint = "Out32")]
		public static extern void Output(int adress, int val);

		[DllImport("inpout32.dll", EntryPoint = "Inp32")]
		public static extern int Input(int adress);
	}
}
