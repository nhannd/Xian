using System;
using System.Collections.Generic;
using System.Drawing;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.InteractiveGraphics;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	public abstract class MeasurementTool : MouseImageViewerTool
	{
		private RoiGraphic _roiGraphic;
		private List<IRoiAnalyzer> _roiAnalyzers;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="tooltipPrefix">The tooltip prefix, which usually describes the tool's function.</param>
		protected MeasurementTool(string tooltipPrefix)
			: base(tooltipPrefix)
		{
			this.Behaviour = MouseButtonHandlerBehaviour.SuppressContextMenu | MouseButtonHandlerBehaviour.SuppressOnTileActivate;
		}

		protected abstract string CreationCommandName { get; }

		public override bool Start(IMouseInformation mouseInformation)
		{
			base.Start(mouseInformation);

			IOverlayGraphicsProvider image = mouseInformation.Tile.PresentationImage as IOverlayGraphicsProvider;

			if (image == null)
				return false;

			if (_roiGraphic != null)
				return _roiGraphic.Start(mouseInformation);

			//When you create a graphic from within a tool (particularly one that needs capture, like a multi-click graphic),
			//see it through to the end of creation.  It's just cleaner, not to mention that if this tool knows how to create it,
			//it should also know how to (and be responsible for) cancelling it and/or deleting it appropriately.
			InteractiveGraphic interactiveGraphic = CreateInteractiveGraphic();

			_roiGraphic = new RoiGraphic(interactiveGraphic, true);

			image.OverlayGraphics.Add(_roiGraphic);
			_roiGraphic.RoiChanged += OnRoiChanged;

			OnRoiCreation(_roiGraphic);
			CreateAnalyzersInternal();

			if (_roiGraphic.Start(mouseInformation))
				return true;

			this.Cancel();
			return false;
		}



		public override bool Track(IMouseInformation mouseInformation)
		{
			if (_roiGraphic != null)
				return _roiGraphic.Track(mouseInformation);

			return false;
		}

		public override bool Stop(IMouseInformation mouseInformation)
		{
			if (_roiGraphic != null)
			{
				if (_roiGraphic.Stop(mouseInformation))
				{
					IOverlayGraphicsProvider image = (IOverlayGraphicsProvider)mouseInformation.Tile.PresentationImage;

					InsertRemoveGraphicUndoableCommand command = InsertRemoveGraphicUndoableCommand.GetRemoveCommand(image.OverlayGraphics, _roiGraphic);
					command.Name = this.CreationCommandName;
					_roiGraphic.ImageViewer.CommandHistory.AddCommand(command);
					return true;
				}
			}

			_roiGraphic = null;
			return false;
		}

		public override void Cancel()
		{
			if (_roiGraphic != null)
				_roiGraphic.Cancel();

			IOverlayGraphicsProvider image = _roiGraphic.ParentPresentationImage as IOverlayGraphicsProvider;
			image.OverlayGraphics.Remove(_roiGraphic);

			_roiGraphic.ParentPresentationImage.Draw();
			_roiGraphic = null;
		}

		public override CursorToken GetCursorToken(Point point)
		{
			if (_roiGraphic != null)
				return _roiGraphic.GetCursorToken(point);

			return null;
		}

		protected virtual void OnRoiCreation(RoiGraphic roiGraphic)
		{
		}

		protected virtual void OnRoiChanged(RoiGraphic roiGraphic)
		{
			if (_roiAnalyzers == null)
				return;

			string calloutText = "";


			foreach (IRoiAnalyzer analyzer in _roiAnalyzers)
			{
				string analysis = analyzer.Analyze(roiGraphic);

				if (analysis != String.Empty)
					calloutText += analysis + "\n";
			}

			roiGraphic.Callout.Text = calloutText;
		}

		protected virtual object[] CreateAnalyzers()
		{
			return null;
		}

		protected abstract InteractiveGraphic CreateInteractiveGraphic();

		private void OnRoiChanged(object sender, EventArgs e)
		{
			RoiGraphic roiGraphic = sender as RoiGraphic;
			OnRoiChanged(roiGraphic);
		}

		private void CreateAnalyzersInternal()
		{
			object[] analyzers = CreateAnalyzers();

			// No analyzers could be found
			if (analyzers == null)
				return;

			_roiAnalyzers = new List<IRoiAnalyzer>();
			
			foreach (object analyzer in analyzers)
			{
				IRoiAnalyzer roiAnalyzer = analyzer as IRoiAnalyzer;

				Platform.CheckForInvalidCast(roiAnalyzer, "analyzer", "IRoiAnalyzer");

				_roiAnalyzers.Add(roiAnalyzer);
			}
		}
	}
}