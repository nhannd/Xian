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
using System.Text;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.ImageServer.Enterprise
{
    /// <summary>
    /// Defines the interface for an non-procedural update broker.
    /// </summary>
    /// <typeparam name="TEntity">The type of the object to be updated/inserted.</typeparam>
    /// <typeparam name="TUpdateBrokerParameters">The parameter type derived from <see cref="UpdateBrokerParameters"/></typeparam>
    /// <remarks>
    /// <para>
    /// Unlike <see cref="IProcedureUpdateBroker"/> brokers which take a fixed number of parameters, a broker 
    /// implementing <see cref="IUpdateBroker"/> provides database update functionality with variable number of parameters.
    /// Only the fields whose values are specified will be updated. This is useful if only certain fields in the record need to be updated.
    /// The broker generates a dynamic UPDATE SQL statement based on the specified parameters.
    /// </para>
    /// <para>
    /// <see cref="IUpdateBroker"/> brokers also provides a method to insert a new record into the database.
    /// </para>
    /// </remarks>
    public interface IUpdateBroker<TEntity, TUpdateBrokerParameters> : IPersistenceBroker
        where TUpdateBrokerParameters : UpdateBrokerParameters
        where TEntity : ServerEntity, new()
    {
        /// <summary>
        /// Updates the entity specified by the <paramref name="entityKey"/> with values specified in <paramref="parameters"/>.
        /// </summary>
        /// <param name="entityKey">The <see cref="ServerEntitykey"/> object whose <see cref="ServerEntityKey.Key"/> references to the object to be updated.</param>
        /// <param name="parameters">The <see cref="UpdateBrokerParameters"/> specifying the columns to be updated.</param>
        /// <returns></returns>
        bool Update(ServerEntityKey entityKey, TUpdateBrokerParameters parameters);

        /// <summary>
        /// Inserts a new entity with field values indicated in <paramref name="parameters"/>.
        /// </summary>
        /// <param name="paramters">The <see cref="UpdateBrokerParameters"/> object which specifies the values for the columns in the new entity.</param>
        /// <returns>References to the newly inserted entity.</returns>
        TEntity Insert(TUpdateBrokerParameters parameters);
     
    }
}
