#if UNIT_TESTS

using System.Threading;

namespace ClearCanvas.ImageViewer.Common.WorkItem.Tests
{
    //TODO (Marmot): should really test this scenario, too.
    internal class TestUISynchronizationContext : SynchronizationContext
    {
        //private readonly object _syncLock = new object();
        //private Thread _thread;
        //private readonly Queue<SendOrPostCallback> _callbacks = new Queue<SendOrPostCallback>();

        //public TestUISynchronizationContext()
        //{
        //    _thread = new Thread(RunThread);
        //    _thread.Name = String.Format("Simulated UI Thread [{0}]", _thread.ManagedThreadId);
        //    _thread.Start();
        //    SynchronizationContext.Current.Post()
        //}

        //private void RunThread(object ignore)
        //{

        //}

        //public void RunPump()
        //{
        //    lock(_syncLock)
        //    {

        //    }

        //}
    }
}

#endif