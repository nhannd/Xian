#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.ServiceModel;
using System.Windows.Forms;
using ClearCanvas.ImageServer.Common.ServiceModel;

namespace ClearCanvas.ImageServer.TestApp
{
    public partial class ProductVerificationTest : Form
    {
        public ProductVerificationTest()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ProductVerificationTester test = new ProductVerificationTester();
            ProductVerificationResponse result = test.Verify();

            ComponentName.Text = String.Format("ComponentName: {0}", result.ComponentName);
            ManifestVerified.Text = String.Format("ManifestVerified: {0}", result.IsManifestValid);
        }
    }

    public class ProductVerificationTester
    {
        public ProductVerificationResponse Verify()
        {
            int port = 9998;
            WSHttpBinding binding = new WSHttpBinding(SecurityMode.None);
            EndpointAddress endpoint = new EndpointAddress(String.Format("http://localhost:{0}/ClearCanvas.ImageServer.Common.ServiceModel.IProductVerificationService", port));

            IProductVerificationService service = ChannelFactory<IProductVerificationService>.CreateChannel(binding, endpoint);


            var result = service.Verify(new ProductVerificationRequest());
            return result;
        }
    }
}
