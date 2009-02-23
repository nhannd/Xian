using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Services.WorkQueue.WebEditStudy
{
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