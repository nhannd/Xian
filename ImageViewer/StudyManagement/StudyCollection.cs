#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// A collection of <see cref="Study"/> objects.
	/// </summary>
	public class StudyCollection : ObservableList<Study>
	{
		/// <summary>
		/// Initializes a new instance of <see cref="StudyCollection"/>.
		/// </summary>
		public StudyCollection()
		{

		}

		internal Study this[string studyInstanceUid]
		{
			get
			{
				return CollectionUtils.SelectFirst(this, delegate(Study study) { return study.StudyInstanceUid == studyInstanceUid; });
			}
		}
	}
}
