namespace ClearCanvas.ImageViewer.StudyManagement
{
	public abstract class StudyLoader : IStudyLoader
	{
		private readonly string _name;
		private IPrefetchingStrategy _prefetchingStrategy;
		private object _currentServer;

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

		public abstract int OnStart(StudyLoaderArgs args);

		public int Start(StudyLoaderArgs studyLoaderArgs)
		{
			_currentServer = studyLoaderArgs.Server;

			return OnStart(studyLoaderArgs);
		}

		public Sop LoadNextSop()
		{
			SopDataSource dataSource = LoadNextSopDataSource();
			if (dataSource == null)
			{
				_currentServer = null;
				return null;
			}

			dataSource.StudyLoaderName = Name;
			dataSource.Server = _currentServer;

			return CreateSop(dataSource);
		}

		#endregion

		protected virtual Sop CreateSop(ISopDataSource dataSource)
		{
			return Sop.Create(dataSource);
		}

		protected abstract SopDataSource LoadNextSopDataSource();
	}
}
