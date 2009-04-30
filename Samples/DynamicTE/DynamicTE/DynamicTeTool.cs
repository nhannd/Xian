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
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.Tools.ImageProcessing.DynamicTe
{
	[MenuAction("activate", "global-menus/MenuTools/MyTools/DynamicTe", Flags = ClickActionFlags.CheckAction)]
	[ButtonAction("activate", "global-toolbars/MyTools/DynamicTe", Flags = ClickActionFlags.CheckAction)]
	[Tooltip("activate", "DynamicTE")]
	[IconSet("activate", IconScheme.Colour, "Icons.DynamicTeToolSmall.png", "Icons.DynamicTeToolMedium.png", "Icons.DynamicTeToolLarge.png")]
	[ClickHandler("activate", "Select")]
	[MouseToolButton(XMouseButtons.Left, false)]
	[CheckedStateObserver("activate", "Active", "ActivationChanged")]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class DynamicTeTool : MouseImageViewerTool, IImageOperation
	{
		private MemorableUndoableCommand _command;
		private ImageOperationApplicator _applicator;

		/// <summary>
		/// Default constructor.  A no-args constructor is required by the
		/// framework.  Do not remove.
		/// </summary>
		public DynamicTeTool()
		{
		}

		private IDynamicTeProvider SelectedDynamicTeProvider
		{
			get
			{
				if (this.SelectedPresentationImage != null)
					return this.SelectedPresentationImage as IDynamicTeProvider;
				else
					return null;
			}
		}

		/// <summary>
		/// Called by the framework to initialize this tool.
		/// </summary>
		public override void Initialize()
		{
			base.Initialize();

		}

		private void CaptureBeginState()
		{
			if (this.SelectedPresentationImage == null)
				return;

			if (!(this.SelectedPresentationImage is IDynamicTeProvider))
				return;

			_applicator = new ImageOperationApplicator(this.SelectedPresentationImage, this);
			_command = new MemorableUndoableCommand(_applicator);
			_command.Name = "Dynamic Te";
			_command.BeginState = _applicator.CreateMemento();
		}

		private void CaptureEndState()
		{
			if (this.SelectedPresentationImage == null)
				return;

			if (!(this.SelectedPresentationImage is IDynamicTeProvider))
				return;

			if (_command == null)
				return;

			_applicator.ApplyToLinkedImages();

			_command.EndState = _applicator.CreateMemento();

			// If the state hasn't changed since MouseDown just return
			if (_command.EndState.Equals(_command.BeginState))
			{
				_command = null;
				return;
			}

			this.Context.Viewer.CommandHistory.AddCommand(_command);
		}

		/// <summary>
		/// Called by framework when the assigned mouse button was pressed.
		/// </summary>
		/// <param name="e">Mouse event args</param>
		/// <returns>True if the event was handled, false otherwise</returns>
		public override bool Start(IMouseInformation mouseInformation)
		{
			base.Start(mouseInformation);
			CaptureBeginState();

			return true;
		}

		/// <summary>
		/// Called by the framework as the mouse moves while the assigned mouse button
		/// is pressed.
		/// </summary>
		/// <param name="e">Mouse event args</param>
		/// <returns>True if the event was handled, false otherwise</returns>
		public override bool Track(IMouseInformation mouseInformation)
		{
			base.Track(mouseInformation);

			IDynamicTeProvider dynamicTeProvider = this.SelectedPresentationImage as IDynamicTeProvider;

			if (dynamicTeProvider == null)
				return false;

			double timeDelta = this.DeltaX * 0.25;
			dynamicTeProvider.DynamicTe.Te += timeDelta;
			dynamicTeProvider.Draw();

			return true;
		}

		/// <summary>
		/// Called by the framework when the assigned mouse button is released.
		/// </summary>
		/// <param name="e">Mouse event args</param>
		/// <returns>True if the event was handled, false otherwise</returns>
		public override bool Stop(IMouseInformation mouseInformation)
		{
			base.Stop(mouseInformation);

			CaptureEndState();

			return false;
		}

		#region IImageOperation Members

		IMemorable IUndoableOperation<IPresentationImage>.GetOriginator(IPresentationImage image)
		{
			IDynamicTeProvider provider = image as IDynamicTeProvider;
			if (provider == null)
				return null;

			return provider as IMemorable;
		}

		bool IUndoableOperation<IPresentationImage>.AppliesTo(IPresentationImage image)
		{
			return image is IDynamicTeProvider;
		}

		void IUndoableOperation<IPresentationImage>.Apply(IPresentationImage image)
		{
			IDynamicTeProvider provider = image as IDynamicTeProvider;

			if (provider != null)
				provider.DynamicTe.Te = this.SelectedDynamicTeProvider.DynamicTe.Te;
		}

		#endregion
	}
}
