#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;
using System;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// Defines a study finder extension point.
	/// </summary>
	[ExtensionPoint()]
	public sealed class StudyFinderExtensionPoint : ExtensionPoint<IStudyFinder>
    {
    }

	/// <summary>
	/// Exception thrown when an <see cref="IStudyFinder"/> with the specified
	/// <see cref="FinderName">name</see> could not be found.
	/// </summary>
	public class StudyFinderNotFoundException : Exception
	{
		internal StudyFinderNotFoundException(string name)
		{
			FinderName = name;
		}

		/// <summary>
		/// Gets the name of the requested <see cref="IStudyFinder"/>.
		/// </summary>
		public readonly string FinderName;
	}

	/// <summary>
	/// A map of <see cref="IStudyFinder"/> objects.
	/// </summary>
    internal sealed class StudyFinderMap : IEnumerable
	{
        private readonly Dictionary<string, IStudyFinder> _studyFinderMap = new Dictionary<string, IStudyFinder>();
		private static readonly Dictionary<string, string> _supportedStudyFinders;

		internal StudyFinderMap()
		{
			CreateStudyFinders();
		}

		static StudyFinderMap()
		{
			StudyFinderMap map = new StudyFinderMap();
			_supportedStudyFinders = new Dictionary<string, string>();
			foreach (IStudyFinder finder in map._studyFinderMap.Values)
				_supportedStudyFinders[finder.Name] = finder.Name;
		}

		public static bool IsStudyFinderSupported(string studyFinderName)
		{
			Platform.CheckForEmptyString(studyFinderName, "studyFinderName");
			return _supportedStudyFinders.ContainsKey(studyFinderName);
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

				if (!_studyFinderMap.ContainsKey(studyFinderName))
					throw new StudyFinderNotFoundException(studyFinderName);

				return _studyFinderMap[studyFinderName];
			}
		}

		private void CreateStudyFinders()
		{
            try
            {
				StudyFinderExtensionPoint xp = new StudyFinderExtensionPoint();
				object[] studyFinders = xp.CreateExtensions();

				foreach (IStudyFinder studyFinder in studyFinders)
					_studyFinderMap.Add(studyFinder.Name, studyFinder);
			}
			catch (NotSupportedException e)
			{
				Platform.Log(LogLevel.Info, e);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
			}
		}

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _studyFinderMap.GetEnumerator();
		}

		#endregion
	}
}
