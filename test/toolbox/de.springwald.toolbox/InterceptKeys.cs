using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace de.springwald.toolbox
{
	public class InterceptKeys
	{
		private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

		private const int WH_KEYBOARD_LL = 13;

		private const int WM_KEYDOWN = 256;

		private static LowLevelKeyboardProc _proc = InterceptKeys.HookCallback;

		private static IntPtr _hookID = IntPtr.Zero;

		public static event EventHandler<EventArgs<int>> KeyPress;

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool UnhookWindowsHookEx(IntPtr hhk);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr GetModuleHandle(string lpModuleName);

		public static void Init()
		{
			InterceptKeys._hookID = InterceptKeys.SetHook(InterceptKeys._proc);
		}

		public static void Dispose()
		{
			InterceptKeys.UnhookWindowsHookEx(InterceptKeys._hookID);
		}

		private static IntPtr SetHook(LowLevelKeyboardProc proc)
		{
			using (Process process = Process.GetCurrentProcess())
			{
				using (ProcessModule processModule = process.MainModule)
				{
					return InterceptKeys.SetWindowsHookEx(13, proc, InterceptKeys.GetModuleHandle(processModule.ModuleName), 0u);
				}
			}
		}

		private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
		{
			if (nCode >= 0 && wParam == (IntPtr)256)
			{
				int value = Marshal.ReadInt32(lParam);
				if (InterceptKeys.KeyPress != null)
				{
					InterceptKeys.KeyPress(null, new EventArgs<int>(value));
				}
			}
			return InterceptKeys.CallNextHookEx(InterceptKeys._hookID, nCode, wParam, lParam);
		}
	}
}
