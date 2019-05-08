using de.springwald.gaitobot2;
using de.springwald.toolbox;
using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.Xml;

namespace GaitoBotEditorCore
{
	public class AIMLCategory : IDisposable
	{
		private string _patternOptimiertBufferFuerVerwandte;

		private string[] _sraisOptimiertBufferFuerVerwandte;

		private Regex _patternRegExOptimiertBufferFuerVerwandte;

		private ArrayList _sraisRegExOptimiertBufferFuerVerwandte;

		private static Regex _regExNurStars;

		private AIMLTopic _topic;

		private XmlNode _node;

		private string _autoKurzName;

		private string _autoTemplateZusammenfassung;

		private string _autoThatZusammenfassung;

		private string _thatBuffer;

		private string _templateBuffer;

		private string _patternBuffer;

		private string[] _sraisBuffer;

		private bool _disposed;

		private static Regex RegExNurStars
		{
			get
			{
				if (AIMLCategory._regExNurStars == null)
				{
					AIMLCategory._regExNurStars = new Regex("\\A([*]| )+\\z");
				}
				return AIMLCategory._regExNurStars;
			}
		}

		private string PatternOptimiertFuerVerwandte
		{
			get
			{
				if (this._patternOptimiertBufferFuerVerwandte == null)
				{
					this._patternOptimiertBufferFuerVerwandte = Normalisierung.EingabePatternOptimieren(this.Pattern, true);
				}
				return this._patternOptimiertBufferFuerVerwandte;
			}
		}

		private string[] SraisOptimiertFuerVerwandte
		{
			get
			{
				if (this._sraisOptimiertBufferFuerVerwandte == null)
				{
					if (this.Srais.Length == 0)
					{
						this._sraisOptimiertBufferFuerVerwandte = new string[0];
					}
					else
					{
						ArrayList arrayList = new ArrayList();
						string[] srais = this.Srais;
						foreach (string text in srais)
						{
							if (!AIMLCategory.RegExNurStars.IsMatch(text))
							{
								string text2 = Normalisierung.EingabePatternOptimieren(text, true);
								if (!arrayList.Contains(text2))
								{
									arrayList.Add(text2);
								}
							}
						}
						this._sraisOptimiertBufferFuerVerwandte = (string[])arrayList.ToArray(typeof(string));
					}
				}
				return this._sraisOptimiertBufferFuerVerwandte;
			}
		}

