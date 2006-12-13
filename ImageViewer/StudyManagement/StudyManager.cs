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
		}

		public StudyTree StudyTree
		{
			get 
			{
				if (_studyTree == null)
					_studyTree = new StudyTree();

				return _studyTree; 
			}
		}

		public StudyFinderMap StudyFinders
		{
			get 
			{
				if (_studyFinders == null)
					_studyFinders = new StudyFinderMap();

				return _studyFinders; 
			}
		}
	
		public StudyLoaderMap StudyLoaders
		{
			get 
			{
				if (_studyLoaders == null)
					_studyLoaders = new StudyLoaderMap();

				return _studyLoaders; 
			}
		}
    }
}
