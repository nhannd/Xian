#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Services.LocalDataStore;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Services.Tools
{
	[ExtensionPoint]
	public sealed class ReindexLocalDataStoreApplicationViewExtensionPoint : ExtensionPoint<IApplicationView>
	{}

	public interface IReindexLocalDataStoreApplicationView : IApplicationView, IDisposable
	{
		void Initialize(ILocalDataStoreReindexer reindexer);
		void RunModal();
	}

	[AssociateView(typeof(ReindexLocalDataStoreApplicationViewExtensionPoint))]
	internal class ReindexLocalDataStoreDesktopApplication : Application
	{
		private LocalDataStoreReindexer _reindexer;
		private ClearCanvas.Common.Utilities.Timer _timer;
		private bool _hasStarted;
		private bool _quit;

		private new IReindexLocalDataStoreApplicationView View
		{
			get { return (IReindexLocalDataStoreApplicationView) base.View; }	
		}

		protected override bool Initialize(string[] args)
		{
			_reindexer = new LocalDataStoreReindexer();
			_reindexer.PropertyChanged += OnReindexPropertyChanged;

			View.Initialize(_reindexer);

			StartTimer(10000);
			return true;
		}
        
		private void StartTimer(int intervalMilliseconds)
		{
			_timer = new ClearCanvas.Common.Utilities.Timer(OnTimer) { IntervalMilliseconds = intervalMilliseconds };
			_timer.Start();
		}

		private void KillTimer()
		{
			if (_timer == null)
				return;

			_timer.Stop();
			_timer = null;
		}

		void OnTimer(object ignore)
		{
			if (_timer == null)
				return;

			if (!_hasStarted) //reindex didn't start for whatever reason.
				TimedQuit(SR.MessageReindexNotStarted);

			KillTimer();

			if (_quit)
				Quit();
		}
		
		private void TimedQuit(string message)
		{
			TimedQuit(message, 10000);
		}

		private void TimedQuit(string message, int intervalMilliseconds)
		{
			if (_quit)
				return;

			if (_timer == null)
				StartTimer(intervalMilliseconds);

			_quit = true;
			if (!String.IsNullOrEmpty(message))
				View.ShowMessageBox(message, MessageBoxActions.Ok);
		}

		private void ShowDialog()
		{
			if (_hasStarted)
				return;

			_hasStarted = true;
			KillTimer();

			View.RunModal();

			if (_reindexer.RunningState == RunningState.Running)
				TimedQuit(SR.MessageReindexWillContinue);
			else
				Quit();
		}

		void OnReindexPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "RunningState")
			{
				if (!_hasStarted)
				{
					switch (_reindexer.RunningState)
					{
						case RunningState.NotRunning:
							_reindexer.Start();
							break;
						case RunningState.Running:
							ShowDialog();
							break;
					}
				}
				else
				{
					if (_reindexer.RunningState == RunningState.NotRunning)
					{
						if (_reindexer.FailedSteps > 0 && !_reindexer.Canceled)
							TimedQuit(SR.MessageReindexFailures);
						else
						{
							//Let the user see the result for a couple of seconds.
							TimedQuit(null, 3000);
						}
					}
					else if (_reindexer.RunningState == RunningState.Unknown)
					{
						TimedQuit(SR.MessageReindexFailure);
					}
				}
			}
		}

		protected override void CleanUp()
		{
			base.CleanUp();

			if (_reindexer == null)
				return;

			_reindexer.Dispose();
			_reindexer = null;
		}
	}
}
