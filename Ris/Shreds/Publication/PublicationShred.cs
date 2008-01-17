using System;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Server.ShredHost;

namespace ClearCanvas.Ris.Shreds.Publication
{
    [ExtensionOf(typeof(ShredExtensionPoint))]
    public class PublicationShred : Shred
    {
        private readonly IPublisher _publisher;

        private readonly EventWaitHandle _stopSignal;

        public PublicationShred()
        {
            _stopSignal = new EventWaitHandle(false, EventResetMode.ManualReset);

            _publisher = new Publisher();
        }

        public override void Start()
        {
            _stopSignal.Reset();

            Thread publisherThread = new Thread(_publisher.Start);
            try
            {
                publisherThread.Start();
                
                Platform.Log(LogLevel.Info, SR.ServiceStartedSuccessfully);
                Console.WriteLine(SR.ServiceStartedSuccessfully);
            }
            catch(Exception e)
            {
                Platform.Log(LogLevel.Fatal, e);
                Platform.Log(LogLevel.Info, SR.ServiceFailedToStart);
            }
            _stopSignal.WaitOne();
            publisherThread.Join();
        }

        public override void Stop()
        {
            try
            {
                _publisher.RequestStop();
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
            return SR.Publication;
        }

        public override string GetDescription()
        {
            return SR.PublicationDescription;
        }

    }
}