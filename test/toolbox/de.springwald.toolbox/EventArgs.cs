using System;

namespace de.springwald.toolbox
{
	public class EventArgs<T> : EventArgs
	{
		private T m_value;

		public T Value
		{
			get
			{
				return this.m_value;
			}
		}

		public EventArgs(T value)
		{
			this.m_value = value;
		}
	}
	public class EventArgs<T, U> : EventArgs<T>
	{
		private U m_value2;

		public U Value2
		{
			get
			{
				return this.m_value2;
			}
		}

		public EventArgs(T value, U value2)
			: base(value)
		{
			this.m_value2 = value2;
		}
	}
}
