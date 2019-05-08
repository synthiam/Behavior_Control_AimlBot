using de.springwald.xml.cursor;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Xml;

namespace de.springwald.xml.dtd
{
	public class DTDNodeEditCheck
	{
		private DTD _dtd;

		public string DenkProtokoll
		{
			get
			{
				return "DenkProtokoll istper Define deaktiviert (DTDNodeEditCheck.cs)";
			}
		}

		public DTDNodeEditCheck(DTD dtd)
		{
			this._dtd = dtd;
		}

		public string[] AnDieserStelleErlaubteTags_(XMLCursorPos zuTestendeCursorPos, bool pcDATAMitAuflisten, bool kommentareMitAufListen)
		{
			XMLCursorPos cursorPos = zuTestendeCursorPos.Clone();
			List<DTDTestmuster> alleTestmuster = this.GetAlleTestmuster(cursorPos);
			List<string> list = new List<string>();
			foreach (DTDTestmuster item in alleTestmuster)
			{
				if (item.Erfolgreich)
				{
					if (item.ElementName == null)
					{
						list.Add("");
					}
					else
					{
						string a = item.ElementName.ToLower();
						if (!(a == "#pcdata"))
						{
							if (a == "#comment")
							{
								if (kommentareMitAufListen)
								{
									list.Add(item.ElementName);
								}
							}
							else
							{
								list.Add(item.ElementName);
							}
						}
						else if (pcDATAMitAuflisten)
						{
							list.Add(item.ElementName);
						}
					}
				}
			}
			return list.ToArray();
		}

		public bool IstDerNodeAnDieserStelleErlaubt(XmlNode node)
		{
			if (node.ParentNode is XmlDocument)
			{
				return true;
			}
			XMLCursorPos xMLCursorPos = new XMLCursorPos();
			xMLCursorPos.CursorSetzen(node, XMLCursorPositionen.CursorAufNodeSelbstVorderesTag);
			string elementNameFromNode = DTD.GetElementNameFromNode(node);
			DTDTestmuster dTDTestmuster = this.CreateTestMuster(elementNameFromNode, xMLCursorPos);
			List<DTDTestmuster> list = new List<DTDTestmuster>();
			list.Add(dTDTestmuster);
			this.PruefeAlleTestmuster(list, xMLCursorPos);
			if (dTDTestmuster.Erfolgreich)
			{
				return true;
			}
			return false;
		}

