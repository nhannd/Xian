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
