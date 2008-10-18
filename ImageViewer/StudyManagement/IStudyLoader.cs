#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
	//TODO: move this stuff into ImageViewer.Services.
	public class StudyLoaderArgs
	{
		private string _studyInstanceUid;
		private object _server;

		public StudyLoaderArgs(string studyInstanceUid, object server)
		{
			_studyInstanceUid = studyInstanceUid;
			_server = server;
		}


		public string StudyInstanceUid
		{
			get { return _studyInstanceUid; }
		}

		public object Server
		{
			get { return _server; }
		}
	}

	/// <summary>
	/// Defines a study loader.
	/// </summary>
	/// <remarks>
	/// <see cref="IStudyLoader"/> abstracts the loading of studies,
	/// allowing different many means of loading studies (e.g., local file system,
	/// DICOM WADO, DICOMDIR CD, streaming, etc.) to be treated in the same way.
	/// </remarks>
    public interface IStudyLoader
    {
		/// <summary>
		/// Gets the name of the study loader.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Gets the study loader's pixel data prefetching strategy.
		/// </summary>
		IPrefetchingStrategy PrefetchingStrategy { get; }

		/// <summary>
		/// Starts the enumeration of images that match the specified
		/// Study Instance UID.
		/// </summary>
		/// <param name="studyLoaderArgs"></param>
		/// <returns>Number of images in study.</returns>
		int Start(StudyLoaderArgs studyLoaderArgs);

		/// <summary>
		/// Loads the next image.
		/// </summary>
		/// <returns>The next <see cref="ImageSop"/> or <b>null</b> if there are
		/// no more images remaining.</returns>
		/// <remarks>
		/// Implementers of <see cref="IStudyLoader"/> should avoid loading pixel data
		/// in this method for performance reasons.
		/// </remarks>
		ImageSop LoadNextImage();
    }
}
