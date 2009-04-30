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
using System.Runtime.Serialization;
using System.Reflection;
using ClearCanvas.Common.Utilities;
using System.Collections;

namespace ClearCanvas.Enterprise.Support
{
    public class DomainObjectInfoExchange<TDomainObject, TDomainObjectInfo> : IInfoExchange
        where TDomainObject : DomainObject, new()
        where TDomainObjectInfo : DomainObjectInfo, new()
    {
        private List<IFieldExchange> _fieldExchangers = new List<IFieldExchange>();

        public DomainObjectInfoExchange()
        {
            // validation
            if(!typeof(TDomainObject).Equals(DomainObjectExchangeBuilder.GetAssociatedDomainClass(typeof(TDomainObjectInfo))))
                throw new Exception("Cannot convert between these types");

            // build the conversion
            _fieldExchangers.AddRange(
                DomainObjectExchangeBuilder.CreateFieldExchangers(typeof(TDomainObject), typeof(TDomainObjectInfo)));
        }

        protected IList<IFieldExchange> FieldExchangers
        {
            get { return _fieldExchangers; }
        }

        public TDomainObjectInfo GetInfoFromObject(TDomainObject obj, IPersistenceContext pctx)
        {
            if (obj == null) return null;
            TDomainObjectInfo info = new TDomainObjectInfo();
            foreach (IFieldExchange fe in _fieldExchangers)
            {
                fe.SetInfoFieldFromObject(obj, info, pctx);
            }
            return info;
        }

        public TDomainObject GetObjectFromInfo(TDomainObjectInfo info, IPersistenceContext pctx)
        {
            if (info == null) return null;
            TDomainObject obj = new TDomainObject();
            foreach (IFieldExchange fe in _fieldExchangers)
            {
                fe.SetObjectFieldFromInfo(obj, info, pctx);
            }
            return obj;
        }

        #region IConversion Members

        object IInfoExchange.GetInfoFromObject(object pobj, IPersistenceContext pctx)
        {
            return GetInfoFromObject((TDomainObject)pobj, pctx);
        }

        object IInfoExchange.GetObjectFromInfo(object info, IPersistenceContext pctx)
        {
            return GetObjectFromInfo((TDomainObjectInfo)info, pctx);
        }

        #endregion
    }

}
