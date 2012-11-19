using System;
using System.Threading;

namespace ClearCanvas.Common.Utilities
{
    public class DisposableLock : IDisposable
    {
        private readonly object _lockObject;
        private readonly bool _lockWasTaken;

        public DisposableLock(object lockObject)
        {
            if (lockObject == null)
                throw new Exception("DisposableLock: lock object cannot be null");
            _lockObject = lockObject;
            Monitor.Enter(_lockObject, ref _lockWasTaken);
        }

        private DisposableLock()
        {
            
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (_lockWasTaken)
                Monitor.Exit(_lockObject);
        }

        #endregion
    }
}