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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Enterprise.Core
{
    /// <summary>
    /// Used by <see cref="IEntityBroker{TEntity, TSearchCriteria}.Load"/> to provide fine control over the loading of entities.
    /// </summary>
    [Flags]
    public enum EntityLoadFlags
    {
        /// <summary>
        /// Default value
        /// </summary>
        None = 0x0000,

        /// <summary>
        /// Forces a version check, causing an exception to be thrown if the version does not match
        /// </summary>
        CheckVersion = 0x0001,

        /// <summary>
        /// Asks for a proxy to the entity, rather than loading the full entity.  There is no guarantee
        /// that this flag will be obeyed, because the underlying implementation may not support proxies,
        /// or the entity may not be proxiable.
        /// </summary>
        Proxy = 0x0002,
    }


    /// <summary>
    /// Base interface for all entity broker interfaces.  An entity broker allows entities to be retrieved
    /// from persistent storage. This interface should not be implemented directly.
    /// Instead, a sub-interface should be defined that extends this interface for a given entity.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="Entity"/> sub-class on which this broker acts</typeparam>
    /// <typeparam name="TSearchCriteria">The <see cref="SearchCriteria"/> subclass corresponding to the entity</typeparam>
    public interface IEntityBroker<TEntity, TSearchCriteria> : IPersistenceBroker
        where TEntity: Entity
        where TSearchCriteria : SearchCriteria
    {
        /// <summary>
        /// Loads the entity referred to by the specified entity reference.
        /// </summary>
        /// <param name="entityRef"></param>
        /// <returns></returns>
        TEntity Load(EntityRef entityRef);

        /// <summary>
        /// Loades the entity referred to by the specified reference, obeying the specified flags
        /// where possible.
        /// </summary>
        /// <param name="entityRef"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        TEntity Load(EntityRef entityRef, EntityLoadFlags flags);

        /// <summary>
        /// Retrieves all entities matching the specified criteria.
        /// </summary>
        /// <remarks>
        /// Caution: this method may return an arbitrarily large result set.
        /// </remarks>
        /// <param name="criteria"></param>
        /// <returns></returns>
        IList<TEntity> Find(TSearchCriteria criteria);

        /// <summary>
        /// Retrieves all entities matching any of the specified criteria (the criteria are combined using OR).
        /// </summary>
        /// <remarks>
        /// Caution: this method may return an arbitrarily large result set.
        /// </remarks>
        /// <param name="criteria"></param>
        /// <returns></returns>
        IList<TEntity> Find(TSearchCriteria[] criteria);

        /// <summary>
        /// Retrieves all entities matching the specified criteria,
        /// constrained by the specified page constraint.
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        IList<TEntity> Find(TSearchCriteria criteria, SearchResultPage page);

        /// <summary>
        /// Retrieves all entities matching any of the specified criteria (the criteria are combined using OR),
        /// constrained by the specified page constraint.
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        IList<TEntity> Find(TSearchCriteria[] criteria, SearchResultPage page);

        /// <summary>
        /// Retrieves all entities matching any of the specified criteria (the criteria are combined using OR),
        /// constrained by the specified page constraint.
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="page"></param>
        /// <param name="cache"></param>
        /// <returns></returns>
        IList<TEntity> Find(TSearchCriteria[] criteria, SearchResultPage page, bool cache);

        /// <summary>
        /// Retrieves the entire set of entities of this class.
        /// </summary>
        /// <remarks>
        /// Caution: this method may return an arbitrarily large result set.
        /// </remarks>
        /// <returns></returns>
        IList<TEntity> FindAll();

		/// <summary>
		/// Retrieves the entire set of entities of this class, optionally including de-activated instances.
		/// </summary>
		/// <remarks>
		/// Caution: this method may return an arbitrarily large result set.
		/// </remarks>
		/// <returns></returns>
		IList<TEntity> FindAll(bool includeDeactivated);

        /// <summary>
        /// Retrieves the first entity matching the specified criteria, or throws a <see cref="EntityNotFoundException"/>
        /// if no matching entity is found.
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        TEntity FindOne(TSearchCriteria criteria);

        /// <summary>
        /// Retrieves the first entity matching any of the specified criteria (the criteria are combined using OR),
        /// or throws a <see cref="EntityNotFoundException"/> if no matching entity is found.
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        TEntity FindOne(TSearchCriteria[] criteria);


        /// <summary>
        /// Returns the number of entities matching the specified criteria.
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        long Count(TSearchCriteria criteria);

        /// <summary>
        /// Returns the number of entities matching any of the specified criteria (the criteria are combined using OR).
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        long Count(TSearchCriteria[] criteria);


        /// <summary>
        /// Makes the specified entity transient (removes it from the persistent store).
        /// </summary>
        /// <param name="entity">The entity instance to remove</param>
        void Delete(TEntity entity);
    }
}
