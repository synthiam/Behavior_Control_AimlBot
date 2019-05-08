using System;

namespace de.springwald.toolbox
{
	public class DebuggerNeueZeileEventArgs : EventArgs
	{
		public string _inhalt;

		public DebuggerNeueZeileEventArgs(string inhalt)
		{
			this._inhalt = inhalt;
		}
	}
}
