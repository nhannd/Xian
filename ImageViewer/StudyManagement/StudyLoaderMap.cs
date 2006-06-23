using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using System.Collections;

namespace ClearCanvas.ImageViewer.StudyManagement
{
    [ExtensionPoint()]
    public class StudyLoaderExtensionPoint : ExtensionPoint<IStudyLoader>
    {
    }

    public class StudyLoaderMap : IEnumerable
    {
        Dictionary<string, IStudyLoader> _studyLoaderMap = new Dictionary<string, IStudyLoader>();

        public StudyLoaderMap(StudyTree studyTree)
        {
            Platform.CheckForNullReference(studyTree, "studyTree");
            CreateStudyLoaders(studyTree);
        }

        public IStudyLoader this[string studyLoaderName]
        {
            get
            {
                Platform.CheckForEmptyString(studyLoaderName, "studyLoaderName");
                return _studyLoaderMap[studyLoaderName];
            }
        }

        private void CreateStudyLoaders(StudyTree studyTree)
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
