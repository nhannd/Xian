namespace ClearCanvas.ImageViewer.Dashboard.Remote
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Text;
    using System.Windows.Forms;
    using ClearCanvas.Dicom;
    using ClearCanvas.Dicom.Network;
    using System.Threading;

    public partial class ServerModifyForm : Form
    {
        public ServerModifyForm()
        {
            InitializeComponent();
            this.CancelButton = _buttonCancel;
            this.AcceptButton = _buttonOk;
        }

        public void Initialize(MenuItemFunction function, TreeView serverTree, TreeNode lastClickedNode)
        {
            switch (function)
            {
                case MenuItemFunction.Modify:
                    ModifyServer(serverTree, lastClickedNode);
                    break;
                case MenuItemFunction.AddNewServer:
                    AddNewServer(serverTree, lastClickedNode);
                    break;
                case MenuItemFunction.Remove:
                    RemoveServer(serverTree, lastClickedNode);
                    break;
                case MenuItemFunction.VerifyServer:
                    VerifyServer(serverTree, lastClickedNode);
                    break;
                default: // TODO
                    throw new System.Exception("Unexpected condition in server modify form.");
            }
        }

        void ModifyServer(TreeView tree, TreeNode lastClickedNode)
        {
            Server storedAE = lastClickedNode.Tag as Server;

            _textName.Text = storedAE.Name;
            _textDescription.Text = storedAE.Description;
            _textAETitle.Text = storedAE.AE;
            _textHost.Text = storedAE.Host;
            _textListeningPort.Text = storedAE.ListeningPort.ToString();
        }

        void RemoveServer(TreeView tree, TreeNode lastClickedNode)
        {
            Server storedAE = lastClickedNode.Tag as Server;

            this.Text = "Remove Server Confirmation";
            _buttonOk.Text = "Confirm";
            _textName.Text = storedAE.Name;
            _textDescription.Text = storedAE.Description;
            _textAETitle.Text = storedAE.AE;
            _textHost.Text = storedAE.Host;
            _textListeningPort.Text = storedAE.ListeningPort.ToString();
            _textName.Enabled = false;
            _textDescription.Enabled = false;
            _textAETitle.Enabled = false;
            _textHost.Enabled = false;
            _textListeningPort.Enabled = false;
        }

        void AddNewServer(TreeView tree, TreeNode lastClickedNode)
        {
            Server storedAE = lastClickedNode.Tag as Server;
            ServerModifyForm form = new ServerModifyForm();

            this.Text = "Add New Server";
            _buttonOk.Text = "Save";
        }

        void VerifyServer(TreeView tree, TreeNode lastClickedNode)
        {
            Server storedAE = lastClickedNode.Tag as Server;

            this.Text = "Verifying....";

            _buttonOk.Visible = true;
            _buttonOk.Enabled = false;
            _buttonCancel.Visible = false;

            // position the Ok button in the centre
            int buttonWidth = _buttonOk.Width;
            _buttonOk.Left = ((this.Width / 2) - (buttonWidth / 2));

            // make hitting the ESC key the same as pressing enter
            this.CancelButton = _buttonOk;

            _textName.Text = storedAE.Name;
            _textDescription.Text = storedAE.Description;
            _textAETitle.Text = storedAE.AE;
            _textHost.Text = storedAE.Host;
            _textListeningPort.Text = storedAE.ListeningPort.ToString();
            _textName.Enabled = false;
            _textDescription.Enabled = false;
            _textAETitle.Enabled = false;
            _textHost.Enabled = false;
            _textListeningPort.Enabled = false;

            ApplicationEntity me = new ApplicationEntity(new HostName("localhost"), new AETitle("CCWORKSTN"), new ListeningPort(4000));
            VerifyThreadObject vto = new VerifyThreadObject(me, storedAE, this);

            Thread t = new Thread(new ThreadStart(vto.WorkMethod));
            t.IsBackground = true;
            t.Start();
        }

        private class VerifyThreadObject
        {
            public VerifyThreadObject(ApplicationEntity clientAE, ApplicationEntity serverAE, Form form)
            {
                _clientAE = clientAE;
                _serverAE = serverAE;
                _form = form;
            }

            public void WorkMethod()
            {
                DicomClient client = new DicomClient(_clientAE);

                if (client.Verify(_serverAE))
                    _isVerifiedSuccessfully = true;
                else
                    _isVerifiedSuccessfully = false;

                if (null == _form || !_form.Created)
                    return;

                _form.BeginInvoke((ThreadStart)delegate()
                    {
                        _form.Controls["_buttonOk"].Enabled = true;
                        if (IsVerifiedSuccessfully)
                            _form.Text = "Verified Successfully";
                        else
                            _form.Text = "Verification Failed";
                    });
            }

            public Boolean IsVerifiedSuccessfully
            {
                get { return _isVerifiedSuccessfully; }
            }

            private ApplicationEntity _clientAE;
            private ApplicationEntity _serverAE;
            private Form _form;
            private Boolean _isVerifiedSuccessfully;
        }

        public Server ModifiedServer
        {
            get
            {
                Int32 port = Int32.Parse(_textListeningPort.Text);
                return new Server(_textName.Text, _textDescription.Text, _textHost.Text, _textAETitle.Text, port);
            }
        }
        
        private Boolean IsFormDataValid
        {
            get
            {
                int dummy;

                if (Int32.TryParse(_textListeningPort.Text, out dummy))
                    return true;

                return false;
            }
        }

        private void _buttonOk_Click(object sender, EventArgs e)
        {
            if (IsFormDataValid)
            {
                this.DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void _buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}