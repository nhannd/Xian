#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
using ClearCanvas.Common.UsageTracking;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageServer.TestApp
{
    public partial class UsageTrackingForm : Form
    {
        private readonly UsageMessage _message;

        public UsageTrackingForm()
        {
            InitializeComponent();
            UsageTracking.MessageEvent += DisplayMessage;

            _message = UsageTracking.GetUsageMessage();

            textBoxVersion.Text = _message.Version;
            textBoxProduct.Text = _message.Product;
            textBoxOS.Text = _message.OS;
            textBoxRegion.Text = _message.Region;
            textBoxLicense.Text = _message.License;
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            _message.Version = textBoxVersion.Text;
            _message.Product = textBoxProduct.Text;
            _message.OS = textBoxOS.Text;
            _message.Region = textBoxRegion.Text;
            _message.License = textBoxLicense.Text;

            if (!string.IsNullOrEmpty(textBoxAppKey.Text) 
                && !string.IsNullOrEmpty(textBoxAppValue.Text))
            {
                UsageApplicationData d = new UsageApplicationData
                                             {
                                                 Key = textBoxAppKey.Text,
                                                 Value = textBoxAppValue.Text
                                             };
                _message.AppData = new List<UsageApplicationData>
                                       {
                                           d
                                       };
            }

            UsageTracking.Register(_message);
        }

        private static void DisplayMessage(object o, ItemEventArgs<DisplayMessage> m)
        {
            MessageBox.Show(m.Item.Message, m.Item.Title);
        }
    }
}
