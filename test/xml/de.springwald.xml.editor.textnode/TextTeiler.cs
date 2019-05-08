using System;
using System.Collections.Generic;
using System.Text;

namespace de.springwald.xml.editor.textnode
{
	public class TextTeiler
	{
		private List<TextTeil> _textTeile;

		private char[] _zeichenZumUmbrechen
		{
			get;
			set;
		}

		public List<TextTeil> TextTeile
		{
			get
			{
				return this._textTeile;
			}
		}

		public TextTeiler(string text, int invertiertStart, int invertiertLaenge, int maxLaengeProZeile, int bereitsLaengeDerZeile, char[] zeichenZumUmbrechen)
		{
			this._zeichenZumUmbrechen = zeichenZumUmbrechen;
			List<TextTeil> rohTeile = this.TextUmbrechenUndAufTeileVerteilen(text, bereitsLaengeDerZeile, maxLaengeProZeile);
			this._textTeile = this.TextTeileGgfInvertieren(rohTeile, invertiertStart, invertiertLaenge);
		}

		private List<TextTeil> TextTeileGgfInvertieren(List<TextTeil> rohTeile, int invertiertStart, int invertiertLaenge)
		{
			if (invertiertLaenge == 0)
			{
				return rohTeile;
			}
			List<TextTeil> list = new List<TextTeil>();
			int num = 0;
			foreach (TextTeil item in rohTeile)
			{
				bool istNeueZeile = item.IstNeueZeile;
				if (invertiertStart > num + item.Text.Length)
				{
					item.Invertiert = false;
					list.Add(item);
				}
				else if (invertiertStart + invertiertLaenge < num)
				{
					list.Add(item);
				}
				else
				{
					int val = invertiertLaenge + invertiertStart - num;
					val = Math.Min(val, item.Text.Length);
					val = Math.Max(val, 0);
					int num2 = 0;
					if (invertiertStart > num)
					{
						TextTeil textTeil = new TextTeil();
						textTeil.Invertiert = false;
						num2 = invertiertStart - num;
						textTeil.Text = item.Text.Substring(0, num2);
						textTeil.IstNeueZeile = istNeueZeile;
						istNeueZeile = false;
						list.Add(textTeil);
						val -= num2;
					}
					int num3 = val;
					TextTeil textTeil2 = new TextTeil();
					textTeil2.Invertiert = true;
					textTeil2.Text = item.Text.Substring(num2, num3);
					textTeil2.IstNeueZeile = istNeueZeile;
					istNeueZeile = false;
					list.Add(textTeil2);
					int num4 = item.Text.Length - (num2 + num3);
					if (num4 > 0)
					{
						TextTeil textTeil3 = new TextTeil();
						textTeil3.Invertiert = false;
						textTeil3.Text = item.Text.Substring(item.Text.Length - num4, num4);
						textTeil3.IstNeueZeile = istNeueZeile;
						list.Add(textTeil3);
					}
				}
				num += item.Text.Length;
			}
			return list;
		}

		private List<TextTeil> TextUmbrechenUndAufTeileVerteilen(string text, int laengeAktZeile, int maxLaengeProZeile)
		{
			if (this._zeichenZumUmbrechen != null)
			{
				char c = '·';
				char[] zeichenZumUmbrechen = this._zeichenZumUmbrechen;
				for (int i = 0; i < zeichenZumUmbrechen.Length; i++)
				{
					char c2 = zeichenZumUmbrechen[i];
					text = text.Replace(c2.ToString(), string.Format("{0}{1}", c2, c));
				}
				bool flag = true;
				List<TextTeil> list = new List<TextTeil>();
				string[] array = text.Split(new char[1]
				{
					c
				}, StringSplitOptions.RemoveEmptyEntries);
				string[] array2 = array;
				foreach (string text2 in array2)
				{
					List<TextTeil> list2 = this.TextAufTextTeileVerteilen(text2, laengeAktZeile, maxLaengeProZeile);
					TextTeil textTeil = null;
					foreach (TextTeil item in list2)
					{
						textTeil = item;
						list.Add(item);
					}
					if (textTeil != null)
					{
						if (flag)
						{
							flag = false;
						}
						else
						{
							textTeil.IstNeueZeile = true;
						}
					}
				}
				return list;
			}
			return this.TextAufTextTeileVerteilen(text, laengeAktZeile, maxLaengeProZeile);
		}

		private List<TextTeil> TextAufTextTeileVerteilen(string text, int laengeAktZeile, int maxLaengeProZeile)
		{
			List<TextTeil> list = new List<TextTeil>();
			StringBuilder stringBuilder = new StringBuilder();
			bool istNeueZeile = false;
			int num = 0;
			int num2 = text.IndexOf(' ', 0);
			while (num < text.Length)
			{
				if (num2 == -1)
				{
					string text2 = text.Substring(num, text.Length - num);
					stringBuilder.Append(text2);
					laengeAktZeile += text2.Length;
					num = text.Length;
				}
				else
				{
					num2++;
					string text3 = text.Substring(num, num2 - num);
					int num3 = maxLaengeProZeile - laengeAktZeile;
					if (num3 <= text3.Length)
					{
						if (num == 0)
						{
							stringBuilder.Append(text3);
							TextTeil textTeil = new TextTeil();
							textTeil.Text = stringBuilder.ToString();
							textTeil.IstNeueZeile = istNeueZeile;
							textTeil.Invertiert = false;
							list.Add(textTeil);
							stringBuilder = new StringBuilder();
							istNeueZeile = true;
							laengeAktZeile = 0;
						}
						else
						{
							TextTeil textTeil2 = new TextTeil();
							textTeil2.Text = stringBuilder.ToString();
							textTeil2.IstNeueZeile = istNeueZeile;
							textTeil2.Invertiert = false;
							list.Add(textTeil2);
							stringBuilder = new StringBuilder();
							stringBuilder.Append(text3);
							istNeueZeile = true;
							laengeAktZeile = text3.Length;
						}
					}
					else
					{
						stringBuilder.Append(text3);
						laengeAktZeile += text3.Length;
					}
					num = num2;
					num2 = text.IndexOf(' ', num);
				}
			}
			if (stringBuilder.Length != 0)
			{
				TextTeil textTeil3 = new TextTeil();
				textTeil3.Text = stringBuilder.ToString();
				textTeil3.IstNeueZeile = istNeueZeile;
				textTeil3.Invertiert = false;
				list.Add(textTeil3);
			}
			return list;
		}
	}
}
