using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Server.ShredHost;

namespace SampleShredCrash
{
    [ExtensionOf(typeof(ShredExtensionPoint))]
    public class SampleShredExtension : ShredBase
    {
        private readonly string _className;
        private readonly string _serviceEndPointName;
        private EventWaitHandle _stopSignal;

        public SampleShredExtension()
        {
            _className = this.GetType().ToString();
            _serviceEndPointName = "SampleShredCrash";
            _stopSignal = new EventWaitHandle(false, EventResetMode.ManualReset);
            System.Diagnostics.Trace.WriteLine(_className + ": constructed");
        }

        public override void Start(int port)
        {
            Platform.Log(_className + "[" + AppDomain.CurrentDomain.FriendlyName + "]: Start invoked");
            _stopSignal.Reset();

            // start up processing thread
            Thread t = new Thread(new ThreadStart(DummyRoutine));
            t.Start();

            // wait for host's stop signal
            _stopSignal.WaitOne();

            // wait for processing thread to finish
            t.Join();
        }

        public override string GetDisplayName()
        {
            return "SampleShredCrash";
        }

        public override void Stop()
        {
            _stopSignal.Set();
        }

        private void DummyRoutine()
        {
            int count = 0;
            while (true)
            {
                // make this shred crash after 60 seconds
                if (count > 60)
                {
                    throw new System.ApplicationException();
                }

                System.Threading.Thread.Sleep(1000);
                if (_stopSignal.WaitOne(100, false))
                    break;
                ++count;
            }
        }
    }
}
