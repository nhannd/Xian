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
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Event args for <see cref="EventBroker.StudyLoadFailed"/>.
	/// </summary>
	public class StudyLoadFailedEventArgs : EventArgs
	{
		internal StudyLoadFailedEventArgs(StudyItem study, Exception error)
		{
			this.Error = error;
			this.Study = study;
		}

		internal StudyLoadFailedEventArgs(LoadStudyArgs loadArgs, Exception error)
		{
			this.LoadArgs = loadArgs;
			this.Error = error;
		}

		/// <summary>
		/// Gets the <see cref="LoadStudyArgs"/> that were used to attempt to load the study.
		/// </summary>
		public readonly LoadStudyArgs LoadArgs;

		/// <summary>
		/// Gets the <see cref="StudyItem"/> that failed to load.
		/// </summary>
		/// <remarks>
		/// This object is generated via a query mechanism, such as <see cref="IStudyFinder"/>
		/// or <see cref="IPriorStudyFinder"/>.
		/// </remarks>
		public readonly StudyItem Study;

		/// <summary>
		/// Gets the <see cref="Exception"/> that occurred.
		/// </summary>
		public readonly Exception Error;
	}

	/// <summary>
	/// Event args for <see cref="EventBroker.StudyLoaded"/>.
	/// </summary>
	public class StudyLoadedEventArgs : EventArgs
	{
		internal StudyLoadedEventArgs(Study study, Exception error)
		{
			this.Error = error;
			this.Study = study;
		}

		/// <summary>
		/// Gets the <see cref="StudyManagement.Study"/> that was loaded.
		/// </summary>
		public readonly Study Study;

		/// <summary>
		/// If <see cref="Study"/> was only partially loaded, this
		/// will contain the <see cref="Exception"/> that describes the
		/// partial load failure.
		/// </summary>
		public readonly Exception Error;
	}
}
