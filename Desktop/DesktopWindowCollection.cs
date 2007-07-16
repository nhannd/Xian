using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Represents the collection of <see cref="DesktopWindow"/> objects in the application.
    /// </summary>
    public class DesktopWindowCollection : DesktopObjectCollection<DesktopWindow>
    {
        private Application _owner;
        private DesktopWindow _activeWindow;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="owner"></param>
        protected internal DesktopWindowCollection(Application owner)
        {
            _owner = owner;
        }

        #region Public methods

        /// <summary>
        /// Gets the currently active window
        /// </summary>
        public DesktopWindow ActiveWindow
        {
            get { return _activeWindow; }
        }

        /// <summary>
        /// Opens a new unnamed desktop window.  The window will have the default application title.
        /// </summary>
        /// <returns></returns>
        public DesktopWindow AddNew()
        {
            return AddNew(new DesktopWindowCreationArgs(null, null));
        }

        /// <summary>
        /// Opens a new desktop window with the specified name.
        /// The window will have the default application title.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public DesktopWindow AddNew(string name)
        {
            return AddNew(new DesktopWindowCreationArgs(null, name));
        }

        /// <summary>
        /// Opens a new desktop window with the specified creation arguments.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public DesktopWindow AddNew(DesktopWindowCreationArgs args)
        {
            DesktopWindow window = CreateWindow(args);
            Open(window);
            return window;
        }

        #endregion

        #region Protected overrides

        protected virtual DesktopWindow CreateWindow(DesktopWindowCreationArgs args)
        {
            IDesktopWindowFactory factory = CollectionUtils.FirstElement<IDesktopWindowFactory>(
                (new DesktopWindowFactoryExtensionPoint()).CreateExtensions()) ?? new DefaultDesktopWindowFactory();

            return factory.CreateWindow(args, _owner);
        }

        protected override void OnItemActivationChangedInternal(ItemEventArgs<DesktopWindow> args)
        {
            if (args.Item.Active)
            {
                // activated
                DesktopWindow lastActive = _activeWindow;

                // set this prior to firing any events, so that a call to ActiveWorkspace property will return correct value
                _activeWindow = args.Item;

                if (lastActive != null)
                {
                    lastActive.RaiseActiveChanged();
                }
                _activeWindow.RaiseActiveChanged();

            }
        }

        protected override void OnItemClosed(ClosedItemEventArgs<DesktopWindow> args)
        {
            if (this.Count == 0)
            {
                // raise pending de-activation event for the last active workspace, before the closing event
                if (_activeWindow != null)
                {
                    DesktopWindow lastActive = _activeWindow;

                    // set this prior to firing any events, so that a call to ActiveWorkspace property will return correct value
                    _activeWindow = null;
                    lastActive.RaiseActiveChanged();
                }
            }

            base.OnItemClosed(args);
        }

        #endregion
    }
}
