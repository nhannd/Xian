using System;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer
{
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

		public readonly LoadStudyArgs LoadArgs;
		public readonly StudyItem Study;
		public readonly Exception Error;
	}

	public class StudyLoadedEventArgs : EventArgs
	{
		internal StudyLoadedEventArgs(Study study, Exception error)
		{
			this.Error = error;
			this.Study = study;
		}

		public readonly Study Study;
		public readonly Exception Error;
	}
}
