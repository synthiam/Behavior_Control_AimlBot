using de.springwald.xml.cursor;
using System;
using System.Collections.Generic;
using System.Xml;

namespace de.springwald.xml.editor
{
	public class XMLUndoHandler : IDisposable
	{
		private bool _disposed = false;

		private int _pos = 0;

		private XmlNode _rootNode;

		private XmlDocument _dokument;

		private List<XMLUndoSchritt> _undoSchritte;

		private bool _interneVeraenderungLaeuft = false;

		private int VorherigeSnapshotPos
		{
			get
			{
				int num = this._pos;
				do
				{
					num--;
				}
				while (num > 0 && !this._undoSchritte[num].IstSnapshot);
				return num;
			}
		}

		public XmlNode RootNode
		{
			get
			{
				return this._rootNode;
			}
		}

		public bool UndoMoeglich
		{
			get
			{
				return this._pos > 0;
			}
		}

		public string NextUndoSnapshotName
		{
			get
			{
				if (this.UndoMoeglich)
				{
					return string.Format(ResReader.Reader.GetString("NameUndoSchritt"), this._undoSchritte[this.VorherigeSnapshotPos].SnapShotName);
				}
				return ResReader.Reader.GetString("KeinUndoSchrittVerfuegbar");
			}
		}

		public XMLUndoHandler(XmlNode rootNode)
		{
			this._undoSchritte = new List<XMLUndoSchritt>();
			this._undoSchritte.Add(new XMLUndoSchritt());
			this._rootNode = rootNode;
			this._dokument = this._rootNode.OwnerDocument;
			this._dokument.NodeChanging += this._dokument_NodeChanging;
			this._dokument.NodeInserted += this._dokument_NodeInserted;
			this._dokument.NodeRemoving += this._dokument_NodeRemoving;
		}

		public void SnapshotSetzen(string snapShotName, XMLCursor cursor)
		{
			this._undoSchritte[this._pos].SnapShotName = snapShotName;
			this._undoSchritte[this._pos].CursorVorher = cursor;
		}

		public XMLCursor Undo()
		{
			if (this.UndoMoeglich)
			{
				this._interneVeraenderungLaeuft = true;
				do
				{
					this._undoSchritte[this._pos].UnDo();
					this._pos--;
				}
				while (this._pos != 0 && !this._undoSchritte[this._pos].IstSnapshot);
				this._interneVeraenderungLaeuft = false;
				return this._undoSchritte[this._pos].CursorVorher;
			}
			return null;
		}

		private void _dokument_NodeRemoving(object sender, XmlNodeChangedEventArgs e)
		{
			if (!this._interneVeraenderungLaeuft)
			{
				if (e.Node is XmlAttribute)
				{
					this.NeuenUndoSchrittAnhaengen(new XMLUndoSchrittAttributRemoved((XmlAttribute)e.Node));
				}
				else
				{
					this.NeuenUndoSchrittAnhaengen(new XMLUndoSchrittNodeRemoved(e.Node));
				}
			}
		}

		private void _dokument_NodeChanging(object sender, XmlNodeChangedEventArgs e)
		{
			if (!this._interneVeraenderungLaeuft)
			{
				this.NeuenUndoSchrittAnhaengen(new XMLUndoSchrittNodeChanged(e.Node, e.OldValue));
			}
		}

		private void _dokument_NodeInserted(object sender, XmlNodeChangedEventArgs e)
		{
			if (!this._interneVeraenderungLaeuft)
			{
				this.NeuenUndoSchrittAnhaengen(new XMLUndoSchrittNodeInserted(e.Node, e.NewParent));
			}
		}

		public void Dispose()
		{
			if (!this._disposed)
			{
				this._dokument.NodeChanging -= this._dokument_NodeChanging;
				this._dokument.NodeInserted -= this._dokument_NodeInserted;
				this._dokument.NodeRemoving -= this._dokument_NodeRemoving;
				this._disposed = true;
			}
		}

		private void NeuenUndoSchrittAnhaengen(XMLUndoSchritt neuerUndoSchritt)
		{
			List<XMLUndoSchritt> list = new List<XMLUndoSchritt>();
			for (int i = this._pos + 1; i < this._undoSchritte.Count; i++)
			{
				list.Add(this._undoSchritte[i]);
			}
			foreach (XMLUndoSchritt item in list)
			{
				this._undoSchritte.Remove(item);
			}
			this._undoSchritte.Add(neuerUndoSchritt);
			this._pos++;
			if (this._pos == this._undoSchritte.Count - 1)
			{
				return;
			}
			throw new Exception("Undo-Pos sollte mit undoSchritte.Count-1 übereinstimmen. Statt dessen pos: " + this._pos + ", _undoSchritte.Count -1:" + (this._undoSchritte.Count - 1));
		}
	}
}
