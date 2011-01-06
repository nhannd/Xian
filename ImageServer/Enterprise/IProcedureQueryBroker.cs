#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.ImageServer.Enterprise
{
    public delegate void ProcedureQueryCallback<T>(T row);

    /// <summary>
    /// Interface used to define stored procedures that that input parameters and return resultant rows.
    /// </summary>
    /// <typeparam name="TInput">Input parameters</typeparam>
    /// <typeparam name="TOutput">The return type</typeparam>
    public interface IProcedureQueryBroker<TInput, TOutput> : IPersistenceBroker
        where TInput : ProcedureParameters
        where TOutput : ServerEntity, new()
    {
        /// <summary>
        /// Retrieves all entities matching the specified criteria.
        /// Caution: this method may return an arbitrarily large result set.
        /// </summary>
        /// <param name="criteria">The search criteria.</param>
        /// <returns>A list of entities.</returns>
        IList<TOutput> Find(TInput criteria);

        /// <summary>
        /// Retrieves all entities matching the specified criteria,
        /// constrained by the specified page constraint.
        /// </summary>
        /// <param name="criteria">The search criteria.</param>
        /// <param name="callback">The delegate which is supplied query results.</param>
        void Find(TInput criteria, ProcedureQueryCallback<TOutput> callback);

		/// <summary>
		/// Retrieves the first entity matching the specified crtiera.
		/// </summary>
		/// <param name="criteria">The search criteria.</param>
		/// <returns>The entity.</returns>
    	TOutput FindOne(TInput criteria);
    }
}
