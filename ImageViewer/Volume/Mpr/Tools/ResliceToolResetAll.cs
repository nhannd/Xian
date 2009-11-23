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
		private bool _canReset = false;

		private void InitializeResetAll()
		{
			_canReset = false;
			foreach (IImageSet imageSet in this.ImageViewer.MprWorkspace.ImageSets)
			{
				foreach (MprDisplaySet displaySet in imageSet.DisplaySets)
				{
					IMprStandardSliceSet sliceSet = displaySet.SliceSet as IMprStandardSliceSet;
					if (sliceSet != null && !sliceSet.IsReadOnly)
						sliceSet.SlicerParamsChanged += OnSliceSetSlicerParamsChanged;
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
						sliceSet.SlicerParamsChanged -= OnSliceSetSlicerParamsChanged;
				}
			}
		}

		public void ResetAll()
		{
			DrawableUndoableCommand command = new DrawableUndoableCommand(this.ImageViewer.PhysicalWorkspace);
			command.Name = SR.CommandMprReset;

			MemorableUndoableCommand toolGroupStateCommand = new MemorableUndoableCommand(this.ToolGroupState);
			toolGroupStateCommand.BeginState = this.ToolGroupState.CreateMemento();
			toolGroupStateCommand.EndState = this.InitialToolGroupStateMemento;
			command.Enqueue(toolGroupStateCommand);

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
	}
}