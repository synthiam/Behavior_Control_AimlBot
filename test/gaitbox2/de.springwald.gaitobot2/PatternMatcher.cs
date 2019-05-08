using System;
using System.Text.RegularExpressions;

namespace de.springwald.gaitobot2
{
	public class PatternMatcher
	{
		private readonly string _eingabe;

		private readonly bool _erfolgreich;

		private readonly Match _match;

		public bool Erfolgreich
		{
			get
			{
				return this._erfolgreich;
			}
		}

		public PatternMatcher(Regex regExObjekt, string eingabe)
		{
			this._eingabe = eingabe;
			this._match = regExObjekt.Match(eingabe);
			this._erfolgreich = this._match.Success;
		}

		public string GetStarInhalt(int starNr)
		{
			if (!this._erfolgreich)
			{
				throw new ApplicationException("Requested STAR for not succesfull match!");
			}
			string groupname = string.Format("star{0}", starNr);
			if (this._match.Groups[groupname] == null)
			{
				throw new ApplicationException(string.Format(ResReader.Reader(null).GetString("InputStarIndexUeberschritten"), starNr, this._eingabe));
			}
			return this._match.Groups[groupname].Value;
		}
	}
}
