using System;
using System.Collections;
using System.Text;

namespace de.springwald.xml.dtd
{
	public class DTDChildElemente
	{
		public enum DTDChildElementArten
		{
			Leer,
			EinzelChild = -1,
			ChildListe = 2
		}

		public enum DTDChildElementAnzahl
		{
			GenauEinmal,
			NullUndMehr = -1,
			NullOderEinmal = 2,
			EinsUndMehr
		}

		public enum DTDChildElementOperatoren
		{
			GefolgtVon,
			Oder = -1
		}

		private DTDChildElementArten _art;

		private DTDChildElementAnzahl _defAnzahl;

		private DTDChildElementOperatoren _operator;

		private ArrayList _children;

		private string _elementName;

		private DTD _dtd;

		private string _quellcode;

		private AlleMoeglichenElementeEinesChildblocks _alleMoeglichenElemente;

		private string _regExAusdruck;

		public string RegExAusdruck
		{
			get
			{
				if (this._regExAusdruck == null)
				{
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.Append("(");
					switch (this._art)
					{
					case DTDChildElementArten.EinzelChild:
						if (this._elementName != "#COMMENT")
						{
							stringBuilder.AppendFormat("((-#COMMENT)*-{0}(-#COMMENT)*)", this._elementName);
						}
						else
						{
							stringBuilder.AppendFormat("(-{0})", this._elementName);
						}
						break;
					case DTDChildElementArten.ChildListe:
						stringBuilder.Append("(");
						for (int i = 0; i < this._children.Count; i++)
						{
							if (i != 0)
							{
								switch (this._operator)
								{
								case DTDChildElementOperatoren.Oder:
									stringBuilder.Append("|");
									break;
								default:
									throw new ApplicationException("Unhandled DTDChildElementOperatoren '" + this._operator + "'");
								case DTDChildElementOperatoren.GefolgtVon:
									break;
								}
							}
							stringBuilder.Append(((DTDChildElemente)this._children[i]).RegExAusdruck);
						}
						stringBuilder.Append(")");
						break;
					default:
						throw new ApplicationException("Unhandled DTDChildElementArt '" + this._art + "'");
					case DTDChildElementArten.Leer:
						break;
					}
					switch (this._defAnzahl)
					{
					case DTDChildElementAnzahl.EinsUndMehr:
						stringBuilder.Append("+");
						break;
					case DTDChildElementAnzahl.NullOderEinmal:
						stringBuilder.Append("?");
						break;
					case DTDChildElementAnzahl.NullUndMehr:
						stringBuilder.Append("*");
						break;
					default:
						throw new ApplicationException("Unhandled DTDChildElementAnzahl '" + this._defAnzahl + "'");
					case DTDChildElementAnzahl.GenauEinmal:
						break;
					}
					stringBuilder.Append(")");
					this._regExAusdruck = stringBuilder.ToString();
				}
				return this._regExAusdruck;
			}
		}

		public AlleMoeglichenElementeEinesChildblocks AlleMoeglichenElemente
		{
			get
			{
				if (this._alleMoeglichenElemente == null)
				{
					this._alleMoeglichenElemente = new AlleMoeglichenElementeEinesChildblocks(this);
				}
				return this._alleMoeglichenElemente;
			}
		}

		public string Quellcode
		{
			get
			{
				return this._quellcode;
			}
		}

		public DTDChildElementArten Art
		{
			get
			{
				return this._art;
			}
		}

		public DTDChildElementAnzahl DefAnzahl
		{
			get
			{
				return this._defAnzahl;
			}
		}

		public DTDChildElementOperatoren Operator
		{
			get
			{
				return this._operator;
			}
		}

		public int AnzahlChildren
		{
			get
			{
				return this._children.Count;
			}
		}

		public string ElementName
		{
			get
			{
				return this._elementName;
			}
		}

		public DTDChildElemente(string childrenQuellcode)
		{
			this._art = DTDChildElementArten.Leer;
			this._children = new ArrayList();
			this._defAnzahl = DTDChildElementAnzahl.GenauEinmal;
			this._elementName = "";
			this._operator = DTDChildElementOperatoren.Oder;
			this._quellcode = childrenQuellcode;
			this._quellcode = this._quellcode.Replace("\t", " ");
			this._quellcode = this._quellcode.Replace("\r\n", " ");
			this._quellcode = this._quellcode.Trim();
			if (this._quellcode.Length == 0)
			{
				this._art = DTDChildElementArten.Leer;
			}
			else
			{
				this.CodeAuslesen();
			}
		}

		protected DTDChildElemente()
		{
		}

