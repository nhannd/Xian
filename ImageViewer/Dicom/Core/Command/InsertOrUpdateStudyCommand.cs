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
        
        public ViewerCommandProcessorContext Context { get { return ProcessorContext as ViewerCommandProcessorContext; } } 

        public InsertOrUpdateStudyCommand(StudyLocation location, DicomMessageBase message, StudyXml xml) : base("Insert or Update Study Command")
        {
            _messageBase = message;
            _studyInstanceUid = message.DataSet[DicomTags.StudyInstanceUid].GetString(0, String.Empty);
            _studyXml = xml;
            _location = location;
        }

        protected override void OnExecute(CommandProcessor theProcessor)
        {
            if (Context.ContextStudy == null)
            {
                var broker = DataAccessContext.GetStudyBroker();
                Context.ContextStudy = broker.GetStudy(_studyInstanceUid);

                if (Context.ContextStudy == null)
                {
                    // This is a bit of a hack to handle batch processing of studies
                    Context.ContextStudy = _location.Study;
                    Context.ContextStudy.StoreTime = Platform.Time;
                    broker.AddStudy(Context.ContextStudy);
                }
            }

            Context.ContextStudy.Deleted = false;            
            Context.ContextStudy.NumberOfStudyRelatedInstances = _studyXml.NumberOfStudyRelatedInstances;
            Context.ContextStudy.NumberOfStudyRelatedSeries = _studyXml.NumberOfStudyRelatedSeries;
            Context.ContextStudy.Update(_messageBase);
        }
    }
}
