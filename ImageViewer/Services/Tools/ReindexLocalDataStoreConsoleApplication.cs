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
