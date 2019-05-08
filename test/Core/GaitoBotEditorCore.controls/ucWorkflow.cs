using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Xml;

namespace GaitoBotEditorCore.controls
{
	public class ucWorkflow : UserControl
	{
		private enum PfeilArten
		{
			VorgaengerTHAT,
			VorgaengerSRAI,
			Redundanz,
			NachfolgerTHAT,
			NachfolgerSRAI
		}

		private int _teilBreite = 200;

		private int _teilHoehe = 60;

		private int _strichLaenge = 60;

		private string _lastPaintContent;

		private int _abstand = 10;

		private Arbeitsbereich _arbeitsbereich;

		private ArrayList _klickbereiche;

		private WorkflowAnalyser _analyser;

		private WorkflowElementCategory _categoryPainter;

		private IContainer components = null;

		public Arbeitsbereich Arbeitsbereich
		{
			set
			{
				if (this._arbeitsbereich != null)
				{
					throw new ApplicationException("Diesem Workflow-Control wurde bereits ein Arbeitsbereich zugewiesen");
				}
				this._arbeitsbereich = value;
				base.Invalidate();
			}
		}

		public AIMLCategory Category
		{
			get
			{
				if (this._categoryPainter == null)
				{
					return null;
				}
				return this._categoryPainter.Category;
			}
			set
			{
				if (value == null)
				{
					this._categoryPainter = null;
					this._analyser = null;
				}
				else
				{
					if (this._categoryPainter != null && this._categoryPainter.Category != value)
					{
						this._categoryPainter = null;
					}
					if (this._categoryPainter == null)
					{
						this._categoryPainter = new WorkflowElementCategory("main", value, this._teilBreite, this._teilHoehe);
						this._categoryPainter.DickerRahmen = true;
						this._categoryPainter.BackgroundColor = Color.FromArgb(255, 240, 255);
						this._analyser = new WorkflowAnalyser(this._arbeitsbereich, value);
					}
				}
				base.Invalidate();
			}
		}

		public ucWorkflow()
		{
			this._klickbereiche = new ArrayList();
			this.InitializeComponent();
		}

		private void ucWorkflow_Load(object sender, EventArgs e)
		{
			base.MouseClick += this.ucWorkflow_MouseClick;
			this.DoubleBuffered = true;
			base.SetStyle(ControlStyles.UserPaint, true);
			base.SetStyle(ControlStyles.DoubleBuffer, true);
			base.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			base.SetStyle(ControlStyles.ResizeRedraw, true);
			base.UpdateStyles();
		}

