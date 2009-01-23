namespace ClearCanvas.ImageViewer.StudyManagement
{
	public abstract class StudyLoader : IStudyLoader
	{
		private readonly string _name;
		private IPrefetchingStrategy _prefetchingStrategy;

		protected StudyLoader(string name)
		{
			_name = name;
		}

		#region IStudyLoader Members

		public string Name
		{
			get { return _name; }
		}

		public IPrefetchingStrategy PrefetchingStrategy
		{
			get { return _prefetchingStrategy; }
			protected set { _prefetchingStrategy = value; }
		}

		public abstract int Start(StudyLoaderArgs studyLoaderArgs);

		public Sop LoadNextSop()
		{
			ISopDataSource dataSource = LoadNextSopDataSource();
			if (dataSource == null)
				return null;

			return Sop.Create(dataSource);
		}

		#endregion

		protected abstract ISopDataSource LoadNextSopDataSource();
	}
}
