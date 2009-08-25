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

using System;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.ImageServer.Common.CommandProcessor;

namespace ClearCanvas.ImageServer.Services.WorkQueue.WebDeleteStudy
{
    internal class RemoveSeriesFromStudyXml : ServerCommand
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

		protected override void OnExecute(ServerCommandProcessor theProcessor)
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