using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Server.ShredHost;

namespace ClearCanvas.Server.ShredHost.RobustnessTesting.FibonacciCalculatorShred
{
	[ExtensionOf(typeof(ShredExtensionPoint))]
	public class FibonacciCalculator : Shred
	{
		private bool _stopRequested = false;
		private Thread _procedureThread = null;
		private int _lastSum = 1;
		private int _nextToLastSum = 1;
		private int _iteration = 0;

		#region IShred Members

		public override void Start()
		{
			_procedureThread = new Thread(new ThreadStart(FibonacciProcedure));
			_procedureThread.Start();
		}

		public override void Stop()
		{
			_stopRequested = true;
			_procedureThread.Join();
		}

		public override string GetDisplayName()
		{
			return "Fibonacci Calculator Shred";
		}

		public override string GetDescription()
		{
			return "ShredHost Robustness Testing Fibonacci Calculator";
		}

		#endregion

		public void FibonacciProcedure()
		{
			while (!_stopRequested)
			{
				Thread.Sleep(5000);
				int newLastSum = _nextToLastSum + _lastSum;
				_nextToLastSum = _lastSum;
				_lastSum = newLastSum;

				Platform.Log(LogLevel.Info, String.Format("Fibonacci Number - {0}", _lastSum));

				++_iteration;
				if (_iteration > 100)
					throw new Exception("PiCalculator throws an exception");

			}
		}
	}
}
