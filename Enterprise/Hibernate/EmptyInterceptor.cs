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
using NHibernate;

namespace ClearCanvas.Enterprise.Hibernate
{
    internal class EmptyInterceptor : IInterceptor
    {
        #region IInterceptor Members

        /// <summary>
        /// Called when an entity is deleted
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <param name="propertyNames"></param>
        /// <param name="types"></param>
        public virtual void OnDelete(object entity, object id, object[] state, string[] propertyNames, NHibernate.Type.IType[] types)
        {
        }

        /// <summary>
        /// Called when a dirty entity is flushed, which implies an update
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="id"></param>
        /// <param name="currentState"></param>
        /// <param name="previousState"></param>
        /// <param name="propertyNames"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public virtual bool OnFlushDirty(object entity, object id, object[] currentState, object[] previousState, string[] propertyNames, NHibernate.Type.IType[] types)
        {
            return false;
        }

        /// <summary>
        /// Called when a new entity is saved for the first time
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <param name="propertyNames"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public virtual bool OnSave(object entity, object id, object[] state, string[] propertyNames, NHibernate.Type.IType[] types)
        {
            return false;

        }

        public virtual int[] FindDirty(object entity, object id, object[] currentState, object[] previousState, string[] propertyNames, NHibernate.Type.IType[] types)
        {
            return null;
        }

        public virtual object Instantiate(Type type, object id)
        {
            return null;
        }

        public virtual object IsUnsaved(object entity)
        {
            return null;
        }

        public virtual void PostFlush(System.Collections.ICollection entities)
        {
        }

        public virtual void PreFlush(System.Collections.ICollection entities)
        {
        }

        public virtual bool OnLoad(object entity, object id, object[] state, string[] propertyNames, NHibernate.Type.IType[] types)
        {
            return false;

        }

        #endregion
    }
}
