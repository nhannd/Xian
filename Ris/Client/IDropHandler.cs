using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// Defines an interface for handling drag and drop operations
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public interface IDropHandler<TItem>
    {
        /// <summary>
        /// Return true if the specified items can be accepted by this handler
        /// </summary>
        /// <param name="dropContext">Provides information about the context of the drop operation.  The argument passed will
        /// typically extend the <see cref="IDropContext"/> interface in order to provide additional data, and the handler 
        /// will need to cast to a known subtype.</param>
        /// <param name="items">The items being dropped</param>
        /// <returns></returns>
        bool CanAcceptDrop(IDropContext dropContext, ICollection<TItem> items);

        /// <summary>
        /// Return true if the specified items were successfully processed by this handler
        /// </summary>
        /// <param name="dropContext">Provides information about the context of the drop operation.  The argument passed will
        /// typically extend the <see cref="IDropContext"/> interface in order to provide additional data, and the handler 
        /// will need to cast to a known subtype.</param>
        /// <param name="items">The items being dropped</param>
        /// <returns></returns>
        bool ProcessDrop(IDropContext dropContext, ICollection<TItem> items);
    }
}
