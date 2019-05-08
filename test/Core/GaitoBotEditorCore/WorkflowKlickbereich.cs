using System.Drawing;

namespace GaitoBotEditorCore
{
	public class WorkflowKlickbereich
	{
		private AIMLCategory _category;

		private Rectangle _bereich;

		public AIMLCategory Category
		{
			get
			{
				return this._category;
			}
		}

		public Rectangle Bereich
		{
			get
			{
				return this._bereich;
			}
		}

		public WorkflowKlickbereich(AIMLCategory category, Rectangle bereich)
		{
			this._category = category;
			this._bereich = bereich;
		}
	}
}
