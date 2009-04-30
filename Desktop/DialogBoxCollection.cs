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
    /// Represents the collection of <see cref="DialogBox"/> objects owned by a desktop window.
    /// </summary>
    internal class DialogBoxCollection : DesktopObjectCollection<DialogBox>
    {
        private DesktopWindow _owner;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="owner">The <see cref="DesktopWindow"/> that owns the dialog box.</param>
        internal DialogBoxCollection(DesktopWindow owner)
		{
            _owner = owner;
		}

        /// <summary>
        /// Creates a new dialog box with the specified arguments.
        /// </summary>
        internal DialogBox AddNew(DialogBoxCreationArgs args)
        {
            DialogBox dialog = CreateDialogBox(args);
            Open(dialog);
            return dialog;
        }

        /// <summary>
        /// Creates a new <see cref="DialogBox"/>.
        /// </summary>
        private DialogBox CreateDialogBox(DialogBoxCreationArgs args)
        {
            IDialogBoxFactory factory = CollectionUtils.FirstElement<IDialogBoxFactory>(
                (new DialogBoxFactoryExtensionPoint()).CreateExtensions()) ?? new DefaultDialogBoxFactory();

            return factory.CreateDialogBox(args, _owner);
        }

    }
}
