using de.springwald.toolbox;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace GaitoBotEditorCore
{
	public class AIMLTopic : IDisposable
	{
		private AIMLDatei _aimlDatei;

		private XmlNode _topicNode;

		private bool _istRootTopic;

		private List<AIMLCategory> __categoryListe;

		private AIMLCategory _zuletztInDiesemTopicGewaehlteCategory;

		public List<AIMLCategory> Categories
		{
			get
			{
				if (this.__categoryListe == null)
				{
					this.__categoryListe = new List<AIMLCategory>();
					XmlNodeList xmlNodeList = this._topicNode.SelectNodes("category");
					foreach (XmlNode item in xmlNodeList)
					{
						this.__categoryListe.Add(new AIMLCategory(item, this));
					}
				}
				return this.__categoryListe;
			}
		}

		public AIMLDatei AIMLDatei
		{
			get
			{
				return this._aimlDatei;
			}
		}

		public XmlNode TopicNode
		{
			get
			{
				return this._topicNode;
			}
		}

		public string Name
		{
			get
			{
				if (this._istRootTopic)
				{
					return "_standard_";
				}
				return this._topicNode.Attributes.GetNamedItem("name").Value.ToString();
			}
			set
			{
				if (!this._istRootTopic)
				{
					this._topicNode.Attributes.GetNamedItem("name").Value = value;
				}
			}
		}

		public bool IstRootTopic
		{
			get
			{
				return this._istRootTopic;
			}
		}

		public AIMLCategory ZuletztInDiesemTopicGewaehlteCategory
		{
			get
			{
				return this._zuletztInDiesemTopicGewaehlteCategory;
			}
			set
			{
				this._zuletztInDiesemTopicGewaehlteCategory = value;
			}
		}

		public AIMLTopic(XmlNode xmlNode, AIMLDatei aimlDatei)
		{
			this._topicNode = xmlNode;
			this._aimlDatei = aimlDatei;
			this._istRootTopic = this._topicNode.Name.Equals("aiml");
			if (!this._istRootTopic && this._topicNode.Attributes.GetNamedItem("name") == null)
			{
				this._topicNode.Attributes.Append(this._topicNode.OwnerDocument.CreateAttribute("", "name", ""));
			}
		}

		public void LoescheCategory(AIMLCategory category)
		{
			if (category != null)
			{
				if (this.Categories == null)
				{
					Debugger.GlobalDebugger.FehlerZeigen("Categories == null", this, "GaitoBotEditorCore.AIMLTopic.LoescheCategory");
				}
				else
				{
					if (this.Categories.Contains(category))
					{
						this.Categories.Remove(category);
					}
					if (this._aimlDatei == null)
					{
						Debugger.GlobalDebugger.FehlerZeigen("_aimlDatei == null", this, "GaitoBotEditorCore.AIMLTopic.LoescheCategory");
					}
					else if (this._aimlDatei.Arbeitsbereich == null)
					{
						Debugger.GlobalDebugger.FehlerZeigen("_aimlDatei.Arbeitsbereich == null", this, "GaitoBotEditorCore.AIMLTopic.LoescheCategory");
					}
					else if (this._aimlDatei.Arbeitsbereich.Verlauf == null)
					{
						Debugger.GlobalDebugger.FehlerZeigen("_aimlDatei.Arbeitsbereich.Verlauf == null", this, "GaitoBotEditorCore.AIMLTopic.LoescheCategory");
					}
					else
					{
						this._aimlDatei.Arbeitsbereich.Verlauf.AlleVerweiseDieserAIMLCategoryEntfernen(category);
						if (category == null)
						{
							throw new ApplicationException("category wurde offenbar nachträglich noch == null");
						}
						try
						{
							category.Delete();
						}
						catch (Exception ex)
						{
							throw new ApplicationException("Fehler bei  category.Delete():" + ex.Message);
						}
						try
						{
							category.Dispose();
						}
						catch (Exception ex2)
						{
							throw new ApplicationException("Fehler bei  category.Dispose():" + ex2.Message);
						}
					}
				}
			}
		}

		public void CategoriesSortieren_()
		{
			ArrayList arrayList = new ArrayList();
			foreach (AIMLCategory category in this.Categories)
			{
				XmlNode value = this._topicNode.RemoveChild(category.ContentNode);
				arrayList.Add(value);
			}
			arrayList.Sort(new AIMLCategoryNodeSortierer());
			foreach (XmlNode item in arrayList)
			{
				this._topicNode.AppendChild(item);
			}
			this.__categoryListe = null;
		}

		public AIMLCategory AddNewCategory()
		{
			AIMLCategory aIMLCategory = AIMLCategory.createNewCategory(this);
			this.Categories.Add(aIMLCategory);
			return aIMLCategory;
		}

		public AIMLCategory AddNewCategory(XmlNode newCategoryNode)
		{
			AIMLCategory aIMLCategory = AIMLCategory.createNewCategory(this, newCategoryNode);
			this.Categories.Add(aIMLCategory);
			return aIMLCategory;
		}

		public void Delete()
		{
			if (this._istRootTopic)
			{
				throw new ApplicationException("Root-Topic can\u00b4t be deleted!");
			}
			this._topicNode.ParentNode.RemoveChild(this._topicNode);
			this.Dispose();
		}

		public static AIMLTopic createNewTopic(AIMLDatei aimlDatei)
		{
			XmlTextReader xmlTextReader = new XmlTextReader("<topic name=\"noname\"></topic>", XmlNodeType.Element, null);
			xmlTextReader.MoveToContent();
			XmlNode xmlNode = aimlDatei.XML.ReadNode(xmlTextReader);
			aimlDatei.XML.DocumentElement.AppendChild(xmlNode);
			return new AIMLTopic(xmlNode, aimlDatei);
		}

		public void Dispose()
		{
			this._topicNode = null;
			this.__categoryListe = null;
		}
	}
}
