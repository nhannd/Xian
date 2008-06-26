using System;
using System.Threading;
using ClearCanvas.Common;

namespace ClearCanvas.ImageServer.Services.Shreds
{
	public abstract class ServiceManagerBase
	{
		#region Private Members
		private ManualResetEvent _threadStop;
		private Thread _theThread = null;
		private bool _stopFlag = false;
		private readonly string _name;
		#endregion

		#region Public Properties
		public bool StopFlag
		{
			get { return _stopFlag; }
		}

		public ManualResetEvent ThreadStop
		{
			get { return _threadStop; }
		}

		public string Name
		{
			get { return _name; }
		}
		#endregion

		#region Protected Abstract Methods
		protected abstract void Initialize();
		protected abstract void Run();
		protected abstract void Stop();
		#endregion

		#region Constructor
		public ServiceManagerBase(string name)
		{
			_name = name;
		}
		#endregion

		#region Public Methods
		public void StartService()
        {
			if (_theThread == null)
			{
				_threadStop = new ManualResetEvent(false);
				_theThread = new Thread(delegate()
				                        	{
				                        		bool bInit = false;
												while (!bInit)
												{
													try
													{
														Initialize();
														bInit = true;
													}
													catch (Exception e)
													{
														Platform.Log(LogLevel.Warn, e, "Unexpected exception intializing {0} service", Name);
													}
													if (!bInit)
													{
														// Wait for 60 seconds before retrying init
														ThreadStop.WaitOne(60000, false);
														ThreadStop.Reset();

														if (_stopFlag)
															return;
													}
												}

												try
												{
													Run();
												}
												catch (Exception e)
												{
													Platform.Log(LogLevel.Error, e, "Unexpected exception running service {0}", Name);
												}

				                        	});
				_theThread.Name = Name;
				_theThread.Start();
			}
        }

        public void StopService()
        {
			_stopFlag = true;
			Stop();

			if (_theThread.IsAlive)
			{
				ThreadStop.Set();
				_theThread.Join();
				_theThread = null;
			}
		}
        #endregion
	}
}
