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
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Annotations
{
	internal static class AnnotationLayoutFactory 
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
					return new StoredAnnotationLayoutProxy(storedLayout);
				
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
	}
}
