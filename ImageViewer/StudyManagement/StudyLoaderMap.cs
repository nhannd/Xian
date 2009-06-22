#region License

// Copyright (c) 2009, ClearCanvas Inc.
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

using System;
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;

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
		internal StudyLoaderNotFoundException(string loaderName)
		{
			LoaderName = loaderName;
		}

		/// <summary>
		/// Gets the name of the requested <see cref="IStudyLoader"/>.
		/// </summary>
		public readonly string LoaderName;
	}
	
	internal sealed class StudyLoaderMap : IEnumerable
    {
        private readonly Dictionary<string, IStudyLoader> _studyLoaderMap = new Dictionary<string, IStudyLoader>();

        public StudyLoaderMap()
        {
			CreateStudyLoaders();
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
				StudyLoaderExtensionPoint xp = new StudyLoaderExtensionPoint();
				object[] studyLoaders = xp.CreateExtensions();

				foreach (IStudyLoader studyLoader in studyLoaders)
					_studyLoaderMap.Add(studyLoader.Name, studyLoader);
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

        public IEnumerator GetEnumerator()
        {
            return _studyLoaderMap.Values.GetEnumerator();
        }

        #endregion
    }
}
