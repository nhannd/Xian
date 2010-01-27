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
using System.Collections.Generic;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Web.Application.Pages.Common;
using ClearCanvas.ImageServer.Web.Common.Data;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies.Move
{
    public partial class Default : BasePage
    {
        #region constants
        private const string QUERY_KEY_STUDY_INSTANCE_UID = "studyuid";
        private const string QUERY_KEY_SERVER_AE = "serverae";
        #endregion constants

        #region Private Members
        private readonly IDictionary<string, string> _uids = new Dictionary<string, string>();
        #endregion

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            StudyController studyController = new StudyController();
            ServerPartitionConfigController partitionConfigController = new ServerPartitionConfigController();

            string serverae = Request.QueryString[QUERY_KEY_SERVER_AE];
            if (!String.IsNullOrEmpty(serverae))
            {
                // Load the Partition
                ServerPartitionSelectCriteria partitionCriteria = new ServerPartitionSelectCriteria();
                partitionCriteria.AeTitle.EqualTo(serverae);
                IList<ServerPartition> list = partitionConfigController.GetPartitions(partitionCriteria);
                this.Move.Partition = list[0];

                for (int i = 1;; i++)
                {
                    string studyuid = Request.QueryString[String.Format("{0}{1}", QUERY_KEY_STUDY_INSTANCE_UID, i)];

                    if (!String.IsNullOrEmpty(studyuid))
                    {
                        _uids.Add(studyuid, serverae);

                        StudySelectCriteria studyCriteria = new StudySelectCriteria();
                        studyCriteria.StudyInstanceUid.EqualTo(studyuid);
                        studyCriteria.ServerPartitionKey.EqualTo(list[0].GetKey());

                        IList<Study> studyList = studyController.GetStudies(studyCriteria);

                        this.Move.StudyGridView.StudyList.Add(studyList[0]);
                        this.Move.StudyGridView.Partition = this.Move.Partition;
                    }
                    else
                        break;
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //Hide the UserPanel information
            IMasterProperties master = Master as IMasterProperties;
            master.DisplayUserInformationPanel = false;
        }
    }
}
