using de.springwald.toolbox;
using System;
using System.Collections.Generic;

namespace GaitoBotEditorCore
{
	public class ArbeitsbereichVerlauf
	{
		private static int _maxVerlauf = 500;

		private ArbeitsbereichFokus _fokus;

		private List<AIMLCategory> _vorher;

		private List<AIMLCategory> _nachher;

		private bool _eigeneNavigationLaeuft;

		public bool ZurueckVerfuegbar
		{
			get
			{
				return this._vorher.Count > 1;
			}
		}

		public bool VorwaertsVerfuegbar
		{
			get
			{
				return this._nachher.Count > 1;
			}
		}

		public event EventHandler Changed;

		protected virtual void ChangedEvent()
		{
			if (this.Changed != null)
			{
				this.Changed(this, EventArgs.Empty);
			}
		}

		public ArbeitsbereichVerlauf(ArbeitsbereichFokus fokus)
		{
			this._fokus = fokus;
			this._fokus.AktAIMLCategoryChanged += this.FokusAktAimlCategoryChanged;
			this._vorher = new List<AIMLCategory>();
			this._nachher = new List<AIMLCategory>();
		}

		public void AlleVerweiseDieserDateiEntfernen(IArbeitsbereichDatei datei)
		{
			if (datei is AIMLDatei)
			{
				this.AlleVerweiseDieserAIMLDateiEntfernen((AIMLDatei)datei);
			}
		}

		public void AlleVerweiseDieserAIMLDateiEntfernen(AIMLDatei datei)
		{
			foreach (AIMLTopic topic in datei.getTopics())
			{
				this.AlleVerweiseDiesesAIMLTopicEntfernen(topic);
			}
		}

		public void AlleVerweiseDiesesAIMLTopicEntfernen(AIMLTopic topic)
		{
			foreach (AIMLCategory category in topic.Categories)
			{
				this.AlleVerweiseDieserAIMLCategoryEntfernen(category);
			}
		}

		public void AlleVerweiseDieserAIMLCategoryEntfernen(AIMLCategory category)
		{
			bool flag = false;
			if (this._vorher.Contains(category))
			{
				this._vorher.Remove(category);
				flag = true;
			}
			if (this._nachher.Contains(category))
			{
				this._nachher.Remove(category);
				flag = true;
			}
			if (flag)
			{
				this.ChangedEvent();
			}
		}

		public void Vorwaerts()
		{
			if (this._nachher.Count >= 2)
			{
				this._eigeneNavigationLaeuft = true;
				AIMLCategory item = this._nachher[this._nachher.Count - 1];
				this._nachher.RemoveAt(this._nachher.Count - 1);
				this._vorher.Add(item);
				if (this._nachher.Count > 0)
				{
					this._fokus.AktAIMLCategory = this._nachher[this._nachher.Count - 1];
					this._vorher.Add(this._nachher[this._nachher.Count - 1]);
				}
				this._eigeneNavigationLaeuft = false;
				this.ChangedEvent();
			}
		}

		public void Zurueck()
		{
			if (this._vorher.Count >= 2)
			{
				this._eigeneNavigationLaeuft = true;
				AIMLCategory item = this._vorher[this._vorher.Count - 1];
				this._vorher.RemoveAt(this._vorher.Count - 1);
				this._nachher.Add(item);
				if (this._vorher.Count > 0)
				{
					this._fokus.AktAIMLCategory = this._vorher[this._vorher.Count - 1];
					this._nachher.Add(this._vorher[this._vorher.Count - 1]);
				}
				this._eigeneNavigationLaeuft = false;
				this.ChangedEvent();
			}
		}

		private void FokusAktAimlCategoryChanged(object sender, EventArgs<AIMLCategory> e)
		{
			if (!this._eigeneNavigationLaeuft && e.Value != null && (this._vorher.Count == 0 || this._vorher[this._vorher.Count - 1] != e.Value))
			{
				this._vorher.Add(e.Value);
				if (this._nachher.Count != 0)
				{
					if (this._nachher[this._nachher.Count - 1] == e.Value)
					{
						this._nachher.RemoveAt(this._nachher.Count - 1);
					}
					else
					{
						this._nachher.Clear();
					}
				}
				while (this._vorher.Count > ArbeitsbereichVerlauf._maxVerlauf)
				{
					this._vorher.RemoveAt(0);
				}
				this.ChangedEvent();
			}
		}
	}
}
