using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop
{
    public class QuittingEventArgs : EventArgs
    {
        private bool _cancel;
        private UserInteraction _interaction;

        public QuittingEventArgs(UserInteraction interaction, bool cancel)
        {
            _interaction = interaction;
            _cancel = cancel;
        }

        public UserInteraction Interaction
        {
            get { return _interaction; }
        }

        public bool Cancel
        {
            get { return _cancel; }
            set { _cancel = _cancel || value; }
        }

    }
}
