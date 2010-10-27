#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="BiographyOverviewComponent"/>
    /// </summary>
    public partial class BiographyOverviewComponentControl : ApplicationComponentUserControl
    {
        private readonly BiographyOverviewComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public BiographyOverviewComponentControl(BiographyOverviewComponent component)
            : base(component)
        {
            InitializeComponent();
            _component = component;

            _overviewLayoutPanel.RowStyles[0].Height = _component.BannerHeight; 

            Control banner = (Control)_component.BannerComponentHost.ComponentView.GuiElement;
            banner.Dock = DockStyle.Fill;
            _bannerPanel.Controls.Add(banner);

            Control content = (Control)_component.ContentComponentHost.ComponentView.GuiElement;
            content.Dock = DockStyle.Fill;
            _contentPanel.Controls.Add(content);
        }
    }
}
