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
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class ColorMapTool : ImageViewerTool
	{
		private class ColorMapActionContainer : UndoableOperation<IPresentationImage>
		{
			private readonly ColorMapTool _ownerTool;
			private readonly MenuAction _action;
			private readonly ColorMapDescriptor _descriptor;

			public ColorMapActionContainer(ColorMapTool ownerTool, ColorMapDescriptor descriptor, int index)
			{
				_ownerTool = ownerTool;
				_descriptor = descriptor;

				string actionId = String.Format("apply{0}", index);
				ActionPath actionPath = new ActionPath(String.Format("imageviewer-contextmenu/ColourMaps/colourMap{0}", index), _ownerTool._resolver);
				_action = new MenuAction(actionId, actionPath, ClickActionFlags.None, _ownerTool._resolver);
				_action.GroupHint = new GroupHint("Tools.Image.Manipulation.Lut.ColourMaps");
				_action.Label = _descriptor.Description;
				_action.SetClickHandler(this.Apply);
			}
			
			public ClickAction Action
			{
				get { return _action; }
			}

			private void Apply()
			{
				ImageOperationApplicator applicator = new ImageOperationApplicator(_ownerTool.SelectedPresentationImage, this);
				UndoableCommand historyCommand = applicator.ApplyToAllImages();
				if (historyCommand != null)
				{
					historyCommand.Name = SR.CommandColorMap;
					_ownerTool.Context.Viewer.CommandHistory.AddCommand(historyCommand);
				}
			}

			public override IMemorable GetOriginator(IPresentationImage image)
			{
				if (image is IColorMapProvider)
					return ((IColorMapProvider) image).ColorMapManager;

				return null;
			}

			public override void Apply(IPresentationImage image)
			{
				((IColorMapManager)GetOriginator(image)).InstallColorMap(_descriptor);
			}
		}

		private readonly ActionResourceResolver _resolver;

		public ColorMapTool()
		{
			_resolver = new ActionResourceResolver(this);
		}

		public override IActionSet Actions
		{
			get
			{
				return new ActionSet(GetActions());
			}
		}

		private IEnumerable<IAction> GetActions()
		{
			if (this.SelectedPresentationImage is IColorMapProvider)
			{
				int i = 0;
				foreach (ColorMapDescriptor descriptor in ((IColorMapProvider)this.SelectedPresentationImage).ColorMapManager.AvailableColorMaps)
				{
					ColorMapActionContainer container = new ColorMapActionContainer(this, descriptor, ++i);
					yield return container.Action;
				}
			}
			else
			{
				yield break;
			}
		}
	}
}
