#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.ImageServer.Common.Command;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Services.WorkQueue.WebDeleteStudy
{
    internal class DeleteSeriesFromDBCommand : ServerDatabaseCommand
    {
        private readonly StudyStorageLocation _location;
        private readonly Series _series;

        public DeleteSeriesFromDBCommand(StudyStorageLocation location, Series series)
            : base(String.Format("Delete Series In DB {0}", series.SeriesInstanceUid))
        {
            _location = location;
            _series = series;
        }

        public Series Series
        {
            get { return _series; }
        }


        protected override void OnExecute(CommandProcessor theProcessor, ClearCanvas.Enterprise.Core.IUpdateContext updateContext)
        {
            IDeleteSeries broker = updateContext.GetBroker<IDeleteSeries>();
            DeleteSeriesParameters criteria = new DeleteSeriesParameters();
            criteria.StudyStorageKey = _location.Key;
            criteria.SeriesInstanceUid = _series.SeriesInstanceUid;
            if (!broker.Execute(criteria))
                throw new ApplicationException("Error occurred when calling DeleteSeries");
        }
    }

    internal class DeleteSeriesFromDBCommandEventArgs:EventArgs
    {
    }
}