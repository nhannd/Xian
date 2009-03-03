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
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;


namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.UserManagement.UserGroups
{
    //
    // Dialog for adding a new device or editting an existing device.
    //
    public partial class AddEditUserGroupsDialog : UserControl
    {
        #region private variables

        private bool _editMode;
        // user being editted/added
        private UserGroupRowData _userGroup;

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
        /// Sets/Gets the current editing user group.
        /// </summary>
        public UserGroupRowData UserGroup
        {
            set
            {
                _userGroup = value;
                // put into viewstate to retrieve later
                ViewState[ "EditedUserGroup"] = _userGroup;
            }
            get { return _userGroup; }
        }

        #endregion // public members

        #region Events

        public delegate bool OnOKClickedEventHandler(UserGroupRowData user);
        public event OnOKClickedEventHandler OKClicked;

        #endregion Events

        #region Private Methods

        private void SaveData()
        {
            if (UserGroup == null)
            {
                UserGroup = new UserGroupRowData();
            }

            UserGroup.Name = GroupName.Text;

            UserGroup.Tokens.Clear();
            foreach (ListItem item in TokenCheckBoxList.Items)
            {
                if (item.Selected)
                {
                    UserGroup.Tokens.Add(new TokenSummary(item.Value, item.Text));
                }
            }

        }

        #endregion

        #region Protected methods

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack == false)
            {
                
                Platform.GetService<IAuthorityAdminService>(
                delegate(IAuthorityAdminService services)
                {
                    IList<AuthorityTokenSummary> tokens = services.ListAuthorityTokens();
                    IList<ListItem> items = CollectionUtils.Map<AuthorityTokenSummary, ListItem>(
                                            tokens,
                                            delegate(AuthorityTokenSummary token)
                                            {
                                                return new ListItem(token.Description, token.Name);
                                            });

                    TokenCheckBoxList.Items.AddRange(CollectionUtils.ToArray(items));
                });
            }
            else
            {
                if (ViewState[ "EditMode"] != null)
                    _editMode = (bool) ViewState[ "EditMode"];

                if (ViewState[ "EditedUserGroup"] != null)
                    _userGroup = ViewState[ "EditedUserGroup"] as UserGroupRowData;
            }
        }


        /// <summary>
        /// Handles event when user clicks on "OK" button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OKButton_Click(object sender, EventArgs e)
        {
            if (Page.IsValid){
                SaveData();

                bool success = false;

                if (OKClicked != null)
                    success = OKClicked(UserGroup);

                if (!success)
                {
                    Show(false);
                }
                else
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
        
        #endregion Protected methods

        #region Public methods

        public void UpdateUI()
        {
            if (EditMode)
            {
                ModalDialog1.Title = App_GlobalResources.SR.DialogEditUserGroupTitle;
                OKButton.EnabledImageURL = ImageServerConstants.ImageURLs.UpdateButtonEnabled;
                OKButton.HoverImageURL = ImageServerConstants.ImageURLs.UpdateButtonHover;
                GroupName.Text = UserGroup.Name;
                OriginalGroupName.Value = UserGroup.Name;
                foreach (TokenSummary token in UserGroup.Tokens)
                {
                    TokenCheckBoxList.Items.FindByValue(token.Name).Selected = true;
                }
            }
            else
            {
                ModalDialog1.Title = App_GlobalResources.SR.DialogAddUserGroupTitle;
                OKButton.EnabledImageURL = ImageServerConstants.ImageURLs.AddButtonEnabled;
                OKButton.HoverImageURL = ImageServerConstants.ImageURLs.AddButtonHover;
            }

            // Update the rest of the fields
            if (UserGroup == null || EditMode == false) 
            {
                GroupName.Text = string.Empty;
                TokenCheckBoxList.SelectedIndex = -1;
            }
        }

        public void Show(bool updateUI)
        {
            if (updateUI)
                UpdateUI();

            ModalDialog1.Show();
        }

        public void Close()
        {
            ModalDialog1.Hide();
        }

        #endregion Public methods
    }
}
