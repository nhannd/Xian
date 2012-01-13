#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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
	[VisibleStateObserver("resetAll", "Visible", "VisibleChanged")]
	[IconSet("resetAll", "Icons.ResetAllToolSmall.png", "Icons.ResetAllToolMedium.png", "Icons.ResetAllToolLarge.png")]
	[Tooltip("resetAll", "TooltipResetAll")]
	[GroupHint("resetAll", "Tools.Volume.MPR.ResetSlicing")]
	partial class ResliceToolGroup
	{
		private event EventHandler _canResetChanged;
		private bool _canReset = false;

		private void InitializeResetAll()
		{
			if (this.ImageViewer == null)
				return;

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
			if (this.ImageViewer == null)
				return;

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