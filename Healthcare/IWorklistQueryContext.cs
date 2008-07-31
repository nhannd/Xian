using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Healthcare
{
    /// <summary>
    /// Defines an interface that provides a <see cref="Worklist"/> with information about the context
    /// in which it is executing.
    /// </summary>
    public interface IWorklistQueryContext
    {
        /// <summary>
        /// Gets the staff on whose behalf the worklist is executing.
        /// </summary>
        Staff Staff { get; }

        /// <summary>
        /// Gets the working <see cref="Facility"/> for which the worklist is executing, or null if the working facility is not known.
        /// </summary>
        Facility WorkingFacility { get; }

		/// <summary>
		/// Gets a value indicating whether the worklist is being invoked in downtime recovery mode.
		/// </summary>
		bool DowntimeRecoveryMode { get; }

        /// <summary>
        /// Gets the <see cref="SearchResultPage"/> that specifies which page of the worklist is requested.
        /// </summary>
        SearchResultPage Page { get; }

        /// <summary>
        /// Obtains an instance of the specified broker.
        /// </summary>
        /// <typeparam name="TBrokerInterface"></typeparam>
        /// <returns></returns>
        TBrokerInterface GetBroker<TBrokerInterface>() where TBrokerInterface : IPersistenceBroker;
    }
}
