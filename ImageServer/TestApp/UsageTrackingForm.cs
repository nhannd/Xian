#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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
            textBoxLicense.Text = _message.LicenseString;
            textBoxComponent.Text = _message.Component;
            textBoxMachineIdentifier.Text = _message.MachineIdentifier;
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            _message.Version = textBoxVersion.Text;
            _message.Product = textBoxProduct.Text;
            _message.OS = textBoxOS.Text;
            _message.Region = textBoxRegion.Text;
            _message.LicenseString = textBoxLicense.Text;
            _message.Component = textBoxComponent.Text;
            _message.MachineIdentifier = textBoxMachineIdentifier.Text;
            _message.MessageType = UsageType.Other;

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

            UsageTracking.Register(_message,UsageTrackingThread.Background);
        }

        private static void DisplayMessage(object o, ItemEventArgs<DisplayMessage> m)
        {
            MessageBox.Show(m.Item.Message, m.Item.Title);
        }
    }
}
