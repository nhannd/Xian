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
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ClearCanvas.ImageViewer.Web.Client.Silverlight.Helpers;

namespace ClearCanvas.ImageViewer.Web.Client.Silverlight
{
    public partial class HelpDialogContent : UserControl
    {
        public HelpDialogContent()
        {
            InitializeComponent();

            ProductInfoTextBlock.Text = ApplicationContext.Current.ViewerVersion;
            ProductInfoTextBlock.Text += "\nPart of the ClearCanvas RIS/PACS";
            CompanyAddressTextBlock.Text = SR.CompanyAddress;
            CompanyWebAddressTextBlock.Text = SR.CompanyWebAddress;
            CopyrightTextBlock.Text = SR.Copyright;


        }
        
    }
}
