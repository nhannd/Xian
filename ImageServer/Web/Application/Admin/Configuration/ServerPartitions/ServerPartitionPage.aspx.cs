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
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Common.Data;

namespace ClearCanvas.ImageServer.Web.Application.Admin.Configuration.ServerPartitions
{
    /// <summary>
    /// Server Partition configuration page.
    /// </summary>
    public partial class ServerPartitionPage : System.Web.UI.Page
    {
        #region Private Members
        // used for database interaction
        private ServerPartitionConfigController _controller = null;

        #endregion

        #region Protected Methods

        protected void Initialize()
        {
            _controller = new ServerPartitionConfigController();

            ServerPartitionPanel.Controller = _controller;

            SetupEventHandlers();
        }

        protected void SetupEventHandlers()
        {
            AddEditPartitionDialog1.OKClicked += delegate(ServerPartition partition)
                                                {
                                                    if (AddEditPartitionDialog1.EditMode)
                                                    {
                                                        // Add partition into db and refresh the list
                                                        if (_controller.UpdatePartition(partition))
                                                        {
                                                            UpdateUI();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        // Add partition into db and refresh the list
                                                        if (_controller.AddPartition(partition))
                                                        {
                                                            UpdateUI();
                                                        }
                                                    }
                                                    
                                                };

            
            ServerPartitionPanel.AddPartitionMethod = delegate
                                                          {
                                                              // display the add dialog
                                                              AddEditPartitionDialog1.Partition = null;
                                                              AddEditPartitionDialog1.EditMode = false;
                                                              AddEditPartitionDialog1.Show();
                                                          };

            ServerPartitionPanel.EditPartitionMethod = delegate(ServerPartition selectedPartition)
                                                           {
                                                               // display the add dialog
                                                               AddEditPartitionDialog1.Partition = selectedPartition;
                                                               AddEditPartitionDialog1.EditMode = true;
                                                               AddEditPartitionDialog1.Show();
                                                           };
        }


        protected void UpdateUI()
        {
            ServerPartitionPanel.UpdateUI();
            UpdatePanel.Update();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Initialize();

           
        }

        #endregion Protected Methods

        protected void Page_Load(object sender, EventArgs e)
        {
            UpdateUI();
        }


    }
}