using System;
using System.Collections.Generic;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Web.Application.Pages.Common;
using ClearCanvas.ImageServer.Web.Common.Data;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Search.Move
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
        }
    }
}
