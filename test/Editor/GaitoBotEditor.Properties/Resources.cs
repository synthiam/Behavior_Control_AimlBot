using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace GaitoBotEditor.Properties
{
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
	[DebuggerNonUserCode]
	[CompilerGenerated]
	public class Resources
	{
		private static ResourceManager resourceMan;

		private static CultureInfo resourceCulture;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static ResourceManager ResourceManager
		{
			get
			{
				if (Resources.resourceMan == null)
				{
					ResourceManager resourceManager = Resources.resourceMan = new ResourceManager("GaitoBotEditor.Properties.Resources", typeof(Resources).Assembly);
				}
				return Resources.resourceMan;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static CultureInfo Culture
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

		public static Bitmap applications_16
		{
			get
			{
				object @object = Resources.ResourceManager.GetObject("applications_16", Resources.resourceCulture);
				return (Bitmap)@object;
			}
		}

		public static Bitmap BINOCULR
		{
			get
			{
				object @object = Resources.ResourceManager.GetObject("BINOCULR", Resources.resourceCulture);
				return (Bitmap)@object;
			}
		}

		public static Bitmap Germany
		{
			get
			{
				object @object = Resources.ResourceManager.GetObject("Germany", Resources.resourceCulture);
				return (Bitmap)@object;
			}
		}

		public static Bitmap Globe
		{
			get
			{
				object @object = Resources.ResourceManager.GetObject("Globe", Resources.resourceCulture);
				return (Bitmap)@object;
			}
		}

		public static Bitmap GRAPH14
		{
			get
			{
				object @object = Resources.ResourceManager.GetObject("GRAPH14", Resources.resourceCulture);
				return (Bitmap)@object;
			}
		}

		public static Bitmap redo_16
		{
			get
			{
				object @object = Resources.ResourceManager.GetObject("redo_16", Resources.resourceCulture);
				return (Bitmap)@object;
			}
		}

		public static Bitmap splashScreen
		{
			get
			{
				object @object = Resources.ResourceManager.GetObject("splashScreen", Resources.resourceCulture);
				return (Bitmap)@object;
			}
		}

		public static Bitmap startseite2
		{
			get
			{
				object @object = Resources.ResourceManager.GetObject("startseite2", Resources.resourceCulture);
				return (Bitmap)@object;
			}
		}

		public static Bitmap United_Kingdom
		{
			get
			{
				object @object = Resources.ResourceManager.GetObject("United Kingdom", Resources.resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal Resources()
		{
		}
	}
}
