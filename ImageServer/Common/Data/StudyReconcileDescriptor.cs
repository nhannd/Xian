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
        private string _userName;
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
            get
            {
                if (_commands == null)
                    _commands = new List<BaseImageLevelUpdateCommand>();
                return _commands;
            }
            set { _commands = value; }
        }

        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
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