using de.springwald.toolbox;
using de.springwald.xml.cursor;
using de.springwald.xml.editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Xml;

namespace GaitoBotEditorCore
{
	public class ArbeitsbereichFokus
	{
		public delegate void AktDateiChangedEventHandler(object sender, EventArgs<IArbeitsbereichDatei> e);

		public class AktAIMLTopicChangedEventArgs : EventArgs
		{
			public AIMLTopic topic;

			public AktAIMLTopicChangedEventArgs(AIMLTopic topic)
			{
				this.topic = topic;
			}
		}

		public delegate void AktAIMLTopicChangedEventHandler(object sender, AktAIMLTopicChangedEventArgs e);

		private IArbeitsbereichDatei _aktDatei;

		private AIMLTopic _aktAIMLTopic;

		private AIMLCategory _aktAIMLCategory;

		private XMLEditor _xmlEditor;

		public IArbeitsbereichDatei AktDatei
		{
			get
			{
				return this._aktDatei;
			}
			set
			{
				bool flag = this._aktDatei != value;
				this._aktDatei = value;
				if (flag)
				{
					this.AktDateiChangedEvent(this._aktDatei);
					if (this._aktDatei is AIMLDatei)
					{
						this.BestesTopicSelektieren();
						this.BesteCategorySelektieren();
					}
					if (this._aktDatei is StartupDatei)
					{
						this.AktAIMLTopic = null;
						this.AktAIMLCategory = null;
						Application.DoEvents();
						this.FokusAufXmlEditorSetzen();
						Application.DoEvents();
						this.XMLCursorInDasErsteTagDerStartUpDateietzen();
						Application.DoEvents();
					}
				}
			}
		}

		public AIMLTopic AktAIMLTopic
		{
			get
			{
				return this._aktAIMLTopic;
			}
			set
			{
				if (value != null && this.AktDatei != value.AIMLDatei)
				{
					this._aktDatei = value.AIMLDatei;
					this.AktDateiChangedEvent(this._aktDatei);
				}
				bool flag = this._aktAIMLTopic != value;
				this._aktAIMLTopic = value;
				if (flag)
				{
					this.AktAIMLTopicChangedEvent(this._aktAIMLTopic);
					this.BesteCategorySelektieren();
				}
				if (this._aktDatei != null && this._aktDatei is AIMLDatei)
				{
					((AIMLDatei)this._aktDatei).ZuletztInDieserDateiGewaehlesTopic = this._aktAIMLTopic;
				}
			}
		}

		public AIMLCategory AktAIMLCategory
		{
			get
			{
				return this._aktAIMLCategory;
			}
			set
			{
				if (value != null)
				{
					if (this.AktDatei != value.AIMLTopic.AIMLDatei)
					{
						this._aktDatei = value.AIMLTopic.AIMLDatei;
						this.AktDateiChangedEvent(this._aktDatei);
					}
					if (this.AktAIMLTopic != value.AIMLTopic)
					{
						this._aktAIMLTopic = value.AIMLTopic;
						this.AktAIMLTopicChangedEvent(this._aktAIMLTopic);
					}
				}
				bool flag = this._aktAIMLCategory != value;
				this._aktAIMLCategory = value;
				if (flag)
				{
					this.AktAIMLCategoryChangedEvent(this._aktAIMLCategory);
					this.XMLCursorInDasPatternTagDerAktuellenCategorySetzen();
				}
				if (this._aktAIMLTopic != null)
				{
					this._aktAIMLTopic.ZuletztInDiesemTopicGewaehlteCategory = this._aktAIMLCategory;
				}
			}
		}

		public XMLEditor XmlEditor
		{
			get
			{
				return this._xmlEditor;
			}
			set
			{
				this._xmlEditor = value;
			}
		}

		public event AktDateiChangedEventHandler AktDateiChanged;

		public event AktAIMLTopicChangedEventHandler AktAIMLTopicChanged;

		public event EventHandler<EventArgs<AIMLCategory>> AktAIMLCategoryChanged;

		protected virtual void AktDateiChangedEvent(IArbeitsbereichDatei datei)
		{
			if (this.AktDateiChanged != null)
			{
				this.AktDateiChanged(this, new EventArgs<IArbeitsbereichDatei>(datei));
			}
		}

		protected virtual void AktAIMLTopicChangedEvent(AIMLTopic topic)
		{
			if (this.AktAIMLTopicChanged != null)
			{
				this.AktAIMLTopicChanged(this, new AktAIMLTopicChangedEventArgs(topic));
			}
		}

