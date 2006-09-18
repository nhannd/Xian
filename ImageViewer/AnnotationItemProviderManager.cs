using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer
{
	[ExtensionPoint()]
	public class AnnotationItemProviderExtensionPoint : ExtensionPoint<IAnnotationItemProvider>
	{ 
	}

	public class AnnotationItemProviderManager  : BasicExtensionPointManager<IAnnotationItemProvider>
	{
		private bool _isLoaded = false;

		public AnnotationItemProviderManager()
		{
		}

		public IEnumerable<IAnnotationItemProvider> ProviderCollection
		{
			get
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
				
				return this.Extensions.AsReadOnly();
			}
		}

		protected override IExtensionPoint GetExtensionPoint()
		{
			return new AnnotationItemProviderExtensionPoint();
		}
	}
}
