using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.Server.ShredHost
{
    public class ShredCommunicationServiceType : IShredCommunication
    {
        public ShredCommunicationServiceType()
        {
            _className = this.GetType().ToString();
            System.Diagnostics.Trace.WriteLine(_className + ": constructed");
        }

        #region IShredCommunication Members

        public void Pause()
        {
            Platform.Log(_className + "[" + AppDomain.CurrentDomain.FriendlyName + "] Pause invoked");
            System.Diagnostics.Trace.WriteLine(_className + ": Pause invoked");
        }

        public void Stop()
        {
            Platform.Log(_className + "[" + AppDomain.CurrentDomain.FriendlyName + "]: Stop invoked");
            System.Diagnostics.Trace.WriteLine(_className + ": Stop invoked");

        }

        public void Unpause()
        {
            Platform.Log(_className + "[" + AppDomain.CurrentDomain.FriendlyName + "]: Unpause invoked");
            System.Diagnostics.Trace.WriteLine(_className + ": Unpause invoked");
        }

        #endregion

        #region Private Members
        private string _className;
        #endregion
    }
}
