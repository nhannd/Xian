
namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Code
{
    /// <summary>
    /// Model object behind the <see cref="StudyDetailsPanel"/>
    /// </summary>
    public class StudyDetails
    {
        private string _studyInstanceUid;
        private string _studyDate;
        private string _studyTime;
        private string _accessionNumber;
        private string _studyId;
        private string _studyDescription;
        private string _referringPhysicianName;
        private int _numberOfStudyRelatedSeries;
        private int _numberOfStudyRelatedInstances;
        private string _status;
        private bool _scheduledForDelete;

        public string StudyInstanceUid
        {
            get { return _studyInstanceUid; }
            set { _studyInstanceUid = value; }
        }

        public string StudyDate
        {
            get { return _studyDate; }
            set { _studyDate = value; }
        }

        public string StudyTime
        {
            get { return _studyTime; }
            set { _studyTime = value; }
        }

        public string AccessionNumber
        {
            get { return _accessionNumber; }
            set { _accessionNumber = value; }
        }

        public string StudyId
        {
            get { return _studyId; }
            set { _studyId = value; }
        }

        public string StudyDescription
        {
            get { return _studyDescription; }
            set { _studyDescription = value; }
        }

        public string ReferringPhysicianName
        {
            get { return _referringPhysicianName; }
            set { _referringPhysicianName = value; }
        }

        public int NumberOfStudyRelatedSeries
        {
            get { return _numberOfStudyRelatedSeries; }
            set { _numberOfStudyRelatedSeries = value; }
        }

        public int NumberOfStudyRelatedInstances
        {
            get { return _numberOfStudyRelatedInstances; }
            set { _numberOfStudyRelatedInstances = value; }
        }

        public string Status
        {
            get { return _status; }
            set { _status = value; }
        }

        public bool ScheduledForDelete
        {
            get { return _scheduledForDelete; }
            set { _scheduledForDelete = value; }
        }
    }
}
