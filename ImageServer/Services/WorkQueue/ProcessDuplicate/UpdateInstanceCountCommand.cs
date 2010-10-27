#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Dicom;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Services.WorkQueue.ProcessDuplicate
{
    internal class UpdateInstanceCountCommand : ServerDatabaseCommand
    {

        #region Private Members

        private readonly StudyStorageLocation _studyLocation;
        private readonly DicomFile _file;

        #endregion

        #region Constructors

        public UpdateInstanceCountCommand(StudyStorageLocation studyLocation, DicomFile file)
            :base("Update Study Count", true)
        {
            _studyLocation = studyLocation;
            _file = file;
        }

        #endregion

        #region Overridden Protected Methods

        protected override void OnExecute(ServerCommandProcessor theProcessor, IUpdateContext updateContext)
        {
            String seriesUid = _file.DataSet[DicomTags.SeriesInstanceUid].ToString();
            String instanceUid = _file.DataSet[DicomTags.SopInstanceUid].ToString();
            
            IDeleteInstance deleteInstanceBroker = updateContext.GetBroker<IDeleteInstance>();
            DeleteInstanceParameters parameters = new DeleteInstanceParameters
                                                      {
                                                          StudyStorageKey = _studyLocation.GetKey(),
                                                          SeriesInstanceUid = seriesUid,
                                                          SOPInstanceUid = instanceUid
                                                      };
            if (!deleteInstanceBroker.Execute(parameters))
            {
                throw new ApplicationException("Unable to update instance count in db");
            }

        }

        #endregion

    }
}