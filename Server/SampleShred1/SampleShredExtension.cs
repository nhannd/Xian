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

namespace SampleShred1
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
            _serviceEndPointName = "SampleShred1";
            _stopSignal = new EventWaitHandle(false, EventResetMode.ManualReset);
            System.Diagnostics.Trace.WriteLine(_className + ": constructed");
        }

        public override void Start()
        {
            Platform.Log(_className + "[" + AppDomain.CurrentDomain.FriendlyName + "]: Start invoked on port " + this.ServicePort.ToString());
            _stopSignal.Reset();
            GlobalStore.CurrentPrime = 2;

            StartHost<SampleShredServiceType, ISampleShred1Interface>(_serviceEndPointName, "Prime number calculator");

            // start up processing thread
            Thread t = new Thread(new ThreadStart(ComputePrimes));
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
            return "SampleShred1";
        }

        public override void Stop()
        {
            Platform.Log(_className + "[" + AppDomain.CurrentDomain.FriendlyName + "]: Stop invoked");
            _stopSignal.Set();
        }

        private void ComputePrimes()
        {
            while (true)
            {
                FindNextPrime();
                if (_stopSignal.WaitOne(100, false))
                    break;
            }
        }

        private void FindNextPrime()
        {
            if (2 == GlobalStore.CurrentPrime)
            {
                GlobalStore.CurrentPrime = 3;
                return;
            }

            int numberToCheck = GlobalStore.CurrentPrime + 2;
            double squareRoot = Math.Sqrt(numberToCheck);

            int factor = 3; // we can start at 3, 'coz numberToCheck will never be even
            while (factor <= squareRoot)
            {
                if (0 == numberToCheck % factor)
                {
                    numberToCheck = numberToCheck + 2;
                    factor = 3;
                }
                else
                {
                    factor += 2;
                }

            }

            GlobalStore.CurrentPrime = numberToCheck;
            return;
        }

        public override string GetDescription()
        {
            return "This shred calculates prime numbers";
        }
    }
}
