using System;
using System.Xml;

namespace GaitoBotEditorCore
{
	public interface IArbeitsbereichDatei
	{
		string ZusatzContentUniqueId
		{
			get;
			set;
		}

		bool ReadOnly
		{
			get;
			set;
		}

		bool NurFuerGaitoBotExportieren
		{
			get;
			set;
		}

		Arbeitsbereich Arbeitsbereich
		{
			get;
		}

		string[] Unterverzeichnisse
		{
			get;
		}

		string Dateiname
		{
			get;
			set;
		}

		bool HatFehler
		{
			get;
		}

		bool IsChanged
		{
			get;
		}

		XmlDocument XML
		{
			get;
		}

		string Titel
		{
			get;
		}

		string SortKey
		{
			get;
		}

		event EventHandler OnChanged;

		void LiesAusString(string inhalt);

		void LiesAusDatei(string dateiname, string arbeitsbereichRootPfad);

		void Save(out bool cancel);

		void LeerFuellen();
	}
}
