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

using System.Threading;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	/// <summary>
	/// A dialog-like graphic for displaying the current progress of some abstract operation in a scene graph.
	/// </summary>
	public partial class ProgressGraphic : CompositeGraphic
	{
		private readonly ProgressBarGraphicStyle _progressBarStyle;
		private readonly ProgressCompositeGraphic _graphics;
		private IProgressGraphicProgressProvider _progressProvider;
		private SynchronizationContext _synchronizationContext;
		private BackgroundTask _updateTask;

		private readonly bool _autoClose;
		private int _animationTick = 100;

		/// <summary>
		/// Initializes a new <see cref="ProgressGraphic"/>.
		/// </summary>
		/// <param name="progressProvider">The provider of progress information for which the <see cref="ProgressGraphic"/> will display updates.</param>
		/// <param name="autoClose">A value indicating whether or not the <see cref="ProgressGraphic"/> should automatically remove and dispose itself when the progress provider reports task completion or cancellation.</param>
		/// <param name="progressBarStyle">The style of progress bar to be displayed.</param>
		public ProgressGraphic(IProgressGraphicProgressProvider progressProvider, bool autoClose, ProgressBarGraphicStyle progressBarStyle)
		{
			_progressProvider = progressProvider;
			_autoClose = autoClose;
			_progressBarStyle = progressBarStyle;

			base.Graphics.Add(_graphics = new ProgressCompositeGraphic(_progressBarStyle));
		}

		/// <summary>
		/// Releases all resources used by this <see cref="ProgressGraphic"/>.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				_progressProvider = null;
				_synchronizationContext = null;

				if (_updateTask != null)
				{
					_updateTask.RequestCancel();
					_updateTask = null;
				}
			}

			base.Dispose(disposing);
		}

		/// <summary>
		/// Gets or sets the delay in milliseconds between progress updates and hence calls to <see cref="Graphic.Draw"/>.
		/// </summary>
		public int AnimationTick
		{
			get { return _animationTick; }
			set { _animationTick = value; }
		}

		/// <summary>
		/// Gets a value indicating whether or not the <see cref="ProgressGraphic"/> will remove and dispose itself when the progress provider reports task completion or cancellation.
		/// </summary>
		public bool AutoClose
		{
			get { return _autoClose; }
		}

		/// <summary>
		/// Forces the <see cref="ProgressGraphic"/> to remove and dispose itself immediately.
		/// </summary>
		/// <remarks>
		/// Calling this method will invoke <see cref="IDrawable.Draw"/> on the scene graph, so do not call this method from a drawing operation.
		/// </remarks>
		public void Close()
		{
			if (base.ParentGraphic != null)
			{
				IDrawable parent = base.ParentGraphic;
				((CompositeGraphic) base.ParentGraphic).Graphics.Remove(this);
				parent.Draw();
			}
			this.Dispose();
		}

		/// <summary>
		/// Called by the framework just before the <see cref="ProgressGraphic"/> is rendered.
		/// </summary>
		public override void OnDrawing()
		{
			if (_synchronizationContext == null)
				_synchronizationContext = SynchronizationContext.Current;

			if (_progressProvider != null)
			{
				float progress;
				string message;

				if (_progressProvider.IsRunning(out progress, out message))
				{
					if (_updateTask == null)
					{
						_updateTask = new BackgroundTask(UpdateProgressBar, true, null);
						_updateTask.Terminated += OnTaskTerminated;
						_updateTask.Run();
					}
				}
				else
				{
					_progressProvider = null;

					if (_updateTask != null)
					{
						_updateTask.RequestCancel();
						_updateTask = null;
					}

					if (_autoClose)
					{
						_synchronizationContext.Post(s => Close(), null);
					}
				}

				_graphics.Progress = progress;
				_graphics.Text = message;
			}

			base.OnDrawing();
		}

		private void UpdateProgressBar(IBackgroundTaskContext context)
		{
			while (!context.CancelRequested)
			{
				Box box = new Box();
				_synchronizationContext.Send(s =>
				                             	{
				                             		((Box) s).Value = this.ParentPresentationImage.Visible;
				                             		this.Draw();
				                             	}, box);
				if (context.CancelRequested || !box.Value)
					break;
				Thread.Sleep(_animationTick);
			}
		}

		private void OnTaskTerminated(object sender, BackgroundTaskTerminatedEventArgs e)
		{
			BackgroundTask updateTask = sender as BackgroundTask;
			if (updateTask != null)
			{
				updateTask.Terminated -= OnTaskTerminated;
				updateTask.Dispose();
			}
			_updateTask = null;
		}

		private class Box
		{
			public bool Value;
		}

		#region Static Helpers

		/// <summary>
		/// Creates and displays a <see cref="ProgressGraphic"/>.
		/// </summary>
		/// <remarks>
		/// This method will invoke the graphic's <see cref="IDrawable.Draw"/> method, so do not call it from a draw routine in the same scene graph!
		/// </remarks>
		/// <param name="task">The <see cref="BackgroundTask"/> to execute. The task is automatically started if it is not already running.</param>
		/// <param name="parentCollection">The graphics collection on which the progress graphic should be shown.</param>
		public static void Show(BackgroundTask task, GraphicCollection parentCollection)
		{
			if (!task.IsRunning)
				task.Run();
			Show(new BackgroundTaskProgressAdapter(task), parentCollection);
		}

		/// <summary>
		/// Creates and displays a <see cref="ProgressGraphic"/>.
		/// </summary>
		/// <remarks>
		/// This method will invoke the graphic's <see cref="IDrawable.Draw"/> method, so do not call it from a draw routine in the same scene graph!
		/// </remarks>
		/// <param name="task">The <see cref="BackgroundTask"/> to execute. The task is automatically started if it is not already running.</param>
		/// <param name="parentCollection">The graphics collection on which the progress graphic should be shown.</param>
		/// <param name="autoClose">A value indicating whether or not the progress graphic should be automatically removed when the task is terminated.</param>
		public static void Show(BackgroundTask task, GraphicCollection parentCollection, bool autoClose)
		{
			if (!task.IsRunning)
				task.Run();
			Show(new BackgroundTaskProgressAdapter(task), parentCollection, autoClose);
		}

		/// <summary>
		/// Creates and displays a <see cref="ProgressGraphic"/>.
		/// </summary>
		/// <remarks>
		/// This method will invoke the graphic's <see cref="IDrawable.Draw"/> method, so do not call it from a draw routine in the same scene graph!
		/// </remarks>
		/// <param name="task">The <see cref="BackgroundTask"/> to execute. The task is automatically started if it is not already running.</param>
		/// <param name="parentCollection">The graphics collection on which the progress graphic should be shown.</param>
		/// <param name="autoClose">A value indicating whether or not the progress graphic should be automatically removed when the task is terminated.</param>
		/// <param name="progressBarGraphicStyle">The style of the progress bar.</param>
		public static void Show(BackgroundTask task, GraphicCollection parentCollection, bool autoClose, ProgressBarGraphicStyle progressBarGraphicStyle)
		{
			if (!task.IsRunning)
				task.Run();
			Show(new BackgroundTaskProgressAdapter(task), parentCollection, autoClose, progressBarGraphicStyle);
		}

		/// <summary>
		/// Creates and displays a <see cref="ProgressGraphic"/>.
		/// </summary>
		/// <remarks>
		/// This method will invoke the graphic's <see cref="IDrawable.Draw"/> method, so do not call it from a draw routine in the same scene graph!
		/// </remarks>
		/// <param name="source">The source from which progress information is retrieved and displayed.</param>
		/// <param name="parentCollection">The graphics collection on which the progress graphic should be shown.</param>
		public static void Show(IProgressGraphicProgressProvider source, GraphicCollection parentCollection)
		{
			Show(source, parentCollection, false, ProgressBarGraphicStyle.Blocks);
		}

		/// <summary>
		/// Creates and displays a <see cref="ProgressGraphic"/>.
		/// </summary>
		/// <remarks>
		/// This method will invoke the graphic's <see cref="IDrawable.Draw"/> method, so do not call it from a draw routine in the same scene graph!
		/// </remarks>
		/// <param name="source">The source from which progress information is retrieved and displayed.</param>
		/// <param name="parentCollection">The graphics collection on which the progress graphic should be shown.</param>
		/// <param name="autoClose">A value indicating whether or not the progress graphic should be automatically removed when the task is terminated.</param>
		public static void Show(IProgressGraphicProgressProvider source, GraphicCollection parentCollection, bool autoClose)
		{
			Show(source, parentCollection, autoClose, ProgressBarGraphicStyle.Blocks);
		}

		/// <summary>
		/// Creates and displays a <see cref="ProgressGraphic"/>.
		/// </summary>
		/// <remarks>
		/// This method will invoke the graphic's <see cref="IDrawable.Draw"/> method, so do not call it from a draw routine in the same scene graph!
		/// </remarks>
		/// <param name="source">The source from which progress information is retrieved and displayed.</param>
		/// <param name="parentCollection">The graphics collection on which the progress graphic should be shown.</param>
		/// <param name="autoClose">A value indicating whether or not the progress graphic should be automatically removed when the task is terminated.</param>
		/// <param name="progressBarGraphicStyle">The style of the progress bar.</param>
		public static void Show(IProgressGraphicProgressProvider source, GraphicCollection parentCollection, bool autoClose, ProgressBarGraphicStyle progressBarGraphicStyle)
		{
			ProgressGraphic progressGraphic = new ProgressGraphic(source, autoClose, progressBarGraphicStyle);
			parentCollection.Add(progressGraphic);
			progressGraphic.Draw();
		}

		#endregion

		#region BackgroundTaskProgressAdapter Class

		private class BackgroundTaskProgressAdapter : IProgressGraphicProgressProvider
		{
			private readonly BackgroundTask _backgroundTask;

			public BackgroundTaskProgressAdapter(BackgroundTask backgroundTask)
			{
				_backgroundTask = backgroundTask;
			}

			public bool IsRunning(out float progress, out string message)
			{
				progress = 0f;
				message = string.Empty;
				if (_backgroundTask != null)
				{
					if (_backgroundTask.LastBackgroundTaskProgress != null)
					{
						progress = _backgroundTask.LastBackgroundTaskProgress.Progress.Percent/100f;
						message = _backgroundTask.LastBackgroundTaskProgress.Progress.Message;
					}
					return _backgroundTask.IsRunning;
				}
				return false;
			}
		}

		#endregion
	}

	#region IProgressProvider Interface

	/// <summary>
	/// Interface for providers of progress information about some abstract operation.
	/// </summary>
	public interface IProgressGraphicProgressProvider
	{
		/// <summary>
		/// Called to get progress information about the abstract operation.
		/// </summary>
		/// <param name="progress">A fractional number between 0 and 1 inclusive indicating the current progress of the abstract operation.</param>
		/// <param name="message">An optional message describing the current progress of the abstract operation.</param>
		/// <returns>A value indicating whether or not the abstract operation is currently running.</returns>
		bool IsRunning(out float progress, out string message);
	}

	#endregion
}