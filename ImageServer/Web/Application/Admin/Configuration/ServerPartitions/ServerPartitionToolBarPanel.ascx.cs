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
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Web.Application.Admin.Configuration.ServerPartitions
{
    /// <summary>
    /// The Partition configuration toolbar.
    /// </summary>
    public partial class ServerPartitionToolBarPanel : System.Web.UI.UserControl
    {
        #region Private members
        
        #endregion Private Members

        #region Public properties
        

        #endregion

        #region Events/Delegates
        /// <summary>
        /// Defines the handler for <seealso cref="OnAddPartitionButtonClick"/> event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="ev"></param>
        public delegate void AddPartitionButtonClick(object sender, ImageClickEventArgs ev);
        /// <summary>
        /// Defines the handler for <seealso cref="DeletePartitionButtonClick"/> event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="ev"></param>
        public delegate void DeletePartitionButtonClick(object sender, ImageClickEventArgs ev);
        /// <summary>
        /// Defines the handler for <seealso cref="EditPartitionButtonClick"/> event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="ev"></param>
        public delegate void EditPartitionButtonClick(object sender, ImageClickEventArgs ev);
        /// <summary>
        /// Defines the handler for <seealso cref="OnRefreshButtonClick"/> event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="ev"></param>
        public delegate void RefreshButtonClick(object sender, ImageClickEventArgs ev);

        /// <summary>
        /// Occurs when user clicks on "Add" button.
        /// </summary>
        /// <remarks>
        /// This event is fired on the server side.
        /// </remarks>
        public event AddPartitionButtonClick OnAddPartitionButtonClick;
        /// <summary>
        /// Occurs when user clicks on "Delete" button.
        /// </summary>
        /// <remarks>
        /// This event is fired on the server side.
        /// </remarks>
        public event DeletePartitionButtonClick OnDeletePartitionButtonClick;
        /// <summary>
        /// Occurs when user clicks on "Edit" button.
        /// </summary>
        /// <remarks>
        /// This event is fired on the server side.
        /// </remarks>
        public event EditPartitionButtonClick OnEditPartitionButtonClick;
        /// <summary>
        /// Occurs when user clicks on "Refresh" button.
        /// </summary>
        /// <remarks>
        /// This event is fired on the server side.
        /// </remarks>
        public event RefreshButtonClick OnRefreshButtonClick;

        /// <summary>
        /// Defines the delegate which the toolbar can use to retrieve the currently selected Partition.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// The toolbar uses the selected Partition to update its UI.
        /// The delegate returns <b>null</b> if there's no Partition being selected. 
        /// </remarks>
        public delegate ServerPartition GetSelectedPartitionDelegate();

        /// <summary>
        /// Sets the delegate to be used by the toolbar to retrieve the selected Partition.
        /// </summary>
        public GetSelectedPartitionDelegate GetSelectedPartition;
        

        #endregion // Events

        #region Public methods

        
       

        #endregion // Public methods

        
        #region protected methods
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected override void OnPreRender(EventArgs e)
        {
            UpdateUI();
            base.OnPreRender(e);

        }

        protected void UpdateUI()
        {
            if (GetSelectedPartition != null)
            {
                ServerPartition parition = GetSelectedPartition();
                if (parition == null)
                {
                    // no Partition being selected

                    EditButton.Enabled = false;
                    EditButton.ImageUrl = "~/images/icons/EditDisabled.png";

                }
                else
                {
                    EditButton.Enabled = true;
                    EditButton.ImageUrl = "~/images/icons/EditEnabled.png";

                }
            }

        }



        protected void AddButton_Click(object sender, ImageClickEventArgs e)
        {

            OnAddPartitionButtonClick(sender, e);
        }

        protected void EditButton_Click(object sender, ImageClickEventArgs e)
        {

            OnEditPartitionButtonClick(sender, e);
        }
        protected void RefreshButton_Click(object sender, ImageClickEventArgs e)
        {
            OnRefreshButtonClick(sender, e);
        }

        #endregion Protected methods


       
        
    }

}
