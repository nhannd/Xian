#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;

namespace ClearCanvas.ImageViewer.AdvancedImaging.Fusion
{
	[MenuAction("show", "global-menus/MenuTools/MenuAdvanced/MenuFusion", "ShowControlPanel")]
	[ButtonAction("show", "global-toolbars/ToolbarAdvanced/ToolbarFusion", "ShowControlPanel")]
	[VisibleStateObserver("show", "Enabled", "EnabledChanged")]
	[IconSet("show", IconScheme.Colour, "Icons.FusionToolSmall.png", "Icons.FusionToolMedium.png", "Icons.FusionToolLarge.png")]
	[ExtensionOf(typeof (ImageViewerToolExtensionPoint))]
	public class FusionTool : ImageViewerTool
	{
		private static readonly Dictionary<IDesktopWindow, IShelf> _shelves = new Dictionary<IDesktopWindow, IShelf>();

		public void ShowControlPanel()
		{
			IDesktopWindow desktopWindow = this.Context.DesktopWindow;
			if (!_shelves.ContainsKey(desktopWindow))
			{
				FusionControlPanelComponent component = new FusionControlPanelComponent(desktopWindow);

				IShelf shelf = ApplicationComponent.LaunchAsShelf(desktopWindow, component, SR.TitleFusion, "ImageFusionCP", ShelfDisplayHint.DockLeft);
				shelf.Closed += OnShelfClosed;
				_shelves.Add(desktopWindow, shelf);
			}
			else
			{
				_shelves[desktopWindow].Activate();
			}
		}

		private static void OnShelfClosed(object sender, ClosedEventArgs e)
		{
			var shelf = (IShelf) sender;
			shelf.Closed -= OnShelfClosed;
			_shelves.Remove(shelf.DesktopWindow);
		}

		public override void Initialize()
		{
			base.Initialize();

			if (this.ImageViewer != null && this.ImageViewer.LogicalWorkspace != null)
				this.Enabled = AtLeastOne(this.ImageViewer.LogicalWorkspace.ImageSets,
				                          imageSet => AtLeastOne(imageSet.DisplaySets, displaySet => displaySet.Descriptor is PETFusionDisplaySetDescriptor));
		}

		private static bool AtLeastOne<T>(IEnumerable<T> collection, Predicate<T> predicate)
		{
			foreach (T item in collection)
			{
				if (predicate.Invoke(item))
					return true;
			}
			return false;
		}
	}
}