#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	public partial class ProgressGraphic : CompositeGraphic
	{
		private readonly ProgressCompositeGraphic _graphics;
		private readonly ProgressBarGraphicStyle _progressBarStyle;
		private readonly BackgroundTask _task;

		private bool _autoClose;
		private string _progressMessage;
		private bool _enableCancel;

		public ProgressGraphic(BackgroundTask task, bool autoClose, ProgressBarGraphicStyle progressBarStyle)
		{
			_task = task;
			_autoClose = autoClose;
			_progressMessage = string.Empty;
			_progressBarStyle = progressBarStyle;
			_enableCancel = _task != null ? _task.SupportsCancel : false;

			base.Graphics.Add(_graphics = new ProgressCompositeGraphic(_progressBarStyle));
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_task != null)
				{
					_task.ProgressUpdated -= OnTaskProgressUpdated;
					_task.Terminated -= OnTaskTerminated;
				}
			}

			base.Dispose(disposing);
		}

		/// <summary>
		/// Starts the <see cref="BackgroundTask"/>.
		/// </summary>
		public void Run()
		{
			if (_task != null)
			{
				_task.ProgressUpdated += OnTaskProgressUpdated;
				_task.Terminated += OnTaskTerminated;

				if (_task.IsRunning)
					UpdateProgress(_task.LastBackgroundTaskProgress);
				else
					_task.Run();
			}
		}

		/// <summary>
		/// Cancels the <see cref="BackgroundTask"/>.
		/// </summary>
		public void Cancel()
		{
			if (_task != null && _task.IsRunning)
			{
				if (_task.SupportsCancel)
				{
					_autoClose = true;
					_task.RequestCancel();
				}
			}
		}

		/// <summary>
		/// &quot;Closes&quot; the progress graphic by removing it from its parent and disposing the graphic.
		/// </summary>
		/// <remarks>
		/// This method will invoke the graphic's <see cref="IDrawable.Draw"/> method, so do not call it from a draw routine in the same scene graph!
		/// </remarks>
		public void Close()
		{
			if (base.ParentGraphic != null)
			{
				IDrawable parent = base.ParentGraphic;
				((CompositeGraphic)base.ParentGraphic).Graphics.Remove(this);
				parent.Draw();
			}
			this.Dispose();
		}

		public override void OnDrawing()
		{
			if (base.ParentPresentationImage != null)
			{
				// We do graphics on a separate composite so that we can constantly fix our spatial transformation to center the contents in the client area
				SpatialTransform transform = base.SpatialTransform;

				float scale = 1 / base.ParentGraphic.SpatialTransform.CumulativeScale;

				PointF srcXAxis = new PointF(1, 0);
				PointF srcOrigin = new PointF(0, 0);
				PointF dstOrigin = base.ParentGraphic.SpatialTransform.ConvertToDestination(srcOrigin);

				// figure out where the positive x axis went to determine the cumulative rotation
				transform.RotationXY = -(int)Vector.SubtendedAngle(base.ParentGraphic.SpatialTransform.ConvertToDestination(srcXAxis) - new SizeF(dstOrigin), srcOrigin, srcXAxis);

				_graphics.SpatialTransform.TranslationX = (base.ParentPresentationImage.ClientRectangle.Width - _graphics.Width) / 2f -dstOrigin.X;
				_graphics.SpatialTransform.TranslationY = (base.ParentPresentationImage.ClientRectangle.Height - _graphics.Height) / 2f - dstOrigin.Y;

				transform.Scale = scale;
			}

			base.OnDrawing();
		}

		#region Task EventHandler

		private void UpdateProgress(BackgroundTaskProgressEventArgs e)
		{
			if (e == null)
				return;

			_graphics.Progress = e.Progress.Percent/100f;
			_graphics.Text = _progressMessage = e.Progress.Message;
			this.Draw();
		}

		private void OnTaskProgressUpdated(object sender, BackgroundTaskProgressEventArgs e)
		{
			UpdateProgress(e);
		}

		private void OnTaskTerminated(object sender, BackgroundTaskTerminatedEventArgs e)
		{
			if (_task != null)
			{
				_task.ProgressUpdated -= OnTaskProgressUpdated;
				_task.Terminated -= OnTaskTerminated;
			}

			if (_autoClose && e.Reason != BackgroundTaskTerminatedReason.Exception)
			{
				this.Close();
			}
			else
			{
				_enableCancel = true;
				switch (e.Reason)
				{
					case BackgroundTaskTerminatedReason.Exception:
						Close();
						break;
					case BackgroundTaskTerminatedReason.Completed:
					case BackgroundTaskTerminatedReason.Cancelled:
					default:
						_graphics.Progress = 1f;
						_graphics.Text = _progressMessage;
						this.Draw();
						break;
				}
			}
		}

		#endregion

		#region Static Helpers

		/// <summary>
		/// Creates and displays a progress graphic.
		/// </summary>
		/// <remarks>
		/// This method will invoke the graphic's <see cref="IDrawable.Draw"/> method, so do not call it from a draw routine in the same scene graph!
		/// </remarks>
		/// <param name="task">The <see cref="BackgroundTask"/> to execute.</param>
		/// <param name="parentCollection">The graphics collection on which the progress graphic should be shown.</param>
		public static void Show(BackgroundTask task, GraphicCollection parentCollection)
		{
			Show(task, parentCollection, false, ProgressBarGraphicStyle.Blocks);
		}

		/// <summary>
		/// Creates and displays a progress graphic.
		/// </summary>
		/// <remarks>
		/// This method will invoke the graphic's <see cref="IDrawable.Draw"/> method, so do not call it from a draw routine in the same scene graph!
		/// </remarks>
		/// <param name="task">The <see cref="BackgroundTask"/> to execute.</param>
		/// <param name="parentCollection">The graphics collection on which the progress graphic should be shown.</param>
		/// <param name="autoClose">A value indicating whether or not the progress graphic should be automatically removed when the task is terminated.</param>
		public static void Show(BackgroundTask task, GraphicCollection parentCollection, bool autoClose)
		{
			Show(task, parentCollection, autoClose, ProgressBarGraphicStyle.Blocks);
		}

		/// <summary>
		/// Creates and displays a progress graphic.
		/// </summary>
		/// <remarks>
		/// This method will invoke the graphic's <see cref="IDrawable.Draw"/> method, so do not call it from a draw routine in the same scene graph!
		/// </remarks>
		/// <param name="task">The <see cref="BackgroundTask"/> to execute.</param>
		/// <param name="parentCollection">The graphics collection on which the progress graphic should be shown.</param>
		/// <param name="autoClose">A value indicating whether or not the progress graphic should be automatically removed when the task is terminated.</param>
		/// <param name="progressBarGraphicStyle">The style of the progress bar.</param>
		public static void Show(BackgroundTask task, GraphicCollection parentCollection, bool autoClose, ProgressBarGraphicStyle progressBarGraphicStyle)
		{
			ProgressGraphic progressGraphic = new ProgressGraphic(task, autoClose, progressBarGraphicStyle);
			parentCollection.Add(progressGraphic);
			progressGraphic.Run();
			progressGraphic.Draw();
		}

		#endregion
	}
}