#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.ServerPartitions
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
                ViewState[ "EditMode"] = value;
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
                ViewState[ "EditedPartition"] = _partition;
				if (value != null && !Page.IsPostBack)
				{
					ServerPartitionValidator.OriginalAeTitle = value.AeTitle;
					PartitionFolderValidator.OriginalPartitionFolder = value.PartitionFolder;
				}
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

            EditPartitionValidationSummary.HeaderText = App_GlobalResources.ErrorMessages.EditPartitionValidationError;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack)
            {
                if (ViewState[ "EditMode"] != null)
                    _editMode = (bool) ViewState[ "EditMode"];

                if (ViewState[ "EditedPartition"] != null)
                {
                    _partition = ViewState[ "EditedPartition"] as ServerPartition;
                    ServerPartitionValidator.OriginalAeTitle = _partition.AeTitle;
                	PartitionFolderValidator.OriginalPartitionFolder = _partition.PartitionFolder;
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
            // Update the title and OK button text. Changing the image is the only way to do this, since the 
            // SkinID cannot be set dynamically after Page_PreInit.
    

            // update the dropdown list
            DuplicateSopDropDownList.Items.Clear();
            foreach (DuplicateSopPolicyEnum policyEnum in DuplicateSopPolicyEnum.GetAll())
            {
            	ListItem item = new ListItem(policyEnum.Description, policyEnum.Lookup);
                DuplicateSopDropDownList.Items.Add(item);
            }

            if (Partition == null)
            {
                AETitleTextBox.Text = "SERVERAE";
                DescriptionTextBox.Text = string.Empty;
                PortTextBox.Text = "104";
                PartitionFolderTextBox.Text = "SERVERAE";
                EnabledCheckBox.Checked = true;
                AutoInsertDeviceCheckBox.Checked = true;
                AcceptAnyDeviceCheckBox.Checked = true;
                DefaultRemotePortTextBox.Text = "104";

                AutoInsertDeviceCheckBox.Enabled = true;
                DefaultRemotePortTextBox.Enabled = true;

                DuplicateSopDropDownList.SelectedIndex = 0;

                MatchPatientName.Checked = true;
                MatchPatientID.Checked = true;
                MatchPatientBirthDate.Checked = true;
                MatchPatientSex.Checked = true;
                MatchAccessionNumber.Checked = true;
                MatchIssuer.Checked = true;
                AuditDeleteStudyCheckBox.Checked = true;  //TODO: Load from system setting instead
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

                DuplicateSopDropDownList.SelectedValue = Partition.DuplicateSopPolicyEnum.Lookup;

                MatchPatientName.Checked = Partition.MatchPatientsName;
                MatchPatientID.Checked = Partition.MatchPatientId;
                MatchPatientBirthDate.Checked = Partition.MatchPatientsBirthDate;
                MatchPatientSex.Checked = Partition.MatchPatientsSex;
                MatchAccessionNumber.Checked = Partition.MatchAccessionNumber;
                MatchIssuer.Checked= Partition.MatchIssuerOfPatientId;
                AuditDeleteStudyCheckBox.Checked = Partition.AuditDeleteStudy; 
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

            Partition.DuplicateSopPolicyEnum = DuplicateSopPolicyEnum.GetAll()[DuplicateSopDropDownList.SelectedIndex];

            Partition.MatchPatientsName = MatchPatientName.Checked;
            Partition.MatchPatientId = MatchPatientID.Checked;
            Partition.MatchPatientsBirthDate = MatchPatientBirthDate.Checked;
            Partition.MatchPatientsSex = MatchPatientSex.Checked;
            Partition.MatchAccessionNumber = MatchAccessionNumber.Checked;
            Partition.MatchIssuerOfPatientId = MatchIssuer.Checked;
            Partition.AuditDeleteStudy = AuditDeleteStudyCheckBox.Checked;
        }

        #endregion Private Methods

        #endregion Protected methods

        #region Public Methods

        /// <summary>
        /// Displays the add device dialog box.
        /// </summary>
        public void Show(bool updateUI)
        {
			if (EditMode)
			{
				ModalDialog.Title = App_GlobalResources.SR.DialogEditPartitionTitle;
				OKButton.EnabledImageURL = ImageServerConstants.ImageURLs.UpdateButtonEnabled;
                OKButton.HoverImageURL = ImageServerConstants.ImageURLs.UpdateButtonHover;
			}
			else
			{
				ModalDialog.Title = App_GlobalResources.SR.DialogAddPartitionTitle;
				OKButton.EnabledImageURL = ImageServerConstants.ImageURLs.AddButtonEnabled;
                OKButton.HoverImageURL = ImageServerConstants.ImageURLs.AddButtonHover;
			}

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
