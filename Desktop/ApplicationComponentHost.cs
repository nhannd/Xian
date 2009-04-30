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

using System;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Abstract base class for application component hosts.
    /// </summary>
    public abstract class ApplicationComponentHost : IApplicationComponentHost
    {
        private IApplicationComponent _component;
        private IApplicationComponentView _componentView;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="component">The component to be hosted.</param>
        protected ApplicationComponentHost(IApplicationComponent component)
        {
            _component = component;
            _component.SetHost(this);
        }

        /// <summary>
        /// Starts the hosted component.
        /// </summary>
        public virtual void StartComponent()
        {
            if (_component.IsStarted)
				throw new InvalidOperationException(SR.ExceptionComponentAlreadyStarted);

            _component.Start();
        }

        /// <summary>
        /// Stops the hosted component.
        /// </summary>
        public virtual void StopComponent()
        {
            if (!_component.IsStarted)
				throw new InvalidOperationException(SR.ExceptionComponentNeverStarted);

            _component.Stop();
        }

        /// <summary>
        /// Gets a value indicating whether the hosted component has been started.
        /// </summary>
        public bool IsStarted
        {
            get { return _component.IsStarted; }
        }

        /// <summary>
        /// Gets the hosted component.
        /// </summary>
        public IApplicationComponent Component
        {
            get { return _component; }
        }

        /// <summary>
        /// Gets the view for the hosted component, creating it if it has not yet been created.
        /// </summary>
        public IApplicationComponentView ComponentView
        {
            get
            {
                if (_componentView == null)
                {
                    _componentView = (IApplicationComponentView)ViewFactory.CreateAssociatedView(_component.GetType());
                    _componentView.SetComponent(_component);
                }
                return _componentView;
            }
        }

        #region IApplicationComponentHost Members

        /// <summary>
        /// Asks the host to exit.
        /// </summary>
        /// <exception cref="NotSupportedException">The host does not support exit requests.</exception>
        public virtual void Exit()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets the associated command history object.
        /// </summary>
        /// <exception cref="NotSupportedException">The host does not support command history.</exception>
        public virtual CommandHistory CommandHistory
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Shows a message box in the associated desktop window.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="buttons"></param>
        /// <returns></returns>
        public virtual DialogBoxAction ShowMessageBox(string message, MessageBoxActions buttons)
        {
            return this.DesktopWindow.ShowMessageBox(message, this.Title, buttons);
        }

        /// <summary>
        /// Asks the host to set the title in the user-interface.
        /// </summary>
        /// <exception cref="NotSupportedException">The host does not support titles.</exception>
        public void SetTitle(string title)
        {
            this.Title = title;
        }

        /// <summary>
        /// Gets or sets the title displayed in the user-interface.
        /// </summary>
        /// <exception cref="NotSupportedException">The host does not support titles.</exception>
        public virtual string Title
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Gets the associated desktop window.
        /// </summary>
        public abstract DesktopWindow DesktopWindow { get; }

        #endregion
    }
}
