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
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Core.Reconcile;

namespace ClearCanvas.ImageServer.Core.Reconcile
{
	class InstanceAlreadyExistsException: Exception
	{
        
	}

	/// <summary>
	/// Save a dicom file
	/// </summary>
	class SaveFileCommand : ServerCommand<ReconcileStudyProcessorContext, DicomFile>
	{
		private ServerCommandProcessor _processor;

		public SaveFileCommand(ReconcileStudyProcessorContext context, DicomFile file)
			: base("SaveFileCommand", true, context, file)
		{
		}

		protected override void OnExecute()
		{
			Platform.CheckForNullReference(Context.DestStorageLocation, "Context.DestStorageLocation");
			DicomFile file = Parameters;
			String seriesInstanceUid = file.DataSet[DicomTags.SeriesInstanceUid].GetString(0, String.Empty);
			String sopInstanceUid = file.DataSet[DicomTags.SopInstanceUid].GetString(0, String.Empty);

			String destPath = Context.DestStorageLocation.GetSopInstancePath(seriesInstanceUid, sopInstanceUid);
            
			if (File.Exists(destPath))
			{
				#region Duplicate SOP

				throw new InstanceAlreadyExistsException();

				#endregion
			}

			_processor = new ServerCommandProcessor("SaveFileCommand Processor");

			_processor.AddCommand(new CreateDirectoryCommand(Context.DestStorageLocation.GetStudyPath()));
			_processor.AddCommand(new CreateDirectoryCommand(Context.DestStorageLocation.GetSeriesPath(seriesInstanceUid)));

			_processor.AddCommand(new SaveDicomFileCommand(destPath, file, true, true));

			if (!_processor.Execute())
			{
				throw new ApplicationException(_processor.FailureReason);
			}
            
		}

		protected override void OnUndo()
		{
			if (_processor!=null)
			{
				_processor.Rollback();
				_processor = null;
			}
		}
	}
}