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
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.InteractiveGraphics;
using ClearCanvas.ImageViewer.RoiGraphics;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	public abstract class MeasurementTool : MouseImageViewerTool
	{
		private int _serialNumber;
		private InteractiveGraphicBuilder _graphicBuilder;
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

		protected abstract string RoiNameFormat { get; }

		protected abstract string CreationCommandName { get; }

		public override bool Start(IMouseInformation mouseInformation)
		{
			base.Start(mouseInformation);

			if (_graphicBuilder != null)
				return _graphicBuilder.Start(mouseInformation);

			IPresentationImage image = mouseInformation.Tile.PresentationImage;
			IOverlayGraphicsProvider provider = image as IOverlayGraphicsProvider;
			if (provider == null)
				return false;

			RoiGraphic roiGraphic = CreateRoiGraphic();

			_graphicBuilder = CreateGraphicBuilder(roiGraphic.Subject);
			_graphicBuilder.GraphicComplete += OnGraphicBuilderComplete;
			_graphicBuilder.GraphicCancelled += OnGraphicBuilderCancelled;

			_undoableCommand = new DrawableUndoableCommand(image);
			_undoableCommand.Enqueue(new InsertGraphicUndoableCommand(DecorateRoiGraphic(roiGraphic), provider.OverlayGraphics, provider.OverlayGraphics.Count));
			_undoableCommand.Name = CreationCommandName;
			_undoableCommand.Execute();

			OnRoiCreation(roiGraphic);

			roiGraphic.Suspend();
			try
			{
				if (_graphicBuilder.Start(mouseInformation))
					return true;
			}
			finally
			{
				roiGraphic.Resume(true);
			}

			this.Cancel();
			return false;
		}

		public override bool Track(IMouseInformation mouseInformation)
		{
			if (_graphicBuilder != null)
				return _graphicBuilder.Track(mouseInformation);

			return false;
		}

		public override bool Stop(IMouseInformation mouseInformation)
		{
			if (_graphicBuilder == null)
				return false;

			if (_graphicBuilder.Stop(mouseInformation))
				return true;

			_graphicBuilder.Graphic.ImageViewer.CommandHistory.AddCommand(_undoableCommand);
			_graphicBuilder = null;
			_undoableCommand = null;
			return false;
		}

		public override void Cancel()
		{
			if (_graphicBuilder == null)
				return;

			_graphicBuilder.Cancel();
		}

		public override CursorToken GetCursorToken(Point point)
		{
			if (_graphicBuilder != null)
				return _graphicBuilder.GetCursorToken(point);

			return base.GetCursorToken(point);
		}

		protected RoiGraphic CreateRoiGraphic()
		{
			//When you create a graphic from within a tool (particularly one that needs capture, like a multi-click graphic),
			//see it through to the end of creation.  It's just cleaner, not to mention that if this tool knows how to create it,
			//it should also know how to (and be responsible for) cancelling it and/or deleting it appropriately.
			IGraphic graphic = CreateGraphic();
			IAnnotationCalloutLocationStrategy strategy = CreateCalloutLocationStrategy();

			RoiGraphic roiGraphic;
			if (strategy == null)
				roiGraphic = new RoiGraphic(graphic);
			else
				roiGraphic = new RoiGraphic(graphic, strategy);

			if (!string.IsNullOrEmpty(this.RoiNameFormat))
				roiGraphic.Name = string.Format(this.RoiNameFormat, ++_serialNumber);
			else
				roiGraphic.Name = "";

			roiGraphic.State = roiGraphic.CreateFocussedSelectedState();

			return roiGraphic;
		}

		protected abstract InteractiveGraphicBuilder CreateGraphicBuilder(IGraphic subjectGraphic);

		protected abstract IGraphic CreateGraphic();

		protected  virtual IGraphic DecorateRoiGraphic(RoiGraphic roiGraphic)
		{
			// Add basic graphics context menu
			return new BasicGraphicToolsControlGraphic(roiGraphic);
		}

		protected virtual IAnnotationCalloutLocationStrategy CreateCalloutLocationStrategy()
		{
			return null;
		}

		protected virtual void OnRoiCreation(RoiGraphic roiGraphic) {}

		private void OnGraphicBuilderComplete(object sender, GraphicEventArgs e)
		{
			_graphicBuilder.GraphicComplete -= OnGraphicBuilderComplete;
			_graphicBuilder.GraphicCancelled -= OnGraphicBuilderCancelled;

			_undoableCommand = null;

			_graphicBuilder = null;
		}

		private void OnGraphicBuilderCancelled(object sender, GraphicEventArgs e)
		{
			_graphicBuilder.GraphicComplete -= OnGraphicBuilderComplete;
			_graphicBuilder.GraphicCancelled -= OnGraphicBuilderCancelled;

			_undoableCommand.Unexecute();
			_undoableCommand = null;

			_graphicBuilder = null;
		}
	}
}