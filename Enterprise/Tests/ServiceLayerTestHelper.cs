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

namespace ClearCanvas.Enterprise.Tests
{
    public class ServiceLayerTestHelper
    {
        /// <summary>
        /// This method exposes the internal ServicLayer.CurrentContext setter.  This method may be used when a ServiceLayer-derived object is manually instantiated 
        /// outside of the normal framework mechanism (eg when testing via NUnit) to manually set the PersistenceContext.  Allows for a mock PersistenceContext to be 
        /// used, which in turn allows mock EntityBroker objects to be used to return test data.
        /// </summary>
        /// <example>
        /// <code>
        ///    _mocks = new Mockery();
        ///    _mockPersistanceContext = _mocks.NewMock&lt;IPersistenceContext&gt;();
        ///    _service= new ServiceLayer();
        ///    ServiceLayerTestHelper.SetServiceLayerPersistenceContext(
        ///        _service, _mockPersistanceContext);
        /// </code>
        /// </example>
        /// <see>ClearCanvas.Ris.Services.Tests.AdtServiceTest</see>
        /// <param name="serviceLayer"></param>
        /// <param name="context"></param>
        public static void SetServiceLayerPersistenceContext(ServiceLayer serviceLayer, IPersistenceContext context)
        {
            serviceLayer.CurrentContext = context;
        }
    }
}
