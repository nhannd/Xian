#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
using System.Security.Permissions;
using ClearCanvas.ImageServer.Model;
using System.Collections.Generic;
using ClearCanvas.ImageServer.Web.Application.Pages.Common;
using ClearCanvas.ImageServer.Web.Common.Data;
using AuthorityTokens=ClearCanvas.ImageServer.Enterprise.Authentication.AuthorityTokens;


namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.FileSystems
{
    [PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Configuration.FileSystems)]
    public partial class Default : BasePage
    {
        #region Private members

        // the controller used for database interaction
        private FileSystemsConfigurationController _controller = new FileSystemsConfigurationController();

        #endregion Private members

        #region Protected methods

        /// <summary>
        /// Set up the event handlers for child controls.
        /// </summary>
        protected void SetupEventHandlers()
        {
            AddEditFileSystemDialog1.OKClicked += delegate(Filesystem fs)
                                                      {
                                                          if (AddEditFileSystemDialog1.EditMode)
                                                          {
                                                              // Commit the new FileSystems into database
                                                              if (_controller.UpdateFileSystem(fs))
                                                              {
                                                                  FileSystemsPanel1.UpdateUI();
                                                              }
                                                          }
                                                          else
                                                          {
                                                              // Commit the new FileSystems into database
                                                              if (_controller.AddFileSystem(fs))
                                                              {
                                                                  FileSystemsPanel1.UpdateUI();
                                                              }
                                                          }
                                                      };
        }


        /// <summary>
        /// Retrieves the Filesystems to be rendered in the page.
        /// </summary>
        /// <returns></returns>
        private IList<Filesystem> GetFilesystems()
        {
            // TODO We may want to add context or user preference here to specify which partitions to load

            IList<Filesystem> list = _controller.GetAllFileSystems();
            return list;
        }

        protected override void OnInit(EventArgs e)
        {
            FileSystemsPanel1.EnclosingPage = this;

            base.OnInit(e);

            _controller = new FileSystemsConfigurationController();

            SetupControls();
            SetupEventHandlers();
        }

        protected void SetupControls()
        {
            FileSystemsPanel1.FileSystems = GetFilesystems();
            AddEditFileSystemDialog1.FilesystemTiers = _controller.GetFileSystemTiers();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            SetPageTitle(App_GlobalResources.Titles.FileSystemsPageTitle);
        }

        #endregion  Protected methods

        #region Public Methods

        public void OnAddFileSystem()
        {
            AddEditFileSystemDialog1.EditMode = false;
            AddEditFileSystemDialog1.FileSystem = null;
            AddEditFileSystemDialog1.Show(true);
        }

        public void OnEditFileSystem(FileSystemsConfigurationController controller, Filesystem fs)
        {
            AddEditFileSystemDialog1.EditMode = true;
            AddEditFileSystemDialog1.FileSystem = fs;
            AddEditFileSystemDialog1.Show(true);
        }

        #endregion
    }
}
