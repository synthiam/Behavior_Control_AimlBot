namespace de.springwald.xml.dtd
{
	public class DTDEntity
	{
		private string _name;

		private string _inhalt;

		private bool _istErsetzungsEntity;

		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				this._name = value;
			}
		}

		public string Inhalt
		{
			get
			{
				return this._inhalt;
			}
			set
			{
				this._inhalt = value;
			}
		}

		public bool IstErsetzungsEntity
		{
			get
			{
				return this._istErsetzungsEntity;
			}
			set
			{
				this._istErsetzungsEntity = value;
			}
		}
	}
}
