using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare
{
    [ExtensionPoint]
    public class NoteboxExtensionPoint : ExtensionPoint<INotebox>
    {
    }

    /// <summary>
    /// Defines an interface to a notebox.
    /// </summary>
    public interface INotebox
    {
        /// <summary>
        /// Queries the notebox for its contents.
        /// </summary>
        /// <param name="nqc"></param>
        /// <returns></returns>
        IList GetItems(INoteboxQueryContext nqc);

        /// <summary>
        /// Queries the notebox for a count of its contents.
        /// </summary>
        /// <param name="nqc"></param>
        /// <returns></returns>
        int GetItemCount(INoteboxQueryContext nqc);
    }

    /// <summary>
    /// Defines an interface that provides a <see cref="Worklist"/> with information about the context
    /// in which it is executing.
    /// </summary>
    public interface INoteboxQueryContext
    {
        /// <summary>
        /// Gets the staff on whose behalf the notebox query is executing.
        /// </summary>
        Staff Staff { get; }

        /// <summary>
        /// Gets the working <see cref="Facility"/> for which the notebox query is executing, or null if the working facility is not known.
        /// </summary>
        Facility WorkingFacility { get; }

        /// <summary>
        /// Gets the <see cref="SearchResultPage"/> that specifies which page of the notebox is requested.
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
