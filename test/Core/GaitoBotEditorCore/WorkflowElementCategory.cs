using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace GaitoBotEditorCore
{
	public class WorkflowElementCategory
	{
		private static Font _drawFontNodeName;

		private static StringFormat _drawFormat;

		private bool _dickerRahmen = false;

		private string _kontext;

		private AIMLCategory _category;

		private int _x = 10;

		private int _y = 10;

		private int _breite = 300;

		private int _hoehe = 50;

		protected Color _backgroundColor = Color.FromArgb(250, 250, 255);

		public Color BackgroundColor
		{
			set
			{
				this._backgroundColor = value;
			}
		}

		public int Breite
		{
			get
			{
				return this._breite;
			}
			set
			{
				this._breite = value;
			}
		}

		public int Hoehe
		{
			get
			{
				return this._hoehe;
			}
		}

		public bool DickerRahmen
		{
			set
			{
				this._dickerRahmen = value;
			}
		}

		public AIMLCategory Category
		{
			get
			{
				return this._category;
			}
		}

		public int X
		{
			get
			{
				return this._x;
			}
			set
			{
				this._x = value;
			}
		}

		public int Y
		{
			get
			{
				return this._y;
			}
			set
			{
				this._y = value;
			}
		}

		public WorkflowElementCategory(string kontext, AIMLCategory category, int breite, int hoehe)
		{
			this._category = category;
			this._breite = breite;
			this._hoehe = hoehe;
			this._kontext = kontext;
			if (WorkflowElementCategory._drawFontNodeName == null)
			{
				WorkflowElementCategory._drawFontNodeName = new Font("Arial", 10f, GraphicsUnit.Pixel);
				WorkflowElementCategory._drawFormat = (StringFormat)StringFormat.GenericTypographic.Clone();
				WorkflowElementCategory._drawFormat.FormatFlags = (WorkflowElementCategory._drawFormat.FormatFlags | StringFormatFlags.MeasureTrailingSpaces);
				WorkflowElementCategory._drawFormat.Trimming = StringTrimming.None;
			}
		}

		public void Paint(PaintEventArgs e)
		{
			if (this._category != null)
			{
				this.zeichneRahmenNachKoordinaten(this._x + 2, this._y + 2, this._x + this._breite - 4, this._y + this._hoehe - 4, 3, this._backgroundColor, Color.Gray, e);
				using (SolidBrush brush = new SolidBrush(Color.Black))
				{
					e.Graphics.DrawString(string.Format("({0}) pattern: {1}", this._kontext.ToUpper(), this._category.AutoKurzNamePattern), WorkflowElementCategory._drawFontNodeName, brush, new RectangleF((float)(this._x + 8), (float)(this._y + 8), (float)(this._breite - 10), 20f), WorkflowElementCategory._drawFormat);
					e.Graphics.DrawString(string.Format("that: {0}", this._category.AutoThatZusammenfassung), WorkflowElementCategory._drawFontNodeName, brush, new RectangleF((float)(this._x + 8), (float)(this._y + 18), (float)(this._breite - 10), 20f), WorkflowElementCategory._drawFormat);
					e.Graphics.DrawString(string.Format("template: {0}", this._category.AutoTemplateZusammenfassung), WorkflowElementCategory._drawFontNodeName, brush, new RectangleF((float)(this._x + 8), (float)(this._y + 28), (float)(this._breite - 10), 20f), WorkflowElementCategory._drawFormat);
					e.Graphics.DrawString(string.Format("file: {0}", this._category.AIMLTopic.AIMLDatei.Titel), WorkflowElementCategory._drawFontNodeName, brush, new RectangleF((float)(this._x + 8), (float)(this._y + 38), (float)(this._breite - 10), 20f), WorkflowElementCategory._drawFormat);
				}
			}
		}

		private void zeichneRahmenNachKoordinaten(int x1, int y1, int x2, int y2, int rundung, Color fuellFarbe, Color rahmenFarbe, PaintEventArgs e)
		{
			using (GraphicsPath graphicsPath = new GraphicsPath())
			{
				graphicsPath.AddLine(x1 + rundung, y1, x2 - rundung, y1);
				graphicsPath.AddLine(x2, y1 + rundung, x2, y2 - rundung);
				graphicsPath.AddLine(x2 - rundung, y2, x1 + rundung, y2);
				graphicsPath.AddLine(x1, y2 - rundung, x1, y1 + rundung);
				graphicsPath.CloseFigure();
				if (fuellFarbe != Color.Transparent)
				{
					using (SolidBrush brush = new SolidBrush(fuellFarbe))
					{
						e.Graphics.FillPath(brush, graphicsPath);
					}
				}
				if (this._dickerRahmen)
				{
					using (Pen pen = new Pen(rahmenFarbe, 2f))
					{
						if (rahmenFarbe != Color.Transparent)
						{
							e.Graphics.DrawPath(pen, graphicsPath);
						}
					}
				}
				else
				{
					using (Pen pen2 = new Pen(rahmenFarbe, 1f))
					{
						if (rahmenFarbe != Color.Transparent)
						{
							e.Graphics.DrawPath(pen2, graphicsPath);
						}
					}
				}
			}
		}
	}
}
