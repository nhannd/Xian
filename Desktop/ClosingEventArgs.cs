using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop
{
    public enum CloseReason
    {
        /// <summary>
        /// The user is trying to close the object directly via the user-interface
        /// </summary>
        UserInterface,

        /// <summary>
        /// The object is being closed programmatically
        /// </summary>
        Program,

        /// <summary>
        /// The application is exiting
        /// </summary>
        ApplicationQuit
    }

    /// <summary>
    /// Event args used for "closing" events, where the request may need to be cancelled
    /// </summary>
    public class ClosingEventArgs : EventArgs
    {
        private CloseReason _reason;
        private bool _cancel;
        private UserInteraction _interaction;

        public ClosingEventArgs(CloseReason reason, UserInteraction interaction)
            : this(reason, interaction, false)
        {
        }

        public ClosingEventArgs(CloseReason reason, UserInteraction interaction, bool cancel)
        {
            _reason = reason;
            _interaction = interaction;
            _cancel = cancel;
        }

        public CloseReason Reason
        {
            get { return _reason; }
        }

        public UserInteraction Interaction
        {
            get { return _interaction; }
        }

        // maybe we can expose this later if needed
        internal bool Cancel
        {
            get { return _cancel; }
            set { _cancel = _cancel || value; }
        }
    }

    public class ClosingItemEventArgs<TItem> : ItemEventArgs<TItem>
    {
        private bool _cancel;
        private CloseReason _reason;
        private UserInteraction _interaction;

        public ClosingItemEventArgs(TItem item, CloseReason reason, UserInteraction interaction, bool cancel)
            :base(item)
        {
            _reason = reason;
            _cancel = cancel;
            _interaction = interaction;
        }

        public CloseReason Reason
        {
            get { return _reason; }
        }

        public UserInteraction Interaction
        {
            get { return _interaction; }
        }


        // maybe we can expose this later if needed
        internal bool Cancel
        {
            get { return _cancel; }
            set
            {
                // don't allow uncancelling
                _cancel = _cancel || value;
            }
        }
    }
}
