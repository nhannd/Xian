#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.Dicom.Utilities.Xml;

namespace ClearCanvas.ImageServer.Services.WorkQueue.WebDeleteStudy
{
    internal class RemoveSeriesFromStudyXml : CommandBase
    {
        private readonly StudyXml _studyXml;
        private readonly string _seriesUid;
        private SeriesXml _oldSeriesXml;
        private readonly string _studyInstanceUid;

        public RemoveSeriesFromStudyXml(StudyXml studyXml, string seriesUid) 
            : base(String.Format("Remove series {0} from study XML of study {1}", seriesUid, studyXml.StudyInstanceUid), true)
        {
            _studyXml = studyXml;
            _seriesUid = seriesUid;
            _studyInstanceUid = studyXml.StudyInstanceUid;
        }

		protected override void OnExecute(CommandProcessor theProcessor)
        {
            // backup
            if (_studyXml.Contains(_seriesUid))
            {
                Platform.Log(LogLevel.Info, "Removing series {0} from StudyXML for study {1}", _seriesUid, _studyInstanceUid);
                _oldSeriesXml = _studyXml[_seriesUid];
                if (!_studyXml.RemoveSeries(_seriesUid))
                    throw new ApplicationException(String.Format("Could not remove series {0} from study {1}", _seriesUid, _studyInstanceUid));
            }
        }

        protected override void OnUndo()
        {
            if (_oldSeriesXml!=null)
            {
                Platform.Log(LogLevel.Info, "Restoring series {0} in StudyXML for study {1}", _seriesUid, _studyInstanceUid);
                _studyXml[_seriesUid] = _oldSeriesXml;
            }
        }
    }
}