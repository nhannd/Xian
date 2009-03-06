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

namespace ClearCanvas.Enterprise.Core
{
    /// <summary>
    /// Defines the interface to a persistent store, and acts as a factory for obtaining instances of 
    /// <see cref="IReadContext"/> and <see cref="IUpdateContext"/>.
    /// </summary>
    public interface IPersistentStore
    {
        /// <summary>
        /// Called by the framework to initialize the persistent store.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Sets the transaction notifier, which the store can use to notify the application of changes
        /// made to its entities.
        /// </summary>
        /// <param name="transactionNotifier"></param>
        void SetTransactionNotifier(ITransactionNotifier transactionNotifier);

        /// <summary>
        /// Obtains an <see cref="IReadContext"/> for use by the application to interact
        /// with this persistent store.
        /// </summary>
        /// <returns>a read context</returns>
        IReadContext OpenReadContext();

        /// <summary>
        /// Obtains an <see cref="IUpdateContext"/> for use by the application to interact
        /// with this persistent store.
        /// </summary>
        /// <returns>a update context</returns>
        IUpdateContext OpenUpdateContext(UpdateContextSyncMode mode);

		/// <summary>
		/// The version associated with the persistent store.
		/// </summary>
		Version Version { get; }
    }
}
