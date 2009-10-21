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
using ClearCanvas.ImageServer.Core.Process;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Core.Rebuild
{
	/// <summary>
	/// Class for rebuilding StudyXml files based on the current contents of the <see cref="StudyXml"/> file.
	/// </summary>
	public class StudyXmlRebuilder
	{
		#region Private Members

		private readonly StudyStorageLocation _location;
		
		#endregion

		#region Constructor

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="location">The location of the Study to rebuild.</param>
		public StudyXmlRebuilder(StudyStorageLocation location)
		{
			_location = location;
		}

		#endregion

		/// <summary>
		/// Do the actual rebuild.  On error, will attempt to reprocess the study.
		/// </summary>
		public void RebuildXml()
		{
			string rootStudyPath = _location.GetStudyPath();

			try
			{
				using (ServerCommandProcessor processor = new ServerCommandProcessor("Rebuild XML"))
				{
					processor.AddCommand(new RebuildStudyXmlCommand(_location.StudyInstanceUid, rootStudyPath));

					if (!processor.Execute())
					{
						throw new ApplicationException(processor.FailureReason, processor.FailureException);
					}

					Platform.Log(LogLevel.Info, "Completed reprocessing Study XML file for study {0}", _location.StudyInstanceUid);
				}
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e, "Unexpected error when rebuilding study XML for directory: {0}",
				             _location.FilesystemPath);
				StudyReprocessor reprocessor = new StudyReprocessor();
				WorkQueue reprocessEntry = reprocessor.ReprocessStudy("Rebuild StudyXml", _location, Platform.Time);
				if (reprocessEntry != null)
				{
					Platform.Log(LogLevel.Error, "Failure attempting to reprocess study: {0}", _location.StudyInstanceUid);
				}
				else
					Platform.Log(LogLevel.Error, "Inserted reprocess request for study: {0}", _location.StudyInstanceUid);
			}
		}
	}
}