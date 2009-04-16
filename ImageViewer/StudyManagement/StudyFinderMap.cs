#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;
using System;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// Defines an a study finder extension point.
	/// </summary>
	[ExtensionPoint()]
	public sealed class StudyFinderExtensionPoint : ExtensionPoint<IStudyFinder>
    {
    }
    
	public class StudyFinderNotFoundException : Exception
	{
		internal StudyFinderNotFoundException(string name)
		{
			FinderName = name;
		}

		public readonly string FinderName;
	}

	/// <summary>
	/// A map of <see cref="IStudyFinder"/> objects.
	/// </summary>
    internal sealed class StudyFinderMap : IEnumerable
	{
        private readonly Dictionary<string, IStudyFinder> _studyFinderMap = new Dictionary<string, IStudyFinder>();

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
