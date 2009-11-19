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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.Volume.Mpr.Tools
{
	[MenuAction("resetAll", "mprviewer-reslicemenu/MenuResetAll", "ResetAll")]
	[MenuAction("resetAll", "imageviewer-contextmenu/MenuResetAll", "ResetAll")]
	[MenuAction("resetAll", "global-menus/MenuTools/MenuMpr/MenuResetAll", "ResetAll")]
	[EnabledStateObserver("resetAll", "CanReset", "CanResetChanged")]
	[IconSet("resetAll", IconScheme.Colour, "Icons.ResetAllToolSmall.png", "Icons.ResetAllToolMedium.png", "Icons.ResetAllToolLarge.png")]
	[Tooltip("resetAll", "TooltipResetAll")]
	[GroupHint("resetAll", "Tools.Volume.MPR.ResetSlicing")]
	partial class ResliceToolGroup
	{
		private event EventHandler _canResetChanged;
		private MprWorkspaceState _mprWorkspaceState;
		private ResliceToolGraphicsState _resliceToolGraphicsState;
		private bool _canReset = false;

		private void InitializeResetAll()
		{
			_canReset = false;
			_mprWorkspaceState = new MprWorkspaceState(this.ImageViewer);
			_resliceToolGraphicsState = new ResliceToolGraphicsState(this);

			foreach (IImageSet imageSet in this.ImageViewer.MprWorkspace.ImageSets)
			{
				foreach (MprDisplaySet displaySet in imageSet.DisplaySets)
				{
					IMprStandardSliceSet sliceSet = displaySet.SliceSet as IMprStandardSliceSet;
					if (sliceSet != null && !sliceSet.IsReadOnly)
					{
						sliceSet.SlicerParamsChanged += OnSliceSetSlicerParamsChanged;
					}
				}
			}
		}

		private void DisposeResetAll()
		{
			foreach (IImageSet imageSet in this.ImageViewer.MprWorkspace.ImageSets)
			{
				foreach (MprDisplaySet displaySet in imageSet.DisplaySets)
				{
					IMprStandardSliceSet sliceSet = displaySet.SliceSet as IMprStandardSliceSet;
					if (sliceSet != null)
					{
						sliceSet.SlicerParamsChanged -= OnSliceSetSlicerParamsChanged;
					}
				}
			}

			if (_mprWorkspaceState != null)
			{
				_mprWorkspaceState.Dispose();
				_mprWorkspaceState = null;
			}

			if (_resliceToolGraphicsState != null)
			{
				_resliceToolGraphicsState.Dispose();
				_resliceToolGraphicsState = null;
			}
		}

		public void ResetAll()
		{
			DrawableUndoableCommand command = new DrawableUndoableCommand(new ImageBoxesDrawable(this.ImageViewer.PhysicalWorkspace));

			MemorableUndoableCommand resliceToolGraphicsStateBeginCommand = new MemorableUndoableCommand(_resliceToolGraphicsState);
			resliceToolGraphicsStateBeginCommand.BeginState = _resliceToolGraphicsState.CreateMemento();
			resliceToolGraphicsStateBeginCommand.EndState = null;
			command.Enqueue(resliceToolGraphicsStateBeginCommand);

			MemorableUndoableCommand mprWorkspaceStateCommand = new MemorableUndoableCommand(_mprWorkspaceState);
			mprWorkspaceStateCommand.BeginState = _mprWorkspaceState.CreateMemento();
			mprWorkspaceStateCommand.EndState = _mprWorkspaceState.InitialState;
			command.Enqueue(mprWorkspaceStateCommand);

			MemorableUndoableCommand resliceToolGraphicsStateEndCommand = new MemorableUndoableCommand(_resliceToolGraphicsState);
			resliceToolGraphicsStateEndCommand.BeginState = null;
			resliceToolGraphicsStateEndCommand.EndState = _resliceToolGraphicsState.InitialState;
			command.Enqueue(resliceToolGraphicsStateEndCommand);

			command.Execute();

			if (this.ImageViewer.CommandHistory != null)
				this.ImageViewer.CommandHistory.AddCommand(command);

			this.CanReset = false;
		}

		public bool CanReset
		{
			get { return _canReset; }
			private set
			{
				if (_canReset != value)
				{
					_canReset = value;
					EventsHelper.Fire(_canResetChanged, this, EventArgs.Empty);
				}
			}
		}

		public event EventHandler CanResetChanged
		{
			add { _canResetChanged += value; }
			remove { _canResetChanged -= value; }
		}

		private void OnSliceSetSlicerParamsChanged(object sender, EventArgs e)
		{
			// if this event fires as a result of a ResetAll operation, then CanReset is already true and this won't trigger CanResetChanged anyway
			this.CanReset = true;
		}

		private class ImageBoxesDrawable : IDrawable
		{
			public event EventHandler Drawing;
			private readonly IPhysicalWorkspace _physicalWorkspace;

			public ImageBoxesDrawable(IPhysicalWorkspace physicalWorkspace)
			{
				_physicalWorkspace = physicalWorkspace;
			}

			public void Draw()
			{
				foreach (IImageBox imageBox in _physicalWorkspace.ImageBoxes)
					imageBox.Draw();
			}
		}

		private class ResliceToolGraphicsState : IMemorable, IDisposable
		{
			private ResliceToolGroup _owner;
			private object _initialState;

			public ResliceToolGraphicsState(ResliceToolGroup owner)
			{
				_owner = owner;
				_initialState = this.CreateMemento();
			}

			public void Dispose()
			{
				_owner = null;
				_initialState = null;
			}

			public object InitialState
			{
				get { return _initialState; }
			}

			public object CreateMemento()
			{
				Dictionary<IMprStandardSliceSet, string> state = new Dictionary<IMprStandardSliceSet, string>();
				foreach (ResliceTool tool in _owner.SlaveTools)
				{
					if (tool.SliceSet != null)
					{
						if (tool.ReferenceImage != null && tool.ReferenceImage.ParentDisplaySet != null)
							state.Add(tool.SliceSet, tool.ReferenceImage.ParentDisplaySet.Description);
					}
				}
				return state;
			}

			public void SetMemento(object memento)
			{
				Dictionary<IMprStandardSliceSet, string> state = memento as Dictionary<IMprStandardSliceSet, string>;
				if (state == null)
					return;

				// build a list of display sets and the reslice tools that are defined on them
				Dictionary<string, List<ResliceTool>> toolsToUpdate = new Dictionary<string, List<ResliceTool>>();
				foreach (ResliceTool tool in _owner.SlaveTools)
				{
					if (tool.SliceSet != null && state.ContainsKey(tool.SliceSet))
					{
						string referenceDisplaySet = state[tool.SliceSet];
						if (!toolsToUpdate.ContainsKey(referenceDisplaySet))
							toolsToUpdate.Add(referenceDisplaySet, new List<ResliceTool>());
						toolsToUpdate[referenceDisplaySet].Add(tool);
					}
				}

				// now find those display sets and fix those tools
				foreach (IImageSet imageSet in _owner.ImageViewer.MprWorkspace.ImageSets)
				{
					foreach (MprDisplaySet displaySet in imageSet.DisplaySets)
					{
						if (displaySet.ImageBox != null && toolsToUpdate.ContainsKey(displaySet.Description))
						{
							foreach (ResliceTool tool in toolsToUpdate[displaySet.Description])
								tool.SetReferenceImage(displaySet.ImageBox.TopLeftPresentationImage);
						}
					}
				}
			}
		}

		private class MprWorkspaceState : IMemorable, IDisposable
		{
			private MprViewerComponent _mprViewer;
			private object _initialState;

			public MprWorkspaceState(MprViewerComponent mprViewer)
			{
				_mprViewer = mprViewer;
				_initialState = this.CreateMemento();
			}

			public void Dispose()
			{
				_mprViewer = null;
				_initialState = null;
			}

			public object InitialState
			{
				get { return _initialState; }
			}

			public object CreateMemento()
			{
				Dictionary<MprDisplaySet, MprDisplaySetMemento> state = new Dictionary<MprDisplaySet, MprDisplaySetMemento>();
				foreach (IImageSet imageSet in _mprViewer.MprWorkspace.ImageSets)
				{
					foreach (MprDisplaySet displaySet in imageSet.DisplaySets)
					{
						IMprStandardSliceSet sliceSet = displaySet.SliceSet as IMprStandardSliceSet;
						if (sliceSet != null)
							state.Add(displaySet, new MprDisplaySetMemento(displaySet));
					}
				}
				return state;
			}

			public void SetMemento(object memento)
			{
				Dictionary<MprDisplaySet, MprDisplaySetMemento> state = memento as Dictionary<MprDisplaySet, MprDisplaySetMemento>;
				if (state == null)
					return;

				foreach (IImageSet imageSet in _mprViewer.MprWorkspace.ImageSets)
				{
					foreach (MprDisplaySet displaySet in imageSet.DisplaySets)
					{
						IMprStandardSliceSet sliceSet = displaySet.SliceSet as IMprStandardSliceSet;
						if (sliceSet != null && state.ContainsKey(displaySet))
						{
							MprDisplaySetMemento mprDisplaySetMemento = state[displaySet];

							if (!sliceSet.IsReadOnly)
							{
								sliceSet.SlicerParams = mprDisplaySetMemento.SlicerParams;
							}

							if (displaySet.ImageBox != null)
							{
								displaySet.ImageBox.TopLeftPresentationImage = displaySet.PresentationImages[mprDisplaySetMemento.TopLeftSliceIndex];
							}
						}
					}
				}
			}

			private class MprDisplaySetMemento
			{
				public readonly int TopLeftSliceIndex;
				public readonly IVolumeSlicerParams SlicerParams;

				public MprDisplaySetMemento(MprDisplaySet mprDisplaySet)
				{
					if (mprDisplaySet.ImageBox != null)
						this.TopLeftSliceIndex = mprDisplaySet.ImageBox.TopLeftPresentationImageIndex;

					if (mprDisplaySet.SliceSet is IMprStandardSliceSet)
						this.SlicerParams = ((IMprStandardSliceSet) mprDisplaySet.SliceSet).SlicerParams;
				}
			}
		}
	}
}