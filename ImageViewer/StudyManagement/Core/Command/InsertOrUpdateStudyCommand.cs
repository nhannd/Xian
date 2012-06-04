#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.ImageViewer.StudyManagement.Core.Storage;

namespace ClearCanvas.ImageViewer.StudyManagement.Core.Command
{
    /// <summary>
    /// Insert a new <see cref="Study"/> object or update an already existing <see cref="Study"/> object.
    /// </summary>
    public class InsertOrUpdateStudyCommand : DataAccessCommand
    {
        public enum UpdateReason
        {
            LiveImport,
            Reprocessing,
            SopsDeleted
        }

        private readonly string _studyInstanceUid;
        private readonly StudyXml _studyXml;
        private readonly UpdateReason _reason;
        private readonly StudyLocation _location;

        
        public ViewerCommandProcessorContext Context { get { return ProcessorContext as ViewerCommandProcessorContext; } } 

        public InsertOrUpdateStudyCommand(StudyLocation location, StudyXml xml, UpdateReason reason) : base("Insert or Update Study Command")
        {
            _studyInstanceUid = xml.StudyInstanceUid;
            _studyXml = xml;
            _reason = reason;
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
                    broker.AddStudy(Context.ContextStudy);
                }
            }

            //Only update the store time if the study is actively being received/imported.
            if (_reason == UpdateReason.LiveImport || Context.ContextStudy.StoreTime == null)
                Context.ContextStudy.StoreTime = Platform.Time;

            if (_reason != UpdateReason.SopsDeleted)
            {
                //Only update these if the study is being updated in an "additive" way (import/receive/re-index).
                //A series deletion, for example, should not update these.
                Context.ContextStudy.Deleted = false;
                Context.ContextStudy.Reindex = false;
            }

            Context.ContextStudy.Update(_studyXml);
        }
    }
}
