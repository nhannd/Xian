#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Configure.ServerPartitions
{
    //
    // Dialog for adding new Partition.
    //
    public partial class AddEditPartitionDialog : UserControl
    {
        #region Private Members

        private bool _editMode;
        // device being editted/added
        private ServerPartition _partition;

        #endregion

        #region Public Properties

        public bool EditMode
        {
            get { return _editMode; }
            set
            {
                _editMode = value;
                ViewState[ClientID + "_EditMode"] = value;
            }
        }

        /// <summary>
        /// Sets/Gets the current editing partition.
        /// </summary>
        public ServerPartition Partition
        {
            set
            {
                _partition = value;
                // put into viewstate to retrieve later
                ViewState[ClientID + "_EdittedPartition"] = _partition;
                if (value != null && !Page.IsPostBack)
                    ServerPartitionValidator.OriginalAeTitle =  value.AeTitle;
            }
            get { return _partition; }
        }

        #endregion // public members

        #region Events

        /// <summary>
        /// Defines the event handler for <seealso cref="OKClicked"/>.
        /// </summary>
        /// <param name="partition">The partition being added.</param>
        public delegate void OnOKClickedEventHandler(ServerPartition partition);

        /// <summary>
        /// Occurs when users click on "OK".
        /// </summary>
        public event OnOKClickedEventHandler OKClicked;

        #endregion Events

        #region Protected Methods

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ServerPartitionTabContainer.ActiveTabIndex = 0;

            AutoInsertDeviceCheckBox.InputAttributes.Add("onclick", "EnableDisable();");

            Page.ClientScript.RegisterClientScriptBlock(GetType(), ClientID,
                                                        @"<script language='javascript'>
                    function EnableDisable()
                    {  
                         var autoInsertCheck = document.getElementById('" +
                                                        AutoInsertDeviceCheckBox.ClientID +
                                                        @"');
                         var defaultPortInput = document.getElementById('" +
                                                        DefaultRemotePortTextBox.ClientID +
                                                        @"');
                         defaultPortInput.disabled = !autoInsertCheck.checked;
                    }
                </script>");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack)
            {
                if (ViewState[ClientID + "_EditMode"] != null)
                    _editMode = (bool) ViewState[ClientID + "_EditMode"];

                if (ViewState[ClientID + "_EdittedPartition"] != null)
                {
                    _partition = ViewState[ClientID + "_EdittedPartition"] as ServerPartition;
                    ServerPartitionValidator.OriginalAeTitle = _partition.AeTitle;
                }
            }
        }


        /// <summary>
        /// Handles event when user clicks on "OK" button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OKButton_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                SaveData();

                if (OKClicked != null)
                    OKClicked(Partition);

                Close();
            }
            else
            {
                Show(false);
            }
        }

        /// <summary>
        /// Handles event when user clicks on "Cancel" button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }
        

        protected void UpdateUI()
        {
            if (EditMode)
            {
                ModalDialog.Title= "Edit Partition";
                OKButton.Text = "Update";
            }
            else
            {
                ModalDialog.Title = "Add Partition";
                OKButton.Text = "Add";
            }

            // update the dropdown list
            DuplicateSopDropDownList.Items.Clear();
            foreach (DuplicateSopPolicyEnum policyEnum in DuplicateSopPolicyEnumHelper.GetAll())
            {
                DuplicateSopDropDownList.Items.Add(new ListItem(DuplicateSopPolicyEnumHelper.GetDescription(policyEnum), policyEnum.ToString()));
            }

            if (Partition == null)
            {
                AETitleTextBox.Text = "SERVERAE";
                DescriptionTextBox.Text = "";
                PortTextBox.Text = "104";
                PartitionFolderTextBox.Text = "SERVERAE";
                EnabledCheckBox.Checked = true;
                AutoInsertDeviceCheckBox.Checked = true;
                AcceptAnyDeviceCheckBox.Checked = true;
                DefaultRemotePortTextBox.Text = "104";

                AutoInsertDeviceCheckBox.Enabled = true;
                DefaultRemotePortTextBox.Enabled = true;

                DuplicateSopDropDownList.SelectedIndex = 0;
            }
            else if (Page.IsValid)
                // only update the UI with the partition if the data is valid, otherwise, keep them on the screen
            {
                AETitleTextBox.Text = Partition.AeTitle;
                DescriptionTextBox.Text = Partition.Description;
                PortTextBox.Text = Partition.Port.ToString();
                PartitionFolderTextBox.Text = Partition.PartitionFolder;
                EnabledCheckBox.Checked = Partition.Enabled;
                AutoInsertDeviceCheckBox.Checked = Partition.AutoInsertDevice;
                AcceptAnyDeviceCheckBox.Checked = Partition.AcceptAnyDevice;
                DefaultRemotePortTextBox.Text = Partition.DefaultRemotePort.ToString();

                DefaultRemotePortTextBox.Enabled = Partition.AutoInsertDevice;

                DuplicateSopDropDownList.SelectedValue = Partition.DuplicateSopPolicyEnum.ToString();

            }
        }


        #region Private Methods


        private void SaveData()
        {
            if (Partition == null)
            {
                Partition = new ServerPartition();
            }


            Partition.Enabled = EnabledCheckBox.Checked;
            Partition.AeTitle = AETitleTextBox.Text;
            Partition.Description = DescriptionTextBox.Text;
            Partition.PartitionFolder = PartitionFolderTextBox.Text;

            int port;
            if (Int32.TryParse(PortTextBox.Text, out port))
                Partition.Port = port;

            Partition.AcceptAnyDevice = AcceptAnyDeviceCheckBox.Checked;
            Partition.AutoInsertDevice = AutoInsertDeviceCheckBox.Checked;
            if (Int32.TryParse(DefaultRemotePortTextBox.Text, out port))
                Partition.DefaultRemotePort = port;

            Partition.DuplicateSopPolicyEnum = DuplicateSopPolicyEnumHelper.GetAll()[DuplicateSopDropDownList.SelectedIndex];
        }

        #endregion Private Methods

        #endregion Protected methods

        #region Public Methods

        /// <summary>
        /// Displays the add device dialog box.
        /// </summary>
        public void Show(bool updateUI)
        {
            if (updateUI)
                UpdateUI();

            if (Page.IsValid)
            {
                ServerPartitionTabContainer.ActiveTabIndex = 0;
            }


            ModalDialog.Show();
        }

        /// <summary>
        /// Dismisses the dialog box.
        /// </summary>
        public void Close()
        {
            ModalDialog.Hide();   
        }

        #endregion Public methods
    }
}
