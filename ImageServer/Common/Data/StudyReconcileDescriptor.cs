using System.Collections.Generic;
using System.Xml.Serialization;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Common.Data
{
    /// <summary>
    /// Represents information encoded in the xml of a <see cref="StudyHistory"/> record of type <see cref="StudyHistoryTypeEnum.StudyReconciled"/>.
    /// </summary>
    /// <remarks>
    /// </remarks>
    [XmlRoot("Reconcile")]
    public class StudyReconcileDescriptor
    {
        #region Private Members
        private bool _auto;
        private List<BaseImageLevelUpdateCommand> _commands;
        private StudyReconcileAction _action;
        private StudyInformation _existingStudyInfo;
        private ImageSetDescriptor _imageSet;
        private string _description;
        #endregion

        #region Public Properties
        /// <summary>
        /// Reconciliation option
        /// </summary>
        public StudyReconcileAction Action
        {
            get { return _action; }
            set { _action = value; }
        }

        /// <summary>
        /// User-defined description.
        /// </summary>
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        /// <summary>
        /// Gets or sets value indicating whether the reconciliation was automatic or manual.
        /// </summary>
        public bool Automatic
        {
            get { return _auto; }
            set { _auto = value; }
        }

        /// <summary>
        /// Gets or sets the <see cref="StudyInformation"/>
        /// </summary>
        public StudyInformation ExistingStudy
        {
            get { return _existingStudyInfo; }
            set { _existingStudyInfo = value; }
        }

        /// <summary>
        /// Gets or sets the <see cref="ImageSetDescriptor"/>
        /// </summary>
        public ImageSetDescriptor ImageSetData
        {
            get { return _imageSet; }
            set { _imageSet = value; }
        }

        /// <summary>
        /// Gets or sets the commands that are part of the reconciliation process.
        /// </summary>
        [XmlArray("Commands")]
        [XmlArrayItem("Command", Type = typeof(AbstractProperty<BaseImageLevelUpdateCommand>))]
        public List<BaseImageLevelUpdateCommand> Commands
        {
            get { return _commands; }
            set { _commands = value; }
        }
        #endregion
    }


    [XmlRoot("Reconcile")]
    public class ReconcileCreateStudyDescriptor : StudyReconcileDescriptor
    {
        public ReconcileCreateStudyDescriptor()
        {
            Action = StudyReconcileAction.CreateNewStudy;
        }
    }

    [XmlRoot("Reconcile")]
    public class ReconcileDiscardImagesDescriptor : StudyReconcileDescriptor
    {
        public ReconcileDiscardImagesDescriptor()
        {
            Action = StudyReconcileAction.Discard;
        }
    }

    [XmlRoot("Reconcile")]
    public class ReconcileMergeToExistingStudyDescriptor : StudyReconcileDescriptor
    {
        public ReconcileMergeToExistingStudyDescriptor()
        {
            Action = StudyReconcileAction.Merge;
        }
    }

    [XmlRoot("Reconcile")]
    public class ReconcileProcessAsIsDescriptor : StudyReconcileDescriptor
    {
        public ReconcileProcessAsIsDescriptor()
        {
            Action = StudyReconcileAction.ProcessAsIs;
        }
    }

}