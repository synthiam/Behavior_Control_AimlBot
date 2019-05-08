using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace de.springwald.xml.Properties
{
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
	[DebuggerNonUserCode]
	[CompilerGenerated]
	internal class Resources
	{
		private static ResourceManager resourceMan;

		private static CultureInfo resourceCulture;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (Resources.resourceMan == null)
				{
					ResourceManager resourceManager = Resources.resourceMan = new ResourceManager("de.springwald.xml.Properties.Resources", typeof(Resources).Assembly);
				}
				return Resources.resourceMan;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return Resources.resourceCulture;
			}
			set
			{
				Resources.resourceCulture = value;
			}
		}

		internal static Bitmap add_16
		{
			get
			{
				object @object = Resources.ResourceManager.GetObject("add_16", Resources.resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap copy_16
		{
			get
			{
				object @object = Resources.ResourceManager.GetObject("copy_16", Resources.resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap cut_16
		{
			get
			{
				object @object = Resources.ResourceManager.GetObject("cut_16", Resources.resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap delete_16
		{
			get
			{
				object @object = Resources.ResourceManager.GetObject("delete_16", Resources.resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap delete_161
		{
			get
			{
				object @object = Resources.ResourceManager.GetObject("delete_161", Resources.resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap home_161
		{
			get
			{
				object @object = Resources.ResourceManager.GetObject("home_161", Resources.resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap paste_16
		{
			get
			{
				object @object = Resources.ResourceManager.GetObject("paste_16", Resources.resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap redo_16
		{
			get
			{
				object @object = Resources.ResourceManager.GetObject("redo_16", Resources.resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap sync
		{
			get
			{
				object @object = Resources.ResourceManager.GetObject("sync", Resources.resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap trash_16
		{
			get
			{
				object @object = Resources.ResourceManager.GetObject("trash_16", Resources.resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap undo_16
		{
			get
			{
				object @object = Resources.ResourceManager.GetObject("undo_16", Resources.resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal Resources()
		{
		}
	}
}
