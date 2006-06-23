using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using System.Collections;

namespace ClearCanvas.Workstation.Model.StudyManagement
{
    [ExtensionPoint()]
    public class StudyFinderExtensionPoint : ExtensionPoint<IStudyFinder>
    {
    }
    
    public class StudyFinderMap : IEnumerable
	{
        Dictionary<string, IStudyFinder> _studyFinderMap = new Dictionary<string, IStudyFinder>();
		
		public StudyFinderMap()
		{
			CreateStudyFinders();
		}

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
