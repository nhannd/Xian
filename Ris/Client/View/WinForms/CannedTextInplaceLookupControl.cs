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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ClearCanvas.Desktop;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common;

namespace ClearCanvas.Ris.Client.View.WinForms
{
    public partial class CannedTextInplaceLookupControl : UserControl
    {
        private ICannedTextLookupHandler _lookupHandler;

        private event EventHandler _committed;
        private event EventHandler _cancelled;

        public CannedTextInplaceLookupControl(ICannedTextLookupHandler lookupHandler)
        {
            InitializeComponent();
            _lookupHandler = lookupHandler;
            _suggestBox.SuggestionProvider = _lookupHandler.SuggestionProvider;
        }

        public event EventHandler Committed
        {
            add { _committed += value; }
            remove { _committed -= value; }
        }

        public event EventHandler Cancelled
        {
            add { _cancelled += value; }
            remove { _cancelled -= value; }
        }

        public object Value
        {
            get { return _suggestBox.Value; }
            set { _suggestBox.Value = value; }
        }

        public event EventHandler ValueChanged
        {
            add { _suggestBox.ValueChanged += value; }
            remove { _suggestBox.ValueChanged -= value; }
        }

        private void _suggestBox_Format(object sender, ListControlConvertEventArgs e)
        {
            e.Value = _lookupHandler.FormatItem(e.ListItem);
        }

        private void _findButton_Click(object sender, EventArgs e)
        {
            try
            {
                object result;
                bool resolved = _lookupHandler.Resolve(_suggestBox.Text, true, out result);
                if (resolved)
                {
                    _suggestBox.Value = result;
                }
            }
            catch (Exception ex)
            {
                // not much we can do here if Resolve throws an exception
                Platform.Log(LogLevel.Error, ex);
            }
        }

        private void _suggestBox_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Return:
                    EventsHelper.Fire(_committed, this, EventArgs.Empty);
                    break;
                case Keys.Escape:
                    EventsHelper.Fire(_cancelled, this, EventArgs.Empty);
                    break;
            }
        }

        private void _suggestBox_KeyPress(object sender, KeyPressEventArgs e)
        {
        }
    }
}