		public DTDChildElemente Clone()
		{
			DTDChildElemente dTDChildElemente = new DTDChildElemente();
			dTDChildElemente._alleMoeglichenElemente = null;
			dTDChildElemente._art = this._art;
			dTDChildElemente._operator = this._operator;
			dTDChildElemente._defAnzahl = this._defAnzahl;
			dTDChildElemente._children = (ArrayList)this._children.Clone();
			dTDChildElemente._dtd = this._dtd;
			dTDChildElemente._elementName = this._elementName;
			dTDChildElemente._quellcode = this._quellcode + "(geklont)";
			return dTDChildElemente;
		}

		public void DTDZuweisen(DTD dtd)
		{
			foreach (DTDChildElemente child in this._children)
			{
				child.DTDZuweisen(dtd);
			}
			this._dtd = dtd;
		}

		public bool AnzahlZulaessig(int anzahl)
		{
			switch (this._defAnzahl)
			{
			case DTDChildElementAnzahl.EinsUndMehr:
				if (anzahl >= 1)
				{
					return true;
				}
				return false;
			case DTDChildElementAnzahl.GenauEinmal:
				if (anzahl == 1)
				{
					return true;
				}
				return false;
			case DTDChildElementAnzahl.NullOderEinmal:
				if (anzahl == 0 || anzahl == 1)
				{
					return true;
				}
				return false;
			case DTDChildElementAnzahl.NullUndMehr:
				if (anzahl >= 0)
				{
					return true;
				}
				return false;
			default:
				throw new ApplicationException(string.Format(ResReader.Reader.GetString("UnbekannteDTDChildElementAnzahl"), this._art));
			}
		}

		public DTDChildElemente Child(int index)
		{
			return (DTDChildElemente)this._children[index];
		}

		private void CodeAuslesen()
		{
			string text = this._quellcode;
			string text2 = text.Substring(text.Length - 1, 1);
			switch (text2)
			{
			case "+":
				this._defAnzahl = DTDChildElementAnzahl.EinsUndMehr;
				text = text.Remove(text.Length - 1, 1);
				break;
			case "*":
				this._defAnzahl = DTDChildElementAnzahl.NullUndMehr;
				text = text.Remove(text.Length - 1, 1);
				break;
			case "?":
				this._defAnzahl = DTDChildElementAnzahl.NullOderEinmal;
				text = text.Remove(text.Length - 1, 1);
				break;
			default:
				this._defAnzahl = DTDChildElementAnzahl.GenauEinmal;
				break;
			}
			text = text.Trim();
			if (text.Substring(0, 1) == "(" && text.Substring(text.Length - 1, 1) == ")")
			{
				text = text.Substring(1, text.Length - 2);
				this.ChildrenAuslesen(text);
			}
			else
			{
				this._art = DTDChildElementArten.EinzelChild;
				this._elementName = text;
			}
		}

		private void ChildrenAuslesen(string code)
		{
			string str = code;
			this._art = DTDChildElementArten.ChildListe;
			int num = 0;
			StringBuilder stringBuilder = new StringBuilder();
			while (code.Length > 0)
			{
				string text = code.Substring(0, 1);
				code = code.Remove(0, 1);
				string a = text;
				if (!(a == "("))
				{
					if (a == ")")
					{
						num--;
					}
				}
				else
				{
					num++;
				}
				if (this.IstOperator(text))
				{
					if (num == 0)
					{
						this._operator = this.GetOperatorFromChar(text);
						string text2 = stringBuilder.ToString().Trim();
						if (text2.Length == 0)
						{
							throw new ApplicationException("Leerer ChildCode gefunden in '" + str + "'");
						}
						this.SpeichereChildElement(text2);
						stringBuilder = new StringBuilder();
					}
					else
					{
						stringBuilder.Append(text);
					}
				}
				else
				{
					stringBuilder.Append(text);
				}
			}
			if (stringBuilder.Length > 0)
			{
				this.SpeichereChildElement(stringBuilder.ToString());
			}
		}

		private void SpeichereChildElement(string code)
		{
			code = code.Trim();
			DTDChildElemente value = new DTDChildElemente(code);
			this._children.Add(value);
		}

		private bool IstOperator(string code)
		{
			if (!(code == "|") && !(code == ","))
			{
				return false;
			}
			return true;
		}

		private DTDChildElementOperatoren GetOperatorFromChar(string code)
		{
			if (!(code == "|"))
			{
				if (code == ",")
				{
					return DTDChildElementOperatoren.GefolgtVon;
				}
				throw new ApplicationException(string.Format(ResReader.Reader.GetString("StringIstKeinOperator"), code));
			}
			return DTDChildElementOperatoren.Oder;
		}
	}
}
