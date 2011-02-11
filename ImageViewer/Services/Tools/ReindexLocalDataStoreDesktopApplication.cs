#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Services.LocalDataStore;
using ClearCanvas.Desktop;
using Timer=ClearCanvas.Common.Utilities.Timer;

namespace ClearCanvas.ImageViewer.Services.Tools
{
	[ExtensionPoint]
	public sealed class ReindexLocalDataStoreApplicationViewExtensionPoint : ExtensionPoint<IApplicationView>
	{}

	public delegate void NotifyDelegate();

	public interface IReindexLocalDataStoreApplicationView : IApplicationView, IDisposable
	{
		void SetDialogTitle(string title);

		void ShowStartupDialog(string message);
		void DismissStartupDialog();
	
		void ShowReindexDialog(ILocalDataStoreReindexer reindexer, NotifyDelegate notifyUserClosed);
		void DismissReindexDialog();

		void ShowMessageBox(string message);
		void DismissMessageBoxes();
	}

	[AssociateView(typeof(ReindexLocalDataStoreApplicationViewExtensionPoint))]
	internal class ReindexLocalDataStoreDesktopApplication : Application
	{
		private LocalDataStoreReindexer _reindexer;
		private Timer _timer;
		private bool _reindexRunning;
		private bool _quitting;

		public ReindexLocalDataStoreDesktopApplication()
		{
			TimeoutSeconds = 10;
		}

		public int TimeoutSeconds { get; set; }

		private new IReindexLocalDataStoreApplicationView View
		{
			get { return (IReindexLocalDataStoreApplicationView) base.View; }	
		}

		protected override bool Initialize(string[] args)
		{
			_reindexer = new LocalDataStoreReindexer();
			_reindexer.PropertyChanged += OnReindexPropertyChanged;
	
			View.SetDialogTitle(SR.ReindexApplicationDialogTitle);

			//Delay showing the startup dialog, just in case the reindex is already running.
			SynchronizationContext synchronizationContext = SynchronizationContext.Current;
			ThreadPool.QueueUserWorkItem(delegate
			                             	{
			                             		Thread.Sleep(500);
												synchronizationContext.Post(ShowStartupDialog, null);
			                             	});

			StartTimer(TimeoutSeconds * 1000);

			return true;
		}

		private void ShowStartupDialog(object state)
		{
			if (_reindexer == null || _reindexRunning)
				return;

			try
			{
				//This'll throw if the reindex one is visible already ... just catch and ignore.
				View.ShowStartupDialog(SR.MessageStartingReindex);
			}
			catch
			{
			}
		}

		private void StartTimer(int intervalMilliseconds)
		{
			KillTimer();
			_timer = new Timer(OnTimer) { IntervalMilliseconds = intervalMilliseconds };
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

			KillTimer();
			if (_quitting)
				Shutdown();
			else
				TimedQuit(SR.MessageReindexNotStarted);
		}
		
		private void TimedQuit(string message)
		{
			TimedQuit(message, 10000);
		}

		private void TimedQuit(string message, int intervalMilliseconds)
		{
			if (!_quitting)
			{
				View.DismissStartupDialog();
				StartTimer(intervalMilliseconds);
				_quitting = true;
			}

			if (!String.IsNullOrEmpty(message))
			{
				View.ShowMessageBox(message);
				if (!_reindexRunning)
					Shutdown();
			}
		}

		private void ShowReindexDialog()
		{
			if (_reindexRunning)
				return;

			KillTimer();

			//Cancel quit and dismiss all dialogs.
			_quitting = false;
			View.DismissMessageBoxes();

			_reindexRunning = true;
			View.ShowReindexDialog(_reindexer, OnReindexDialogClosed); //this is a modal call.
		}

		private void OnReindexDialogClosed()
		{
			if (_reindexRunning)
				TimedQuit(SR.MessageReindexWillContinue);
		}

		void OnReindexPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName != "RunningState")
				return;

			if (!_reindexRunning)
			{
				switch (_reindexer.RunningState)
				{
					case RunningState.NotRunning:
						if (!_reindexer.Start())
							TimedQuit(SR.MessageReindexNotStarted);
						break;
					case RunningState.Running:
						ShowReindexDialog();
						break;
					case RunningState.Unknown:
						TimedQuit(SR.MessageReindexFailure);
						break;
				}
			}
			else
			{
				switch (_reindexer.RunningState)
				{
					case RunningState.NotRunning:
						_reindexRunning = false;
						if (_reindexer.FailedSteps > 0 && !_reindexer.Canceled)
						{
							TimedQuit(SR.MessageReindexFailures);
						}
						else
						{
							//Let the user see the result for a few seconds.
							TimedQuit(null, 5000);
						}
						break;
					case RunningState.Unknown:
						_reindexRunning = false;
						TimedQuit(SR.MessageReindexFailure);
						break;
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
