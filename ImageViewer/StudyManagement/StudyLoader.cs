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
namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// Abstract base class for <see cref="IStudyLoader"/>.
	/// </summary>
	public abstract class StudyLoader : IStudyLoader
	{
		private readonly string _name;
		private IPrefetchingStrategy _prefetchingStrategy;
		private object _currentServer;

		/// <summary>
		/// Constructs a new <see cref="StudyLoader"/> with the given <paramref name="name"/>.
		/// </summary>
		protected StudyLoader(string name)
		{
			_name = name;
		}

		#region IStudyLoader Members

		/// <summary>
		/// Gets the name of the study loader.
		/// </summary>
		public string Name
		{
			get { return _name; }
		}

		/// <summary>
		/// Gets or sets the study loader's pixel data prefetching strategy.
		/// </summary>
		public IPrefetchingStrategy PrefetchingStrategy
		{
			get { return _prefetchingStrategy; }
			protected set { _prefetchingStrategy = value; }
		}

		/// <summary>
		/// Called by <see cref="Start"/> to begin prefetching.
		/// </summary>
		protected abstract int OnStart(StudyLoaderArgs studyLoaderArgs);

		/// <summary>
		/// Creates a <see cref="Sop"/> from the given <see cref="ISopDataSource"/>.
		/// </summary>
		protected virtual Sop CreateSop(ISopDataSource dataSource)
		{
			return Sop.Create(dataSource);
		}

		/// <summary>
		/// Loads the next <see cref="SopDataSource"/> from which a
		/// <see cref="Sop"/> will be created.
		/// </summary>
		/// <returns>The next <see cref="SopDataSource"/> or <b>null</b> if there are none remaining.</returns>
		/// <remarks>
		/// Implementers of <see cref="StudyLoader"/> should avoid loading pixel data
		/// in this method for performance reasons.
		/// </remarks>
		protected abstract SopDataSource LoadNextSopDataSource();


		/// <summary>
		/// Starts the enumeration of images that match the specified
		/// Study Instance UID.
		/// </summary>
		/// <param name="studyLoaderArgs"></param>
		/// <returns>Number of images in study.</returns>
		public int Start(StudyLoaderArgs studyLoaderArgs)
		{
			_currentServer = studyLoaderArgs.Server;

			try
			{
				return OnStart(studyLoaderArgs);
			}
			catch(LoadStudyException)
			{
				throw;
			}
			catch(Exception e)
			{
				throw new LoadStudyException(studyLoaderArgs.StudyInstanceUid, e);
			}
		}

		/// <summary>
		/// Loads the next <see cref="Sop"/>.
		/// </summary>
		/// <returns>The next <see cref="Sop"/> or <b>null</b> if there are none remaining.</returns>
		/// <remarks>
		/// Implementers of <see cref="IStudyLoader"/> should avoid loading pixel data
		/// in this method for performance reasons.
		/// </remarks>
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
	}
}
