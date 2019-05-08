using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;

namespace de.springwald.xml.dtd
{
	public class DTDElement
	{
		private string _name;

		private DTDChildElemente _children;

		private Regex _childrenRegExObjekt;

		private StringCollection _alleElementNamenWelcheAlsDirektesChildZulaessigSind;

		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				this._name = value;
			}
		}

		public DTDChildElemente ChildElemente
		{
			get
			{
				return this._children;
			}
			set
			{
				this._children = value;
			}
		}

		public StringCollection AlleElementNamenWelcheAlsDirektesChildZulaessigSind
		{
			get
			{
				if (this._alleElementNamenWelcheAlsDirektesChildZulaessigSind == null)
				{
					this._alleElementNamenWelcheAlsDirektesChildZulaessigSind = this.GetDTDElementeNamenAusChildElementen_(this._children);
					this._alleElementNamenWelcheAlsDirektesChildZulaessigSind.Add("#COMMENT");
				}
				return this._alleElementNamenWelcheAlsDirektesChildZulaessigSind;
			}
		}

		public List<DTDAttribut> Attribute
		{
			get;
			set;
		}

		public Regex ChildrenRegExObjekt
		{
			get
			{
				if (this._childrenRegExObjekt == null)
				{
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.Append(">");
					stringBuilder.Append(this._children.RegExAusdruck);
					stringBuilder.Append("<");
					this._childrenRegExObjekt = new Regex(stringBuilder.ToString());
				}
				return this._childrenRegExObjekt;
			}
		}

		private StringCollection GetDTDElementeNamenAusChildElementen_(DTDChildElemente children)
		{
			StringCollection stringCollection = new StringCollection();
			switch (children.Art)
			{
			case DTDChildElemente.DTDChildElementArten.EinzelChild:
				if (!stringCollection.Contains(children.ElementName))
				{
					stringCollection.Add(children.ElementName);
				}
				break;
			case DTDChildElemente.DTDChildElementArten.ChildListe:
				for (int i = 0; i < children.AnzahlChildren; i++)
				{
					StringEnumerator enumerator = this.GetDTDElementeNamenAusChildElementen_(children.Child(i)).GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							string current = enumerator.Current;
							if (!stringCollection.Contains(current))
							{
								stringCollection.Add(current);
							}
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
				break;
			default:
				throw new ApplicationException(string.Format(ResReader.Reader.GetString("UnbekannteDTDChildElementArt"), children.Art));
			case DTDChildElemente.DTDChildElementArten.Leer:
				break;
			}
			return stringCollection;
		}
	}
}
