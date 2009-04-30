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
using System.Web;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.ImageServer.Web.Common
{
    /// <summary>
    /// 
    /// </summary>
    public class HttpContextData: IDisposable
    {
        private readonly IPersistentStore _store = PersistentStoreRegistry.GetDefaultStore();
        private const string CUSTOM_DATA_ENTRY = "CUSTOM_DATA_ENTRY";
        private IReadContext _readContext;
        private readonly object _syncRoot = new object();

        private HttpContextData()
        {
        }

        static public HttpContextData Current
        {
            get
            {
                lock( HttpContext.Current.Items.SyncRoot)
                {
                    HttpContextData instance = HttpContext.Current.Items[CUSTOM_DATA_ENTRY] as HttpContextData;
                    if (instance == null)
                    {
                        instance = new HttpContextData();
                        HttpContext.Current.Items[CUSTOM_DATA_ENTRY] = instance;
                    }
                    return instance;
                }
                
            }
        }

        public IReadContext ReadContext
        {
            get
            {
                lock (_syncRoot)
                {
                    if (_readContext == null)
                    {
                        _readContext = _store.OpenReadContext();
                    }
                    return _readContext;
                }
                
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (_readContext!=null)
            {
                _readContext.Dispose();
            }
        }

        #endregion
    }
}