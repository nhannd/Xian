using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// Defines an a study loader extension point.
	/// </summary>
	[ExtensionPoint()]
    public class StudyLoaderExtensionPoint : ExtensionPoint<IStudyLoader>
    {
    }

    internal sealed class StudyLoaderMap : IEnumerable
    {
        Dictionary<string, IStudyLoader> _studyLoaderMap = new Dictionary<string, IStudyLoader>();

        public StudyLoaderMap()
        {
			CreateStudyLoaders();
        }

        public IStudyLoader this[string studyLoaderName]
        {
            get
            {
                Platform.CheckForEmptyString(studyLoaderName, "studyLoaderName");
                return _studyLoaderMap[studyLoaderName];
            }
        }

        private void CreateStudyLoaders()
        {
            StudyLoaderExtensionPoint xp = new StudyLoaderExtensionPoint();
            object[] studyLoaders = xp.CreateExtensions();

            foreach (IStudyLoader studyLoader in studyLoaders)
                _studyLoaderMap.Add(studyLoader.Name, studyLoader);
        }

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return _studyLoaderMap.GetEnumerator();
        }

        #endregion

    }
}
