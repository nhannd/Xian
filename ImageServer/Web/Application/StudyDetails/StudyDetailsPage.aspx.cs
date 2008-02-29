using System;
using System.Collections;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Web.Application.Common;
using ClearCanvas.ImageServer.Web.Common.Data;

namespace ClearCanvas.ImageServer.Web.Application.StudyDetails
{
    public partial class StudyDetailsPage : BasePage
    {
        private const string QUERY_KEY_STUDY_INSTANCE_UID = "siuid";
        private const string QUERY_KEY_SERVER_AE = "serverae";


        private string _studyInstanceUid = null;
        private string _serverae = null;
        private Model.ServerPartition _partition = null;
        private Model.Study _study = null;

        public ServerPartition Partition
        {
            get { return _partition; }
            set { _partition = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            _studyInstanceUid = Request.QueryString[QUERY_KEY_STUDY_INSTANCE_UID];
            _serverae = Request.QueryString[QUERY_KEY_SERVER_AE];

            if (!String.IsNullOrEmpty(_studyInstanceUid) && !String.IsNullOrEmpty(_serverae)) 
            {
                
                ServerPartitionDataAdapter adaptor = new ServerPartitionDataAdapter();
                ServerPartitionSelectCriteria criteria = new ServerPartitionSelectCriteria();
                criteria.AeTitle.EqualTo(Request.QueryString[QUERY_KEY_SERVER_AE]);
                IList<Model.ServerPartition> partitions = adaptor.Get(criteria);
                if (partitions != null && partitions.Count>0)
                {
                    Partition = partitions[0];

                    LoadStudy();
                    RenderStudyDetails();
                }

                
            }
            
            
            
        }

        private void LoadStudy()
        {
            if (String.IsNullOrEmpty(_studyInstanceUid))
                return;

            if (_partition == null)
                return;

            StudyAdaptor studyAdaptor = new StudyAdaptor();
            StudySelectCriteria criteria = new StudySelectCriteria();
            criteria.StudyInstanceUid.EqualTo(_studyInstanceUid);
            criteria.ServerPartitionKey.EqualTo(Partition.GetKey());
            IList<Model.Study> studies =  studyAdaptor.Get(criteria);

            if (studies!=null && studies.Count>0)
            {
                // there should be only one study
                _study  = studies[0];
            }
            
        }

        private void RenderStudyDetails()
        {
            if (_study!=null)
            {
                //PatientSummaryPanel.Summary = PatientSummaryAssembler.CreatePatientSummary(_study);
                StudyDetailsPanel.Study = _study;
                StudyDetailsPanel.Partition = _partition;
            }
            else
            {
                Response.Write("<Br>NO STUDY FOUND<Br>");
            }
        }


    }
}
