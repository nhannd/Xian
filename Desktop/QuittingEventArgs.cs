using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Provides data about the <see cref="Application.Quitting"/> event.
    /// </summary>
    public class QuittingEventArgs : EventArgs
    {
        private bool _cancel;
        private UserInteraction _interaction;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="interaction"></param>
        /// <param name="cancel"></param>
        public QuittingEventArgs(UserInteraction interaction, bool cancel)
        {
            _interaction = interaction;
            _cancel = cancel;
        }

        /// <summary>
        /// Gets the user interaction policy.
        /// </summary>
        public UserInteraction Interaction
        {
            get { return _interaction; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the operation should be cancelled.
        /// </summary>
        public bool Cancel
        {
            get { return _cancel; }
            set { _cancel = _cancel || value; }
        }

    }
}
