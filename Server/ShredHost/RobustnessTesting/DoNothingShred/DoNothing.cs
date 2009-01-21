using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Server.ShredHost;


namespace ClearCanvas.Server.ShredHost.RobustnessTesting.DoNothingShred
{
	[ExtensionOf(typeof(ShredExtensionPoint))]
	public class DoNothingShred : Shred
	{
		private bool _stopRequested = false;
		private Thread _procedureThread = null;
		private int _iteration = 0;

		#region IShred Members

		public override void Start()
		{
			_procedureThread = new Thread(new ThreadStart(DoNothingProcedure));
			_procedureThread.Start();
		}

		public override void Stop()
		{
			_stopRequested = true;
			_procedureThread.Join();
		}

		public override string GetDisplayName()
		{
			return "Do Nothing Shred";
		}

		public override string GetDescription()
		{
			return "ShredHost Robustness Testing Do Nothing Shred";
		}

		#endregion

		public void DoNothingProcedure()
		{
			while (!_stopRequested)
			{
				Thread.Sleep(5000);
				Platform.Log(LogLevel.Info, "Do Nothing Shred heartbeat");
			}

			++_iteration;
			if (_iteration > 100)
				throw new Exception("PiCalculator throws an exception");

		}
	}
}
