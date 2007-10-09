using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// Provides data for <see cref="StudyCollection"/> events.
	/// </summary>
	public class StudyEventArgs : CollectionEventArgs<Study>
	{
		/// <summary>
		/// Initializes a new instance of <see cref="StudyEventArgs"/>.
		/// </summary>
		public StudyEventArgs()
		{

		}

		/// <summary>
		/// Initializes a new instance of <see cref="StudyEventArgs"/> with
		/// a specified <see cref="Study"/>.
		/// </summary>
		/// <param name="study"></param>
		public StudyEventArgs(Study study)
		{
			base.Item  = study;
		}

		/// <summary>
		/// Gets the <see cref="Study"/>.
		/// </summary>
		public Study Study { get { return base.Item; } }
	}
}
