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
