using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.InputManagement;
using System.Diagnostics;
using ClearCanvas.ImageViewer.BaseTools;

namespace ClearCanvas.ImageViewer.Tools.ImageProcessing.DynamicTe
{
	[MenuAction("activate", "global-menus/MenuTools/MenuToolsMyTools/TeMappingTool", Flags = ClickActionFlags.CheckAction)]
	[ButtonAction("activate", "global-toolbars/ToolbarMyTools/TeMappingTool", Flags = ClickActionFlags.CheckAction)]
	[Tooltip("activate", "Dynamic Te")]
	[IconSet("activate", IconScheme.Colour, "Icons.DynamicTeToolSmall.png", "Icons.DynamicTeToolMedium.png", "Icons.DynamicTeToolLarge.png")]
	[ClickHandler("activate", "Select")]
	[MouseToolButton(XMouseButtons.Left, false)]
	[CheckedStateObserver("activate", "Active", "ActivationChanged")]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class DynamicTeTool : MouseImageViewerTool
	{
		private UndoableCommand _command;
		private DynamicTeApplicator _applicator;

		/// <summary>
		/// Default constructor.  A no-args constructor is required by the
		/// framework.  Do not remove.
		/// </summary>
		public DynamicTeTool()
		{
			// TODO: to change the mouse button that this tool is assigned to,
			// change the value passed to the base constructor

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

			_applicator = new DynamicTeApplicator(this.SelectedPresentationImage);
			_command = new UndoableCommand(_applicator);
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
	}
}
