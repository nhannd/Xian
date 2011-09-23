#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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

        /// <summary>
        /// Creates zero or more <see cref="IDisplaySet"/>s from the given <see cref="Study"/>.
        /// </summary>
        /// <returns>Zero or more <see cref="IDisplaySet"/>s.</returns>
        List<IDisplaySet> CreateDisplaySets(Study study);
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
		public virtual void SetStudyTree(StudyTree studyTree)
		{
			_studyTree = studyTree;
			PresentationImageFactory.SetStudyTree(_studyTree);
		}

		/// <summary>
		/// Creates zero or more <see cref="IDisplaySet"/>s from the given <see cref="Series"/>.
		/// </summary>
		/// <returns>Zero or more <see cref="IDisplaySet"/>s.</returns>
		public abstract List<IDisplaySet> CreateDisplaySets(Series series);

        /// <summary>
        /// Creates zero or more <see cref="IDisplaySet"/>s from the given <see cref="Study"/>.
        /// </summary>
        /// <returns>Zero or more <see cref="IDisplaySet"/>s.</returns>
        public virtual List<IDisplaySet> CreateDisplaySets(Study study)
        {
            var displaySets = new List<IDisplaySet>();
            foreach (var series in study.Series)
               displaySets.AddRange(CreateDisplaySets(series));
            return displaySets;
        }

	    #endregion
	}
}