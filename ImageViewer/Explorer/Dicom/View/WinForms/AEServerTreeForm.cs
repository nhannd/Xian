using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ClearCanvas.ImageViewer.Explorer.Dicom.View.WinForms
{
    public partial class AEServerTreeForm : UserControl
    {
        public AEServerTreeForm()
        {
            InitializeComponent();
        }

        public TextBox ServerName
        {
            get { return _serverName; }
        }

        public TextBox ServerDesc
        {
            get { return _serverDesc; }
        }

        public TextBox ServerAE
        {
            get { return _serverAE; }
        }

        public TextBox ServerHost
        {
            get { return _serverHost; }
        }

        public TextBox ServerPort
        {
            get { return _serverPort; }
        }

        public TreeView AeserverTree
        {
            get { return _aeserverTree; }
        }

    }
}
