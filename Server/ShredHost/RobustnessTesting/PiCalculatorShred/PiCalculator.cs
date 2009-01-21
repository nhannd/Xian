using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Server.ShredHost;

namespace ClearCanvas.Server.ShredHost.RobustnessTesting.PiCalculatorShred
{
	[ExtensionOf(typeof(ShredExtensionPoint))]
	public class PiCalculatorShred : Shred
	{
		private bool _stopRequested = false;
		private Thread _procedureThread = null;
		private int _iteration = 0;

		public PiCalculatorShred()
		{

		}

		public override void Start()
		{
			_procedureThread = new Thread(new ThreadStart(PiProcedure));
			_procedureThread.Start();
		}

		public override void Stop()
		{
			_stopRequested = true;
			_procedureThread.Join();
		}

		public void PiProcedure()
		{
			Random generator = new Random((Int32)DateTime.Now.Ticks);
			int hits = 0;
			int throws = 0;

			while (!_stopRequested)
			{
				Thread.Sleep(500);
				double x = generator.NextDouble();
				double y = generator.NextDouble();
				if ((x * x + y * y) <= 1.0)
				{
					hits++;
					throws++;
				}
				else
				{
					throws++;
				}

				Platform.Log(LogLevel.Info, String.Format("Pi is approximately  - {0:f}", ((double) hits/ throws)*4));

				++_iteration;
				if (_iteration > 100)
				{
					Exception inner = new Exception("Pi Calculator new inner exception");
					Exception e = new Exception("Pi Calculator throws this exception", inner);
					throw e;
				}
			}
		}
		public override string GetDescription()
		{
			return "Part of robustness testing, this shred calculates approximate value of Pi";
		}

		public override string GetDisplayName()
		{
			return "Pi Calculator Shred";
		}

	}
}