		public void RefreshAnzeige()
		{
			if (this._categoryPainter != null)
			{
				AIMLCategory category = this._categoryPainter.Category;
				if (category != null)
				{
					XmlNode contentNode = this._categoryPainter.Category.ContentNode;
					if (contentNode != null)
					{
						string outerXml = contentNode.OuterXml;
						if (outerXml != this._lastPaintContent)
						{
							this._analyser.FlushBuffer();
							base.Invalidate();
						}
					}
				}
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			e.Graphics.Clear(Color.White);
			this._teilBreite = Math.Max(50, Math.Min(400, base.Parent.Width / 4));
			this._strichLaenge = Math.Min(50, this._teilBreite / 3);
			this.Anzeigen(e);
		}

		private void Anzeigen(PaintEventArgs e)
		{
			this._klickbereiche = new ArrayList();
			if (this._categoryPainter != null && this._categoryPainter.Category != null && this._categoryPainter.Category.ContentNode != null)
			{
				string text = this._lastPaintContent = this._categoryPainter.Category.ContentNode.OuterXml;
				int abstand = this._abstand;
				int num = this._abstand;
				int num2 = abstand;
				int num3 = num + this._teilBreite + this._strichLaenge;
				int num4 = num2;
				int val = num2;
				num2 = abstand;
				this.CategorieListeZeichnen(e, ref num, ref num2, this._analyser.ThatVorgaenger, PfeilArten.VorgaengerTHAT, new Point(num3, num4 + this._teilHoehe / 2));
				if (num2 != abstand)
				{
					num2 += this._abstand;
				}
				this.CategorieListeZeichnen(e, ref num, ref num2, this._analyser.SraiVorgaenger, PfeilArten.VorgaengerSRAI, new Point(num3, num4 + this._teilHoehe / 2));
				num2 += this._abstand;
				val = Math.Max(num2, val);
				num += this._teilBreite + this._abstand + this._strichLaenge;
				num = num3;
				num2 = num4;
				this.AktuelleKategorieZeichnen(e, ref num, ref num2);
				num2 += this._abstand;
				val = Math.Max(num2, val);
				this.CategorieListeZeichnen(e, ref num, ref num2, this._analyser.Duplikate, PfeilArten.Redundanz, new Point(num3 + this._teilBreite / 2, num4 + this._teilHoehe));
				num2 += this._abstand;
				val = Math.Max(num2, val);
				num += this._teilBreite + this._abstand + this._strichLaenge;
				num2 = abstand;
				this.CategorieListeZeichnen(e, ref num, ref num2, this._analyser.ThatNachfolger, PfeilArten.NachfolgerTHAT, new Point(num3 + this._teilBreite, num4 + this._teilHoehe / 2));
				if (num2 != abstand)
				{
					num2 += this._abstand;
				}
				this.CategorieListeZeichnen(e, ref num, ref num2, this._analyser.SraiNachfolger, PfeilArten.NachfolgerSRAI, new Point(num3 + this._teilBreite, num4 + this._teilHoehe / 2));
				num2 += this._abstand;
				val = Math.Max(num2, val);
				num = (base.Width = num + (this._teilBreite + this._abstand));
				base.Height = val;
			}
		}

		private void CategorieListeZeichnen(PaintEventArgs e, ref int x, ref int y, List<AIMLCategory> liste, PfeilArten pfeilArt, Point dockPunktAnAktKategorie)
		{
			Pen pen = null;
			Pen pen2 = null;
			Color backgroundColor = Color.White;
			string kontext = "";
			switch (pfeilArt)
			{
			case PfeilArten.VorgaengerTHAT:
			case PfeilArten.NachfolgerTHAT:
				kontext = "THAT";
				backgroundColor = Color.FromArgb(240, 240, 255);
				break;
			case PfeilArten.VorgaengerSRAI:
			case PfeilArten.NachfolgerSRAI:
				kontext = "SRAI";
				backgroundColor = Color.FromArgb(210, 210, 230);
				break;
			case PfeilArten.Redundanz:
				kontext = "IDENT";
				backgroundColor = Color.FromArgb(250, 250, 255);
				break;
			}
			switch (pfeilArt)
			{
			case PfeilArten.VorgaengerTHAT:
			case PfeilArten.VorgaengerSRAI:
			case PfeilArten.NachfolgerTHAT:
			case PfeilArten.NachfolgerSRAI:
				pen = new Pen(Color.Gray, 1f);
				pen2 = new Pen(Color.Gray, 1f);
				pen2.EndCap = LineCap.ArrowAnchor;
				break;
			case PfeilArten.Redundanz:
				pen = new Pen(Color.LightBlue, 1f);
				pen2 = new Pen(Color.LightBlue, 1f);
				pen2.EndCap = LineCap.ArrowAnchor;
				break;
			}
			foreach (AIMLCategory item in liste)
			{
				WorkflowElementCategory workflowElementCategory = new WorkflowElementCategory(kontext, item, this._teilBreite, this._teilHoehe);
				workflowElementCategory.X = x;
				workflowElementCategory.Y = y;
				workflowElementCategory.BackgroundColor = backgroundColor;
				workflowElementCategory.Paint(e);
				this._klickbereiche.Add(new WorkflowKlickbereich(workflowElementCategory.Category, new Rectangle(workflowElementCategory.X, workflowElementCategory.Y, workflowElementCategory.Breite, workflowElementCategory.Hoehe)));
				switch (pfeilArt)
				{
				case PfeilArten.NachfolgerTHAT:
				case PfeilArten.NachfolgerSRAI:
				{
					Point pt = new Point(x, y + this._teilHoehe / 2);
					Point pt2 = new Point(x - this._strichLaenge / 3, y + this._teilHoehe / 2);
					Point point = new Point(x - this._strichLaenge / 3, dockPunktAnAktKategorie.Y);
					e.Graphics.DrawLine(pen2, pt2, pt);
					e.Graphics.DrawLine(pen, pt2, point);
					e.Graphics.DrawLine(pen, point, dockPunktAnAktKategorie);
					break;
				}
				case PfeilArten.Redundanz:
				{
					Point pt = new Point(x, y + this._teilHoehe / 2);
					Point pt2 = new Point(x - this._strichLaenge / 4, y + this._teilHoehe / 2);
					Point point = new Point(x - this._strichLaenge / 4, dockPunktAnAktKategorie.Y + 10);
					Point point2 = new Point(dockPunktAnAktKategorie.X, dockPunktAnAktKategorie.Y + 10);
					e.Graphics.DrawLine(pen2, pt2, pt);
					e.Graphics.DrawLine(pen, pt2, point);
					e.Graphics.DrawLine(pen, point, point2);
					e.Graphics.DrawLine(pen2, point2, dockPunktAnAktKategorie);
					break;
				}
				case PfeilArten.VorgaengerTHAT:
				case PfeilArten.VorgaengerSRAI:
				{
					Point pt = new Point(x + this._teilBreite, y + this._teilHoehe / 2);
					Point pt2 = new Point(x + this._teilBreite + this._strichLaenge / 2, y + this._teilHoehe / 2);
					Point point = new Point(x + this._teilBreite + this._strichLaenge / 2, dockPunktAnAktKategorie.Y);
					e.Graphics.DrawLine(pen, pt2, pt);
					e.Graphics.DrawLine(pen, pt2, point);
					e.Graphics.DrawLine(pen2, point, dockPunktAnAktKategorie);
					break;
				}
				}
				y += this._teilHoehe + this._abstand;
			}
			if (pen != null)
			{
				pen.Dispose();
			}
			if (pen2 != null)
			{
				pen2.Dispose();
			}
		}

		private void ucWorkflow_MouseClick(object sender, MouseEventArgs e)
		{
			if (this._arbeitsbereich != null)
			{
				Point pt = new Point(e.X, e.Y);
				foreach (WorkflowKlickbereich item in this._klickbereiche)
				{
					if (item.Bereich.Contains(pt) && item.Category != null && item.Category != this._arbeitsbereich.Fokus.AktAIMLCategory)
					{
						this._arbeitsbereich.Fokus.AktAIMLCategory = item.Category;
					}
				}
			}
		}

		private void AktuelleKategorieZeichnen(PaintEventArgs e, ref int x, ref int y)
		{
			this._categoryPainter.X = x;
			this._categoryPainter.Y = y;
			this._categoryPainter.Breite = this._teilBreite;
			this._categoryPainter.Paint(e);
			y += this._teilHoehe + this._abstand;
			this._klickbereiche.Add(new WorkflowKlickbereich(this._categoryPainter.Category, new Rectangle(this._categoryPainter.X, this._categoryPainter.Y, this._categoryPainter.Breite, this._categoryPainter.Hoehe)));
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ucWorkflow));
			base.SuspendLayout();
			componentResourceManager.ApplyResources(this, "$this");
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = Color.FromArgb(255, 192, 192);
			base.Name = "ucWorkflow";
			base.Load += this.ucWorkflow_Load;
			base.ResumeLayout(false);
		}
	}
}
