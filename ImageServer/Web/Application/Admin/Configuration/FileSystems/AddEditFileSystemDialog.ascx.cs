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
using System.Collections.Generic;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Web.Application.Admin.Configuration.FileSystems
{
    //
    // Dialog for adding a new device or editting an existing one.
    //
    public partial class AddFilesystemDialog : UserControl
    {
        #region private variables
        // The server partitions that the new device can be associated with
        // This list will be determined by the user level permission.
        private IList<FilesystemTierEnum> _tiers = new List<FilesystemTierEnum>();

        private bool _editMode = false;
        private Filesystem _filesystem;
        #endregion

        #region public members
        /// <summary>
        /// Sets or gets the list of filesystem tiers which users are allowed to pick.
        /// </summary>
        public IList<FilesystemTierEnum> FilesystemTiers
        {
            set
            {
                _tiers = value;
            }

            get
            {
                return _tiers;
            }
        }

        /// <summary>
        /// Sets the dialog in edit mode or gets a value indicating whether the dialog is in edit mode.
        /// </summary>
        public bool EditMode
        {
            set { 
                _editMode = value;
                ViewState[ ClientID+"_EditMode"] = value;
            }
            get { return _editMode; }
        }

        /// <summary>
        /// Sets or gets the filesystem users are working on.
        /// </summary>
        public Filesystem FileSystem
        {
            set { _filesystem = value;
                ViewState[ ClientID+"_FileSystem"] = value;
            }
            get { return _filesystem; }
        }

        #endregion // public members

        #region Events
        /// <summary>
        /// Defines the event handler for <seealso cref="OKClicked"/>.
        /// </summary>
        /// <param name="filesystem">The device being added.</param>
        public delegate void OKClickedEventHandler(Filesystem filesystem);
        /// <summary>
        /// Occurs when users click on "OK".
        /// </summary>
        public event OKClickedEventHandler OKClicked;

        #endregion Events

        #region Public delegates


        #endregion // public delegates

        #region Protected methods

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
        }

        
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            
            // Set up the popup extender
            // These settings could been done in the aspx page as well
            // but if we are to javascript to display, that won't work.
            ModalPopupExtender1.PopupControlID = DialogPanel.UniqueID;
            ModalPopupExtender1.TargetControlID = DummyPanel.UniqueID;
            ModalPopupExtender1.BehaviorID = ModalPopupExtender1.UniqueID;

            ModalPopupExtender1.DropShadow = true;
            ModalPopupExtender1.PopupDragHandleControlID = TitleBarPanel.UniqueID;


            OKButton.OnClientClick = ClientID + "_clearFields()";

            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), this.ClientID,
                        @"<script language='javascript'>

                            function ValidationFilesystemPathParams()
                            {
                                control = document.getElementById('" + PathTextBox.ClientID + @"');
                                params = new Array();
                                params.path=control.value;

                                return params;
                            }


                            function AddEditFileSystemsDialog_HideHelpImage(helpImgID)
                            {
                                img = document.getElementById(helpImgID);
                                img.style.visibility = 'hidden';
                            }
                            function AddEditFileSystemsDialog_ClearField(fieldID)
                            {
                                txtbox = document.getElementById(fieldID);
                                txtbox.style.backgroundColor = '';
                            }

                            function " + ClientID + @"_clearFields()
                            {
                                
                                AddEditFileSystemsDialog_ClearField('" + DescriptionTextBox.ClientID + @"');
                                AddEditFileSystemsDialog_ClearField('" + PathTextBox.ClientID + @"');

                                
                            }
                        </script>");

           
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            
            if (Page.IsPostBack == false)
            {

            }
            else
            {
                // reload the filesystem information user is working on
                if (ViewState[ ClientID+"_EditMode"]!=null)
                    _editMode = (bool) ViewState[ ClientID+"_EditMode"];

                FileSystem = ViewState[ ClientID+"_FileSystem"] as Filesystem;
            }
        }


        /// <summary>
        /// Handles event when user clicks on "OK" button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OKButton_Click(object sender, EventArgs e)
        {
            if (FileSystem == null)
            {
                // create a filesystem 
                FileSystem = new Filesystem();
                FileSystem.LowWatermark = 80.00M;
                FileSystem.HighWatermark = 90.00M;
            }

            FileSystem.Description = DescriptionTextBox.Text;
            FileSystem.FilesystemPath = PathTextBox.Text;
            FileSystem.ReadOnly = ReadCheckBox.Checked && WriteCheckBox.Checked == false;
            FileSystem.WriteOnly = WriteCheckBox.Checked && ReadCheckBox.Checked == false;
            FileSystem.Enabled = ReadCheckBox.Checked || WriteCheckBox.Checked;

            Decimal lowWatermark;
            if (Decimal.TryParse(LowWatermarkTextBox.Text, out lowWatermark))
                FileSystem.LowWatermark = lowWatermark;

            Decimal highWatermark;
            if (Decimal.TryParse(HighWatermarkTextBox.Text, out highWatermark))
                FileSystem.HighWatermark = highWatermark;

            FileSystem.FilesystemTierEnum = FilesystemTiers[TiersDropDownList.SelectedIndex];
            

            if (Page.IsValid)
            {
                if (OKClicked != null)
                    OKClicked(FileSystem);
                Close();
            }
            else
            {
                // TODO: Add mechanism to select the first tab where the error occurs
                Show();
            }
        
           
            
            
        }

        
        protected void ReadOnlyCheckBox_Init(object sender, EventArgs e)
        {

        }

        protected void CancelButton_Click(object sender, EventArgs e)
        {

        }

        #endregion Protected methods


        #region Public methods
        /// <summary>
        /// Displays the add device dialog box.
        /// </summary>
        public void Show()
        {

            // update the dropdown list
            TiersDropDownList.Items.Clear();
            foreach (FilesystemTierEnum tier in _tiers)
            {
                TiersDropDownList.Items.Add(new ListItem(tier.Description, tier.Enum.ToString()));
            }
            
            if (EditMode)
            {
                // set the dialog box title and OK button text
                TitleLabel.Text = "Edit Filesystem";
                OKButton.Text = "Update";

                // set the data using the info in the filesystem to be editted
                DescriptionTextBox.Text = FileSystem.Description;
                PathTextBox.Text = FileSystem.FilesystemPath;
                ReadCheckBox.Checked = FileSystem.Enabled && (FileSystem.ReadOnly || (FileSystem.WriteOnly == false));
                WriteCheckBox.Checked = FileSystem.Enabled && (FileSystem.WriteOnly || (FileSystem.ReadOnly == false));
                LowWatermarkTextBox.Text = FileSystem.LowWatermark.ToString();
                HighWatermarkTextBox.Text = FileSystem.HighWatermark.ToString();
                TiersDropDownList.SelectedValue = FileSystem.FilesystemTierEnum.Enum.ToString();
                PercentFullLabel.Text = FileSystem.PercentFull.ToString();
            }
            else
            {
                // set the dialog box title and OK button text
                TitleLabel.Text = "Add Filesystem";
                OKButton.Text = "Add";

                // Clear input
                DescriptionTextBox.Text = "";
                PathTextBox.Text = "";
                ReadCheckBox.Checked = true;
                WriteCheckBox.Checked = true;
                LowWatermarkTextBox.Text = "80.00";
                HighWatermarkTextBox.Text = "90.00";
                PercentFullLabel.Text = "0.00";

                TiersDropDownList.SelectedIndex = 0;
            }

            TabContainer1.ActiveTabIndex = 0;

            UpdatePanel.Update();
            ModalPopupExtender1.Show();
        }

        /// <summary>
        /// Dismisses the dialog box.
        /// </summary>
        public void Close()
        {
            // 
            // Clear all boxes
            //
            // STRANGE AJAX BUG?: 
            //      This block of code will cause 
            //      WebForms.PageRequestManagerServerErrorException: Status code 500 
            //      when other buttons are pressed AFTER the add device dialog box is dismissed.
            //
            //  Move the entire block into Show()
            //
            //  AETitleTextBox.Text = "<Enter AE Title>";
            //  ActiveCheckBox.Checked = false;
            //  DHCPCheckBox.Checked = false;
            //  DescriptionTextBox.Text = "<Enter Description>";
            //  PortTextBox.Text = "<Port #>";


            ModalPopupExtender1.Hide();
        }

        #endregion Public methods

        
    }
 
}
