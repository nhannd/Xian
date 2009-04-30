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

using System.Collections.Generic;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.ImageServer.Enterprise
{
    public delegate void SelectCallback<T>(T row);

    /// <summary>
    /// An interface for an <see cref="IPersistenceBroker"/> for accessing <see cref="ServerEntity"/>
    /// objects in the persistent store.
    /// </summary>
    /// <remarks>
    /// This interface allows for loading, updating, inserting, and selecting <see cref="ServerEntity"/>
    /// derived objects from the database.  
    /// </remarks>
    /// <typeparam name="TServerEntity"></typeparam>
    /// <typeparam name="TSelectCriteria"></typeparam>
    /// <typeparam name="TUpdateColumns"></typeparam>
    public interface IEntityBroker<TServerEntity, TSelectCriteria, TUpdateColumns> : IPersistenceBroker
        where TServerEntity : ServerEntity, new()
        where TSelectCriteria : EntitySelectCriteria
        where TUpdateColumns : EntityUpdateColumns
    {
        /// <summary>
        /// Loads the <see cref="ServerEntity"/> referred to by the specified entity reference.
        /// </summary>
        /// <param name="entityRef">The key for the entity to load.</param>
        /// <returns></returns>
        TServerEntity Load(ServerEntityKey entityRef);

        /// <summary>
        /// Retrieves all entities matching the specified criteria.
        /// Caution: this method may return an arbitrarily large result set.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>A list of <see cref="ServerEntity"/> objects.</returns>
        IList<TServerEntity> Find(TSelectCriteria criteria);

        /// <summary>
        /// Retrieves all entities matching the specified criteria.
        /// Caution: this method may return an arbitrarily large result set.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="maxRows">Number of  rows to return.</param>
        /// <param name="startIndex">The start index (zero based) from which rows should be returned.</param>
        /// <returns>A list of <see cref="ServerEntity"/> objects.</returns>
        IList<TServerEntity> Find(TSelectCriteria criteria, int startIndex, int maxRows);

		/// <summary>
		/// Retrieves an entity matching the specified criteria.
		/// </summary>
		/// <param name="criteria">The criteria.</param>
		/// <returns>A <see cref="ServerEntity"/> object, or null if no results.</returns>
		TServerEntity FindOne(TSelectCriteria criteria);

        /// <summary>
        /// Retrieves all entities matching the specified criteria.
        /// Caution: this method may return an arbitrarily large result set.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="callback">A callback which is called for each result found.</param>
        void Find(TSelectCriteria criteria, SelectCallback<TServerEntity> callback);

        /// <summary>
        /// Retrieves all entities matching the specified criteria.
        /// Caution: this method may return an arbitrarily large result set.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="maxRows">Number of  rows to return.</param>
		/// <param name="startIndex">The start index (zero based) from which rows should be returned.</param>
		/// <param name="callback">A callback which is called for each result found.</param>
		void Find(TSelectCriteria criteria, int startIndex, int maxRows, SelectCallback<TServerEntity> callback);

        /// <summary>
        /// Retrieves a count of the entities within the persistent store that
        /// match the specified criteria.
        /// </summary>
        /// <param name="criteria">The input criteria</param>
        /// <returns>The number or resultant rows.</returns>
        int Count(TSelectCriteria criteria);

        /// <summary>
        /// Updates the entity specified by the <paramref name="entityKey"/> with values specified in <paramref="parameters"/>.
        /// </summary>
        /// <param name="entityKey">The <see cref="ServerEntityKey"/> object whose <see cref="ServerEntityKey.Key"/> references to the object to be updated.</param>
        /// <param name="parameters">The <see cref="EntityUpdateColumns"/> specifying the columns to be updated.</param>
        /// <returns></returns>
        bool Update(ServerEntityKey entityKey, TUpdateColumns parameters);

        /// <summary>
        /// Updates the entity specified by the <paramref name="entity"/>.
        /// </summary>
        /// <param name="entity">The <see cref="ServerEntityKey"/> object whose <see cref="ServerEntityKey.Key"/> references to the object to be updated.</param>
        /// <returns></returns>
        bool Update(TServerEntity entity);


		/// <summary>
		/// Updates the entity specified by the <paramref name="criteria"/> with values updated specified in <paramref="parameters"/>.
		/// </summary>
		/// <param name="criteria">The criteria for the WHERE clause of the update.</param>
		/// <param name="parameters">The <see cref="EntityUpdateColumns"/> specifying the columns to be updated.</param>
		/// <returns></returns>
		bool Update(TSelectCriteria criteria, TUpdateColumns parameters);

        /// <summary>
        /// Inserts a new entity with field values indicated in <paramref name="parameters"/>.
        /// </summary>
        /// <param name="parameters">The <see cref="EntityUpdateColumns"/> object which specifies the values for the columns in the new entity.</param>
        /// <returns>The newly inserted entity.</returns>
        TServerEntity Insert(TUpdateColumns parameters);

        /// <summary>
        /// Delete an entity.
        /// </summary>
        /// <param name="entityKey">The key for the entity to delete.</param>
        /// <returns>true on success, false on failure</returns>
        bool Delete(ServerEntityKey entityKey);

        /// <summary>
        /// Delete entities matching specified criteria.
        /// </summary>
        /// <param name="criteria">The criteria for the entities to delete.</param>
        /// <returns>Number of records deleted</returns>
        int Delete(TSelectCriteria criteria);
    }
}
