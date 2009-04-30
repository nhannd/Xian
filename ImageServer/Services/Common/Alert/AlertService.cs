#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Enterprise;

namespace ClearCanvas.ImageServer.Services.Common.Alert
{
    /// <summary>
    /// Alert record service
    /// </summary>
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    public class AlertService : IApplicationServiceLayer, IAlertService
    {
        #region Private Members
        private IAlertServiceExtension[] _extensions;
        #endregion

        #region Private Methods
        
        private IAlertServiceExtension[] GetExtensions()
        {
            if (_extensions == null)
            {
                _extensions =
                    CollectionUtils.ToArray<IAlertServiceExtension>(new AlertServiceExtensionPoint().CreateExtensions());
            }

            return _extensions;
        }

        #endregion

        #region IAlertService Members

        public void GenerateAlert(ImageServer.Common.Alert alert)
        {
            IAlertServiceExtension[] extensions = GetExtensions();
            foreach(IAlertServiceExtension ext in extensions)
            {
                try
                {
                    ext.OnAlert(alert);    
                }
                catch(Exception e)
                {
                    Platform.Log(LogLevel.Error, e, "Error occurred when calling {0} OnAlert()", ext.GetType());
                }
            }

        }
       

        #endregion
    }
}