		private List<DTDTestmuster> GetAlleTestmuster(XMLCursorPos cursorPos)
		{
			List<DTDTestmuster> list = new List<DTDTestmuster>();
			if (cursorPos.AktNode == null)
			{
				throw new ApplicationException("GetAlleTestmuster: cursorPos.AktNode=NULL!");
			}
			switch (cursorPos.PosAmNode)
			{
			default:
				throw new ApplicationException(string.Format("unknown cursorPos.StartPos.PosAmNode '{0}' detected.", cursorPos.PosAmNode));
			case XMLCursorPositionen.CursorVorDemNode:
			case XMLCursorPositionen.CursorAufNodeSelbstVorderesTag:
			case XMLCursorPositionen.CursorAufNodeSelbstHinteresTag:
			case XMLCursorPositionen.CursorInDemLeeremNode:
			case XMLCursorPositionen.CursorInnerhalbDesTextNodes:
			case XMLCursorPositionen.CursorHinterDemNode:
				if (!(cursorPos.AktNode is XmlComment))
				{
					StringCollection stringCollection;
					if (cursorPos.PosAmNode == XMLCursorPositionen.CursorInDemLeeremNode)
					{
						DTDElement dTDElement = this._dtd.DTDElementByName(cursorPos.AktNode.Name, false);
						stringCollection = ((dTDElement != null) ? dTDElement.AlleElementNamenWelcheAlsDirektesChildZulaessigSind : new StringCollection());
					}
					else if (cursorPos.AktNode.OwnerDocument == null)
					{
						stringCollection = new StringCollection();
					}
					else if (cursorPos.AktNode == cursorPos.AktNode.OwnerDocument.DocumentElement)
					{
						stringCollection = new StringCollection();
					}
					else
					{
						DTDElement dTDElement2 = this._dtd.DTDElementByName(cursorPos.AktNode.ParentNode.Name, false);
						stringCollection = ((dTDElement2 != null) ? dTDElement2.AlleElementNamenWelcheAlsDirektesChildZulaessigSind : new StringCollection());
					}
					StringEnumerator enumerator = stringCollection.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							string current = enumerator.Current;
							DTDTestmuster item = this.CreateTestMuster(current, cursorPos);
							list.Add(item);
						}
					}
					finally
					{
						IDisposable disposable = enumerator as IDisposable;
						if (disposable != null)
						{
							disposable.Dispose();
						}
					}
				}
				this.PruefeAlleTestmuster(list, cursorPos);
				return list;
			}
		}

		private void PruefeAlleTestmuster(List<DTDTestmuster> alleMuster, XMLCursorPos cursorPos)
		{
			XmlNode aktNode = cursorPos.AktNode;
			DTDElement dTDElement;
			if (cursorPos.PosAmNode == XMLCursorPositionen.CursorInDemLeeremNode)
			{
				dTDElement = this._dtd.DTDElementByName(DTD.GetElementNameFromNode(aktNode), false);
				goto IL_00dd;
			}
			if (aktNode.OwnerDocument != null && aktNode.OwnerDocument.DocumentElement != null)
			{
				if (aktNode != aktNode.OwnerDocument.DocumentElement)
				{
					dTDElement = this._dtd.DTDElementByName(DTD.GetElementNameFromNode(aktNode.ParentNode), false);
					goto IL_00dd;
				}
				foreach (DTDTestmuster item in alleMuster)
				{
					if (item.ElementName == aktNode.Name)
					{
						item.Erfolgreich = true;
					}
				}
			}
			return;
			IL_00dd:
			foreach (DTDTestmuster item2 in alleMuster)
			{
				if (dTDElement == null)
				{
					item2.Erfolgreich = false;
				}
				else if (!item2.Erfolgreich)
				{
					item2.Erfolgreich = this.PasstMusterInElement(item2, dTDElement);
				}
			}
		}

		private bool PasstMusterInElement(DTDTestmuster muster, DTDElement element)
		{
			Match match = element.ChildrenRegExObjekt.Match(muster.VergleichStringFuerRegEx);
			return match.Success;
		}

		private DTDTestmuster CreateTestMuster(string elementName, XMLCursorPos cursorPos)
		{
			XmlNode aktNode = cursorPos.AktNode;
			XMLCursorPositionen posAmNode = cursorPos.PosAmNode;
			DTDTestmuster dTDTestmuster;
			if (posAmNode == XMLCursorPositionen.CursorInDemLeeremNode)
			{
				dTDTestmuster = new DTDTestmuster(elementName, DTD.GetElementNameFromNode(aktNode));
				dTDTestmuster.AddElement(elementName);
			}
			else
			{
				if (aktNode.ParentNode is XmlDocument)
				{
					throw new ApplicationException(ResReader.Reader.GetString("FuerRootElementKeinTestmuster"));
				}
				dTDTestmuster = new DTDTestmuster(elementName, DTD.GetElementNameFromNode(aktNode.ParentNode));
				for (XmlNode xmlNode = aktNode.ParentNode.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
				{
					if (!(xmlNode is XmlWhitespace))
					{
						if (xmlNode == aktNode)
						{
							if (xmlNode is XmlComment)
							{
								dTDTestmuster.AddElement("#COMMENT");
							}
							else if (this._dtd.DTDElementByName(DTD.GetElementNameFromNode(aktNode), false) != null)
							{
								switch (cursorPos.PosAmNode)
								{
								case XMLCursorPositionen.CursorAufNodeSelbstVorderesTag:
								case XMLCursorPositionen.CursorAufNodeSelbstHinteresTag:
									if (elementName != null)
									{
										dTDTestmuster.AddElement(elementName);
									}
									break;
								case XMLCursorPositionen.CursorHinterDemNode:
									if (elementName == null)
									{
										throw new ApplicationException("CreateTestMuster: Löschen darf bei XMLCursorPositionen.CursorHinterDemNode nicht geprüft werden!");
									}
									dTDTestmuster.AddElement(DTD.GetElementNameFromNode(aktNode));
									dTDTestmuster.AddElement(elementName);
									break;
								case XMLCursorPositionen.CursorInDemLeeremNode:
									if (elementName == null)
									{
										throw new ApplicationException("CreateTestMuster: Löschen darf bei XMLCursorPositionen.CursorHinterDemNode nicht geprüft werden!");
									}
									throw new ApplicationException("CreateTestMuster: CursorInDemLeeremNode can\u00b4t be handled at this place!");
								case XMLCursorPositionen.CursorVorDemNode:
									if (elementName == null)
									{
										throw new ApplicationException("CreateTestMuster: Löschen darf bei XMLCursorPositionen.CursorVorDemNode nicht geprüft werden!");
									}
									dTDTestmuster.AddElement(elementName);
									dTDTestmuster.AddElement(DTD.GetElementNameFromNode(aktNode));
									break;
								case XMLCursorPositionen.CursorInnerhalbDesTextNodes:
									if (elementName == null)
									{
										throw new ApplicationException("CreateTestMuster: Löschen darf bei XMLCursorPositionen.CursorInnerhalbDesTextNodes nicht geprüft werden!");
									}
									if (DTD.GetElementNameFromNode(aktNode) != "#PCDATA")
									{
										throw new ApplicationException("CreateTestMuster: CursorInnerhalbDesTextNodes angegeben, aber node.name=" + DTD.GetElementNameFromNode(aktNode));
									}
									dTDTestmuster.AddElement("#PCDATA");
									dTDTestmuster.AddElement(elementName);
									dTDTestmuster.AddElement("#PCDATA");
									break;
								default:
									throw new ApplicationException("Unknown XMLCursorPositionen value:" + cursorPos.PosAmNode);
								}
							}
						}
						else
						{
							dTDTestmuster.AddElement(DTD.GetElementNameFromNode(xmlNode));
						}
					}
				}
			}
			return dTDTestmuster;
		}

		private string ElementName(DTDElement element)
		{
			if (element == null)
			{
				return "[null]";
			}
			return element.Name;
		}
	}
}
