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

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;

namespace ClearCanvas.ImageViewer.Tools.ImageProcessing.Filter
{
	// We decorate FilterTool with the DropDownButtonAction attribute
	// and set the path such that it shows up in the main toolbar. We specify that the
	// contents of the menu are to retrieved using the DropDownMenuModel property.
	[DropDownAction("apply", "global-toolbars/ToolbarStandard/ToolbarFilter", "DropDownMenuModel")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[Tooltip("apply", "Filters")]
	[IconSet("apply", IconScheme.Colour, "Icons.FilterToolSmall.png", "Icons.FilterToolMedium.png", "Icons.FilterToolLarge.png")]

	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class FilterTool : ImageViewerTool
	{ 
		public FilterTool()
		{
		}

		// We have to provide the dropdown button with the data to populate the dropdown menu.
		public ActionModelNode DropDownMenuModel
		{
			get
			{
				// The filter tools are ImageViewerToolExtensions, so we have to get the
				// actions from the ImageViewerComponent. Note that while 
				// ImageViewerComponent.ExportedActions gets *all* the actions associated with
				// the ImageViewerComponent, the fact that we specify the site (i.e.
				// imageviewer-filterdropdownmenu) when we call CreateModel will cause 
				// the model to only contain those actions which have that site specified
				// in its path.

				return ActionModelRoot.CreateModel(
					this.GetType().FullName,
					"imageviewer-filterdropdownmenu",
					this.ImageViewer.ExportedActions);
			}
		}
	}
}
