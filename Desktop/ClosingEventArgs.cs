using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Defines possible reasons why a <see cref="DesktopObject"/> might close.
    /// </summary>
    public enum CloseReason
    {
        /// <summary>
        /// The user is closing the object directly via the user-interface.
        /// </summary>
        UserInterface,

        /// <summary>
        /// The object is being closed programmatically.
        /// </summary>
        Program,

        /// <summary>
        /// The application is exiting.
        /// </summary>
        ApplicationQuit
    }

    /// <summary>
    /// Provides data for Closing events, where the request may need to be cancelled.
    /// </summary>
    public class ClosingEventArgs : EventArgs
    {
        private CloseReason _reason;
        private bool _cancel;
        private UserInteraction _interaction;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="reason"></param>
        /// <param name="interaction"></param>
        public ClosingEventArgs(CloseReason reason, UserInteraction interaction)
            : this(reason, interaction, false)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="reason"></param>
        /// <param name="interaction"></param>
        /// <param name="cancel"></param>
        public ClosingEventArgs(CloseReason reason, UserInteraction interaction, bool cancel)
        {
            _reason = reason;
            _interaction = interaction;
            _cancel = cancel;
        }

        /// <summary>
        /// Gets the reason the object is closing.
        /// </summary>
        public CloseReason Reason
        {
            get { return _reason; }
        }

        /// <summary>
        /// Gets the user-interaction policy for this closing operation, which handlers must abide by.
        /// </summary>
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

    /// <summary>
    /// Provides data for Closing events, where the request may need to be cancelled.
    /// </summary>
    public class ClosingItemEventArgs<TItem> : ItemEventArgs<TItem>
    {
        private bool _cancel;
        private CloseReason _reason;
        private UserInteraction _interaction;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="item"></param>
        /// <param name="reason"></param>
        /// <param name="interaction"></param>
        /// <param name="cancel"></param>
        public ClosingItemEventArgs(TItem item, CloseReason reason, UserInteraction interaction, bool cancel)
            :base(item)
        {
            _reason = reason;
            _cancel = cancel;
            _interaction = interaction;
        }

        /// <summary>
        /// Gets the reason the item is closing.
        /// </summary>
        public CloseReason Reason
        {
            get { return _reason; }
        }

        /// <summary>
        /// Gets the user-interaction policy for this closing operation, which handlers must abide by.
        /// </summary>
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
