using System;
using System.Collections.Generic;
using System.Reflection;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Comparers;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	public partial class StackTool
	{
		public sealed class SortMenuItemFactoryExtensionPoint : ExtensionPoint<ISortMenuItemFactory>
		{
			public SortMenuItemFactoryExtensionPoint()
			{
			}
		}

		[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
		public sealed class ImageComparerFactoryMethodAttribute : Attribute
		{
			public readonly string Name;
			public readonly string Description;

			public ImageComparerFactoryMethodAttribute(string name, string description)
			{
				Platform.CheckForEmptyString(name, "name");
				Platform.CheckForEmptyString(description, "description");

				Name = name;
				Description = description;
			}
		}

		public interface ISortMenuItemFactory
		{
			List<ISortMenuItem> Create();
		}

		public class SortMenuItemFactory : ISortMenuItemFactory
		{
			public SortMenuItemFactory()
			{
			}

			#region ISortMenuItemFactory Members

			public virtual List<ISortMenuItem> Create()
			{
				List<ISortMenuItem> sorters = new List<ISortMenuItem>();

				IResourceResolver resolver = new ResourceResolver(this.GetType().Assembly);

				MethodInfo[] methods = this.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
				foreach (MethodInfo method in methods)
				{
					Type type = typeof(ImageComparerFactoryMethodAttribute);
					if (method.IsDefined(type, false))
					{
						ImageComparerFactoryMethodAttribute attr =
							(ImageComparerFactoryMethodAttribute)method.GetCustomAttributes(type, false)[0];

						IComparer<IPresentationImage> comparer = method.Invoke(this, null) as IComparer<IPresentationImage>;
						if (comparer != null)
						{
							string description = resolver.LocalizeString(attr.Description);
							sorters.Add(new SortMenuItem(attr.Name, description, comparer));
						}
					}
				}

				return sorters;
			}

			#endregion
		}

		[ExtensionOf(typeof(SortMenuItemFactoryExtensionPoint))]
		public class DefaultSortMenuItemFactory : SortMenuItemFactory
		{
			public DefaultSortMenuItemFactory()
			{
			}

			[ImageComparerFactoryMethod("Image Number", "SortByImageNumberDescription")]
			public static IComparer<IPresentationImage> CreateImageAndFrameNumberComparer()
			{
				return new InstanceAndFrameNumberComparer();
			}

			[ImageComparerFactoryMethod("Image Number (Reverse)", "SortByReverseImageNumberDescription")]
			public static IComparer<IPresentationImage> CreateReverseImageAndFrameNumberComparer()
			{
				return new InstanceAndFrameNumberComparer(true);
			}

			[ImageComparerFactoryMethod("Acquisition Time", "SortByAcquisitionTimeDescription")]
			public static IComparer<IPresentationImage> CreateAcquisitionTimeComparer()
			{
				return new AcquisitionTimeComparer();
			}

			[ImageComparerFactoryMethod("Acquisition Time (Reverse)", "SortByReverseAcquisitionTimeDescription")]
			public static IComparer<IPresentationImage> CreateReverseAcquisitionTimeComparer()
			{
				return new AcquisitionTimeComparer(true);
			}

			[ImageComparerFactoryMethod("Slice Location", "SortBySliceLocationDescription")]
			public static IComparer<IPresentationImage> CreateSliceLocationComparer()
			{
				return new SliceLocationComparer();
			}

			[ImageComparerFactoryMethod("Slice Location (Reverse)", "SortByReverseSliceLocationDescription")]
			public static IComparer<IPresentationImage> CreateReverseSliceLocationComparer()
			{
				return new SliceLocationComparer(true);
			}
		}
	}
}
