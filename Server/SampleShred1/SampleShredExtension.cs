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
        private SampleShred1Settings _settings;
        private EventWaitHandle _stopSignal;

        public SampleShredExtension()
        {
            _className = this.GetType().ToString();
            _serviceEndPointName = "SampleShred1";
            _stopSignal = new EventWaitHandle(false, EventResetMode.ManualReset);
            System.Diagnostics.Trace.WriteLine(_className + ": constructed");

            _settings = new SampleShred1Settings();
        }

        ~SampleShredExtension()
        {
            _settings.Save();
        }

        public override void Start(int port)
        {
            Platform.Log(_className + "[" + AppDomain.CurrentDomain.FriendlyName + "]: Start invoked");
            _stopSignal.Reset();
            GlobalStore.CurrentPrime = 2;

            StartHost<SampleShredServiceType, ISampleShred1Interface>(port, _serviceEndPointName, "Prime number calculator");

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

        public override string GetFriendlyName()
        {
            return _settings.FriendlyName;
        }

        public override void Stop()
        {
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
    }
}
