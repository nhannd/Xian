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

using System.Drawing;
namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Holds parameters that control the creation of a <see cref="DialogBox"/>.
    /// </summary>
    public class DialogBoxCreationArgs : DesktopObjectCreationArgs
    {
        private IApplicationComponent _component;
        private DialogSizeHint _sizeHint;
        private Size _size = Size.Empty;

        /// <summary>
        /// Constructor.
        /// </summary>
        public DialogBoxCreationArgs()
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="component">The component to be hosted in the dialog.</param>
        /// <param name="title">The title to assign to the dialog.</param>
        /// <param name="name">The name/identifier of the dialog.</param>
        /// <param name="sizeHint">The size hint for the dialog.</param>
        public DialogBoxCreationArgs(IApplicationComponent component, string title, string name, DialogSizeHint sizeHint)
            :base(title, name)
        {
            _component = component;
            _sizeHint = sizeHint;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="component">The component to be hosted in the dialog.</param>
        /// <param name="title">The title to assign to the dialog.</param>
        /// <param name="name">The name/identifier of the dialog.</param>
        /// <param name="size">The size of the dialog in pixels.</param>
        public DialogBoxCreationArgs(IApplicationComponent component, string title, string name, Size size)
            : base(title, name)
        {
            _component = component;
            _size = size;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="component">The component to be hosted in the dialog.</param>
        /// <param name="title">The title to assign to the dialog.</param>
        /// <param name="name">The name/identifier of the dialog.</param>
        public DialogBoxCreationArgs(IApplicationComponent component, string title, string name)
            : base(title, name)
        {
            _component = component;
        }

        /// <summary>
        /// Gets or sets the component to host.
        /// </summary>
        public IApplicationComponent Component
        {
            get { return _component; }
            set { _component = value; }
        }

        /// <summary>
        /// Gets or sets the size hint for the dialog box.
        /// </summary>
        /// <seealso cref="Size"/>
        public DialogSizeHint SizeHint
        {
            get { return _sizeHint; }
            set { _sizeHint = value; }
        }

        /// <summary>
        /// Gets or sets an explicit size for the dialog in pixels.  If specified, this property will override <see cref="SizeHint"/>. 
        /// </summary>
        public Size Size
        {
            get { return _size; }
            set { _size = value; }
        }
    }
}
