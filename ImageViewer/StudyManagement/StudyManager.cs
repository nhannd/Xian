using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	public class StudyManager
	{
		private StudyTree _studyTree;
		private StudyFinderMap _studyFinders;
        private StudyLoaderMap _studyLoaders;

		public StudyManager()
		{
		    _studyTree = new StudyTree();
		    _studyFinders = new StudyFinderMap();
            _studyLoaders = new StudyLoaderMap(_studyTree);
		}

		public StudyTree StudyTree
		{
			get { return _studyTree; }
		}

		public StudyFinderMap StudyFinders
		{
			get { return _studyFinders; }
		}
	
		public StudyLoaderMap StudyLoaders
		{
			get { return _studyLoaders; }
		}

    }
}
