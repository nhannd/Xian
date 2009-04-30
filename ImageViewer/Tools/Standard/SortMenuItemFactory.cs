#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

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
