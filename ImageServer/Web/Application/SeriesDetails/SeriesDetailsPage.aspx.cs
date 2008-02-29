using System;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Web.Common.Data;

namespace ClearCanvas.ImageServer.Web.Application.SeriesDetails
{
    public partial class SeriesDetailsPage : System.Web.UI.Page
    {

        private const string QUERY_KEY_SERVER_AE = "serverae";
        private const string QUERY_KEY_STUDY_INSTANCE_UID = "studyuid";
        private const string QUERY_KEY_SERIES_INSTANCE_UID = "seriesuid";

        private string _serverae = null;
        private string _studyInstanceUid = null;
        private string _seriesInstanceUid = null;

        private Model.ServerPartition _partition;
        private Model.Study _study;
        private Model.Series _series;

        public ServerPartition Partition
        {
            get { return _partition; }
            set { _partition = value; }
        }

        public Model.Study Study
        {
            get { return _study; }
            set { _study = value; }
        }

        public Model.Series Series
        {
            get { return _series; }
            set { _series = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            _studyInstanceUid = Request.QueryString[QUERY_KEY_STUDY_INSTANCE_UID];
            _serverae = Request.QueryString[QUERY_KEY_SERVER_AE];
            _seriesInstanceUid = Request.QueryString[QUERY_KEY_SERIES_INSTANCE_UID];

            LoadSeriesDetails();
        }

        public override void RenderControl(HtmlTextWriter writer)
        {
            base.RenderControl(writer);

            if (_series==null)
            {
                Response.Write("<Br>NO SERIES FOUND<Br>");
            }
        }

        private void LoadSeriesDetails()
        {
            if (!String.IsNullOrEmpty(_serverae) && !String.IsNullOrEmpty(_studyInstanceUid) && !String.IsNullOrEmpty(_seriesInstanceUid))
            {
                ServerPartitionDataAdapter adaptor = new ServerPartitionDataAdapter();
                ServerPartitionSelectCriteria criteria = new ServerPartitionSelectCriteria();
                criteria.AeTitle.EqualTo(_serverae);

                IList<Model.ServerPartition> partitions = adaptor.Get(criteria);
                if (partitions != null && partitions.Count > 0)
                {
                    Partition = partitions[0];

                    StudyAdaptor studyAdaptor = new StudyAdaptor();
                    StudySelectCriteria studyCriteria = new StudySelectCriteria();
                    studyCriteria.StudyInstanceUid.EqualTo(_studyInstanceUid);
                    studyCriteria.ServerPartitionKey.EqualTo(Partition.GetKey());
                    IList<Model.Study> studies = studyAdaptor.Get(studyCriteria);

                    if (studies != null && studies.Count > 0)
                    {
                        // there should be only one study
                        _study = studies[0];

                        SeriesSearchAdaptor seriesAdaptor = new SeriesSearchAdaptor();
                        SeriesSelectCriteria seriesCriteria = new SeriesSelectCriteria();
                        seriesCriteria.SeriesInstanceUid.EqualTo(_seriesInstanceUid);
                        IList<Model.Series> series = seriesAdaptor.Get(seriesCriteria);

                        if (series != null && series.Count > 0)
                        {
                            _series = series[0];
                        }
                    }

                }
            }

            if (_series != null)
            {
                SeriesDetailsPanel1.Study = Study;
                SeriesDetailsPanel1.Series = Series;
            }
        }

    }
}
