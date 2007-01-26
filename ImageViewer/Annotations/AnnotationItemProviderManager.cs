using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Annotations
{
	[ExtensionPoint()]
	public class AnnotationItemProviderExtensionPoint : ExtensionPoint<IAnnotationItemProvider>
	{ 
	}

	public sealed class AnnotationItemProviderManager  : BasicExtensionPointManager<IAnnotationItemProvider>
	{
		private static AnnotationItemProviderManager _instance;
		private bool _isLoaded = false;

		private AnnotationItemProviderManager()
		{
		}

		protected override IExtensionPoint GetExtensionPoint()
		{
			return new AnnotationItemProviderExtensionPoint();
		}

		private void Initialize()
		{
			if (_isLoaded == false)
			{
				_isLoaded = true;

				try
				{
					this.LoadExtensions();
				}
				catch (Exception e)
				{
					Platform.Log(e); //don't throw.
				}
			}
		}

		public IList<IAnnotationItemProvider> Providers
		{
			get
			{
				Initialize();
				return this.Extensions.AsReadOnly();
			}
		}

		public IList<IAnnotationItem> AnnotationItems
		{
			get
			{
				List<IAnnotationItem> items = new List<IAnnotationItem>();
				foreach (IAnnotationItemProvider provider in this.Providers)
					items.AddRange(provider.GetAnnotationItems());

				return items;
			}
		}

		public static AnnotationItemProviderManager Instance
		{
			get
			{
				if (_instance == null)
					_instance = new AnnotationItemProviderManager();

				return _instance;
			}
		}

		public IAnnotationItem GetAnnotationItem(string annotationItemIdentifier)
		{
			foreach (IAnnotationItemProvider provider in this.Providers)
			{
				foreach (IAnnotationItem item in provider.GetAnnotationItems())
				{
					if (item.GetIdentifier() == annotationItemIdentifier)
						return item;
				}
			}

			return null;
		}
	}
}
