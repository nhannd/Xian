using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ClearCanvas.Desktop.ExtensionBrowser.View.WinForms
{
    public partial class BrowserControl : UserControl
    {
        public BrowserControl()
        {
            InitializeComponent();
        }

        public TreeView PluginTree
        {
            get { return _pluginTree; }
        }

        public TreeView ExtPointTree
        {
            get { return _extPointTree; }
        }
    }
}
