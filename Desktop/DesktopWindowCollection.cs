#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Represents the collection of <see cref="DesktopWindow"/> objects in the application.
    /// </summary>
    public sealed class DesktopWindowCollection : DesktopObjectCollection<DesktopWindow>
    {
        private Application _owner;
        private DesktopWindow _activeWindow;

        /// <summary>
        /// Constructor.
        /// </summary>
        internal DesktopWindowCollection(Application owner)
        {
            _owner = owner;
        }

        #region Public methods

        /// <summary>
        /// Gets the currently active window.
        /// </summary>
        public DesktopWindow ActiveWindow
        {
            get { return _activeWindow; }
        }

        /// <summary>
        /// Opens a new unnamed desktop window.
        /// </summary>
        public DesktopWindow AddNew()
        {
            return AddNew(new DesktopWindowCreationArgs(null, null));
        }

        /// <summary>
        /// Opens a new desktop window with the specified name.
        /// </summary>
        /// <remarks>
        /// <see cref="DesktopWindow"/> names must be unique within a collection or an exception will be thrown.
        /// </remarks>
        public DesktopWindow AddNew(string name)
        {
            return AddNew(new DesktopWindowCreationArgs(null, name));
        }

        /// <summary>
        /// Opens a new desktop window with the specified creation arguments.
        /// </summary>
        public DesktopWindow AddNew(DesktopWindowCreationArgs args)
        {
            DesktopWindow window = CreateWindow(args);
            Open(window);
            return window;
        }

        #endregion

        #region Protected overrides

		/// <summary>
		/// Called when a <see cref="DesktopWindow"/> item's <see cref="DesktopObject.InternalActiveChanged"/> event
		/// has fired.
		/// </summary>
		protected sealed override void OnItemActivationChangedInternal(ItemEventArgs<DesktopWindow> args)
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

		/// <summary>
		/// Called when a <see cref="DesktopWindow"/> item's <see cref="DesktopObject.Closed"/> event
		/// has fired.
		/// </summary>
		protected sealed override void OnItemClosed(ClosedItemEventArgs<DesktopWindow> args)
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

        /// <summary>
        /// Creates a new <see cref="DesktopWindow"/>.
        /// </summary>
        private DesktopWindow CreateWindow(DesktopWindowCreationArgs args)
        {
            IDesktopWindowFactory factory = CollectionUtils.FirstElement<IDesktopWindowFactory>(
                (new DesktopWindowFactoryExtensionPoint()).CreateExtensions()) ?? new DefaultDesktopWindowFactory();

            return factory.CreateWindow(args, _owner);
        }

    }
}
