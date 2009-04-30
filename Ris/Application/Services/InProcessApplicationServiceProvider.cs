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
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Ris.Application.Services
{
    /// <summary>
    /// This service provider allows the application server to make use of application services internally
    /// by providing these services in-process.
    /// </summary>
    [ExtensionOf(typeof(ServiceProviderExtensionPoint))]
    public class InProcessApplicationServiceProvider : IServiceProvider
    {
         private IServiceFactory _serviceFactory;

        public InProcessApplicationServiceProvider()
        {
            _serviceFactory = new ServiceFactory(new ApplicationServiceExtensionPoint());

			// exception logging occurs outside of the main persistence context
			// JR: is there any point in logging exceptions from the in-process provider?  Or is this just redundant?
			//_serviceFactory.Interceptors.Add(new ExceptionLoggingAdvice());

			// add persistence context advice, that controls the persistence context for the main transaction
			_serviceFactory.Interceptors.Add(new PersistenceContextAdvice());

			// add audit advice inside of main persistence context advice,
			// so that the audit record will be inserted as part of the main transaction (this applies only to update contexts)
			_serviceFactory.Interceptors.Add(new AuditAdvice());
		}

        #region IServiceProvider Members

        public object GetService(Type serviceType)
        {
            if (_serviceFactory.HasService(serviceType))
            {
                return _serviceFactory.GetService(serviceType);
            }
            else
            {
                return null;    // as per MSDN
            }
        }

        #endregion
   }
}
