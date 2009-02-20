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
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	public abstract class MeasurementTool<T> : MouseImageViewerTool
		where T : RoiInfo, new()
	{
		private Type _interactiveGraphicType = null;
		private DelayedEventPublisher _roiChangedDelayedEventPublisher;

		private RoiGraphic _createRoiGraphic;
		private InteractiveGraphic _currentChangingRoi;

		private DrawableUndoableCommand _undoableCommand;

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

			base.ImageViewer.EventBroker.CloneCreated += OnCloneCreated;
			_roiChangedDelayedEventPublisher = new DelayedEventPublisher(OnDelayedRoiChanged);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_roiChangedDelayedEventPublisher != null)
				{
					_roiChangedDelayedEventPublisher.Dispose();
					_roiChangedDelayedEventPublisher = null;
				}
				
				base.ImageViewer.EventBroker.CloneCreated -= OnCloneCreated;
			}

			base.Dispose(disposing);
		}

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
			_createRoiGraphic.RoiChanged += OnRoiChanged;

			_undoableCommand = new DrawableUndoableCommand(image);
			_undoableCommand.Enqueue(new InsertGraphicUndoableCommand(_createRoiGraphic, provider.OverlayGraphics, provider.OverlayGraphics.Count));
			_undoableCommand.Name = CreationCommandName;
			_undoableCommand.Execute();

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

			_createRoiGraphic.ImageViewer.CommandHistory.AddCommand(_undoableCommand);
			_undoableCommand = null;
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
			_interactiveGraphicType = interactiveGraphic.GetType();

			IRoiCalloutLocationStrategy strategy = CreateCalloutLocationStrategy();

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

		protected virtual IRoiCalloutLocationStrategy CreateCalloutLocationStrategy()
		{
			return null;
		}

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
			//bool active = false;
			bool active = roiGraphic.State is MoveGraphicState ||
						  roiGraphic.State is MoveControlPointGraphicState ||
						  roiGraphic.State is CreateGraphicState;

			//return;

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

					try
					{
						foreach (IRoiAnalyzer<T> analyzer in _roiAnalyzers)
						{
							string analysis = analyzer.Analyze(roiInfo);
							if (!String.IsNullOrEmpty(analysis))
								builder.AppendLine(analysis);
						}

						text = builder.ToString();
					}
					catch (Exception e)
					{
						Platform.Log(LogLevel.Error, e);
						text = SR.MessageRoiAnalysisError;
					}
				}
			}

			roiGraphic.Callout.Text = text ?? SR.ToolsMeasurementNoValue;
		}

		private void SetCurrentChangingRoi(InteractiveGraphic roi)
		{
			if (_currentChangingRoi == null)
			{
				_currentChangingRoi = roi;
				//_currentChangingRoi.StateChanged += OnRoiStateChanged;
			}
		}

		private void ResetActivelyChangingRoi()
		{
			if (_currentChangingRoi != null)
			{
				//_currentChangingRoi.StateChanged -= OnRoiStateChanged;
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

		private void OnCloneCreated(object sender, CloneCreatedEventArgs e)
		{
			if (e.Clone is RoiGraphic)
			{
				RoiGraphic roiGraphic = (RoiGraphic)e.Clone;
				if (roiGraphic.Roi.GetType() == _interactiveGraphicType)
					roiGraphic.RoiChanged += OnRoiChanged;
			}
		}
	}
}
