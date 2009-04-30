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
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.ImageServer.Common.CommandProcessor;

namespace ClearCanvas.ImageServer.Services.ServiceLock.FilesystemRebuildXml
{
	/// <summary>
	/// Insert DICOM file into a <see cref="StudyXml"/> file and save to disk.
	/// </summary>
	public class InsertInstanceXmlCommand : ServerCommand
	{
		#region Private Members

		private readonly StudyXml _stream;
		private readonly string _path;
		#endregion


		#region Constructors

		public InsertInstanceXmlCommand(StudyXml stream, string path)
			: base("Insert into Study XML", false)
		{
			Platform.CheckForNullReference(stream, "StudyStream object");
			Platform.CheckForNullReference(path, "Path to DICOM File");

			_stream = stream;
			_path = path;
		}

		#endregion

		
		#region Overridden Protected Methods

		protected override void OnExecute()
		{
			long fileSize;
			if (!File.Exists(_path))
			{
				Platform.Log(LogLevel.Error, "Unexpected error finding file to add to XML {0}", _path);
				throw new ApplicationException("Unexpected error finding file to add to XML {0}" + _path);
			}

			FileInfo finfo = new FileInfo(_path);
			fileSize = finfo.Length;

			DicomFile dicomFile = new DicomFile(_path);
			dicomFile.Load(DicomReadOptions.StorePixelDataReferences);

			// Setup the insert parameters
			if (false == _stream.AddFile(dicomFile, fileSize))
			{
				Platform.Log(LogLevel.Error, "Unexpected error adding SOP to XML Study Descriptor for file {0}",
							 _path);
				throw new ApplicationException("Unexpected error adding SOP to XML Study Descriptor for SOP: " +
											   dicomFile.MediaStorageSopInstanceUid);
			}
		}

		protected override void OnUndo()
		{

		}

		#endregion
	}
}
