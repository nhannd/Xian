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
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Common.WebControls;


namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.FileSystems
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

        private bool _editMode;
        private Filesystem _filesystem;

        #endregion

        #region public members

        /// <summary>
        /// Sets or gets the list of filesystem tiers which users are allowed to pick.
        /// </summary>
        public IList<FilesystemTierEnum> FilesystemTiers
        {
            set { _tiers = value; }

            get { return _tiers; }
        }

        /// <summary>
        /// Sets the dialog in edit mode or gets a value indicating whether the dialog is in edit mode.
        /// </summary>
        public bool EditMode
        {
            set
            {
                _editMode = value;
                ViewState[ "EditMode"] = value;
            }
            get { return _editMode; }
        }

        /// <summary>
        /// Sets or gets the filesystem users are working on.
        /// </summary>
        public Filesystem FileSystem
        {
            set
            {
                _filesystem = value;
                ViewState[ "_FileSystem"] = value;
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

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            RegisterClientSideScripts();


            HighWatermarkTextBox.Attributes["onkeyup"] = "RecalculateWatermark()";
            LowWatermarkTextBox.Attributes["onkeyup"] = "RecalculateWatermark()";

            EditFileSystemValidationSummary.HeaderText = App_GlobalResources.ErrorMessages.EditFileSystemValidationError;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack == false)
            {
            }
            else
            {
                // reload the filesystem information user is working on
                if (ViewState[ "EditMode"] != null)
                    _editMode = (bool)ViewState[ "EditMode"];

                FileSystem = ViewState[ "_FileSystem"] as Filesystem;
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
                    OKClicked(FileSystem);
                Close();
            }
            else
            {
                // TODO: Add mechanism to select the first tab where the error occurs
                Show(false);
            }
        }

        

        protected void CancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        #endregion Protected methods

        #region Private methods

        private void RegisterClientSideScripts()
        {
            ScriptTemplate template = new ScriptTemplate(typeof(AddFilesystemDialog).Assembly, "ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.FileSystems.Filesystem.js");
            template.Replace("@@HW_PERCENTAGE_INPUT_CLIENTID@@", HighWatermarkTextBox.ClientID);
            template.Replace("@@HW_SIZE_CLIENTID@@", HighWatermarkSize.ClientID);
            template.Replace("@@LW_PERCENTAGE_INPUT_CLIENTID@@", LowWatermarkTextBox.ClientID);
            template.Replace("@@LW_SIZE_CLIENTID@@", LowWaterMarkSize.ClientID);
            template.Replace("@@PATH_INPUT_CLIENTID@@", PathTextBox.ClientID);
            template.Replace("@@TOTAL_SIZE_INDICATOR_CLIENTID@@", TotalSizeIndicator.ClientID);
            template.Replace("@@USED_SIZE_INDICATOR_CLIENTID@@", UsedSizeIndicator.ClientID);
            template.Replace("@@TOTAL_SIZE_CLIENTID@@", TotalSize.ClientID);
            template.Replace("@@USED_SIZE_CLIENTID@@", AvailableSize.ClientID);

            Page.ClientScript.RegisterClientScriptBlock(GetType(), ClientID+"_scripts", template.Script, true);
        }


        private void UpdateUI()
        {
            // Update the title and OK button text. Changing the image is the only way to do this, since the 
            // SkinID cannot be set dynamically after Page_PreInit.
            if (EditMode)
            {
                ModalDialog.Title = App_GlobalResources.SR.DialogEditFileSystemTitle;
                OKButton.EnabledImageURL = ImageServerConstants.ImageURLs.UpdateButtonEnabled;
                OKButton.HoverImageURL = ImageServerConstants.ImageURLs.UpdateButtonHover;
            }
            else
            {
                ModalDialog.Title = App_GlobalResources.SR.DialogAddFileSystemTitle;
                OKButton.EnabledImageURL = ImageServerConstants.ImageURLs.AddButtonEnabled;
                OKButton.HoverImageURL = ImageServerConstants.ImageURLs.AddButtonHover;
            }

            // update the dropdown list
            TiersDropDownList.Items.Clear();
            foreach (FilesystemTierEnum tier in _tiers)
            {
                TiersDropDownList.Items.Add(new ListItem(tier.Description, tier.Lookup));
            }

            if (FileSystem == null)
            {
                // Clear input
                DescriptionTextBox.Text = string.Empty;
                PathTextBox.Text = string.Empty;
                ReadCheckBox.Checked = true;
                WriteCheckBox.Checked = true;
                LowWatermarkTextBox.Text = (80.00f).ToString();
                HighWatermarkTextBox.Text = (90.00f).ToString();

                TiersDropDownList.SelectedIndex = 0;
            }
            else if (Page.IsValid)
            {
                // set the data using the info in the filesystem to be editted
                DescriptionTextBox.Text = FileSystem.Description;
                PathTextBox.Text = FileSystem.FilesystemPath;
                ReadCheckBox.Checked = FileSystem.Enabled && (FileSystem.ReadOnly || (FileSystem.WriteOnly == false));
                WriteCheckBox.Checked = FileSystem.Enabled && (FileSystem.WriteOnly || (FileSystem.ReadOnly == false));
                LowWatermarkTextBox.Text = FileSystem.LowWatermark.ToString();
                HighWatermarkTextBox.Text = FileSystem.HighWatermark.ToString();
                TiersDropDownList.SelectedValue = FileSystem.FilesystemTierEnum.Lookup;
            }
        }

        private void SaveData()
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
            if (Decimal.TryParse(LowWatermarkTextBox.Text, NumberStyles.Number, null, out lowWatermark))
                FileSystem.LowWatermark = lowWatermark;

            Decimal highWatermark;
			if (Decimal.TryParse(HighWatermarkTextBox.Text, NumberStyles.Number, null, out highWatermark))
                FileSystem.HighWatermark = highWatermark;

            FileSystem.FilesystemTierEnum = FilesystemTiers[TiersDropDownList.SelectedIndex];
        }

       

        #endregion Private methods

        #region Public methods

        /// <summary>
        /// Displays the add device dialog box.
        /// </summary>
        public void Show(bool updateUI)
        {
            if (updateUI)
                UpdateUI();

            if (Page.IsValid)
            {
                TabContainer1.ActiveTabIndex = 0;
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

        public AddFilesystemDialog()
        {
            _editMode = false;
        }

        #endregion Public methods
    }
}
