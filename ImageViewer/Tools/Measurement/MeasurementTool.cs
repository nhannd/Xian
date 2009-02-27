#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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

using System.Drawing;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.InteractiveGraphics;
using ClearCanvas.ImageViewer.RoiGraphics;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	public abstract class MeasurementTool : MouseImageViewerTool
	{
		private RoiGraphic _createRoiGraphic;

		private DrawableUndoableCommand _undoableCommand;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="tooltipPrefix">The tooltip prefix, which usually describes the tool's function.</param>
		protected MeasurementTool(string tooltipPrefix)
			: base(tooltipPrefix)
		{
			this.Behaviour |= MouseButtonHandlerBehaviour.SuppressContextMenu | MouseButtonHandlerBehaviour.SuppressOnTileActivate;
		}

		protected abstract string CreationCommandName { get; }

		public override bool Start(IMouseInformation mouseInformation)
		{
			base.Start(mouseInformation);

			if (_createRoiGraphic != null)
				return _createRoiGraphic.Start(mouseInformation);

			IPresentationImage image = mouseInformation.Tile.PresentationImage;
			IOverlayGraphicsProvider provider = image as IOverlayGraphicsProvider;
			if (provider == null)
				return false;

			_createRoiGraphic = CreateRoiGraphic();

			_undoableCommand = new DrawableUndoableCommand(image);
			_undoableCommand.Enqueue(new InsertGraphicUndoableCommand(_createRoiGraphic, provider.OverlayGraphics, provider.OverlayGraphics.Count));
			_undoableCommand.Name = CreationCommandName;
			_undoableCommand.Execute();

			OnRoiCreation(_createRoiGraphic);

			if (_createRoiGraphic.Start(mouseInformation))
				return true;

			this.Cancel();
			return false;
		}

		public override bool Track(IMouseInformation mouseInformation)
		{
			if (_createRoiGraphic != null)
				return _createRoiGraphic.Track(mouseInformation);

			return false;
		}

		public override bool Stop(IMouseInformation mouseInformation)
		{
			if (_createRoiGraphic == null)
				return false;

			if (_createRoiGraphic.Stop(mouseInformation))
				return true;

			_createRoiGraphic.ImageViewer.CommandHistory.AddCommand(_undoableCommand);
			_undoableCommand = null;
			_createRoiGraphic = null;
			return false;
		}

		public override void Cancel()
		{
			if (_createRoiGraphic == null)
				return;

			_createRoiGraphic.Cancel();

			_undoableCommand.Unexecute();
			_undoableCommand = null;

			_createRoiGraphic = null;
		}

		public override CursorToken GetCursorToken(Point point)
		{
			if (_createRoiGraphic != null)
				return _createRoiGraphic.GetCursorToken(point);

			return null;
		}

		protected RoiGraphic CreateRoiGraphic()
		{
			//When you create a graphic from within a tool (particularly one that needs capture, like a multi-click graphic),
			//see it through to the end of creation.  It's just cleaner, not to mention that if this tool knows how to create it,
			//it should also know how to (and be responsible for) cancelling it and/or deleting it appropriately.
			InteractiveGraphic interactiveGraphic = CreateInteractiveGraphic();
			IAnnotationCalloutLocationStrategy strategy = CreateCalloutLocationStrategy();

			RoiGraphic roiGraphic;
			if (strategy == null)
				roiGraphic = new RoiGraphic(interactiveGraphic);
			else
				roiGraphic = new RoiGraphic(interactiveGraphic, strategy);

			roiGraphic.Name = this.ToString();
			roiGraphic.State = this.CreateCreateState(roiGraphic);

			return roiGraphic;
		}

		protected abstract InteractiveGraphic CreateInteractiveGraphic();

		protected abstract GraphicState CreateCreateState(RoiGraphic roiGraphic);

		protected virtual IAnnotationCalloutLocationStrategy CreateCalloutLocationStrategy()
		{
			return null;
		}

		protected virtual void OnRoiCreation(RoiGraphic roiGraphic)
		{
		}
	}
}
