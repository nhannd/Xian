#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
