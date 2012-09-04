#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Windows.Forms;

namespace ClearCanvas.ImageServer.TestApp
{
    public partial class Startup : Form
    {
        public Startup()
        {
            InitializeComponent();
        }


        private void TestRule_Click(object sender, EventArgs e)
        {
            var test = new TestDicomFileForm();
            test.Show();
        }

        private void TestHeaderStreamButton_Click(object sender, EventArgs e)
        {
            var test = new TestHeaderStreamingForm();
            test.Show();
        }

        private void buttonCompression_Click(object sender, EventArgs e)
        {
            var test = new TestCompressionForm();
            test.Show();
        }

        private void buttonEditStudy_Click(object sender, EventArgs e)
        {
            var test = new TestEditStudyForm();
            test.Show();
        }

        private void RandomImageSender_Click(object sender, EventArgs e)
        {
            var test = new TestSendImagesForm();
            test.Show();
        }

        private void ExtremeStreaming_Click(object sender, EventArgs e)
        {
            var test = new ImageStreamingStressTest();
            test.Show();
        }


        private void UsageTracking_Click(object sender, EventArgs e)
        {
            var form = new UsageTrackingForm();
            form.Show();
        }

        private void ProductVerify_Click(object sender, EventArgs e)
        {
            var test = new ProductVerificationTest();
            test.Show();
        }

        private void DatabaseGenerator_Click(object sender, EventArgs e)
        {
            var form = new GenerateDatabase();
            form.Show();
        }
    }
}