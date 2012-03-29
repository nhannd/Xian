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
using ClearCanvas.ImageViewer.Common.LocalDataStore;
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
		void ShowStartupDialog(string message);
		void DismissStartupDialog();
	
		void ShowReindexDialog(ILocalDataStoreReindexer reindexer, NotifyDelegate notifyUserClosed);
		void DismissReindexDialog();

		void ShowMessageBox(string message);
		void DismissMessageBoxes();
	}

    // TODO (Marmot) - Use CC.IV.Dicom.Core.ReindexProcessor directly here, bypass the WorkQueue.
	[AssociateView(typeof(ReindexLocalDataStoreApplicationViewExtensionPoint))]
	internal class ReindexLocalDataStoreDesktopApplication : Application
	{
		private LocalDataStoreReindexer _reindexer;
		private Timer _timer;
		private int _startTicks;
		private bool _hasReindexStarted;
		private bool _isReindexRunning;

		public ReindexLocalDataStoreDesktopApplication()
		{
			TimeoutSeconds = 10;
		}

		public int TimeoutSeconds { get; set; }

		private new IReindexLocalDataStoreApplicationView View
		{
			get { return (IReindexLocalDataStoreApplicationView) base.View; }	
		}

		private void DisposeReindexer()
		{
			if (_reindexer == null)
				return;

			_reindexer.Dispose();
			_reindexer = null;
		}

		protected override void CleanUp()
		{
			base.CleanUp();
			DisposeReindexer();
		}

		protected override string GetName()
		{
			return SR.ReindexApplicationName;
		}

		protected override bool Initialize(string[] args)
		{
			_reindexer = new LocalDataStoreReindexer();
			_reindexer.PropertyChanged += OnReindexPropertyChanged;
	
			StartTimer();
			return true;
		}

		private void StartTimer()
		{
			_startTicks = Environment.TickCount;
			const int oneSecond = 1000;
			_timer = new Timer(OnTimer, null, oneSecond);
			_timer.Start();
		}

		private void KillTimer()
		{
			if (_timer == null)
				return;

			_timer.Stop();
			_timer = null;
		}

		private bool IsInStartupPhase { get { return _timer != null && _reindexer != null && !_hasReindexStarted; } }

		private void OnTimer(object ignore)
		{
			if (!IsInStartupPhase)
			{
				View.DismissStartupDialog();
				KillTimer();
				return;
			}

			double elapsedSeconds = TimeSpan.FromMilliseconds(Environment.TickCount - _startTicks).TotalSeconds;
			if (elapsedSeconds >= TimeoutSeconds)
			{
				OnError(SR.MessageReindexNotStarted);
			}
			else
			{
				//Keep trying to start until it succeeds.
				if (!_reindexer.Start())
					ShowStartupDialog();
				else
					KillTimer();
			}
		}

		private void ShowStartupDialog()
		{
			try
			{
				//This'll throw if the reindex one is visible already ... just catch and ignore.
				View.ShowStartupDialog(SR.MessageStartingReindex);
			}
			catch
			{
			}
		}

		private void OnReindexStarted()
		{
			KillTimer();
			View.DismissMessageBoxes();
			View.DismissStartupDialog();

			_hasReindexStarted = _isReindexRunning = true;
			View.ShowReindexDialog(_reindexer, OnReindexDialogClosed);
		}

		private void OnReindexDialogClosed()
		{
			KillTimer();
			View.DismissReindexDialog();
			DisposeReindexer();

			if (_isReindexRunning) //the user closed the dialog.
				View.ShowMessageBox(SR.MessageReindexWillContinue);

			Shutdown();
		}

		private void OnReindexCompleted()
		{
			KillTimer();
			View.DismissReindexDialog();
			DisposeReindexer();

			View.ShowMessageBox(SR.MessageReindexCompleted);
			Shutdown();
		}

		private void OnReindexCanceled()
		{
			KillTimer();
			DisposeReindexer();
			Shutdown();
		}

		private void OnError(string message)
		{
			KillTimer();
			View.DismissMessageBoxes();
			View.DismissStartupDialog();
			View.DismissReindexDialog();
			DisposeReindexer();

			View.ShowMessageBox(message);
			Shutdown();
		}

		void OnReindexPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (_reindexer == null)
				return;

			if (_hasReindexStarted && _reindexer.Canceled)
			{
				//If the reindex has started and been subsequently canceled, then cut to the chase.
				_isReindexRunning = false;
				OnReindexCanceled();
				return;
			}

			if (e.PropertyName != "RunningState")
				return;

			if (!_isReindexRunning)
			{
				switch (_reindexer.RunningState)
				{
					case RunningState.NotRunning:
						break; //We could try to start the reindex here, but that's done by the startup timer anyway.
					case RunningState.Running:
						OnReindexStarted();
						break;
					case RunningState.Unknown:
						OnError(SR.MessageReindexFailure);
						break;
				}
			}
			else
			{
				switch (_reindexer.RunningState)
				{
					case RunningState.NotRunning:
						_isReindexRunning = false;
						if (_reindexer.FailedSteps > 0 && !_reindexer.Canceled)
							OnError(SR.MessageReindexFailures);
						else if (_reindexer.Canceled)
							OnReindexCanceled();
						else
							OnReindexCompleted();
						break;
					case RunningState.Unknown:
						_isReindexRunning = false;
						OnError(SR.MessageReindexFailure);
						break;
				}
			}
		}
	}
}
