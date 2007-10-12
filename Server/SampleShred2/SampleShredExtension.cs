#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Server.ShredHost;

namespace SampleShred2
{
    [ExtensionOf(typeof(ShredExtensionPoint))]
    public class SampleShredExtension : WcfShred
    {
        private readonly string _className;
        private readonly string _serviceEndPointName;
        private EventWaitHandle _stopSignal;

        public SampleShredExtension()
        {
            _className = this.GetType().ToString();
            _serviceEndPointName = "SampleShred2";
            _stopSignal = new EventWaitHandle(false, EventResetMode.ManualReset);
            System.Diagnostics.Trace.WriteLine(_className + ": constructed");
        }

        public override void Start()
        {
            Platform.Log(_className + "[" + AppDomain.CurrentDomain.FriendlyName + "]: Start invoked on port " + this.SharedHttpPort.ToString());
            _stopSignal.Reset();
            GlobalStore.Score = 0;
            GlobalStore.Darts = 0;

            StartHttpHost<SampleShredServiceType, ISampleShred2Interface>(_serviceEndPointName, "pi digits calculator");

            // start up processing thread
            Thread t = new Thread(new ThreadStart(ComputePi));
            t.Start();

            // wait for host's stop signal
            _stopSignal.WaitOne();
            
            // wait for processing thread to finish
            t.Join();

            // stop hosting the service
            StopHost(_serviceEndPointName);
        }

        public override string GetDisplayName()
        {
            return _serviceEndPointName;
        }

        public override void Stop()
        {
            Platform.Log(_className + "[" + AppDomain.CurrentDomain.FriendlyName + "]: Stop invoked");
            _stopSignal.Set();
        }

        private void ComputePi()
        {
            while (true)
            {
                FindNextPi();
                if (_stopSignal.WaitOne(50, false))
                    break;
            }
        }

        private void FindNextPi()
        {
            double dartX;
            double dartY;

            dartX = GlobalStore.RandomGenerator.NextDouble();
            dartY = GlobalStore.RandomGenerator.NextDouble();

            if (((dartX * dartX) + (dartY * dartY)) <= 1.0)
                GlobalStore.Score++;
            GlobalStore.Darts++;

            GlobalStore.CurrentPi = 4.0 * (double)GlobalStore.Score / (double)GlobalStore.Darts;
            return;
        }



        public override string GetDescription()
        {
            return "This shred calculates the value of pi based on the probability of throwing darts into one quadrant of a circle.";
        }
    }
}
