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
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Holds the parameters that specify the study to be loaded.
	/// </summary>
	public class LoadStudyArgs
	{
		private string _studyInstanceUid;
		private object _server;
		private string _studyLoaderName;

		/// <summary>
		/// Constructs a new <see cref="LoadStudyArgs"/> using the specified parameters.
		/// </summary>
		/// <param name="studyInstanceUid">The Study Instance UID of the study to be loaded.</param>
		/// <param name="server">An object specifying the server to retrieve the study from, such as
		/// <code>null</code> for the local server or an <see cref="ApplicationEntity"/> object specifying the remote server.</param>
		/// <param name="studyLoaderName">The name of the <see cref="IStudyLoader"/> to use, which is specified by <see cref="IStudyLoader.Name"/>.</param>
		public LoadStudyArgs(
			string studyInstanceUid, 
			object server, 
			string studyLoaderName)
		{
			Platform.CheckForNullReference(studyLoaderName, "studyLoaderName");
			Platform.CheckForNullReference(studyInstanceUid, "studyInstanceUids");

			_studyInstanceUid = studyInstanceUid;
			_server = server;
			_studyLoaderName = studyLoaderName;
		}

		internal LoadStudyArgs(StudyItem studyItem)
		: this(studyItem.StudyInstanceUID, studyItem.Server, studyItem.StudyLoaderName)
		{
		}

		/// <summary>
		/// Gets the Study Instance UID of the study to be loaded.
		/// </summary>
		public string StudyInstanceUid
		{
			get { return _studyInstanceUid; }
		}

		/// <summary>
		/// Gets the server to load the study from, such as
		/// <code>null</code> for the local server or an <see cref="ApplicationEntity"/> object specifying the remote server.
		/// </summary>
		public object Server
		{
			get { return _server; }
		}

		/// <summary>
		/// Gets the name of the <see cref="IStudyLoader"/> to use, which is specified by <see cref="IStudyLoader.Name"/>.
		/// </summary>
		public string StudyLoaderName
		{
			get { return _studyLoaderName; }
		}
	}
}
