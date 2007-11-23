#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Common;

namespace ClearCanvas.ImageServer.Web.Application.Admin.Configuration.ServerPartitions
{
    //
    // Dialog for adding new Partition.
    //
    public partial class AddEditPartitionDialog : UserControl
    {
        #region private variables
        private bool _editMode;
        // device being editted/added
        private ServerPartition _partition;
        
        #endregion

        #region public members

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
                this._partition = value;
                // put into viewstate to retrieve later
                ViewState[ClientID + "_EdittedPartition"] = _partition;
            }
            get
            {
                return _partition;
            }

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

        #region Public delegates


        #endregion // public delegates

        #region Protected methods
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
            ModalPopupExtender1.Drag = true;
            ModalPopupExtender1.PopupDragHandleControlID = TitleBarPanel.UniqueID;


            // Register a javascript that can be called to popup this dialog on the client
            // 
            Page.RegisterClientScriptBlock("popupThisWindow",
                      @"<script language='javascript'>
                        function ShowAddPartitionDialog()
                        {  
                            var ctrl = $find('" + ModalPopupExtender1.UniqueID + @"'); 
                            ctrl.show();
                        }
                    </script>");


            OKButton.OnClientClick = ClientID + "_clearFields()";

            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), this.ClientID,
                        @"<script language='javascript'>

                            function AddEditPartitionDialog_ClearField(fieldID)
                            {
                                txtbox = document.getElementById(fieldID);
                                txtbox.style.backgroundColor = '';
                            }

                            function " + ClientID + @"_clearFields()
                            {
                                
                                AddEditPartitionDialog_ClearField('" + AETitleTextBox.ClientID + @"');
                                AddEditPartitionDialog_ClearField('" + PortTextBox.ClientID + @"');
                                AddEditPartitionDialog_ClearField('" + PartitionFolderTextBox.ClientID + @"');
                                
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
                if (ViewState[ClientID + "_EditMode"] != null)
                    _editMode = (bool)ViewState[ClientID + "_EditMode"];

                if (ViewState[ClientID + "_EdittedPartition"] != null)
                    _partition = ViewState[ClientID + "_EdittedPartition"] as ServerPartition;
            }
        }


        /// <summary>
        /// Handles event when user clicks on "OK" button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OKButton_Click(object sender, EventArgs e)
        {
            if (Partition == null)
            {
                Partition = new ServerPartition();
            }

            Partition.Enabled = EnabledCheckBox.Checked;
            Partition.AeTitle = AETitleTextBox.Text;
            Partition.Description = DescriptionTextBox.Text;
            Partition.PartitionFolder = PartitionFolderTextBox.Text;
            Partition.Port = Int32.Parse(PortTextBox.Text);

            // TODO: Add input validation here


            if (OKClicked != null)
                OKClicked(Partition);

            Close();

        }

        #endregion Protected methods


        #region Public methods
        /// <summary>
        /// Displays the add device dialog box.
        /// </summary>
        public void Show()
        {
            if (EditMode)
            {
                TitleLabel.Text = "Edit Partition";
                OKButton.Text = "Update";
            }
            else
            {
                TitleLabel.Text = "Add Partition";
                OKButton.Text = "Add";
            }

            if (Partition== null)
            {
                AETitleTextBox.Text = "";
                DescriptionTextBox.Text = "";
                PortTextBox.Text = "0";
                PartitionFolderTextBox.Text = "";
                EnabledCheckBox.Checked = false;
            }
            else
            {
                AETitleTextBox.Text = Partition.AeTitle;
                DescriptionTextBox.Text = Partition.Description;
                PortTextBox.Text = Partition.Port.ToString();
                PartitionFolderTextBox.Text = Partition.PartitionFolder;
                EnabledCheckBox.Checked = Partition.Enabled;
            }


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
