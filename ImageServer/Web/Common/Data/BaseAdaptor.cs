#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Enterprise;

namespace ClearCanvas.ImageServer.Web.Common.Data
{
    public class BaseAdaptor<TServerEntity, TIEntity, TCriteria, TColumns>
        where TServerEntity : ServerEntity, new()
        where TCriteria : EntitySelectCriteria, new()
        where TColumns : EntityUpdateColumns
        where TIEntity : IEntityBroker<TServerEntity, TCriteria, TColumns>
    {
        #region Private Members

        private readonly IPersistentStore _store = PersistentStoreRegistry.GetDefaultStore();

        #endregion Private Members

        #region Protected Properties

        protected IPersistentStore PersistentStore
        {
            get { return _store; }
        }

        #endregion

        #region Public Methods

        public IList<TServerEntity> Get()
        {
            using (IReadContext ctx = PersistentStore.OpenReadContext())
            {
                TIEntity find = ctx.GetBroker<TIEntity>();
                TCriteria criteria = new TCriteria();
                IList<TServerEntity> list = find.Find(criteria);

                return list;
            }
        }

        public TServerEntity Get(ServerEntityKey key)
        {
            using (IReadContext ctx = PersistentStore.OpenReadContext())
            {
                TIEntity select = ctx.GetBroker<TIEntity>();
                return select.Load(key);
            }
        }

        public IList<TServerEntity> Get(TCriteria criteria)
        {
            using (IReadContext ctx = PersistentStore.OpenReadContext())
            {
                TIEntity select = ctx.GetBroker<TIEntity>();
                return select.Find(criteria);
            }
        }

        public bool Add(TColumns param)
        {
            try
            {
                using (IUpdateContext context = PersistentStore.OpenUpdateContext(UpdateContextSyncMode.Flush))
                {
                    TIEntity update = context.GetBroker<TIEntity>();

                    update.Insert(param);

                    context.Commit();
                }
                return true;
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e, "Unexpected exception adding {0}", typeof (TServerEntity));
                return false;
            }
        }

        public bool Update(ServerEntityKey key, TColumns param)
        {
            try
            {
                using (IUpdateContext context = PersistentStore.OpenUpdateContext(UpdateContextSyncMode.Flush))
                {
                    TIEntity update = context.GetBroker<TIEntity>();

                    update.Update(key, param);

                    context.Commit();
                }
                return true;
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e, "Unexpected exception updating {0}", typeof (TServerEntity));
                return false;
            }
        }

        public bool Delete(ServerEntityKey key)
        {
            try
            {
                using (IUpdateContext context = PersistentStore.OpenUpdateContext(UpdateContextSyncMode.Flush))
                {
                    TIEntity update = context.GetBroker<TIEntity>();

                    if (!update.Delete(key))
                        return false;

                    context.Commit();
                }
                return true;
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e, "Unexpected exception updating {0}", typeof (TServerEntity));
                return false;
            }
        }

        #endregion
    }
}
