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
using AjaxControlToolkit;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Common;
using System.Collections.Generic;
using ClearCanvas.ImageServer.Web.Common;

namespace ImageServerWebApplication.Admin.Configuration.ServerPartitions
{
    /// <summary>
    /// Server Partition configuration page.
    /// </summary>
    public partial class ServerPartitionPage : System.Web.UI.Page
    {
        #region Private Members
        // used for database interaction
        private ServerParitionConfigController _controller = null;

        #endregion

        #region Protected Methods

        protected void Initialize()
        {
            _controller = new ServerParitionConfigController();

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