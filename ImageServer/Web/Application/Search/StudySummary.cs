using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Web.Application.Search
{
    /// <summary>
    /// Holds summary information of a <see cref="Study"/>.
    /// </summary>
    /// <remarks>
    /// This class serves as the behind-the-scene structure for study list grid view <see cref="StudyListGridView"/> in study search page.
    /// It may contain more or less study information than a <see cref="Study"/>.
    /// </remarks>
    public class StudySummary
    {
        #region Private members
        private ServerEntityKey _ref;
        private string _patientID;
        private string _patientName;
        private string _studyDate;
        private string _accessionNumber;
        private string _studyDescription;
        private int _numberOfRelatedSeries;
        private int _numberOfRelatedInstances;

        #endregion Private members

        
        #region Public Properties

        public ServerEntityKey GUID
        {
            get { return _ref; }
            set { _ref = value; }
        }

        public string PatientID
        {
            get { return _patientID; }
            set { _patientID = value; }
        }

        public string PatientsName
        {
            get { return _patientName; }
            set { _patientName = value; }
        }

        public string StudyDate
        {
            get { return _studyDate; }
            set { _studyDate = value; }
        }

        public string AccessionNumber
        {
            get { return _accessionNumber; }
            set { _accessionNumber = value; }
        }

        public string StudyDescription
        {
            get { return _studyDescription; }
            set { _studyDescription = value; }
        }

        public int NumberOfRelatedSeries
        {
            get { return _numberOfRelatedSeries; }
            set { _numberOfRelatedSeries = value; }
        }

        public int NumberOfRelatedInstances
        {
            get { return _numberOfRelatedInstances; }
            set { _numberOfRelatedInstances = value; }
        }

        #endregion Public Properties
    }
}
