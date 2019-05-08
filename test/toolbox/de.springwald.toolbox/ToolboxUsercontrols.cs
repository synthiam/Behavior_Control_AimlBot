using System;
using System.Drawing;
using System.Windows.Forms;

namespace de.springwald.toolbox
{
	public abstract class ToolboxUsercontrols
	{
		public static void GroesseAnInhaltAnpassen(UserControl steuerelement, int plusX, int plusY)
		{
			Point point = ToolboxUsercontrols.GroesseMaxPoint(steuerelement, plusX, plusY);
			steuerelement.Width = point.X;
			steuerelement.Height = point.Y;
		}

		public static Point GroesseMaxPoint(UserControl steuerelement, int plusX, int plusY)
		{
			Point result = new Point(0, 0);
			foreach (Control control in steuerelement.Controls)
			{
				if (control.Visible)
				{
					int x = result.X;
					int left = control.Left;
					Size clientSize = control.ClientSize;
					result.X = Math.Max(x, left + clientSize.Width);
					int y = result.Y;
					int top = control.Top;
					clientSize = control.ClientSize;
					result.Y = Math.Max(y, top + clientSize.Height);
				}
			}
			result.X += plusX;
			result.Y += plusY;
			return result;
		}

		public static float MeasureDisplayStringWidth(Graphics graphics, string text, Font font, StringFormat format)
		{
			if (text.Length < 1)
			{
				return 0f;
			}
			text = text.Replace(" ", ".");
			if (format == null)
			{
				format = (StringFormat)StringFormat.GenericTypographic.Clone();
				format.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
				format.Trimming = StringTrimming.None;
			}
			return graphics.MeasureString(text, font, 64000, format).Width;
		}
	}
}
