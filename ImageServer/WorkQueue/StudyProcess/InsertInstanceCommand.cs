#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Services.WorkQueue.StudyProcess
{
    public class InsertInstanceCommand : ServerCommand
    {
        #region Private Members

        private IReadContext _read;
        private DicomFile _file;
        private StudyStorageLocation _storageLocation;
        #endregion

        public InsertInstanceCommand(IReadContext read, DicomFile file, StudyStorageLocation location)
            : base("Insert Instance into Database")
        {
            Platform.CheckForNullReference(read, "File name");
            Platform.CheckForNullReference(file, "Dicom File object");
            Platform.CheckForNullReference(location, "Study Storage Location");

            _file = file;
            _storageLocation = location;
            _read = read;
        }

        public override void Execute()
        {
            // Setup the insert parameters
            InstanceInsertParameters parms = new InstanceInsertParameters();
            _file.DataSet.LoadDicomFields(parms);
            parms.ServerPartitionKey = _storageLocation.ServerPartitionKey;
            parms.StatusEnum = StatusEnum.GetEnum("Online");

            // Get the Insert Instance broker and do the insert
            IInsertInstance insert = _read.GetBroker<IInsertInstance>();
            IList<InstanceKeys> keys = insert.Execute(parms);

            // If the Request Attributes Sequence is in the dataset, do an insert.
            if (_file.DataSet.Contains(DicomTags.RequestAttributesSequence))
            {
                DicomAttributeSQ attribute = _file.DataSet[DicomTags.RequestAttributesSequence] as DicomAttributeSQ;
                if (!attribute.IsEmpty)
                {
                    foreach (DicomSequenceItem sequenceItem in (DicomSequenceItem[])attribute.Values)
                    {
                        RequestAttributesInsertParameters requestParms = new RequestAttributesInsertParameters();
                        sequenceItem.LoadDicomFields(requestParms);
                        requestParms.SeriesKey = keys[0].SeriesKey;

                        IInsertRequestAttributes insertRequest = _read.GetBroker<IInsertRequestAttributes>();
                        insertRequest.Execute(requestParms);
                    }
                }
            }
        }

        public override void Undo()
        {

        }
    }
}