		protected virtual void AktAIMLCategoryChangedEvent(AIMLCategory category)
		{
			if (this.AktAIMLCategoryChanged != null)
			{
				this.AktAIMLCategoryChanged(this, new EventArgs<AIMLCategory>(category));
			}
		}

		public void FokusAufXmlEditorSetzen()
		{
			if (this._xmlEditor != null)
			{
				this._xmlEditor.ZeichnungsSteuerelement.Focus();
			}
		}

		public void XMLCursorInDasErsteTagDerStartUpDateietzen()
		{
			if (this._aktDatei != null && this._aktDatei is StartupDatei && ((StartupDatei)this._aktDatei).XML.DocumentElement == this._xmlEditor.RootNode)
			{
				XmlNode firstChild = ((StartupDatei)this._aktDatei).XML.DocumentElement.FirstChild;
				if (firstChild != null)
				{
					if (firstChild.ChildNodes.Count == 0)
					{
						this._xmlEditor.CursorRoh.BeideCursorPosSetzen(firstChild, XMLCursorPositionen.CursorInDemLeeremNode);
					}
					else
					{
						XmlNode firstChild2 = firstChild.FirstChild;
						if (firstChild2 != null)
						{
							this._xmlEditor.CursorRoh.BeideCursorPosSetzen(firstChild2, XMLCursorPositionen.CursorVorDemNode);
						}
					}
				}
			}
		}

		public void XMLCursorInDasPatternTagDerAktuellenCategorySetzen()
		{
			if (this._xmlEditor != null && this._aktAIMLCategory != null && this._aktAIMLCategory.ContentNode == this._xmlEditor.RootNode)
			{
				XmlNode contentNode = this._aktAIMLCategory.ContentNode;
				if (contentNode != null)
				{
					XmlNode firstChild = contentNode.FirstChild;
					if (firstChild != null && !(firstChild.Name != "pattern"))
					{
						if (firstChild.ChildNodes.Count == 0)
						{
							this._xmlEditor.CursorRoh.BeideCursorPosSetzen(firstChild, XMLCursorPositionen.CursorInDemLeeremNode);
						}
						else
						{
							XmlNode firstChild2 = firstChild.FirstChild;
							if (firstChild2 != null)
							{
								this._xmlEditor.CursorRoh.BeideCursorPosSetzen(firstChild2, XMLCursorPositionen.CursorVorDemNode);
							}
						}
					}
				}
			}
		}

		public void BestesTopicSelektieren()
		{
			if (this._aktDatei == null)
			{
				this.AktAIMLTopic = null;
			}
			else if (!(this._aktDatei is AIMLDatei))
			{
				this.AktAIMLTopic = null;
			}
			else
			{
				AIMLDatei aIMLDatei = (AIMLDatei)this._aktDatei;
				IOrderedEnumerable<AIMLTopic> source = from t in aIMLDatei.getTopics()
				orderby t.Name
				select t;
				AIMLTopic zuletztInDieserDateiGewaehlesTopic = aIMLDatei.ZuletztInDieserDateiGewaehlesTopic;
				if (zuletztInDieserDateiGewaehlesTopic != null && source.Contains(zuletztInDieserDateiGewaehlesTopic))
				{
					this.AktAIMLTopic = zuletztInDieserDateiGewaehlesTopic;
				}
				else if (source.Count() == 0)
				{
					this.AktAIMLTopic = null;
				}
				else
				{
					this.AktAIMLTopic = source.First();
				}
			}
		}

		public void BesteCategorySelektieren()
		{
			if (this._aktAIMLTopic == null)
			{
				this.AktAIMLCategory = null;
			}
			else
			{
				List<AIMLCategory> categories = this._aktAIMLTopic.Categories;
				AIMLCategory zuletztInDiesemTopicGewaehlteCategory = this._aktAIMLTopic.ZuletztInDiesemTopicGewaehlteCategory;
				AIMLCategory aIMLCategory = null;
				if (zuletztInDiesemTopicGewaehlteCategory != null && categories.Contains(zuletztInDiesemTopicGewaehlteCategory))
				{
					aIMLCategory = zuletztInDiesemTopicGewaehlteCategory;
				}
				if (aIMLCategory == null)
				{
					aIMLCategory = ((categories.Count != 0) ? categories[0] : null);
				}
				this.AktAIMLCategory = aIMLCategory;
			}
		}
	}
}
