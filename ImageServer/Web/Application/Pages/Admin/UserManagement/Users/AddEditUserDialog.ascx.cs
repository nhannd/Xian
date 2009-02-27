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
using System.Web.UI;
using System.Web.UI.WebControls;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Common.Admin.AuthorityGroupAdmin;
using ClearCanvas.Enterprise.Common.Admin.UserAdmin;
using ClearCanvas.ImageServer.Common.Services.Admin;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Application.Controls;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;
using ClearCanvas.ImageServer.Web.Common.WebControls.Validators;


namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.UserManagement.Users
{
    //
    // Dialog for adding a new device or editting an existing device.
    //
    public partial class AddEditUserDialog : UserControl
    {
        #region private variables

        private bool _editMode;
        // user being editted/added
        private UserRowData _user;

        #endregion

        #region public members

        /// <summary>
        /// Sets or gets the value which indicates whether the dialog is in edit mode.
        /// </summary>
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
        /// Sets/Gets the current editing user.
        /// </summary>
        public UserRowData User
        {
            set
            {
                _user = value;
                // put into viewstate to retrieve later
                ViewState[ "EditedUser"] = _user;
            }
            get { return _user; }
        }

        public ConditionalRequiredFieldValidator UserNameValidator
        {
            get { return UserNameRequiredFieldValidator; }
        }

        public InvalidInputIndicator UsernameIndicator
        {
            get { return UserLoginHelpId;  }
        }

        #endregion // public members

        #region Events

        /// <summary>
        /// Defines the event handler for <seealso cref="OKClicked"/>.
        /// </summary>
        /// <param name="user">The user being added.</param>
        public delegate bool OnOKClickedEventHandler(UserRowData user);

        /// <summary>
        /// Occurs when users click on "OK".
        /// </summary>
        public event OnOKClickedEventHandler OKClicked;

        #endregion Events

        #region Public delegates

        #endregion // public delegates

        #region Protected methods

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack == false)
            {
                Platform.GetService<IAuthorityAdminService>(
                delegate(IAuthorityAdminService service)
                {
                    IList<AuthorityGroupSummary> list = service.ListAllAuthorityGroups();
                    IList<ListItem> items = CollectionUtils.Map<AuthorityGroupSummary, ListItem>(
                        list,
                        delegate(AuthorityGroupSummary summary)
                        {
                            return new ListItem(summary.Name, summary.AuthorityGroupRef.Serialize());
                        }
                        );
                    UserGroupListBox.Items.AddRange(CollectionUtils.ToArray(items));
                });

            }
            else
            {
                if (ViewState[ "EditMode"] != null)
                    _editMode = (bool) ViewState[ "EditMode"];

                if (ViewState[ "EditedUser"] != null)
                    _user = ViewState[ "EditedUser"] as UserRowData;
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

                bool success = false;

                if (OKClicked != null)
                    success = OKClicked(User);

                if (!success)
                {
                    UsernameIndicator.Show();
                    UserNameValidator.Text = "Username already exists";
                    Show(false);
                } else
                {
                    Close();
                }
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
        
        private void SaveData()
        {
            if (User == null)
            {
                User = new UserRowData();
            }
            
            if(EditMode)
            {
                User.Enabled = UserEnabledCheckbox.Checked;
            } 
            else
            {
                User.UserName = UserLoginId.Text;
                User.Enabled = true;
            }

            User.DisplayName = DisplayName.Text;

            User.UserGroups.Clear();
            foreach (ListItem item in UserGroupListBox.Items)
            {
                if (item.Selected)
                {
                    User.UserGroups.Add(new UserGroup(
                        item.Value, item.Text));
                }
            } 
  
        }

        #endregion Protected methods

        #region Public methods

        public void UpdateUI()
        {
            if (EditMode)
            {
                ModalDialog1.Title = App_GlobalResources.SR.DialogEditUserTitle;
                OKButton.EnabledImageURL = ImageServerConstants.ImageURLs.UpdateButtonEnabled;
                OKButton.HoverImageURL = ImageServerConstants.ImageURLs.UpdateButtonHover;
                UserNameRow.Visible = false;
                EnabledRow.Visible = true;
            }
            else
            {
                ModalDialog1.Title = App_GlobalResources.SR.DialogAddUserTitle;
                OKButton.EnabledImageURL = ImageServerConstants.ImageURLs.AddButtonEnabled;
                OKButton.HoverImageURL = ImageServerConstants.ImageURLs.AddButtonHover;
                EnabledRow.Visible = false;
                UserNameRow.Visible = true;
            }

            // Update the rest of the fields
            if (User == null || EditMode == false)
            {
                UserLoginId.Text = string.Empty;
                DisplayName.Text = string.Empty;
                UserEnabledCheckbox.Checked = false;
                UserGroupListBox.SelectedIndex = -1;
            }
            else if (Page.IsValid)
            {
                UserLoginId.Text = User.UserName;
                DisplayName.Text = User.DisplayName;
                UserEnabledCheckbox.Checked = User.Enabled;

                List<UserGroup> groups = User.UserGroups;

                foreach(UserGroup group in groups)
                {
                    UserGroupListBox.Items.FindByText(group.Name).Selected = true;
                }
            }
        }

        /// <summary>
        /// Displays the add/edit device dialog box.
        /// </summary>
        public void Show(bool updateUI)
        {
            if (updateUI)
                UpdateUI();

            ModalDialog1.Show();
        }

        /// <summary>
        /// Dismisses the dialog box.
        /// </summary>
        public void Close()
        {
            ModalDialog1.Hide();
        }

        #endregion Public methods
    }
}
