#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using System.Collections;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare.Workflow
{
    public abstract class WorklistItemBase : IWorklistItem
    {
        private IWorklistItemKey _key;

        protected WorklistItemBase(IWorklistItemKey key)
        {
            _key = key;
        }

        public IWorklistItemKey Key
        {
            get { return _key; }
            set { _key = value; }
        }

        public override bool Equals(object obj)
        {
            WorklistItemBase that = (WorklistItemBase)obj;
            return (that != null) &&
                (this.Key == that.Key);
        }

        public override int GetHashCode()
        {
            return this.Key.GetHashCode();
        }
    }

    public abstract class WorklistBase : IWorklist
    {
        #region IWorklist Members

        public virtual IList GetWorklist(Staff currentUserStaff, IPersistenceContext context)
        { return null; }

        public virtual int GetWorklistCount(Staff currentUserStaff, IPersistenceContext context)
        { return -1; }

        public virtual string DisplayName
        {
            get { return this.GetType().Name; }
        }

        #endregion
    }

    public abstract class WorklistBase<T> : WorklistBase where T : IPersistenceBroker
    {
        protected static T GetBroker(IPersistenceContext context)
        {
            return context.GetBroker<T>();
        }
    }
}
