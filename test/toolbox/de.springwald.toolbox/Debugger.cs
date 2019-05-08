using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace de.springwald.toolbox
{
	public class Debugger
	{
		public enum ProtokollTypen
		{
			BenutzerHandlung = 1,
			TechnischerVorgang,
			TechnischerMilestone,
			Fehlermeldung
		}

		private const string _linie = "---------------------------------------------------------";

		private string _protokollDateiname;

		private StreamWriter _protokollWriter;

		private static Debugger _globalDebugger;

		public string ProtokollDateiname
		{
			get
			{
				return this._protokollDateiname;
			}
		}

		public static Debugger GlobalDebugger
		{
			get
			{
				if (Debugger._globalDebugger == null)
				{
					Debugger._globalDebugger = new Debugger();
				}
				return Debugger._globalDebugger;
			}
		}

		public event DebuggerNeueZeileEventHandler NeueProtokollZeileEvent;

		protected virtual void NeueProtokollZeile(DebuggerNeueZeileEventArgs e)
		{
			if (this.NeueProtokollZeileEvent != null)
			{
				this.NeueProtokollZeileEvent(this, e);
			}
		}

		public void SetzeProtokollDateiname(string dateiname, bool fehlerAnzeigen)
		{
			if (this._protokollDateiname != null)
			{
				if (fehlerAnzeigen)
				{
					this.FehlerZeigen(string.Format("Es wurde bereits die Protokolldateiname '{0}' zugewiesen", this._protokollDateiname), "Debugger", "set ProtokollDateiname");
				}
			}
			else
			{
				this._protokollDateiname = dateiname;
				if (File.Exists(this._protokollDateiname))
				{
					try
					{
						File.Delete(this._protokollDateiname);
					}
					catch (Exception arg)
					{
						if (fehlerAnzeigen)
						{
							this.FehlerZeigen(string.Format("Konnte Protokolldatei '{0}' nicht löschen: \n\n{1}", this._protokollDateiname, arg), "Debugger", "set ProtokollDateiname");
						}
					}
				}
				try
				{
					this._protokollWriter = new StreamWriter(this._protokollDateiname, false, Encoding.GetEncoding("ISO-8859-15"));
					this._protokollWriter.AutoFlush = true;
				}
				catch (Exception arg2)
				{
					if (fehlerAnzeigen)
					{
						this.FehlerZeigen(string.Format("Konnte Protokolldatei '{0}' nicht anlegen: \n\n{1}", this._protokollDateiname, arg2), "Debugger", "set ProtokollDateiname");
						this._protokollDateiname = null;
						this._protokollWriter = null;
					}
					return;
				}
				this.SchreibeZeile("---------------------------------------------------------");
				this.SchreibeZeile(string.Format("Protokollstart: {0}", DateTime.Now));
				this.SchreibeZeile("---------------------------------------------------------");
			}
		}

		public void Protokolliere(string Meldung)
		{
			this.Protokolliere(Meldung, ProtokollTypen.TechnischerVorgang);
		}

		public void Protokolliere(string meldung, ProtokollTypen meldungsTyp)
		{
			switch (meldungsTyp)
			{
			case ProtokollTypen.BenutzerHandlung:
				this.SchreibeZeile(string.Format("USER> {0}", meldung));
				break;
			case ProtokollTypen.TechnischerVorgang:
				this.SchreibeZeile(string.Format("   ... {0}", meldung));
				break;
			case ProtokollTypen.TechnischerMilestone:
				this.SchreibeZeile(string.Format(">>> {0}", meldung));
				break;
			case ProtokollTypen.Fehlermeldung:
				this.SchreibeZeile("---------------------------------------------------------");
				this.SchreibeZeile(string.Format("ERROR: {0}", meldung));
				this.SchreibeZeile("---------------------------------------------------------");
				break;
			default:
				this.SchreibeZeile(string.Format("? ... {0}", meldung));
				break;
			}
		}

		public void FehlerZeigen(string fehlermeldung, object woAufgetretenObjekt, string woAufgetretenKontext)
		{
			this.Protokolliere(string.Format("{0}\n({1}->{2})", fehlermeldung, woAufgetretenObjekt, woAufgetretenKontext), ProtokollTypen.Fehlermeldung);
			MessageBox.Show(fehlermeldung, woAufgetretenObjekt.ToString() + "->" + woAufgetretenKontext);
		}

		public void ProtokollDateiSchliessen()
		{
			if (this._protokollWriter != null)
			{
				this._protokollWriter.Close();
			}
		}

		private void SchreibeZeile(string inhalt)
		{
			if (this._protokollWriter != null)
			{
				this._protokollWriter.WriteLine(inhalt);
				this._protokollWriter.Flush();
			}
			this.NeueProtokollZeile(new DebuggerNeueZeileEventArgs(inhalt));
		}
	}
}