		private Regex PatternRegExOptimiertFuerVerwandte
		{
			get
			{
				if (this._patternRegExOptimiertBufferFuerVerwandte == null)
				{
					string patternOptimiertFuerVerwandte = this.PatternOptimiertFuerVerwandte;
					patternOptimiertFuerVerwandte = patternOptimiertFuerVerwandte.Replace("*", ".*?");
					patternOptimiertFuerVerwandte = patternOptimiertFuerVerwandte.Replace("+", "\\+");
					patternOptimiertFuerVerwandte = string.Format("<srai>{0}</srai>", patternOptimiertFuerVerwandte);
					this._patternRegExOptimiertBufferFuerVerwandte = new Regex(patternOptimiertFuerVerwandte, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
				}
				return this._patternRegExOptimiertBufferFuerVerwandte;
			}
		}

		private ArrayList SraisRegExOptimiertFuerVerwandte
		{
			get
			{
				if (this._sraisRegExOptimiertBufferFuerVerwandte == null)
				{
					this._sraisRegExOptimiertBufferFuerVerwandte = new ArrayList();
					string[] sraisOptimiertFuerVerwandte = this.SraisOptimiertFuerVerwandte;
					foreach (string text in sraisOptimiertFuerVerwandte)
					{
						string text2 = text;
						text2 = text2.Replace("*", ".*?");
						text2 = text2.Replace("+", "\\+");
						text2 = string.Format("\\A{0}\\z", text2);
						Regex value = new Regex(text2, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
						this._sraisRegExOptimiertBufferFuerVerwandte.Add(value);
					}
				}
				return this._sraisRegExOptimiertBufferFuerVerwandte;
			}
		}

		public XmlNode ContentNode
		{
			get
			{
				return this._node;
			}
		}

		public AIMLTopic AIMLTopic
		{
			get
			{
				return this._topic;
			}
		}

		public string AutoKomplettZusammenfassung
		{
			get
			{
				if (this.AutoThatZusammenfassung == "")
				{
					return string.Format("{0} >> {1}", this.AutoKurzNamePattern, this.AutoTemplateZusammenfassung);
				}
				return string.Format("{0} ({1}) >> {2}", this.AutoKurzNamePattern, this.AutoThatZusammenfassung, this.AutoTemplateZusammenfassung);
			}
		}

		public string AutoThatZusammenfassung
		{
			get
			{
				try
				{
					if (this._autoThatZusammenfassung == null)
					{
						XmlNode xmlNode = this._node.SelectSingleNode("that");
						if (xmlNode == null)
						{
							this._autoThatZusammenfassung = "";
						}
						else
						{
							this._autoThatZusammenfassung = xmlNode.InnerXml;
							this._autoThatZusammenfassung = this._autoThatZusammenfassung.Trim('\n', '\t', '\r', '\v');
						}
					}
				}
				catch (Exception)
				{
					return string.Format(ResReader.Reader.GetString("NodeNichtVorhandenOderFehlerhaft"), "that");
				}
				return this._autoThatZusammenfassung;
			}
		}

		public string AutoKurzNamePattern
		{
			get
			{
				try
				{
					if (this._autoKurzName == null)
					{
						this._autoKurzName = this._node.SelectSingleNode("pattern").InnerXml;
					}
				}
				catch (Exception)
				{
					return string.Format(ResReader.Reader.GetString("NodeNichtVorhandenOderFehlerhaft"), "pattern");
				}
				return this._autoKurzName;
			}
		}

		public string AutoTemplateZusammenfassung
		{
			get
			{
				try
				{
					if (this._autoTemplateZusammenfassung == null)
					{
						this._autoTemplateZusammenfassung = this._node.SelectSingleNode("template").InnerXml;
						this._autoTemplateZusammenfassung = this._autoTemplateZusammenfassung.Trim('\n', '\t', '\r', '\v');
					}
				}
				catch (Exception)
				{
					return string.Format(ResReader.Reader.GetString("NodeNichtVorhandenOderFehlerhaft"), "template");
				}
				return this._autoTemplateZusammenfassung;
			}
		}

		public string That
		{
			get
			{
				if (this._thatBuffer == null)
				{
					try
					{
						XmlNode xmlNode = this._node.SelectSingleNode("that");
						if (xmlNode == null)
						{
							this._thatBuffer = "";
						}
						else
						{
							this._thatBuffer = ToolboxStrings.UmlauteAussschreiben(xmlNode.InnerXml);
						}
					}
					catch (Exception)
					{
						this._thatBuffer = string.Format(ResReader.Reader.GetString("NodeNichtVorhandenOderFehlerhaft"), "that");
					}
				}
				return this._thatBuffer;
			}
		}

		public string Pattern
		{
			get
			{
				if (this._patternBuffer == null)
				{
					try
					{
						XmlNode xmlNode = this._node.SelectSingleNode("pattern");
						if (xmlNode == null)
						{
							this._patternBuffer = "";
						}
						else
						{
							this._patternBuffer = ToolboxStrings.UmlauteAussschreiben(xmlNode.InnerXml);
						}
					}
					catch (Exception)
					{
						this._patternBuffer = string.Format(ResReader.Reader.GetString("NodeNichtVorhandenOderFehlerhaft"), "pattern");
					}
				}
				return this._patternBuffer;
			}
		}

		public string Template
		{
			get
			{
				if (this._templateBuffer == null)
				{
					try
					{
						XmlNode xmlNode = this._node.SelectSingleNode("template");
						if (xmlNode == null)
						{
							this._templateBuffer = "";
						}
						else
						{
							this._templateBuffer = ToolboxStrings.UmlauteAussschreiben(xmlNode.InnerXml);
						}
					}
					catch (Exception)
					{
						return string.Format(ResReader.Reader.GetString("NodeNichtVorhandenOderFehlerhaft"), "template");
					}
				}
				return this._templateBuffer;
			}
		}

		public string[] Srais
		{
			get
			{
				if (this._sraisBuffer == null)
				{
					MatchCollection matchCollection = Regex.Matches(this.Template, "<srai>.+?</srai>");
					if (matchCollection.Count == 0)
					{
						this._sraisBuffer = new string[0];
					}
					else
					{
						ArrayList arrayList = new ArrayList();
						foreach (Match item in matchCollection)
						{
							string value = item.Value;
							value = Regex.Replace(item.Value, "<star .*?/>", "*");
							value = value.Replace("<srai>", "");
							value = value.Replace("</srai>", "");
							if (!arrayList.Contains(value))
							{
								arrayList.Add(value);
							}
						}
						this._sraisBuffer = (string[])arrayList.ToArray(typeof(string));
					}
				}
				return this._sraisBuffer;
			}
		}

		public bool IstThatNachfolger(AIMLCategory potentiellerVorgaenger)
		{
			if (potentiellerVorgaenger == this)
			{
				return false;
			}
			if (potentiellerVorgaenger.AIMLTopic.Name == this.AIMLTopic.Name)
			{
				if (potentiellerVorgaenger.Template.Contains("<srai>"))
				{
					return false;
				}
				if (this.That == "" || this.That == "*")
				{
					return false;
				}
				if (potentiellerVorgaenger.Template == this.That)
				{
					return true;
				}
			}
			return false;
		}

		public bool IstSraiNachfolger(AIMLCategory potentiellerVorgaenger)
		{
			if (potentiellerVorgaenger == this)
			{
				return false;
			}
			if (potentiellerVorgaenger.AIMLTopic.Name == this.AIMLTopic.Name)
			{
				if (potentiellerVorgaenger.Srais.Length == 0)
				{
					return false;
				}
				foreach (Regex item in potentiellerVorgaenger.SraisRegExOptimiertFuerVerwandte)
				{
					if (item.IsMatch(this.Pattern))
					{
						return true;
					}
				}
				string[] sraisOptimiertFuerVerwandte = potentiellerVorgaenger.SraisOptimiertFuerVerwandte;
				foreach (string input in sraisOptimiertFuerVerwandte)
				{
					if (!AIMLCategory.RegExNurStars.IsMatch(this.Pattern) && this.PatternRegExOptimiertFuerVerwandte.IsMatch(input))
					{
						return true;
					}
				}
			}
			return false;
		}

		public bool IstDuplikat(AIMLCategory potentiellesDuplikat)
		{
			if (potentiellesDuplikat == this)
			{
				return false;
			}
			if (potentiellesDuplikat.AIMLTopic.Name == this.AIMLTopic.Name && potentiellesDuplikat.Pattern == this.Pattern && potentiellesDuplikat.That == this.That)
			{
				return true;
			}
			return false;
		}

		public AIMLCategory(XmlNode categoryNode, AIMLTopic topic)
		{
			this._node = categoryNode;
			this._topic = topic;
			this._node.OwnerDocument.NodeChanged += this.OwnerDocument_NodeChanged;
			this._node.OwnerDocument.NodeInserted += this.OwnerDocument_NodeChanged;
			this._node.OwnerDocument.NodeRemoved += this.OwnerDocument_NodeChanged;
		}

		public void BufferLeeren()
		{
			this._autoKurzName = null;
			this._autoTemplateZusammenfassung = null;
			this._autoThatZusammenfassung = null;
			this._thatBuffer = null;
			this._templateBuffer = null;
			this._patternBuffer = null;
			this._patternOptimiertBufferFuerVerwandte = null;
			this._sraisBuffer = null;
			this._sraisOptimiertBufferFuerVerwandte = null;
			this._patternRegExOptimiertBufferFuerVerwandte = null;
			this._sraisRegExOptimiertBufferFuerVerwandte = null;
		}

		public static AIMLCategory createNewCategory(AIMLTopic topic)
		{
			XmlTextReader xmlTextReader = new XmlTextReader("<category><pattern></pattern><template></template></category>", XmlNodeType.Element, null);
			xmlTextReader.MoveToContent();
			XmlNode xmlNode = topic.TopicNode.OwnerDocument.ReadNode(xmlTextReader);
			topic.TopicNode.AppendChild(xmlNode);
			return new AIMLCategory(xmlNode, topic);
		}

		public static AIMLCategory createNewCategory(AIMLTopic topic, XmlNode newCategoryNode)
		{
			topic.TopicNode.AppendChild(newCategoryNode);
			return new AIMLCategory(newCategoryNode, topic);
		}

		public void Delete()
		{
			this._node.ParentNode.RemoveChild(this._node);
			this.Dispose();
		}

		private void OwnerDocument_NodeChanged(object sender, XmlNodeChangedEventArgs e)
		{
			if (e.Node == this._node)
			{
				this.BufferLeeren();
			}
			else
			{
				switch (e.Action)
				{
				case XmlNodeChangedAction.Change:
					if (ToolboxXML.IstChild(e.Node, this._node))
					{
						this.BufferLeeren();
					}
					break;
				case XmlNodeChangedAction.Insert:
					if (e.NewParent == this._node)
					{
						this.BufferLeeren();
					}
					else if (ToolboxXML.IstChild(e.NewParent, this._node))
					{
						this.BufferLeeren();
					}
					break;
				case XmlNodeChangedAction.Remove:
					if (e.OldParent == this._node)
					{
						this.BufferLeeren();
					}
					else if (ToolboxXML.IstChild(e.OldParent, this._node))
					{
						this.BufferLeeren();
					}
					break;
				}
			}
		}

		public void Dispose()
		{
			if (!this._disposed)
			{
				this._disposed = true;
				this._node.OwnerDocument.NodeChanged -= this.OwnerDocument_NodeChanged;
				this._node.OwnerDocument.NodeInserted -= this.OwnerDocument_NodeChanged;
				this._node.OwnerDocument.NodeRemoved -= this.OwnerDocument_NodeChanged;
				this._node = null;
				this.BufferLeeren();
			}
		}
	}
}
