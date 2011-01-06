#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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
