using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Server.ShredHost;
using System.Threading;

namespace ClearCanvas.Ris.Shreds
{
	public abstract class RisShredBase : Shred
	{
		private bool _isStarted = false;
        private List<IProcessor> _processors = new List<IProcessor>();
        private List<Thread> _processorThreads = new List<Thread>();

        //TODO: make this into a setting
        private readonly TimeSpan _shutDownTimeOut = new TimeSpan(0, 0, 60);

		#region Shred overrides

		public override void Start()
		{
            if (!_isStarted)
			{
				StartUp();
			}
		}

		public override void Stop()
		{
            if (_isStarted)
			{
				ShutDown();
			}
		}

		#endregion

        protected abstract IList<IProcessor> GetProcessors();

        /// <summary>
        /// Starts all processors returned by <see cref="GetProcessors"/>.  This method is transactional:
        /// either all processors start, or none do (those that have already started are stopped). This method
        /// will not throw under any circumstances.  Failure is silent. (TODO: is this desirable??)
        /// </summary>
        private void StartUp()
		{
            if (_isStarted)
                return;

            // set flag immediately so we can't be re-entered
            _isStarted = true;

            Platform.Log(LogLevel.Info, string.Format(SR.ShredStarting, this.GetDisplayName()));

            _processorThreads.Clear();
            _processors.Clear();

			try
			{

                // attempt to start all processors - if any throws an exception, abort
				foreach (IProcessor processor in GetProcessors())
				{
                    Thread thread = StartProcessorThread(processor);

                    // if thread started successfully, add to lists
                    _processorThreads.Add(thread);
                    _processors.Add(processor);
				}

				Platform.Log(LogLevel.Info, string.Format(SR.ShredStartedSuccessfully, this.GetDisplayName()));
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e, string.Format(SR.ShredFailedToStart, this.GetDisplayName()));

                // stop any processors that have already started
                ShutDown();
			}
		}

        /// <summary>
        /// Stops all running processors.  This method is guaranteed to succeed,
        /// in the sense that it will not throw under any circumstances.  However,
        /// if a processor thread does not stop within the specified time out, it 
        /// will be abandoned.
        /// </summary>
		private void ShutDown()
		{
            if (!_isStarted)
                return;

			Platform.Log(LogLevel.Info, string.Format(SR.ShredStopping, this.GetDisplayName()));

            // request all processors to stop
			foreach (IProcessor processor in _processors)
			{
                try
                {
                    // ask processor to stop
                    // this call is not expected to throw under any circumstances
                    processor.RequestStop();
                }
                catch (Exception e)
                {
                    // in case it throws for some reason
                    Platform.Log(LogLevel.Error, e);
                }
            }

            // wait for all processor threads to stop
            foreach (Thread thread in _processorThreads)
            {
                try
                {
                    thread.Join(_shutDownTimeOut);
                }
                catch (ThreadStateException e)
                {
                    // can get here if the thread was not started properly
                    Platform.Log(LogLevel.Debug, e);
                }
            }

            Platform.Log(LogLevel.Info, string.Format(SR.ShredStoppedSuccessfully, this.GetDisplayName()));

            // only set the flag here
            _isStarted = false;
		}

        /// <summary>
        /// Starts the specified processor on a dedicated thread, and returns the
        /// <see cref="Thread"/> object.
        /// </summary>
        /// <param name="processor"></param>
        /// <returns></returns>
        private Thread StartProcessorThread(IProcessor processor)
        {
            Thread thread = new Thread(
                delegate()
                {
                    try
                    {
                        processor.Run();
                    }
                    catch (Exception e)
                    {
                        Platform.Log(LogLevel.Error, e);
                    }
                });

            thread.Start();
            return thread;
        }

    }
}
