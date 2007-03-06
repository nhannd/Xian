using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using System.Collections;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// Defines an a study finder extension point.
	/// </summary>
	[ExtensionPoint()]
    public class StudyFinderExtensionPoint : ExtensionPoint<IStudyFinder>
    {
    }
    
	/// <summary>
	/// A map of <see cref="IStudyFinder"/> objects.
	/// </summary>
    public sealed class StudyFinderMap : IEnumerable
	{
        Dictionary<string, IStudyFinder> _studyFinderMap = new Dictionary<string, IStudyFinder>();

		internal StudyFinderMap()
		{
			CreateStudyFinders();
		}

		/// <summary>
		/// Gets the <see cref="IStudyFinder"/> with the specified name.
		/// </summary>
		/// <param name="studyFinderName"></param>
		/// <returns></returns>
        public IStudyFinder this[string studyFinderName]
		{
			get
			{
				Platform.CheckForEmptyString(studyFinderName, "studyFinderName");
				return _studyFinderMap[studyFinderName];
			}
		}

		private void CreateStudyFinders()
		{
            StudyFinderExtensionPoint xp = new StudyFinderExtensionPoint();
            object[] studyFinders = xp.CreateExtensions();

            foreach (IStudyFinder studyFinder in studyFinders)
				_studyFinderMap.Add(studyFinder.Name, studyFinder);
		}

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _studyFinderMap.GetEnumerator();
		}

		#endregion

    }
}
