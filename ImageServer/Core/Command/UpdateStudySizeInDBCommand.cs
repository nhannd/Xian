#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common.Command;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Core.Command
{
    /// <summary>
    /// Command to update the Study Size in the database.
    /// </summary>
    public class UpdateStudySizeInDBCommand : ServerDatabaseCommand
    {
        #region Private Members
        const decimal KB = 1024;
        private readonly StudyStorageLocation _location;
        private decimal _studySizeInKB;
        private readonly RebuildStudyXmlCommand _rebuildCommand;
        #endregion

        #region Constructors
        public UpdateStudySizeInDBCommand(StudyStorageLocation location)
            : base("Update Study Size In DB")
        {
            _location = location;

            // this may take a few ms so it's better to do it here instead in OnExecute()
            StudyXml studyXml = _location.LoadStudyXml();
            _studySizeInKB = studyXml.GetStudySize() / KB;
        }

        public UpdateStudySizeInDBCommand(StudyStorageLocation location, RebuildStudyXmlCommand rebuildCommand)
            : base("Update Study Size In DB")
        {
            _location = location;

            _rebuildCommand = rebuildCommand;
        }
        #endregion

        protected override void OnExecute(CommandProcessor theProcessor, IUpdateContext updateContext)
        {
            if (_rebuildCommand != null) _studySizeInKB = _rebuildCommand.StudyXml.GetStudySize() / KB;

            Study study = _location.Study ?? Study.Find(updateContext, _location.Key);

            if (study != null && study.StudySizeInKB != _studySizeInKB)
            {
                var broker = updateContext.GetBroker<IStudyEntityBroker>();
                var parameters = new StudyUpdateColumns
                                     {
                                         StudySizeInKB = _studySizeInKB
                                     };
                if (!broker.Update(study.Key, parameters))
                    throw new ApplicationException("Unable to update study size in the database");
            }
        }
    }
}