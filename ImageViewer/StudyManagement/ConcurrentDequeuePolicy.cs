using System;
using System.Collections.Generic;
using System.Threading;


namespace ClearCanvas.ImageViewer.StudyManagement
{
    public class ConcurrentDequeuePolicy
    {
        private int _concurrentFrames;
        private int _maxConcurrentFrames;

        private readonly ISet<object> _waitObjects ;
        private readonly object _waitListLock;

        public ConcurrentDequeuePolicy()
        {
            _maxConcurrentFrames = Math.Max(PrefetchSettings.Default.MaxConcurrentFetches, 1);

            _waitListLock = new object();
            _waitObjects = new HashSet<object>();

        }

        public void RegisterWaitObject(object waitObject)
        {
            lock(_waitListLock)
            {
                _waitObjects.Add(waitObject);
            }
        }

        public void UnregisterWaitObject(object waitObject)
        {
            lock (_waitListLock)
            {
                _waitObjects.Remove(waitObject);
            }
        }

        public bool CanDequeue()
        {
            return Thread.VolatileRead(ref _concurrentFrames) < _maxConcurrentFrames;
        }

        public void BeginFetch()
        {
            Interlocked.Increment(ref _concurrentFrames);
        }

        public void EndFetch()
        {
            Interlocked.Decrement(ref _concurrentFrames);
            Notify();

        }

        private void Notify()
        {
            var waitObjects = new HashSet<object>(_waitObjects);
            foreach (var waitObject in waitObjects)
            {
                lock(waitObject)
                {
                    Monitor.PulseAll(waitObject); 
                }
            }
        }

    }
}