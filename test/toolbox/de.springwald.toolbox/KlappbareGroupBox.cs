using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace de.springwald.toolbox
{
	[ToolboxBitmap(typeof(KlappbareGroupBox))]
	public class KlappbareGroupBox : GroupBox
	{
		public delegate void CollapseBoxClickedEventHandler(object sender);

		private Color _buttonBackgroundColor = Color.White;

		private Color _buttonDrawColor = Color.Black;

		private Rectangle m_toggleRect = new Rectangle(8, 2, 11, 11);

		private bool m_collapsed = false;

		private bool m_bResizingFromCollapse = false;

		private const int m_collapsedHeight = 20;

		private Size m_FullSize = Size.Empty;

		private IContainer components = null;

		public Color BorderColor
		{
			get
			{
				return this._buttonDrawColor;
			}
			set
			{
				this._buttonDrawColor = value;
				base.Invalidate();
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int FullHeight
		{
			get
			{
				return this.m_FullSize.Height;
			}
		}

		[DefaultValue(false)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsCollapsed
		{
			get
			{
				return this.m_collapsed;
			}
			set
			{
				if (value != this.m_collapsed)
				{
					this.m_collapsed = value;
					if (!value)
					{
						base.Size = this.m_FullSize;
					}
					else
					{
						this.m_bResizingFromCollapse = true;
						base.Height = 20;
						this.m_bResizingFromCollapse = false;
					}
					foreach (Control control in base.Controls)
					{
						control.Visible = !value;
					}
					base.Invalidate();
				}
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int CollapsedHeight
		{
			get
			{
				return 20;
			}
		}

		public event CollapseBoxClickedEventHandler CollapseBoxClickedEvent;

		public KlappbareGroupBox()
		{
			this.InitializeComponent();
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			if (this.m_toggleRect.Contains(e.Location))
			{
				this.ToggleCollapsed();
			}
			else
			{
				base.OnMouseUp(e);
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			this.HandleResize();
			this.DrawGroupBox(e.Graphics);
			this.DrawToggleButton(e.Graphics);
		}

		private void DrawGroupBox(Graphics g)
		{
			Rectangle clientRectangle = base.ClientRectangle;
			int x = clientRectangle.X;
			clientRectangle = base.ClientRectangle;
			int y = clientRectangle.Y + 6;
			clientRectangle = base.ClientRectangle;
			int width = clientRectangle.Width;
			clientRectangle = base.ClientRectangle;
			Rectangle bounds = new Rectangle(x, y, width, clientRectangle.Height - 6);
			GroupBoxRenderer.DrawGroupBox(g, bounds, (GroupBoxState)(base.Enabled ? 1 : 2));
			StringFormat stringFormat = new StringFormat();
			int num = bounds.X + 8 + this.m_toggleRect.Width + 2;
			int num2 = (int)g.MeasureString(this.Text, this.Font).Width;
			num2 = ((num2 < 1) ? 1 : num2);
			int num3 = num + num2 + 1;
			using (SolidBrush brush = new SolidBrush(this.BackColor))
			{
				g.FillRectangle(brush, 8, 2, num3 - 8, 12);
			}
			using (SolidBrush brush2 = new SolidBrush(this.ForeColor))
			{
				g.DrawString(this.Text, this.Font, brush2, (float)num, 0f);
			}
		}

		private void DrawToggleButton(Graphics g)
		{
			using (SolidBrush brush = new SolidBrush(this._buttonBackgroundColor))
			{
				g.FillRectangle(brush, this.m_toggleRect);
				using (Pen pen = new Pen(this._buttonDrawColor))
				{
					g.DrawRectangle(pen, this.m_toggleRect.Left + 1, this.m_toggleRect.Top + 1, 8, 8);
				}
				using (Pen pen2 = new Pen(this._buttonDrawColor))
				{
					g.DrawLine(pen2, this.m_toggleRect.Left + 3, this.m_toggleRect.Bottom - 6, this.m_toggleRect.Right - 4, this.m_toggleRect.Bottom - 6);
				}
				if (this.IsCollapsed)
				{
					using (Pen pen3 = new Pen(this._buttonDrawColor))
					{
						g.DrawLine(pen3, this.m_toggleRect.Left + 5, this.m_toggleRect.Top + 3, this.m_toggleRect.Left + 5, this.m_toggleRect.Bottom - 4);
					}
				}
			}
		}

		private void ToggleCollapsed()
		{
			this.IsCollapsed = !this.IsCollapsed;
			if (this.CollapseBoxClickedEvent != null)
			{
				this.CollapseBoxClickedEvent(this);
			}
		}

		private void HandleResize()
		{
			if (!this.m_bResizingFromCollapse && !this.m_collapsed)
			{
				this.m_FullSize = base.Size;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(KlappbareGroupBox));
			base.SuspendLayout();
			base.Name = "KlappbareGroupBox";
			base.ResumeLayout(false);
		}
	}
}
