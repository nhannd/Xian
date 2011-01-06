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
using ClearCanvas.ImageViewer.Services.LocalDataStore;

namespace ClearCanvas.ImageViewer.Services.Tools
{
	internal class ReindexLocalDataStoreConsoleApplication
	{
		private LocalDataStoreReindexer _reindexer;
		private bool _quitting;
		private bool _calledStart;
		private bool _hasStarted;
		private int? _lastProgressReportTicks;
		private ManualResetEvent _quitEvent;

		internal bool NoWait { get; set; }

		public void Run()
		{
			using (_reindexer = new LocalDataStoreReindexer())
			{
				_reindexer.PropertyChanged += OnPropertyChanged;
				_quitEvent = new ManualResetEvent(false);
				
				Console.WriteLine("Determining reindex state ...");
				const int tenSeconds = 10000;
				bool signaled = _quitEvent.WaitOne(tenSeconds, false);
				if (signaled)
					return;

				if (!_hasStarted)
				{
					Console.WriteLine(SR.MessageReindexNotStarted);
					Environment.ExitCode = -1;
					return;
				}

				_quitEvent.WaitOne();
			}
		}

		private float PercentComplete
		{
			get { return 100* _reindexer.TotalProcessed / (float)_reindexer.TotalToProcess; }	
		}

		private void Quit()
		{
			_quitting = true;
			const int threeSeconds = 3000;
			Thread.Sleep(threeSeconds);
			_quitEvent.Set();
		}

		private void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (_quitting)
				return;

			if (e.PropertyName == "RunningState")
			{
				if (!_hasStarted)
				{
					switch (_reindexer.RunningState)
					{
						case RunningState.NotRunning:
							_calledStart = true;
							Console.WriteLine(SR.MessageStartingReindex);
							_reindexer.Start();
							break;
						case RunningState.Running:
							_hasStarted = true;
							if (!_calledStart)
								Console.WriteLine(SR.MessageReindexRunning);
							
							if (NoWait)
							{
								Console.WriteLine(SR.MessageNoWaitSpecified);
								Quit();
							}
							break;
					}
				}
				else
				{
					switch (_reindexer.RunningState)
					{
						case  RunningState.NotRunning:
								if (_reindexer.FailedSteps > 0 && !_reindexer.Canceled)
								{
									Console.WriteLine(SR.MessageReindexFailures);
									Environment.ExitCode = -1;
								}
								else if (_reindexer.Canceled)
									Console.WriteLine(SR.MessageReindexCancelled);
							break;
						case RunningState.Unknown:
							Console.WriteLine(SR.MessageReindexFailure);
							Environment.ExitCode = -1; 
							break;
					}
					
					Quit();
				}
			}
			else if (e.PropertyName == "StatusMessage")
			{
				if (_reindexer.RunningState == RunningState.Running && _reindexer.TotalProcessed == _reindexer.TotalToProcess)
					Console.WriteLine(_reindexer.StatusMessage);
			}
			else if (e.PropertyName == "TotalProcessed")
			{
				if (!_hasStarted)
					return;

				float percentComplete = PercentComplete;
				int millisecondsSinceLastProgressReport = _lastProgressReportTicks.HasValue
				                                      	? Environment.TickCount - _lastProgressReportTicks.Value
				                                      	: int.MaxValue;

				if (millisecondsSinceLastProgressReport > 1000 || percentComplete == 100)
				{
					_lastProgressReportTicks = Environment.TickCount;
					Console.WriteLine("... {0:F1}%", percentComplete);
				}
			}
		}
	}
}
