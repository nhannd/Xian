#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Dicom.Iod;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// Holds the parameters that specify the study to be loaded.
	/// </summary>
	public class StudyLoaderArgs
	{
		private readonly string _studyInstanceUid;
        private readonly IApplicationEntity _server;

		/// <summary>
		/// Constructs a new <see cref="StudyLoaderArgs"/> using the specified parameters.
		/// </summary>
		/// <param name="studyInstanceUid">The Study Instance UID of the study to be loaded.</param>
		/// <param name="server">The server from which the study should be loaded.</param>
		public StudyLoaderArgs(string studyInstanceUid, IApplicationEntity server)
		{
			_studyInstanceUid = studyInstanceUid;
			_server = server;
		}

		/// <summary>
		/// Gets the Study Instance UID of the study to be loaded.
		/// </summary>
		public string StudyInstanceUid
		{
			get { return _studyInstanceUid; }
		}

		/// <summary>
		/// Gets the server to load the study from.
		/// </summary>
        public IApplicationEntity Server
		{
			get { return _server; }
		}
	}

    //TODO (Marmot): Can this stuff be moved to Common? Arguably almost everything in this namespace could move.
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
		/// Loads the next <see cref="Sop"/>.
		/// </summary>
		/// <returns>The next <see cref="Sop"/> or <b>null</b> if there are none remaining.</returns>
		/// <remarks>
		/// Implementers of <see cref="IStudyLoader"/> should avoid loading pixel data
		/// in this method for performance reasons.
		/// </remarks>
		Sop LoadNextSop();
    }
}
