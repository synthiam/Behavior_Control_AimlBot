using System;
using System.Collections;
using System.Drawing;

namespace de.springwald.toolbox
{
	public class RectangleCollection : CollectionBase
	{
		public Rectangle this[int index]
		{
			get
			{
				return (Rectangle)base.List[index];
			}
			set
			{
				base.List[index] = value;
			}
		}

		public int Add(Rectangle value)
		{
			return base.List.Add(value);
		}

		public int IndexOf(Rectangle value)
		{
			return base.List.IndexOf(value);
		}

		public void Insert(int index, Rectangle value)
		{
			base.List.Insert(index, value);
		}

		public void Remove(Rectangle value)
		{
			base.List.Remove(value);
		}

		public bool Contains(Rectangle value)
		{
			return base.List.Contains(value);
		}

		protected override void OnInsert(int index, object value)
		{
			if (value is Rectangle)
			{
				return;
			}
			throw new ArgumentException("value must be of type Rectangle.", "value");
		}

		protected override void OnRemove(int index, object value)
		{
			if (value is Rectangle)
			{
				return;
			}
			throw new ArgumentException("value must be of type Rectangle.", "value");
		}

		protected override void OnSet(int index, object oldValue, object newValue)
		{
			if (newValue is Rectangle)
			{
				return;
			}
			throw new ArgumentException("newValue must be of type Rectangle.", "newValue");
		}

		protected override void OnValidate(object value)
		{
			if (value is Rectangle)
			{
				return;
			}
			throw new ArgumentException("value must be of type Rectangle.");
		}
	}
}
