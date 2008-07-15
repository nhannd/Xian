using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer
{
	public class LoadStudyArgs
	{
		private string _studyInstanceUid;
		private object _server;
		private string _studyLoaderName;

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

		public string StudyInstanceUid
		{
			get { return _studyInstanceUid; }
		}

		public object Server
		{
			get { return _server; }
		}

		public string StudyLoaderName
		{
			get { return _studyLoaderName; }
		}
	}
}
