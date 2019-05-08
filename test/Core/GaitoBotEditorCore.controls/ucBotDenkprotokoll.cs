using de.springwald.gaitobot2;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

namespace GaitoBotEditorCore.controls
{
	public class ucBotDenkprotokoll : UserControl
	{
		public class CategoryAngewaehltEventArgs : EventArgs
		{
			public WissensCategory Category;

			public CategoryAngewaehltEventArgs(WissensCategory category)
			{
				this.Category = category;
			}
		}

		public delegate void CategoryAngewaehltEventHandler(object sender, CategoryAngewaehltEventArgs e);

		private Arbeitsbereich _arbeitsbereich;

		private IContainer components = null;

		private ListView listViewSchritte;

		private Label labelDenkprotokoll;

		private ImageList imageListDenkprotokoll;

		public Arbeitsbereich Arbeitsbereich
		{
			set
			{
				if (this._arbeitsbereich != null)
				{
					throw new ApplicationException("Diesem ucBotDenkProtokoll wurde bereits ein Arbeitsbereich zugeordnet");
				}
				this._arbeitsbereich = value;
			}
		}

		public List<BotDenkProtokollSchritt> Denkprotokoll
		{
			set
			{
				this.DenkprotokollAnzeigen(value);
			}
		}

		public event CategoryAngewaehltEventHandler CategoryAngewaehlt;

		protected virtual void CategoryAngewaehltEvent(WissensCategory category)
		{
			if (this.CategoryAngewaehlt != null)
			{
				this.CategoryAngewaehlt(this, new CategoryAngewaehltEventArgs(category));
			}
		}

		public ucBotDenkprotokoll()
		{
			this.InitializeComponent();
		}

		private void ucBotDenkprotokoll_Load(object sender, EventArgs e)
		{
			this.SteuerelementeBeschriften();
			base.Resize += this.ucBotDenkprotokoll_Resize;
			this.listViewSchritte.MouseMove += this.listViewSchritte_MouseMove;
		}

		private void listViewSchritte_MouseMove(object sender, MouseEventArgs e)
		{
			ListViewItem itemAt = this.listViewSchritte.GetItemAt(e.X, e.Y);
			if (itemAt == null)
			{
				this.Cursor = Cursors.Default;
			}
			else if (itemAt.Tag == null)
			{
				this.Cursor = Cursors.Default;
			}
			else
			{
				this.Cursor = Cursors.Hand;
			}
		}

		private void ucBotDenkprotokoll_Resize(object sender, EventArgs e)
		{
			this.labelDenkprotokoll.Top = 5;
			this.labelDenkprotokoll.Left = 0;
			this.listViewSchritte.Left = 0;
			this.listViewSchritte.Top = this.labelDenkprotokoll.Top + this.labelDenkprotokoll.Height;
			ListView listView = this.listViewSchritte;
			Size clientSize = base.ClientSize;
			listView.Height = clientSize.Height - this.listViewSchritte.Top;
			ListView listView2 = this.listViewSchritte;
			clientSize = base.ClientSize;
			listView2.Width = clientSize.Width;
			clientSize = this.listViewSchritte.ClientSize;
			int width = clientSize.Width;
			int num = width;
			if (this.listViewSchritte.Columns.Count == 2)
			{
				this.listViewSchritte.Columns[0].Width = width / 3;
				num -= this.listViewSchritte.Columns[0].Width;
				this.listViewSchritte.Columns[1].Width = num;
			}
		}

		private void DenkprotokollAnzeigen(List<BotDenkProtokollSchritt> denkprotokoll)
		{
			this.listViewSchritte.Items.Clear();
			foreach (BotDenkProtokollSchritt item in denkprotokoll)
			{
				this.listViewSchritte.Items.Add(this.Zeile(item));
			}
		}

