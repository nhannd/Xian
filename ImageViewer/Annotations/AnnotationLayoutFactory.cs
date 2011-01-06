#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Annotations
{
	public static class AnnotationLayoutFactory 
	{
        [Cloneable(true)]
        private class StoredAnnotationLayoutProxy : IAnnotationLayout
        {
            //TODO: later, we may want to just get the annotation boxes directly from the factory (so that settings changes are reflected right away).
            [CloneCopyReference]
            private StoredAnnotationLayout _layout;
            private bool _visible = true;
            
            private StoredAnnotationLayoutProxy()
            {
            }

            public StoredAnnotationLayoutProxy(StoredAnnotationLayout layout)
            {
                _layout = layout;
            }

            #region IAnnotationLayout Members

            public IEnumerable<AnnotationBox> AnnotationBoxes
            {
                get { return _layout.AnnotationBoxes; }
            }

            public bool Visible
            {
                get { return _visible;}
                set { _visible = value; }
            }

            #endregion
        }

		private static readonly List<IAnnotationItemProvider> _providers;

        private static readonly object _syncLock = new object();
        private static readonly Dictionary<string, StoredAnnotationLayout> _layoutCache;

		static AnnotationLayoutFactory()
		{
            _layoutCache = new Dictionary<string, StoredAnnotationLayout>();
            AnnotationLayoutStore.Instance.StoreChanged += delegate { ClearCache(); };

            _providers = new List<IAnnotationItemProvider>();

			try
			{
				foreach (object extension in new AnnotationItemProviderExtensionPoint().CreateExtensions())
					_providers.Add((IAnnotationItemProvider) extension);
			}
			catch(NotSupportedException e)
			{
				Platform.Log(LogLevel.Info, e);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
			}
		}

        private static void ClearCache()
        {
            lock(_syncLock)
            {
                _layoutCache.Clear();
            }
        }

        private static StoredAnnotationLayout GetStoredLayout(string layoutId)
        {
            lock (_syncLock)
            {
                if (_layoutCache.ContainsKey(layoutId))
                    return _layoutCache[layoutId];

                StoredAnnotationLayout layout = AnnotationLayoutStore.Instance.GetLayout(layoutId, AvailableAnnotationItems);
                if (layout != null)
                    _layoutCache[layoutId] = layout;

                return layout;
            }
        }
        
        private static IEnumerable<IAnnotationItem> AvailableAnnotationItems
		{
			get
			{
				foreach (IAnnotationItemProvider provider in _providers)
				{
					foreach (IAnnotationItem item in provider.GetAnnotationItems())
						yield return item;
				}
			}
		}
		
		internal static IAnnotationItem GetAnnotationItem(string annotationItemIdentifier)
		{
			foreach (IAnnotationItemProvider provider in _providers)
			{
				foreach (IAnnotationItem item in provider.GetAnnotationItems())
				{
					if (item.GetIdentifier() == annotationItemIdentifier)
						return item;
				}
			}

			return null;
		}

		public static IAnnotationLayout CreateLayout(string storedLayoutId)
		{
			try
			{
                StoredAnnotationLayout storedLayout = GetStoredLayout(storedLayoutId);
				if (storedLayout != null)
					return storedLayout.Clone();
				
				//just return an empty layout.
				return new AnnotationLayout();
			}
			catch(Exception e)
			{
				Platform.Log(LogLevel.Error, e);

				AnnotationLayout layout = new AnnotationLayout();
				IAnnotationItem item = new BasicTextAnnotationItem("errorbox", "errorbox", SR.LabelError, SR.MessageErrorLoadingAnnotationLayout);
				AnnotationBox box = new AnnotationBox(new RectangleF(0.5F,0.90F, 0.5F, 0.10F), item);
				box.Bold = true;
				box.Color = "Red";
				box.Justification = AnnotationBox.JustificationBehaviour.Right;
				box.NumberOfLines = 5;
				box.VerticalAlignment = AnnotationBox.VerticalAlignmentBehaviour.Bottom;

				layout.AnnotationBoxes.Add(box);
				return layout;
			}
		}

		#region Unit Test Support

#if UNIT_TESTS

		/// <summary>
		/// Forces the <see cref="AnnotationLayoutFactory"/> to be reinitialized.
		/// </summary>
		/// <remarks>
		/// This may be necessary because the list of <see cref="IAnnotationLayoutProvider"/>s as well as individual layouts are cached.
		/// Unit tests relying on <see cref="IAnnotationItem"/>s may need to reset the caches to a pristine state, particularly if other
		/// unit tests have been using different extension factories.
		/// </remarks>
		public static void Reinitialize()
		{
			ClearCache();
			try
			{
				_providers.Clear();
				foreach (object extension in new AnnotationItemProviderExtensionPoint().CreateExtensions())
					_providers.Add((IAnnotationItemProvider) extension);
			}
			catch (NotSupportedException e)
			{
				Platform.Log(LogLevel.Info, e);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
			}
		}

#endif

		#endregion
	}
}
