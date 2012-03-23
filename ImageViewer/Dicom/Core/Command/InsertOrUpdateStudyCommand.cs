#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.ImageViewer.StudyManagement.Storage;

namespace ClearCanvas.ImageViewer.Dicom.Core.Command
{
    public class InsertOrUpdateStudyCommand : DataAccessCommand
    {
        private readonly DicomMessageBase _messageBase;
        private readonly string _studyInstanceUid;
        private readonly StudyXml _studyXml;

        public Study Study { get; private set; }        

        public InsertOrUpdateStudyCommand(DicomMessageBase message, StudyXml xml) : base("Insert or Update Study Command")
        {
            _messageBase = message;
            _studyInstanceUid = message.DataSet[DicomTags.StudyInstanceUid].GetString(0, String.Empty);
            _studyXml = xml;
        }

        protected override void OnExecute(CommandProcessor theProcessor)
        {
            var broker = DataAccessContext.GetStudyBroker();
            Study = broker.GetStudy(_studyInstanceUid);
            bool created = false;

            if (Study == null)
            {
                Study = new Study();
                created = true;
                Study.DeleteTime = DateTime.Now.AddDays(1);
                Study.Deleted = false;
            }

            Study.Initialize(_messageBase);

            Study.NumberOfStudyRelatedInstances = _studyXml.NumberOfStudyRelatedInstances;
            Study.NumberOfStudyRelatedSeries = _studyXml.NumberOfStudyRelatedSeries;

            if (created)
                broker.AddStudy(Study);
        }
    }
}
