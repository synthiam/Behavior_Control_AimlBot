using System.Xml;

namespace de.springwald.gaitobot2
{
	internal class ConditionStatus
	{
		public string Name
		{
			get;
			set;
		}

		public string Value
		{
			get;
			set;
		}

		public string Contains
		{
			get;
			set;
		}

		public string Exists
		{
			get;
			set;
		}

		public bool KannSchonSchliessen
		{
			get
			{
				if (this.Name != null)
				{
					if (this.Value != null)
					{
						return true;
					}
					if (this.Exists != null)
					{
						return true;
					}
					if (this.Contains != null)
					{
						return true;
					}
				}
				return false;
			}
		}

		public bool Erfuellt(GaitoBotSession session)
		{
			if (!this.KannSchonSchliessen)
			{
				return false;
			}
			if (this.GetIsEmpty(session))
			{
				if (this.Exists != null)
				{
					return this.Exists == "false";
				}
				if (this.Contains != null)
				{
					return false;
				}
				if (this.Value != null)
				{
					return false;
				}
			}
			else
			{
				if (this.Exists != null)
				{
					return this.Exists == "true";
				}
				string inhalt = this.GetInhalt(session);
				if (this.Value != null)
				{
					return inhalt.ToLower() == this.Value.ToLower();
				}
				if (this.Contains != null)
				{
					return inhalt.IndexOf(this.Contains.ToLower()) != -1;
				}
			}
			return false;
		}

		public void AttributeAusNodeHinzufuegen(XmlNode node)
		{
			if (node.Attributes["name"] != null)
			{
				this.Name = node.Attributes["name"].Value;
			}
			if (node.Attributes["value"] != null)
			{
				this.Value = node.Attributes["value"].Value;
			}
			if (node.Attributes["contains"] != null)
			{
				this.Contains = node.Attributes["contains"].Value;
			}
			if (node.Attributes["exists"] != null)
			{
				this.Exists = node.Attributes["exists"].Value;
			}
		}

		public ConditionStatus Clone()
		{
			ConditionStatus conditionStatus = new ConditionStatus();
			conditionStatus.Name = this.Name;
			conditionStatus.Value = this.Value;
			conditionStatus.Contains = this.Contains;
			conditionStatus.Exists = this.Exists;
			return conditionStatus;
		}

		private bool GetIsEmpty(GaitoBotSession session)
		{
			string standardConditionContent = StandardGlobaleEigenschaften.GetStandardConditionContent(this.Name);
			if (standardConditionContent == null)
			{
				return session.UserEigenschaften.IsEmpty(this.Name);
			}
			return false;
		}

		private string GetInhalt(GaitoBotSession session)
		{
			string standardConditionContent = StandardGlobaleEigenschaften.GetStandardConditionContent(this.Name);
			if (standardConditionContent == null)
			{
				return session.UserEigenschaften.Lesen(this.Name).ToLower();
			}
			return standardConditionContent;
		}
	}
}
