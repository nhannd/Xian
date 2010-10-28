#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

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
            Platform.Log(LogLevel.Info,_className + "[" + AppDomain.CurrentDomain.FriendlyName + "] Pause invoked");
            System.Diagnostics.Trace.WriteLine(_className + ": Pause invoked");
        }

        public void Stop()
        {
            Platform.Log(LogLevel.Info, _className + "[" + AppDomain.CurrentDomain.FriendlyName + "]: Stop invoked");
            System.Diagnostics.Trace.WriteLine(_className + ": Stop invoked");

        }

        public void Unpause()
        {
            Platform.Log(LogLevel.Info, _className + "[" + AppDomain.CurrentDomain.FriendlyName + "]: Unpause invoked");
            System.Diagnostics.Trace.WriteLine(_className + ": Unpause invoked");
        }

        #endregion

        #region Private Members
        private string _className;
        #endregion
    }
}
