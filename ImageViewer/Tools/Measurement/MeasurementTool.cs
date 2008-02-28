using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.InteractiveGraphics;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	public abstract class MeasurementTool<T> : MouseImageViewerTool
		where T : RoiInfo, new()
	{
		private DelayedEventPublisher _roiChangedDelayedEventPublisher;

		private RoiGraphic _createRoiGraphic;
		private InteractiveGraphic _currentChangingRoi;

		private List<IRoiAnalyzer<T>> _roiAnalyzers;

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

		public override void Initialize()
		{
			base.Initialize();

			_roiChangedDelayedEventPublisher = new DelayedEventPublisher(OnDelayedRoiChanged);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && _roiChangedDelayedEventPublisher != null)
			{
				_roiChangedDelayedEventPublisher.Dispose();
				_roiChangedDelayedEventPublisher = null;
			}

			base.Dispose(disposing);
		}

		public override bool Start(IMouseInformation mouseInformation)
		{
			base.Start(mouseInformation);

			if (_createRoiGraphic != null)
				return _createRoiGraphic.Start(mouseInformation);

			IOverlayGraphicsProvider image = mouseInformation.Tile.PresentationImage as IOverlayGraphicsProvider;
			if (image == null)
				return false;

			_createRoiGraphic = CreateRoiGraphic();
			_createRoiGraphic.RoiChanged += OnRoiChanged;

			image.OverlayGraphics.Add(_createRoiGraphic);
			OnRoiCreation(_createRoiGraphic);
			CreateAnalyzersInternal();

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

			ResetActivelyChangingRoi();

			IOverlayGraphicsProvider image = (IOverlayGraphicsProvider)mouseInformation.Tile.PresentationImage;

			InsertRemoveGraphicUndoableCommand command = InsertRemoveGraphicUndoableCommand.GetRemoveCommand(image.OverlayGraphics, _createRoiGraphic);
			command.Name = this.CreationCommandName;
			_createRoiGraphic.ImageViewer.CommandHistory.AddCommand(command);
			_createRoiGraphic = null;
			return false;
		}

		public override void Cancel()
		{
			if (_createRoiGraphic == null)
				return;

			ResetActivelyChangingRoi();
			_createRoiGraphic.RoiChanged -= OnRoiChanged;

			_createRoiGraphic.Cancel();

			// Cancel pending delayed event.
			_roiChangedDelayedEventPublisher.Cancel();

			IOverlayGraphicsProvider image = (IOverlayGraphicsProvider)_createRoiGraphic.ParentPresentationImage;
			image.OverlayGraphics.Remove(_createRoiGraphic);

			_createRoiGraphic.ParentPresentationImage.Draw();
			_createRoiGraphic = null;
		}

		public override CursorToken GetCursorToken(Point point)
		{
			if (_createRoiGraphic != null)
				return _createRoiGraphic.GetCursorToken(point);

			return null;
		}

		protected virtual RoiGraphic CreateRoiGraphic()
		{
			//When you create a graphic from within a tool (particularly one that needs capture, like a multi-click graphic),
			//see it through to the end of creation.  It's just cleaner, not to mention that if this tool knows how to create it,
			//it should also know how to (and be responsible for) cancelling it and/or deleting it appropriately.
			InteractiveGraphic interactiveGraphic = CreateInteractiveGraphic();

			RoiGraphic roiGraphic = new RoiGraphic(interactiveGraphic, true);
			roiGraphic.Name = this.ToString();

			return roiGraphic;
		}

		protected abstract InteractiveGraphic CreateInteractiveGraphic();

		protected virtual IEnumerable<IRoiAnalyzer<T>> CreateAnalyzers()
		{
			foreach (IRoiAnalyzer<T> analyzer in new RoiAnalyzerExtensionPoint<T>().CreateExtensions())
				yield return analyzer;
		}

		protected virtual void OnRoiCreation(RoiGraphic roiGraphic)
		{
		}

		protected virtual void OnRoiChanged(RoiGraphic roiGraphic)
		{
			bool active = roiGraphic.Roi.State is MoveGraphicState ||
			              roiGraphic.Roi.State is MoveControlPointGraphicState ||
			              roiGraphic.Roi.State is CreateGraphicState;

			if (!active)
			{
				// the roi is inactive, focused or selected, but not actively
				// moving or stretching; just do the calculation immediately.
				OnDelayedRoiChanged(roiGraphic);
			}
			else
			{
				_roiChangedDelayedEventPublisher.Publish(roiGraphic, EventArgs.Empty);
				SetCurrentChangingRoi(roiGraphic.Roi);
				Analyze(roiGraphic, RoiAnalysisMode.Responsive);
			}
		}

		protected virtual void OnDelayedRoiChanged(RoiGraphic roiGraphic)
		{
			Analyze(roiGraphic, RoiAnalysisMode.Normal);
			roiGraphic.Draw();
		}

		private void Analyze(RoiGraphic roiGraphic, RoiAnalysisMode analysisMode)
		{
			string text = null;

			if (_roiAnalyzers != null && _roiAnalyzers.Count > 0)
			{
				T roiInfo = new T();
				roiInfo.Initialize(roiGraphic.Roi);
				roiInfo.Mode = analysisMode;

				if (roiInfo.IsValid())
				{
					StringBuilder builder = new StringBuilder();

					foreach (IRoiAnalyzer<T> analyzer in _roiAnalyzers)
					{
						string analysis = analyzer.Analyze(roiInfo);
						if (!String.IsNullOrEmpty(analysis))
							builder.AppendLine(analysis);
					}

					text = builder.ToString();
				}
			}

			roiGraphic.Callout.Text = text ?? SR.ToolsMeasurementNoValue;
		}

		private void SetCurrentChangingRoi(InteractiveGraphic roi)
		{
			if (_currentChangingRoi == null)
			{
				_currentChangingRoi = roi;
				_currentChangingRoi.StateChanged += OnRoiStateChanged;
			}
		}

		private void ResetActivelyChangingRoi()
		{
			if (_currentChangingRoi != null)
			{
				_currentChangingRoi.StateChanged -= OnRoiStateChanged;
				_currentChangingRoi = null;
			}
		}

		private void OnRoiStateChanged(object sender, GraphicStateChangedEventArgs e)
		{
			ResetActivelyChangingRoi();

			// We use the state change to force analysis of the currently changing
			// Roi because it is otherwise possible (if the user clicked quickly again
			// to start creating a new Roi) for the currently roi's text to fail to update.
			// We can't do this in the Stop() method because it doesn't work for Rois
			// that are not in the create state.
			_roiChangedDelayedEventPublisher.PublishNow();
		}

		private void OnDelayedRoiChanged(object sender, EventArgs e)
		{
			OnDelayedRoiChanged(sender as RoiGraphic);
		}

		private void OnRoiChanged(object sender, EventArgs e)
		{
			OnRoiChanged(sender as RoiGraphic);
		}

		private void CreateAnalyzersInternal()
		{
			if (_roiAnalyzers != null)
				return;

			_roiAnalyzers = new List<IRoiAnalyzer<T>>(CreateAnalyzers());
		}
	}
}
