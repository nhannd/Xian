#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Threading;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	//TODO (CR Sept 2010): the BackgroundTask is essentially being used as a timer, so may as well
	//just use the Timer class in Common.Utilities, which also automatically marshals calls over to the UI thread.
	//Could also improve the efficiency a little and only draw if the progress has changed or draw has been
	//called externally.

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
					if (_updateTask == null && _synchronizationContext != null)
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

					if (_autoClose && _synchronizationContext != null)
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
			//TODO (CR Sept 2010): just use a timer, and all this could be done on the UI thread.

			while (!context.CancelRequested)
			{
				// if we didn't get a SynchronizationContext from the thread that calls .Draw(), then we can't update the progress bar
				SynchronizationContext synchronizationContext = _synchronizationContext;
				if (synchronizationContext == null)
					break;

				Box box = new Box();
				synchronizationContext.Send(s =>
				                            	{
				                            		if (this.ParentPresentationImage != null)
				                            		{
				                            			((Box) s).Value = this.ParentPresentationImage.Visible;
				                            			this.Draw();
				                            		}
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

		//TODO (CR Sept 2010): I'd prefer to get rid of this if we can.

		/// <summary>
		/// Gets a value indicating whether or not directly drawing a <see cref="ProgressGraphic"/> from the current thread is valid.
		/// </summary>
		/// <remarks>
		/// If the value of this property is true, then the call to draw a <see cref="ProgressGraphic"/> must be marshalled to
		/// a thread with a valid <see cref="SynchronizationContext"/> in order for progress updates to occur automatically.
		/// </remarks>
		public static bool RequiresInvoke
		{
			get { return SynchronizationContext.Current == null; }
		}

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
		//TODO (CR Sept 2010): this isn't really a clear API.  It's accounting for the fact that the progress operation
		//is actually asynchronous and needs to get all the information at once.  It would simplify things quite a bit
		//if the progress provider object provided the updates (events, callback interface) and this
		//class immediately marshalled it over to the UI thread to deal with it.

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