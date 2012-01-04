#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Diagnostics;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.Tools.Standard;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Tools.Standard.Configuration;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[MouseToolButton(XMouseButtons.Right, true)]
	[DefaultMouseToolButton(XMouseButtons.Middle)]

	[MenuAction("activate", "global-menus/MenuTools/MenuStandard/MenuWindowLevel", "Select", Flags = ClickActionFlags.CheckAction)]
	[MenuAction("activate", "imageviewer-contextmenu/MenuWindowLevel", "Select", Flags = ClickActionFlags.CheckAction)]
	[DropDownButtonAction("activate", "global-toolbars/ToolbarStandard/ToolbarWindowLevel", "Select", "PresetDropDownMenuModel", Flags = ClickActionFlags.CheckAction, KeyStroke = XKeys.W)]
	[CheckedStateObserver("activate", "Active", "ActivationChanged")]
	[TooltipValueObserver("activate", "Tooltip", "TooltipChanged")]
	[MouseButtonIconSet("activate", "Icons.WindowLevelToolSmall.png", "Icons.WindowLevelToolMedium.png", "Icons.WindowLevelToolLarge.png")]
	[GroupHint("activate", "Tools.Image.Manipulation.Lut.WindowLevel")]

	[KeyboardAction("incrementwindowwidth", "imageviewer-keyboard/ToolsStandardWindowLevel/IncrementWindowWidth", "IncrementWindowWidth", KeyStroke = XKeys.Right)]
	[KeyboardAction("decrementwindowwidth", "imageviewer-keyboard/ToolsStandardWindowLevel/DecrementWindowWidth", "DecrementWindowWidth", KeyStroke = XKeys.Left)]
	[KeyboardAction("incrementwindowcenter", "imageviewer-keyboard/ToolsStandardWindowLevel/IncrementWindowCenter", "IncrementWindowCenter", KeyStroke = XKeys.Up)]
	[KeyboardAction("decrementwindowcenter", "imageviewer-keyboard/ToolsStandardWindowLevel/DecrementWindowCenter", "DecrementWindowCenter", KeyStroke = XKeys.Down)]

	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public partial class WindowLevelTool : MouseImageViewerTool
	{
		private readonly VoiLutImageOperation _operation;
		private MemorableUndoableCommand _memorableCommand;
		private ImageOperationApplicator _applicator;
		private ToolModalityBehaviorHelper _toolBehavior;

		public WindowLevelTool()
			: base(SR.TooltipWindowLevel)
		{
			this.CursorToken = new CursorToken("Icons.WindowLevelToolSmall.png", this.GetType().Assembly);
			_operation = new VoiLutImageOperation(Apply);
		}

		public override void Initialize()
		{
			base.Initialize();

			_toolBehavior = new ToolModalityBehaviorHelper(ImageViewer);
		}

		public override event EventHandler TooltipChanged
		{
			add { base.TooltipChanged += value; }
			remove { base.TooltipChanged -= value; }
		}

		private IVoiLutManager GetSelectedImageVoiLutManager()
		{
			return _operation.GetOriginator(this.SelectedPresentationImage) as IVoiLutManager;
		}

		private bool CanWindowLevel()
		{
			IVoiLutManager manager = GetSelectedImageVoiLutManager();
			return manager != null && manager.Enabled && manager.VoiLut is IVoiLutLinear;
		}

		private void CaptureBeginState()
		{
			if (!CanWindowLevel())
				return;

			IVoiLutManager originator = GetSelectedImageVoiLutManager();
			_applicator = new ImageOperationApplicator(this.SelectedPresentationImage, _operation);
			_memorableCommand = new MemorableUndoableCommand(originator);
			_memorableCommand.BeginState = originator.CreateMemento();
		}

		private void CaptureEndState()
		{
			if (!CanWindowLevel() || _memorableCommand == null)
				return;

			if (this.SelectedVoiLutProvider.VoiLutManager.VoiLut is IBasicVoiLutLinear)
			{
				_memorableCommand.EndState = GetSelectedImageVoiLutManager().CreateMemento();
				UndoableCommand applicatorCommand = _toolBehavior.Behavior.SelectedImageWindowLevelTool ? null : _applicator.ApplyToLinkedImages();
				DrawableUndoableCommand historyCommand = new DrawableUndoableCommand(this.SelectedPresentationImage);

				if (!_memorableCommand.EndState.Equals(_memorableCommand.BeginState))
					historyCommand.Enqueue(_memorableCommand);
				if (applicatorCommand != null)
					historyCommand.Enqueue(applicatorCommand);

				if (historyCommand.Count > 0)
				{
					historyCommand.Name = SR.CommandWindowLevel;
					this.Context.Viewer.CommandHistory.AddCommand(historyCommand);
				}
			}
		}

		private void IncrementWindowWidth()
		{
			IncrementWindowWithUndo(this.CurrentSensitivity, 0);
		}

		private void DecrementWindowWidth()
		{
			IncrementWindowWithUndo(-this.CurrentSensitivity, 0);
		}

		private void IncrementWindowCenter()
		{
			IncrementWindowWithUndo(0, this.CurrentSensitivity);
		}

		private void DecrementWindowCenter()
		{
			IncrementWindowWithUndo(0, -this.CurrentSensitivity);
		}

		private void IncrementWindow(double windowIncrement, double levelIncrement)
		{
			if (!CanWindowLevel())
				return;

			CodeClock counter = new CodeClock();
			counter.Start();

			IVoiLutManager manager = this.SelectedVoiLutProvider.VoiLutManager;

			IVoiLutLinear linearLut = manager.VoiLut as IVoiLutLinear;
			IBasicVoiLutLinear standardLut = linearLut as IBasicVoiLutLinear;
			if (standardLut == null)
			{
				BasicVoiLutLinear installLut = new BasicVoiLutLinear(linearLut.WindowWidth, linearLut.WindowCenter);
				manager.InstallVoiLut(installLut);
			}

			standardLut = manager.VoiLut as IBasicVoiLutLinear; 
			standardLut.WindowWidth += windowIncrement;
			standardLut.WindowCenter += levelIncrement;
			this.SelectedVoiLutProvider.Draw();

			counter.Stop();

			string str = String.Format("WindowLevel: {0}\n", counter.ToString());
			Trace.Write(str);
		}

		private void IncrementWindowWithUndo(double windowIncrement, double levelIncrement)
		{
			this.CaptureBeginState();
			this.IncrementWindow(windowIncrement, levelIncrement);
			this.CaptureEndState();
		}

		private void Apply(IPresentationImage image)
		{
			IVoiLutLinear selectedLut = (IVoiLutLinear)this.SelectedVoiLutProvider.VoiLutManager.VoiLut;

			IVoiLutProvider provider = ((IVoiLutProvider)image);
			if (!(provider.VoiLutManager.VoiLut is IBasicVoiLutLinear))
			{
				BasicVoiLutLinear installLut = new BasicVoiLutLinear(selectedLut.WindowWidth, selectedLut.WindowCenter);
				provider.VoiLutManager.InstallVoiLut(installLut);
			}

			IBasicVoiLutLinear lut = (IBasicVoiLutLinear)provider.VoiLutManager.VoiLut;
			lut.WindowWidth = selectedLut.WindowWidth;
			lut.WindowCenter = selectedLut.WindowCenter;
		}

		public override bool Start(IMouseInformation mouseInformation)
		{
			base.Start(mouseInformation);

			CaptureBeginState();

			return true;
		}

		public override bool Track(IMouseInformation mouseInformation)
		{
			base.Track(mouseInformation);

			double sensitivity = this.CurrentSensitivity;
			IncrementWindow(this.DeltaX * sensitivity, this.DeltaY * sensitivity);

			return true;
		}

		public override bool Stop(IMouseInformation mouseInformation)
		{
			base.Stop(mouseInformation);

			this.CaptureEndState();

			return false;
		}

		public override void Cancel()
		{
			this.CaptureEndState();
		}
	}
}
