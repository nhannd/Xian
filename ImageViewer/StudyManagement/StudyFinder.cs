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

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// Abstract base class for an <see cref="IStudyFinder"/>.
	/// </summary>
	/// <remarks>
	/// <see cref="IStudyFinder"/> abstracts the finding of studies,
	/// allowing many means of finding studies (e.g., local database,
	/// DICOM query, DICOMDIR, etc.) to be treated in the same way..
	/// </remarks>
	public abstract class StudyFinder : IStudyFinder
	{
		private readonly string _name;

		/// <summary>
		/// Constructs a new <see cref="StudyFinder"/> with the given <paramref name="name"/>.
		/// </summary>
		/// <param name="name"></param>
		protected StudyFinder(string name)
		{
			_name = name;
		}

		#region IStudyFinder Members

		/// <summary>
		/// Gets the name of the study finder.
		/// </summary>
		public string Name
		{
			get { return _name; }
		}

		/// <summary>
		/// Queries for studies on a target server matching the specified query parameters.
		/// </summary>
		public abstract StudyItemList Query(QueryParameters queryParams, object targetServer);

		#endregion
	}
}