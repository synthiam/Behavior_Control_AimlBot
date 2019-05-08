using System.Collections.Generic;

namespace GaitoBotEditorCore
{
	internal class WorkflowAnalyser
	{
		private Arbeitsbereich _arbeitsbereich;

		private AIMLCategory _kategorie;

		public List<AIMLCategory> ThatNachfolger
		{
			get
			{
				List<AIMLCategory> list = new List<AIMLCategory>();
				foreach (AIMLDatei item in this._arbeitsbereich.Dateiverwaltung.AlleAimlDateien)
				{
					foreach (AIMLTopic topic in item.getTopics())
					{
						foreach (AIMLCategory category in topic.Categories)
						{
							if (category != this._kategorie && category.IstThatNachfolger(this._kategorie))
							{
								list.Add(category);
							}
						}
					}
				}
				return list;
			}
		}

		public List<AIMLCategory> SraiNachfolger
		{
			get
			{
				List<AIMLCategory> list = new List<AIMLCategory>();
				foreach (AIMLDatei item in this._arbeitsbereich.Dateiverwaltung.AlleAimlDateien)
				{
					foreach (AIMLTopic topic in item.getTopics())
					{
						foreach (AIMLCategory category in topic.Categories)
						{
							if (category != this._kategorie && category.IstSraiNachfolger(this._kategorie))
							{
								list.Add(category);
							}
						}
					}
				}
				return list;
			}
		}

		public List<AIMLCategory> SraiVorgaenger
		{
			get
			{
				List<AIMLCategory> list = new List<AIMLCategory>();
				foreach (AIMLDatei item in this._arbeitsbereich.Dateiverwaltung.AlleAimlDateien)
				{
					foreach (AIMLTopic topic in item.getTopics())
					{
						foreach (AIMLCategory category in topic.Categories)
						{
							if (category != this._kategorie && this._kategorie.IstSraiNachfolger(category))
							{
								list.Add(category);
							}
						}
					}
				}
				return list;
			}
		}

		public List<AIMLCategory> ThatVorgaenger
		{
			get
			{
				List<AIMLCategory> list = new List<AIMLCategory>();
				foreach (AIMLDatei item in this._arbeitsbereich.Dateiverwaltung.AlleAimlDateien)
				{
					foreach (AIMLTopic topic in item.getTopics())
					{
						foreach (AIMLCategory category in topic.Categories)
						{
							if (category != this._kategorie && this._kategorie.IstThatNachfolger(category))
							{
								list.Add(category);
							}
						}
					}
				}
				return list;
			}
		}

		public List<AIMLCategory> Duplikate
		{
			get
			{
				List<AIMLCategory> list = new List<AIMLCategory>();
				foreach (AIMLDatei item in this._arbeitsbereich.Dateiverwaltung.AlleAimlDateien)
				{
					foreach (AIMLTopic topic in item.getTopics())
					{
						foreach (AIMLCategory category in topic.Categories)
						{
							if (category != this._kategorie && this._kategorie.IstDuplikat(category))
							{
								list.Add(category);
							}
						}
					}
				}
				return list;
			}
		}

		public WorkflowAnalyser(Arbeitsbereich arbeitsbereich, AIMLCategory kategorie)
		{
			this._arbeitsbereich = arbeitsbereich;
			this._kategorie = kategorie;
		}

		public void FlushBuffer()
		{
		}
	}
}