		private ListViewItem Zeile(BotDenkProtokollSchritt schritt)
		{
			ListViewItem listViewItem = new ListViewItem();
			if (schritt.Category != null)
			{
				listViewItem.Tag = schritt.Category;
			}
			switch (schritt.Art)
			{
			case BotDenkProtokollSchritt.SchrittArten.PassendeKategorieGefunden:
				listViewItem.ImageKey = "categoryfound";
				listViewItem.SubItems.Add(new ListViewItem.ListViewSubItem());
				listViewItem.SubItems[0].Text = ResReader.Reader.GetString("passendeKategorieGefunden");
				listViewItem.SubItems[1].Text = this.BotCategoryBeschreibung(schritt.Category);
				break;
			case BotDenkProtokollSchritt.SchrittArten.Warnung:
				listViewItem.ImageKey = "warnung";
				listViewItem.SubItems.Add(new ListViewItem.ListViewSubItem());
				listViewItem.SubItems[0].Text = ResReader.Reader.GetString("Warnung");
				listViewItem.SubItems[1].Text = schritt.Meldung;
				break;
			case BotDenkProtokollSchritt.SchrittArten.Info:
				listViewItem.ImageKey = "info";
				listViewItem.SubItems.Add(new ListViewItem.ListViewSubItem());
				listViewItem.SubItems[0].Text = ResReader.Reader.GetString("DenkprotokollInfo");
				listViewItem.SubItems[1].Text = schritt.Meldung;
				break;
			case BotDenkProtokollSchritt.SchrittArten.Eingabe:
				listViewItem.ImageKey = "eingabe";
				listViewItem.SubItems.Add(new ListViewItem.ListViewSubItem());
				listViewItem.SubItems[0].Text = ResReader.Reader.GetString("DenkProtokollEingabe");
				listViewItem.SubItems[1].Text = schritt.Meldung;
				break;
			default:
				listViewItem.SubItems.Add(new ListViewItem.ListViewSubItem());
				listViewItem.SubItems[1].Text = schritt.Meldung;
				break;
			}
			return listViewItem;
		}

		private void SteuerelementeBeschriften()
		{
			this.labelDenkprotokoll.Text = ResReader.Reader.GetString("labelDenkprotokoll");
			this.listViewSchritte.Columns.Add(ResReader.Reader.GetString("DenkprotokollColumnArt"));
			this.listViewSchritte.Columns.Add(ResReader.Reader.GetString("DenkprotokollColumnInhalt"));
		}

		private void listViewSchritte_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.listViewSchritte.SelectedItems.Count == 1)
			{
				ListViewItem listViewItem = this.listViewSchritte.SelectedItems[0];
				if (listViewItem.Tag != null)
				{
					WissensCategory category = (WissensCategory)listViewItem.Tag;
					this.CategoryAngewaehltEvent(category);
				}
			}
		}

		private string BotCategoryBeschreibung(WissensCategory category)
		{
			XmlNode categoryNode = category.CategoryNode;
			if (categoryNode != null)
			{
				AIMLCategory categoryForCategoryNode = this._arbeitsbereich.GetCategoryForCategoryNode(categoryNode);
				if (categoryForCategoryNode != null)
				{
					return categoryForCategoryNode.AutoKomplettZusammenfassung;
				}
			}
			return string.Format("{0}|THAT:{1}|TOPIC:{2}", category.Pattern, category.That, category.ThemaName);
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
			this.components = new Container();
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ucBotDenkprotokoll));
			this.listViewSchritte = new ListView();
			this.imageListDenkprotokoll = new ImageList(this.components);
			this.labelDenkprotokoll = new Label();
			base.SuspendLayout();
			this.listViewSchritte.BorderStyle = BorderStyle.None;
			this.listViewSchritte.Cursor = Cursors.Default;
			this.listViewSchritte.FullRowSelect = true;
			this.listViewSchritte.GridLines = true;
			this.listViewSchritte.HideSelection = false;
			componentResourceManager.ApplyResources(this.listViewSchritte, "listViewSchritte");
			this.listViewSchritte.MultiSelect = false;
			this.listViewSchritte.Name = "listViewSchritte";
			this.listViewSchritte.SmallImageList = this.imageListDenkprotokoll;
			this.listViewSchritte.UseCompatibleStateImageBehavior = false;
			this.listViewSchritte.View = View.Details;
			this.listViewSchritte.SelectedIndexChanged += this.listViewSchritte_SelectedIndexChanged;
			this.imageListDenkprotokoll.ImageStream = (ImageListStreamer)componentResourceManager.GetObject("imageListDenkprotokoll.ImageStream");
			this.imageListDenkprotokoll.TransparentColor = Color.Transparent;
			this.imageListDenkprotokoll.Images.SetKeyName(0, "warnung");
			this.imageListDenkprotokoll.Images.SetKeyName(1, "categoryfound");
			this.imageListDenkprotokoll.Images.SetKeyName(2, "info");
			this.imageListDenkprotokoll.Images.SetKeyName(3, "eingabe");
			componentResourceManager.ApplyResources(this.labelDenkprotokoll, "labelDenkprotokoll");
			this.labelDenkprotokoll.Name = "labelDenkprotokoll";
			componentResourceManager.ApplyResources(this, "$this");
			base.AutoScaleMode = AutoScaleMode.Font;
			base.Controls.Add(this.labelDenkprotokoll);
			base.Controls.Add(this.listViewSchritte);
			base.Name = "ucBotDenkprotokoll";
			base.Load += this.ucBotDenkprotokoll_Load;
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
