using System.Collections.Specialized;

namespace de.springwald.xml.dtd
{
	public class AlleMoeglichenElementeEinesChildblocks
	{
		private StringCollection _gefundeneElemente;

		public StringCollection GefundeneElemente
		{
			get
			{
				return this._gefundeneElemente;
			}
		}

		public AlleMoeglichenElementeEinesChildblocks(DTDChildElemente childBlock)
		{
			this._gefundeneElemente = new StringCollection();
			this.Durchsuchen(childBlock);
		}

		private void Durchsuchen(DTDChildElemente childBlock)
		{
			switch (childBlock.Art)
			{
			case DTDChildElemente.DTDChildElementArten.Leer:
				break;
			case (DTDChildElemente.DTDChildElementArten)1:
				break;
			case DTDChildElemente.DTDChildElementArten.EinzelChild:
				this.ElementMerken(childBlock.ElementName);
				break;
			case DTDChildElemente.DTDChildElementArten.ChildListe:
				for (int i = 0; i < childBlock.AnzahlChildren; i++)
				{
					this.Durchsuchen(childBlock.Child(i));
				}
				break;
			}
		}

		private void ElementMerken(string elementName)
		{
			if (!this._gefundeneElemente.Contains(elementName))
			{
				this._gefundeneElemente.Add(elementName);
			}
		}
	}
}
