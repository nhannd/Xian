using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ClearCanvas.Common;

namespace ClearCanvas.Server.ShredHost
{
    internal class ShredInfo : MarshalByRefObject
    {
        public ShredInfo(Thread thread, AppDomain shredDomain, IShred shred)
        {
            Platform.CheckForNullReference(thread, "thread");
            Platform.CheckForNullReference(shred, "shred");
            Platform.CheckForNullReference(shredDomain, "shredDomain");

            _shredDomain = shredDomain;
            _shredThread = thread;
            _shred = shred;
        }

        #region Properties
        private Thread _shredThread;
        private IShred _shred;
        private AppDomain _shredDomain;

        public AppDomain ShredDomain
        {
            get { return _shredDomain; }
        }
	
        public IShred Shred
        {
            get { return _shred; }
        }
	

        public Thread ShredThreadObject
        {
            get { return _shredThread; }
        }
	
        #endregion
    }
}
