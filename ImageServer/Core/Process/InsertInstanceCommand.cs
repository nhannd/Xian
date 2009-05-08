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

using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Core.Process
{
	/// <summary>
	/// ServerCommand for inserting a DICOM File into the persistent store.
	/// </summary>
	public class InsertInstanceCommand : ServerDatabaseCommand
	{
		#region Private Members

		private readonly DicomFile _file;
		private readonly StudyStorageLocation _storageLocation;
		private InstanceKeys _insertKey;
		#endregion

		#region Public Properties
		public InstanceKeys InsertKeys
		{
			get { return _insertKey; }
		}
		#endregion

		public InsertInstanceCommand(DicomFile file, StudyStorageLocation location)
			: base("Insert Instance into Database", true)
		{
			Platform.CheckForNullReference(file, "Dicom File object");
			Platform.CheckForNullReference(location, "Study Storage Location");

			_file = file;
			_storageLocation = location;
		}

		protected override void OnExecute(IUpdateContext updateContext)
		{
			// Setup the insert parameters
			InsertInstanceParameters parms = new InsertInstanceParameters();
			_file.LoadDicomFields(parms);
			parms.ServerPartitionKey = _storageLocation.ServerPartitionKey;
			parms.StudyStorageKey = _storageLocation.Key;

			// Get the Insert Instance broker and do the insert
			IInsertInstance insert = updateContext.GetBroker<IInsertInstance>();

			if (_file.DataSet.Contains(DicomTags.SpecificCharacterSet))
			{
				string cs = _file.DataSet[DicomTags.SpecificCharacterSet].ToString();;
				parms.SpecificCharacterSet = cs;
			}

			_insertKey = insert.FindOne(parms);

			// If the Request Attributes Sequence is in the dataset, do an insert.
			if (_file.DataSet.Contains(DicomTags.RequestAttributesSequence))
			{
				DicomAttributeSQ attribute = _file.DataSet[DicomTags.RequestAttributesSequence] as DicomAttributeSQ;
				if (!attribute.IsEmpty)
				{
					foreach (DicomSequenceItem sequenceItem in (DicomSequenceItem[]) attribute.Values)
					{
						RequestAttributesInsertParameters requestParms = new RequestAttributesInsertParameters();
						sequenceItem.LoadDicomFields(requestParms);
						requestParms.SeriesKey = _insertKey.SeriesKey;

						IInsertRequestAttributes insertRequest = updateContext.GetBroker<IInsertRequestAttributes>();
						insertRequest.Execute(requestParms);
					}
				}
			}
		}
	}
}