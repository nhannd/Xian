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

using System.Collections.Generic;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Defines a factory for <see cref="IDisplaySet"/>s.
	/// </summary>
	public interface IDisplaySetFactory
	{
		/// <summary>
		/// Sets the <see cref="StudyTree"/> where the factory can search for referenced <see cref="Sop"/>s.
		/// </summary>
		void SetStudyTree(StudyTree studyTree);

		/// <summary>
		/// Creates zero or more <see cref="IDisplaySet"/>s from the given <see cref="Series"/>.
		/// </summary>
		/// <returns>Zero or more <see cref="IDisplaySet"/>s.</returns>
		List<IDisplaySet> CreateDisplaySets(Series series);
	}

	/// <summary>
	/// Abstract base implementation of <see cref="IDisplaySetFactory"/>.
	/// </summary>
	public abstract class DisplaySetFactory : IDisplaySetFactory
	{
		private StudyTree _studyTree;
		private readonly IPresentationImageFactory _presentationImageFactory;

		/// <summary>
		/// Protected constructor.
		/// </summary>
		protected DisplaySetFactory()
			: this(new PresentationImageFactory())
		{
		}

		/// <summary>
		/// Protected constructor.
		/// </summary>
		/// <param name="presentationImageFactory">The <see cref="IPresentationImageFactory"/>
		/// used to create the <see cref="IPresentationImage"/>s that populate the constructed <see cref="IDisplaySet"/>s.</param>
		protected DisplaySetFactory(IPresentationImageFactory presentationImageFactory)
		{
			Platform.CheckForNullReference(presentationImageFactory, "presentationImageFactory");
			_presentationImageFactory = presentationImageFactory;
		}

		/// <summary>
		/// Gets the <see cref="StudyTree"/> where the factory can look for referenced <see cref="Sop"/>s.
		/// </summary>
		protected StudyTree StudyTree
		{
			get { return _studyTree; }
		}

		/// <summary>
		/// Gets the <see cref="IPresentationImageFactory"/> used to create the <see cref="IPresentationImage"/>s
		/// that populate the constructed <see cref="IDisplaySet"/>s.
		/// </summary>
		protected IPresentationImageFactory PresentationImageFactory
		{
			get { return _presentationImageFactory; }	
		}

		#region IDisplaySetFactory Members

		/// <summary>
		/// Sets the <see cref="StudyManagement.StudyTree"/> where the factory can search for referenced <see cref="Sop"/>s.
		/// </summary>
		public void SetStudyTree(StudyTree studyTree)
		{
			_studyTree = studyTree;
			PresentationImageFactory.SetStudyTree(_studyTree);
		}

		/// <summary>
		/// Creates zero or more <see cref="IDisplaySet"/>s from the given <see cref="Series"/>.
		/// </summary>
		/// <returns>Zero or more <see cref="IDisplaySet"/>s.</returns>
		public abstract List<IDisplaySet> CreateDisplaySets(Series series);

		#endregion
	}
}