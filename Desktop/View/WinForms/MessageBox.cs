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
using System.Collections.Generic;
using System.Windows.Forms;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop.View.WinForms
{
	[ClearCanvas.Common.ExtensionOf(typeof(ClearCanvas.Common.MessageBoxExtensionPoint))]
    [GuiToolkit(GuiToolkitID.WinForms)]
	public class MessageBox : IMessageBox
	{
        /// <summary>
        /// Maps <see cref="ClearCanvas.Common.MessageBoxButtons"/> values to 
        /// <see cref="System.Windows.Forms.MessageBoxButtons"/> values.
        /// </summary>
        private static Dictionary<int, System.Windows.Forms.MessageBoxButtons> _buttonMap;
        private static Dictionary<DialogResult, int> _resultMap;

        static MessageBox() {

            _buttonMap = new Dictionary<int, System.Windows.Forms.MessageBoxButtons>();
            _buttonMap.Add((int)MessageBoxActions.Ok, MessageBoxButtons.OK);
            _buttonMap.Add((int)MessageBoxActions.OkCancel, MessageBoxButtons.OKCancel);
            _buttonMap.Add((int)MessageBoxActions.YesNo, MessageBoxButtons.YesNo);
            _buttonMap.Add((int)MessageBoxActions.YesNoCancel, MessageBoxButtons.YesNoCancel);

            _resultMap = new Dictionary<DialogResult, int>();
            _resultMap.Add(DialogResult.OK, (int)DialogBoxAction.Ok);
            _resultMap.Add(DialogResult.Cancel, (int)DialogBoxAction.Cancel);
            _resultMap.Add(DialogResult.Yes, (int)DialogBoxAction.Yes);
            _resultMap.Add(DialogResult.No, (int)DialogBoxAction.No);
        }

        public MessageBox()
        {
            // better not assume that SWF exists on a non-windows platform
            if (!Platform.IsWin32Platform)
                throw new NotSupportedException();
        }

		public void Show(string message)
		{
            Show(message, MessageBoxActions.Ok);
		}

        public DialogBoxAction Show(string message, ClearCanvas.Common.MessageBoxActions buttons)
        {
            return Show(message, null, buttons, null);
        }

        internal DialogBoxAction Show(string message, string title, ClearCanvas.Common.MessageBoxActions buttons, IWin32Window owner)
        {
            title = string.IsNullOrEmpty(title) ? Application.Name : string.Format("{0} - {1}", Application.Name, title);
            DialogResult dr = System.Windows.Forms.MessageBox.Show(owner,
                message, title, _buttonMap[(int)buttons]);
            return (DialogBoxAction)_resultMap[dr];
        }
    }
}
