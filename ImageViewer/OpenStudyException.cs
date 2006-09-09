using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer
{
	public class OpenStudyException : Exception
	{
		private bool _atLeastOneImageFailedToLoad;
		private bool _studyCouldNotBeLoaded;

		public OpenStudyException() { }
		
		public OpenStudyException(string message) : base(message) { }
		
		public OpenStudyException(string message, Exception inner) : base(message, inner) { }

		public bool AtLeastOneImageFailedToLoad
		{
			get { return _atLeastOneImageFailedToLoad; }
			set { _atLeastOneImageFailedToLoad = value; }
		}

		public bool StudyCouldNotBeLoaded
		{
			get { return _studyCouldNotBeLoaded; }
			set { _studyCouldNotBeLoaded = value; }
		}
	}
}
