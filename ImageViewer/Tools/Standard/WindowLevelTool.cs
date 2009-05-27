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
using System.Diagnostics;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.Tools.Standard;
using ClearCanvas.ImageViewer.BaseTools;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[MouseToolButton(XMouseButtons.Right, true)]
	[DefaultMouseToolButton(XMouseButtons.Middle)]

	[MenuAction("activate", "global-menus/MenuTools/MenuStandard/MenuWindowLevel", "Select", Flags = ClickActionFlags.CheckAction)]
	[KeyboardAction("activate", "imageviewer-keyboard/ToolsStandardWindowLevel/Activate", "Select", KeyStroke = XKeys.W)]
	[MenuAction("activate", "imageviewer-contextmenu/MenuWindowLevel", "Select", Flags = ClickActionFlags.CheckAction)]
	[DropDownButtonAction("activate", "global-toolbars/ToolbarStandard/ToolbarWindowLevel", "Select", "PresetDropDownMenuModel", Flags = ClickActionFlags.CheckAction)]
	[CheckedStateObserver("activate", "Active", "ActivationChanged")]
	[TooltipValueObserver("activate", "Tooltip", "TooltipChanged")]
	[IconSet("activate", IconScheme.Colour, "Icons.WindowLevelToolSmall.png", "Icons.WindowLevelToolMedium.png", "Icons.WindowLevelToolLarge.png")]
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

		public WindowLevelTool()
			: base(SR.TooltipWindowLevel)
		{
			this.CursorToken = new CursorToken("Icons.WindowLevelToolSmall.png", this.GetType().Assembly);
			_operation = new VoiLutImageOperation(Apply);
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
			return manager != null && manager.GetLut() is IVoiLutLinear;
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

			if (this.SelectedVoiLutProvider.VoiLutManager.GetLut() is IBasicVoiLutLinear)
			{
				_memorableCommand.EndState = GetSelectedImageVoiLutManager().CreateMemento();
				UndoableCommand applicatorCommand = _applicator.ApplyToLinkedImages();
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
			IncrementWindowWithUndo(10, 0);
		}

		private void DecrementWindowWidth()
		{
			IncrementWindowWithUndo(-10, 0);
		}

		private void IncrementWindowCenter()
		{
			IncrementWindowWithUndo(0, 10);
		}

		private void DecrementWindowCenter()
		{
			IncrementWindowWithUndo(0, -10);
		}

		private void IncrementWindow(double windowIncrement, double levelIncrement)
		{
			if (!CanWindowLevel())
				return;

			CodeClock counter = new CodeClock();
			counter.Start();

			IVoiLutManager manager = this.SelectedVoiLutProvider.VoiLutManager;

			IVoiLutLinear linearLut = manager.GetLut() as IVoiLutLinear;
			IBasicVoiLutLinear standardLut = linearLut as IBasicVoiLutLinear;
			if (standardLut == null)
			{
				BasicVoiLutLinear installLut = new BasicVoiLutLinear(linearLut.WindowWidth, linearLut.WindowCenter);
				manager.InstallLut(installLut);
			}

			standardLut = manager.GetLut() as IBasicVoiLutLinear; 
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
			IVoiLutLinear selectedLut = (IVoiLutLinear)this.SelectedVoiLutProvider.VoiLutManager.GetLut();

			IVoiLutProvider provider = ((IVoiLutProvider)image);
			if (!(provider.VoiLutManager.GetLut() is IBasicVoiLutLinear))
			{
				BasicVoiLutLinear installLut = new BasicVoiLutLinear(selectedLut.WindowWidth, selectedLut.WindowCenter);
				provider.VoiLutManager.InstallLut(installLut);
			}

			IBasicVoiLutLinear lut = (IBasicVoiLutLinear)provider.VoiLutManager.GetLut();
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

			IncrementWindow(this.DeltaX * 10, this.DeltaY * 10);

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
