namespace de.springwald.xml.editor
{
	public class XMLEditorPaintPos
	{
		public int ZeilenStartX
		{
			get;
			set;
		}

		public int ZeilenEndeX
		{
			get;
			set;
		}

		public int PosX
		{
			get;
			set;
		}

		public int PosY
		{
			get;
			set;
		}

		public int HoeheAktZeile
		{
			get;
			set;
		}

		public int BisherMaxX
		{
			get;
			set;
		}

		public XMLEditorPaintPos()
		{
			this.ZeilenStartX = 0;
			this.ZeilenEndeX = 0;
			this.PosX = 0;
			this.PosY = 0;
			this.HoeheAktZeile = 0;
			this.BisherMaxX = this.PosX;
		}

		public XMLEditorPaintPos Clone()
		{
			return new XMLEditorPaintPos
			{
				ZeilenStartX = this.ZeilenStartX,
				ZeilenEndeX = this.ZeilenEndeX,
				PosX = this.PosX,
				PosY = this.PosY,
				HoeheAktZeile = this.HoeheAktZeile,
				BisherMaxX = this.BisherMaxX
			};
		}
	}
}
