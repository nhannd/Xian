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

namespace ClearCanvas.ImageServer.Web.Application.Admin.Configuration.FileSystems
{
    /// <summary>
    /// The FileSystem configuration toolbar.
    /// </summary>
    public partial class FileSystemsToolBar : System.Web.UI.UserControl
    {
        #region Private members
        
        #endregion Private Members

        #region Public properties
        

        #endregion

        #region Events/Delegates
        /// <summary>
        /// Defines the handler for <seealso cref="OnAddFileSystemButtonClick"/> event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="ev"></param>
        public delegate void AddFileSystemButtonClick(object sender, ImageClickEventArgs ev);
        /// <summary>
        /// Defines the handler for <seealso cref="DeleteFileSystemButtonClick"/> event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="ev"></param>
        public delegate void DeleteFileSystemButtonClick(object sender, ImageClickEventArgs ev);
        /// <summary>
        /// Defines the handler for <seealso cref="EditFileSystemButtonClick"/> event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="ev"></param>
        public delegate void EditFileSystemButtonClick(object sender, ImageClickEventArgs ev);
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
        public event AddFileSystemButtonClick OnAddFileSystemButtonClick;
        /// <summary>
        /// Occurs when user clicks on "Delete" button.
        /// </summary>
        /// <remarks>
        /// This event is fired on the server side.
        /// </remarks>
        public event DeleteFileSystemButtonClick OnDeleteFileSystemButtonClick;
        /// <summary>
        /// Occurs when user clicks on "Edit" button.
        /// </summary>
        /// <remarks>
        /// This event is fired on the server side.
        /// </remarks>
        public event EditFileSystemButtonClick OnEditFileSystemButtonClick;
        /// <summary>
        /// Occurs when user clicks on "Refresh" button.
        /// </summary>
        /// <remarks>
        /// This event is fired on the server side.
        /// </remarks>
        public event RefreshButtonClick OnRefreshButtonClick;

        /// <summary>
        /// Defines the delegate which the toolbar can use to retrieve the currently selected FileSystem.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// The toolbar uses the selected FileSystem to update its UI.
        /// The delegate returns <b>null</b> if there's no FileSystem being selected. 
        /// </remarks>
        public delegate Filesystem GetSelectedFileSystemDelegate();

        /// <summary>
        /// Sets the delegate to be used by the toolbar to retrieve the selected FileSystem.
        /// </summary>
        public GetSelectedFileSystemDelegate GetSelectedFileSystem;
        

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
            if (GetSelectedFileSystem != null)
            {
                Filesystem dev = GetSelectedFileSystem();
                if (dev == null)
                {
                    // no FileSystem being selected

                    EditButton.Enabled = false;
                    EditButton.ImageUrl = "~/images/edit_disabled.gif";

                    DeleteButton.Enabled = false;
                    DeleteButton.ImageUrl = "~/images/delete_disabled.gif";

                }
                else
                {
                    EditButton.Enabled = true;
                    EditButton.ImageUrl = "~/images/edit2.gif";

                    DeleteButton.Enabled = true;
                    DeleteButton.ImageUrl = "~/images/delete2.gif";
                }
            }

        }



        protected void AddButton_Click(object sender, ImageClickEventArgs e)
        {

            OnAddFileSystemButtonClick(sender, e);
        }
        protected void DeleteButton_Click(object sender, ImageClickEventArgs e)
        {
            OnDeleteFileSystemButtonClick(sender, e);
        }
        protected void EditButton_Click(object sender, ImageClickEventArgs e)
        {

            OnEditFileSystemButtonClick(sender, e);
        }
        protected void RefreshButton_Click(object sender, ImageClickEventArgs e)
        {
            OnRefreshButtonClick(sender, e);
        }

        #endregion Protected methods


       
        
    }

}
