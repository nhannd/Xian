using System;
using System.Collections;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Event args for the <see cref="ISuggestionProvider.SuggestionsProvided"/> event.
    /// </summary>
    public class SuggestionsProvidedEventArgs : EventArgs
    {
        private readonly IList _items;

        /// <summary>
        /// Constructor.
        /// </summary>
        public SuggestionsProvidedEventArgs(IList items)
        {
            _items = items;
        }

        /// <summary>
        /// Gets the list of suggested items.
        /// </summary>
        public IList Items
        {
            get { return _items; }
        }
    }

    /// <summary>
    /// Defines an interface to an object that provides suggestions dynamically based on text entered by the user.
    /// </summary>
    /// <remarks>
    /// This interface has been designed so that the implementation may optionally provide suggestions
    /// in an asynchronous manner.  The <see cref="SetQuery"/> method is called repeatedly as the 
    /// user modifies the query string in the user-interface.  The implementation can respond by raising
    /// the <see cref="SuggestionsProvided"/> event to provide a list of suggestions based on the query.
    /// </remarks>
    public interface ISuggestionProvider
    {
        /// <summary>
        /// Notifies the user-interfaces that an updated list of suggestions is available.
        /// </summary>
        event EventHandler<SuggestionsProvidedEventArgs> SuggestionsProvided;

        /// <summary>
        /// Called by the user-inteface to inform this object of changes in the user query text.
        /// </summary>
        void SetQuery(string query);
    }
}
