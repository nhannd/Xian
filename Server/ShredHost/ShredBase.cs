using System;
using System.Collections.Generic;
using System.Text;


namespace ClearCanvas.Server.ShredHost
{
    public abstract class ShredBase : MarshalByRefObject, IShred
    {
        public ShredBase()
        {

        }

        #region IShred Members
        public abstract void Start();
        public abstract void Stop();
        public abstract string GetDisplayName();
        public abstract string GetDescription();
        #endregion       
    }
}
