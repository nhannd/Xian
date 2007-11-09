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
using System.Web.UI.WebControls;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Common;

namespace ImageServerWebApplication.Admin.Configuration.ServerPartitions
{
    /// <summary>
    /// Dialog box for updating partition.
    /// </summary>
    public partial class EditPartitionDialog : System.Web.UI.UserControl
    {
        #region Private members
        // partition being editted
        private ServerPartition _partition;
        #endregion


        #region Public Properties
        

        /// <summary>
        /// Sets/Gets the partition being currently editted.
        /// </summary>
        public ServerPartition Partition
        {
            set
            {
                this._partition = value;
                // put into viewstate to retrieve later
                ViewState["EdittedPartition"] = _partition;
            }
            get
            {
                return _partition;
            }

        }

        #endregion //Public Properties


        #region Events

        /// <summary>
        /// Defines the event handler for <seealso cref="OKClicked"/>
        /// </summary>
        /// <param name="newPartition">The partition being editted.</param>
        public delegate void OKClickedEventHandler(ServerPartition newPartition);

        /// <summary>
        /// Occurs when users click on "OK" button.
        /// </summary>
        public event OKClickedEventHandler OKClicked;

        #endregion Events

        #region protected methods
        protected  override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ModalPopupExtender1.PopupControlID = DialogPanel.UniqueID;
            ModalPopupExtender1.TargetControlID = DummyPanel.UniqueID;
            //ModalPopupExtender1.OkControlID = OKButton.UniqueID;
            ModalPopupExtender1.CancelControlID = CancelButton.UniqueID;

            ModalPopupExtender1.DropShadow = true;
            ModalPopupExtender1.Drag = true;
            ModalPopupExtender1.PopupDragHandleControlID = TitleLabel.UniqueID;

            ModalPopupExtender1.Hide();
        }
        
        protected void Page_Load(object sender, EventArgs e)
        {

            if (Page.IsPostBack == false)
            {
                
            }
            else
            {
                // reload the partition information that was editted
                _partition = ViewState["EdittedPartition"] as ServerPartition;
            }


        }

        protected void UpdateUIData()
        {
            AETitleTextBox.Text = Partition.AeTitle;
            DescriptionTextBox.Text = Partition.Description;
            EnabledCheckBox.Checked = Partition.Enabled;
            PortTextBox.Text = Partition.Port.ToString();
            FolderTextBox.Text = Partition.PartitionFolder;
        }

        protected void OKButton_Click(object sender, EventArgs e)
        {
            Partition.Enabled = EnabledCheckBox.Checked;
            Partition.AeTitle = AETitleTextBox.Text;
            Partition.Description = DescriptionTextBox.Text;
            Partition.Port = Int32.Parse(PortTextBox.Text);
            Partition.PartitionFolder = FolderTextBox.Text;

            if (OKClicked != null)
                OKClicked(Partition);

            Close();

        }
        
        #endregion // protected methods


        #region Public Methods
        /// <summary>
        /// Displays the edit dialog box.
        /// </summary>
        public void Show()
        {
            UpdateUIData();
            UpdatePanel.Update();
            ModalPopupExtender1.Show();
        }

        /// <summary>
        /// Dismisses the edit dialog box.
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        public void Close()
        {

            /* 
             FOR SOME REASON, THIS WILL CAUSE EXCEPTION
             
             // Clear all boxes... 
             AETitleTextBox.Text = "<Enter AE Title>";
             ActiveCheckBox.Checked = false;
             DHCPCheckBox.Checked = false;
             DescriptionTextBox.Text = "<Enter Description>";
             PortTextBox.Text = "<Port #>";

             */


            ModalPopupExtender1.Hide();
        }

        #endregion Public methods

        
    }

}

