using de.springwald.xml.cursor;

namespace de.springwald.xml.editor
{
	public class XMLUndoSchritt
	{
		protected string _snapshotName;

		protected XMLCursor _cursorVorher;

		public bool IstSnapshot
		{
			get
			{
				return this._snapshotName != null && this._snapshotName != "";
			}
		}

		public string SnapShotName
		{
			get
			{
				return this._snapshotName;
			}
			set
			{
				this._snapshotName = value;
			}
		}

		public XMLCursor CursorVorher
		{
			get
			{
				return this._cursorVorher;
			}
			set
			{
				this._cursorVorher = value.Clone();
			}
		}

		public XMLUndoSchritt()
		{
			this._snapshotName = null;
		}

		public virtual void UnDo()
		{
		}
	}
}
