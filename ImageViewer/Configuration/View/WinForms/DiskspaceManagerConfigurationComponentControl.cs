using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ClearCanvas.ImageViewer.Configuration.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="DiskspaceManagerConfigurationComponent"/>
    /// </summary>
    public partial class DiskspaceManagerConfigurationComponentControl : UserControl
    {
        private DiskspaceManagerConfigurationComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public DiskspaceManagerConfigurationComponentControl(DiskspaceManagerConfigurationComponent component)
        {
            InitializeComponent();

            _component = component;

            // TODO add .NET databindings to _component
        }
    }
}
