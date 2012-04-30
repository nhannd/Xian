#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// Defines an a study loader extension point.
	/// </summary>
	[ExtensionPoint()]
	public sealed class StudyLoaderExtensionPoint : ExtensionPoint<IStudyLoader>
    {
    }

	/// <summary>
	/// Exception thrown when an <see cref="IStudyLoader"/> with the specified
	/// <see cref="LoaderName">name</see> could not be found.
	/// </summary>
	public class StudyLoaderNotFoundException : Exception
	{
        internal StudyLoaderNotFoundException()
        {}

        internal StudyLoaderNotFoundException(Exception innerException)
            : base("Study loader could not be found.", innerException)
        {
            LoaderName = "";
        }

		internal StudyLoaderNotFoundException(string loaderName)
		{
			LoaderName = loaderName;
		}

		/// <summary>
		/// Gets the name of the requested <see cref="IStudyLoader"/>, if it was specified.
		/// </summary>
		public readonly string LoaderName;
	}
	
	internal sealed class StudyLoaderMap : IEnumerable
    {
        private readonly Dictionary<string, IStudyLoader> _studyLoaderMap = new Dictionary<string, IStudyLoader>();
		private static readonly Dictionary<string, string> _supportedStudyLoaders;

        public StudyLoaderMap()
        {
			CreateStudyLoaders();
        }

		static StudyLoaderMap()
		{
			var map = new StudyLoaderMap();
			_supportedStudyLoaders = new Dictionary<string, string>();
			foreach (IStudyLoader loader in map._studyLoaderMap.Values)
				_supportedStudyLoaders[loader.Name] = loader.Name;
		}

		public static bool IsStudyLoaderSupported(string studyLoaderName)
		{
			Platform.CheckForEmptyString(studyLoaderName, "studyLoaderName");
			return _supportedStudyLoaders.ContainsKey(studyLoaderName);
		}

        public IStudyLoader this[string studyLoaderName]
        {
            get
            {
                Platform.CheckForEmptyString(studyLoaderName, "studyLoaderName");
				if (_studyLoaderMap.ContainsKey(studyLoaderName))
					return _studyLoaderMap[studyLoaderName];
            	else
					throw new StudyLoaderNotFoundException(studyLoaderName);
            }
        }

		private void CreateStudyLoaders()
		{
			try
			{
				foreach (IStudyLoader studyLoader in StudyLoader.CreateAll())
					_studyLoaderMap.Add(studyLoader.Name, studyLoader);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
			}
		}

    	#region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return _studyLoaderMap.Values.GetEnumerator();
        }

        #endregion
    }
}
