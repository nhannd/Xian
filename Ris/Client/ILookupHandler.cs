using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// Defines an interface to an object that supports a lookup field on the user-interface.
    /// </summary>
    public interface ILookupHandler
    {
        /// <summary>
        /// Attempts to resolve a query to a single item, optionally interacting with the user.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="query">The text query.</param>
        /// <param name="interactive">True if interaction with the user is allowed.</param>
        /// <param name="result">The singular result.</param>
        /// <returns>True if the query was resolved to a singular item, otherwise false.</returns>
        bool Resolve(string query, bool interactive, out object result);

        /// <summary>
        /// Formats an item for display in the user-interface.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        string FormatItem(object item);

        /// <summary>
        /// Gets a suggestion provider that provides suggestions for the lookup field.
        /// </summary>
        ISuggestionProvider SuggestionProvider { get; }
    }
}
