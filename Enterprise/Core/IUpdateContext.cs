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

namespace ClearCanvas.Enterprise.Core
{
    /// <summary>
    /// Defines the interface to an update context.  An update context allows the application read
    /// data from a persistent store, and to synchronize the persistent store with changes made to that data.
    /// </summary>
    public interface IUpdateContext : IPersistenceContext
    {
        /// <summary>
        /// Gets or sets the entity validator that validates domain objects prior to persistence.
        /// </summary>
        //IValidator Validator { get; set; }

        /// <summary>
        /// Gets or sets the change-set recorder that the context will use to create
        /// a record of the changes that were made.
        /// </summary>
        /// <remarks>
        /// Setting this property to null will effectively disable this auditing.
        /// </remarks>
        IEntityChangeSetRecorder ChangeSetRecorder { get; set; }

        /// <summary>
        /// Attempts to flush and commit all changes made within this update context to the persistent store.
        /// If this operation succeeds, the state of the persistent store will be syncrhonized with the state
        /// of all domain objects that are attached to this context, and the context can continue to be used
        /// for read operations only.
        /// If the operation fails, an exception will be thrown.
        /// </summary>
        void Commit();
    }
}
