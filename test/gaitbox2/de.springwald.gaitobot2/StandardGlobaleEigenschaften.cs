using System;
using System.Globalization;

namespace de.springwald.gaitobot2
{
	public class StandardGlobaleEigenschaften
	{
		public static string GetStandardConditionContent(string name)
		{
			name = name.ToLower().Trim();
			try
			{
				if (name.StartsWith("datetime."))
				{
					return StandardGlobaleEigenschaften.GetDateTimeCondition(name);
				}
			}
			catch (Exception ex)
			{
				return string.Format("Error processing GetDateTimeCondition '{0}': {1} ", name, ex.Message);
			}
			return null;
		}

		private static string GetDateTimeCondition(string name)
		{
			DateTime now = DateTime.Now;
			int num;
			switch (name)
			{
			case "datetime.now.day":
				num = now.Day;
				return num.ToString();
			case "datetime.now.dayofweek":
				switch (now.DayOfWeek)
				{
				case DayOfWeek.Monday:
					return "1";
				case DayOfWeek.Tuesday:
					return "2";
				case DayOfWeek.Wednesday:
					return "3";
				case DayOfWeek.Thursday:
					return "4";
				case DayOfWeek.Friday:
					return "5";
				case DayOfWeek.Saturday:
					return "6";
				case DayOfWeek.Sunday:
					return "7";
				default:
					throw new ApplicationException(string.Format("Unbehandelter DayOfWeek '{0}'", now.DayOfWeek));
				}
			case "datetime.now.dayofweekname":
				return CultureInfo.CurrentCulture.DateTimeFormat.DayNames[(int)now.DayOfWeek].ToString();
			case "datetime.now.dayofyear":
				num = now.DayOfYear;
				return num.ToString();
			case "datetime.now.hour":
				num = now.Hour;
				return num.ToString();
			case "datetime.now.millisecond":
				num = now.Millisecond;
				return num.ToString();
			case "datetime.now.minute":
				num = now.Minute;
				return num.ToString();
			case "datetime.now.month":
				num = now.Month;
				return num.ToString();
			case "datetime.now.monthname":
				return DateTimeFormatInfo.CurrentInfo.MonthNames[now.Month - 1];
			case "datetime.now.second":
				num = now.Second;
				return num.ToString();
			case "datetime.now.year":
				num = now.Year;
				return num.ToString();
			default:
				throw new ApplicationException(string.Format("Unbehandelte DateTime-Syntax '{0}'", name));
			}
		}
	}
}
