using System;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Server.ShredHost;

namespace ClearCanvas.Ris.Server
{
    [ExtensionOf(typeof(ShredExtensionPoint))]
    public class Shred : ClearCanvas.Server.ShredHost.Shred
    {
        private readonly Processor _server;

        private readonly EventWaitHandle _stopSignal;

        public Shred()
        {
            _stopSignal = new EventWaitHandle(false, EventResetMode.ManualReset);

			_server = new Processor();
        }

        public override void Start()
        {
            _stopSignal.Reset();

            Thread processorThread = new Thread(_server.Start);
            try
            {
                processorThread.Start();
                
                Platform.Log(LogLevel.Info, SR.ServiceStartedSuccessfully);
                Console.WriteLine(SR.ServiceStartedSuccessfully);
            }
            catch(Exception e)
            {
                Platform.Log(LogLevel.Fatal, e);
                Platform.Log(LogLevel.Info, SR.ServiceFailedToStart);
            }
            _stopSignal.WaitOne();
            processorThread.Join();
        }

        public override void Stop()
        {
            try
            {
                _server.RequestStop();
                _stopSignal.Set();

                Platform.Log(LogLevel.Info, SR.ServiceStoppedSuccessfully);
                Console.WriteLine(SR.ServiceStoppedSuccessfully);
            }
            catch(Exception e)
            {
                Platform.Log(LogLevel.Fatal, e);
                Platform.Log(LogLevel.Info, SR.ServiceFailedToStop);
            }
        }

        public override string GetDisplayName()
        {
            return SR.TitleRisServer;
        }

        public override string GetDescription()
        {
            return SR.MessageRisServerDescription;
        }
    }
}
