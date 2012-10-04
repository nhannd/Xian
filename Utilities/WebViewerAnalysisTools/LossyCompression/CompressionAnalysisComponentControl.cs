#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ClearCanvas.Utilities.WebViewerAnalysisTools.LossyCompression
{
    public partial class CompressionAnalysisComponentControl : UserControl
    {
        private CompressionAnalysisComponent _component;

        public CompressionAnalysisComponentControl(CompressionAnalysisComponent component)
        {
            InitializeComponent();
            _component = component;
        }

        private void LosslessBMPVSLossyBMP_Click(object sender, EventArgs e)
        {
            _component.CompareLosslessBMPVsLossyBMP((int) DICOMCompressionQuality.Value);
        }

        private void LosslessBMPvsLosslessJPEG_Click(object sender, EventArgs e)
        {
            _component.CompareBitmapAndJPEGOfLosslessImage((int)JPEGCompressionQuality.Value);
        }

        private void losslessBMPVsLossyJPEG_Click(object sender, EventArgs e)
        {
            _component.CompareLosslessBMPvsLossyJPEG((int) DICOMCompressionQuality.Value,
                                                     (int) JPEGCompressionQuality.Value);
        }

    }
}
