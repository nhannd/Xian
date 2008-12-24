using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Services.WorkQueue.WebEditStudy
{
    /// <summary>
    /// Decoded information of a <see cref="StudyHistory"/> record of type "WebEdited"
    /// </summary>
    /// <remarks>
    /// Use <see cref="CreateFrom"/> to create an instance of <see cref="WebEditStudyHistoryRecord"/>
    /// from a <see cref="StudyHistory"/>
    /// </remarks>
    public class WebEditStudyHistoryRecord
    {
        #region Private Fields
        private DateTime _insertTime;
        private StudyStorageLocation _studyStorageLocation;
        private StudyInformation _originalStudy;
        private WebEditStudyHistoryChangeDescription _updateDescription;
        #endregion

        #region Public Properties
        public DateTime InsertTime
        {
            get { return _insertTime;  }
            set { _insertTime = value; }
        }

        public StudyStorageLocation StudyStorageLocation
        {
            get { return _studyStorageLocation;  }
            set { _studyStorageLocation = value; }
        }

        public StudyInformation OriginalStudyData
        {
            get { return _originalStudy; }
            set { _originalStudy = value; }
        }

        public WebEditStudyHistoryChangeDescription UpdateDescription
        {
            get { return _updateDescription; }
            set { _updateDescription = value; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// ****** For internal use only *******
        /// </summary>
        private WebEditStudyHistoryRecord()
        {
        }
        #endregion

        #region Public Static Methods

        /// <summary>
        /// Creates an instance of <see cref="WebEditStudyHistoryRecord"/>
        /// from a <see cref="StudyHistory"/>
        /// </summary>
        /// <param name="historyRecord"></param>
        /// <returns></returns>
        public static WebEditStudyHistoryRecord CreateFrom(StudyHistory historyRecord)
        {
            Platform.CheckTrue(historyRecord.StudyHistoryTypeEnum == StudyHistoryTypeEnum.WebEdited,
                               "History record has invalid history record type");

            WebEditStudyHistoryRecord record = new WebEditStudyHistoryRecord();
            record.InsertTime = historyRecord.InsertTime;
            record.StudyStorageLocation = StudyStorageLocation.FindStorageLocations(StudyStorage.Load(historyRecord.StudyStorageKey))[0];
            record.OriginalStudyData =
                StudyInformation.CreateFrom(
                    Study.Find(record.StudyStorageLocation.StudyInstanceUid,
                               ServerPartition.Load(record.StudyStorageLocation.ServerPartitionKey)));

            record.UpdateDescription = XmlUtils.Deserialize<WebEditStudyHistoryChangeDescription>(historyRecord.ChangeDescription);
            return record;
        }
        #endregion
    }


    /// <summary>
    /// Decoded information of the ChangeDescription field of a <see cref="StudyHistory"/> 
    /// record whose type is "WebEdited"
    /// </summary>
    public class WebEditStudyHistoryChangeDescription
    {
        #region Private Fields
        private List<BaseImageLevelUpdateCommand> _commands;
        private EditType _editType;
        #endregion

        #region Public Properties

        /// <summary>
        /// Type of the edit operation occured on the study.
        /// </summary>
        [XmlElement("EditType")]
        public EditType EditType
        {
            get { return _editType; }
            set { _editType = value; }
        }


        /// <summary>
        /// List of <see cref="BaseImageLevelUpdateCommand"/> that were executed on the study.
        /// </summary>
        [XmlArrayItem("Command", Type=typeof(AbstractProperty<BaseImageLevelUpdateCommand>))]
        public List<BaseImageLevelUpdateCommand> UpdateCommands
        {
            get
            {
                return _commands;
            }
            set
            {
                _commands = value;
            }
        }

        #endregion
    }


}