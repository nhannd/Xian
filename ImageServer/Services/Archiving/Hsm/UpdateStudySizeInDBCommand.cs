using System;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Services.Archiving.Hsm
{
    /// <summary>
    /// Command to update the Study Size in the database.
    /// </summary>
    public class UpdateStudySizeInDBCommand : ServerDatabaseCommand
    {
        #region Private Members
        const decimal KB = 1024;
        private readonly StudyStorageLocation _location;
        private readonly decimal _studySizeInKB; 
        #endregion

        #region Constructors
        public UpdateStudySizeInDBCommand(StudyStorageLocation location)
            : base("Update Study Size In DB", true)
        {
            _location = location;

            // this may take a few ms so it's better to do it here instead in OnExecute()
            StudyXml studyXml = _location.LoadStudyXml();
            _studySizeInKB = studyXml.GetStudySize() / KB;
        } 
        #endregion

        protected override void OnExecute(ServerCommandProcessor theProcessor, IUpdateContext updateContext)
        {
            Study study = _location.Study ?? Study.Find(updateContext, _location.Key);

            if (study.StudySizeInKB != _studySizeInKB)
            {
                IStudyEntityBroker broker = updateContext.GetBroker<IStudyEntityBroker>();
                StudyUpdateColumns parameters = new StudyUpdateColumns()
                                                    {
                                                        StudySizeInKB = _studySizeInKB
                                                    };
                if (!broker.Update(study.Key, parameters))
                    throw new ApplicationException("Unable to update study size in the database");
            }
            
        }
    }
}