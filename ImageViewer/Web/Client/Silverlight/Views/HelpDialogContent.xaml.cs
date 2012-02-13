#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Windows.Controls;
using ClearCanvas.ImageViewer.Web.Client.Silverlight.Resources;

namespace ClearCanvas.ImageViewer.Web.Client.Silverlight.Views
{
    public partial class HelpDialogContent : UserControl
    {
        public HelpDialogContent()
        {
            InitializeComponent();

            ProductInfoTextBlock.Text = ApplicationContext.Current.ViewerVersion;
            ProductInfoTextBlock.Text += "\n" + SR.PartOfClearCanvasRISPACS;
            CompanyAddressTextBlock.Text = SR.CompanyAddress;
            CompanyWebAddressTextBlock.Text = SR.CompanyWebAddress;
            CopyrightTextBlock.Text = SR.Copyright;
        }        
    }
}
