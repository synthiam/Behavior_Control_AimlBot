using de.springwald.toolbox;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GaitoBotEditorCore
{
	public class ArbeitsbereichVerwaltung
	{
		private List<Arbeitsbereich> _arbeitsbereiche;

		public List<Arbeitsbereich> VorhandeneNochNichtGeladeneArbeitsbereiche
		{
			get
			{
				List<Arbeitsbereich> list = new List<Arbeitsbereich>();
				string[] directories = Directory.GetDirectories(Config.GlobalConfig.ArbeitsbereicheSpeicherVerzeichnis);
				foreach (string str in directories)
				{
					string pfadkorrekt = str + "\\";
					if ((from a in this._arbeitsbereiche
					where a.Arbeitsverzeichnis == pfadkorrekt
					select a).Count() <= 0 && File.Exists(pfadkorrekt + "metainfos.ser"))
					{
						Arbeitsbereich item = new Arbeitsbereich(pfadkorrekt);
						list.Add(item);
					}
				}
				return list;
			}
		}

		public List<Arbeitsbereich> Arbeitsbereiche
		{
			get
			{
				return this._arbeitsbereiche;
			}
		}

		public event EventHandler<EventArgs<Arbeitsbereich>> ArbeitsbereichAddedEvent;

		public event EventHandler<EventArgs<Arbeitsbereich>> ArbeitsbereichEntferntEvent;

		protected virtual void ArbeitsbereichAdded(Arbeitsbereich arbeitsbereich)
		{
			if (this.ArbeitsbereichAddedEvent != null)
			{
				this.ArbeitsbereichAddedEvent(this, new EventArgs<Arbeitsbereich>(arbeitsbereich));
			}
		}

		protected virtual void ArbeitsbereichEntfernt(Arbeitsbereich arbeitsbereich)
		{
			if (this.ArbeitsbereichEntferntEvent != null)
			{
				this.ArbeitsbereichEntferntEvent(this, new EventArgs<Arbeitsbereich>(arbeitsbereich));
			}
		}

		public ArbeitsbereichVerwaltung()
		{
			this._arbeitsbereiche = new List<Arbeitsbereich>();
		}

		public void ProgrammSollBeendetWerden(out bool cancel)
		{
			foreach (Arbeitsbereich item in this._arbeitsbereiche)
			{
				bool flag = false;
				item.ArbeitsbereichSollGeschlossenWerden(out flag);
				if (flag)
				{
					cancel = true;
					return;
				}
			}
			cancel = false;
		}

		public bool NeuenArbeitsbereichErstellenUndOeffnen()
		{
			bool flag = false;
			string name = InputBox.Show(ResReader.Reader.GetString("NamenFuerNeuenArbeitsbereichEingeben"), ResReader.Reader.GetString("NeuenArbeitsbereicherzeugen"), ResReader.Reader.GetString("unbenannt"), out flag);
			if (flag)
			{
				return false;
			}
			Arbeitsbereich arbeitsbereich = new Arbeitsbereich();
			arbeitsbereich.Name = name;
			this._arbeitsbereiche.Add(arbeitsbereich);
			this.ArbeitsbereichAdded(arbeitsbereich);
			return true;
		}

		public bool VorhandenenArbeitsbereichOeffnen(string arbeitsbereichVerzeichnis)
		{
			Arbeitsbereich arbeitsbereich = new Arbeitsbereich(arbeitsbereichVerzeichnis);
			this._arbeitsbereiche.Add(arbeitsbereich);
			this.ArbeitsbereichAdded(arbeitsbereich);
			return true;
		}

		public void ArbeitsbereichEntfernen(Arbeitsbereich arbeitsbereich)
		{
			if (this._arbeitsbereiche.Contains(arbeitsbereich))
			{
				this._arbeitsbereiche.Remove(arbeitsbereich);
				this.ArbeitsbereichEntfernt(arbeitsbereich);
			}
		}
	}
}
