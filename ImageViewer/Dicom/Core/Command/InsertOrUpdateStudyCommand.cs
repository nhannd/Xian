#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.ImageViewer.StudyManagement.Storage;

namespace ClearCanvas.ImageViewer.Dicom.Core.Command
{
    /// <summary>
    /// Insert a new <see cref="Study"/> object or update an already existing <see cref="Study"/> object.
    /// </summary>
    public class InsertOrUpdateStudyCommand : DataAccessCommand
    {
        private readonly DicomMessageBase _messageBase;
        private readonly string _studyInstanceUid;
        private readonly StudyXml _studyXml;
        private readonly StudyLocation _location;

        public bool Created { get; private set; }
        public Study Study { get; private set; }        

        public InsertOrUpdateStudyCommand(StudyLocation location, DicomMessageBase message, StudyXml xml) : base("Insert or Update Study Command")
        {
            _messageBase = message;
            _studyInstanceUid = message.DataSet[DicomTags.StudyInstanceUid].GetString(0, String.Empty);
            _studyXml = xml;
            _location = location;
        }

        protected override void OnExecute(CommandProcessor theProcessor)
        {
            var broker = DataAccessContext.GetStudyBroker();
            Study = broker.GetStudy(_studyInstanceUid);
            Created = false;

            if (Study == null)
            {
                // This is a bit of a hack to handle batch processing of studies
                Study = _location.Study;
                Created = true;
                Study.DeleteTime = DateTime.Now.AddDays(1);
                Study.StoreTime = Platform.Time;                
            }

            Study.Deleted = false;

            Study.Initialize(_messageBase);
            
            Study.NumberOfStudyRelatedInstances = _studyXml.NumberOfStudyRelatedInstances;
            Study.NumberOfStudyRelatedSeries = _studyXml.NumberOfStudyRelatedSeries;

            if (Created)
                broker.AddStudy(Study);
        }
    }
}
