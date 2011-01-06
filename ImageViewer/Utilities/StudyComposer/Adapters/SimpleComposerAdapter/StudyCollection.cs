#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.ComponentModel;

namespace ClearCanvas.ImageViewer.Utilities.StudyComposer.Adapters.SimpleComposerAdapter
{
	public sealed class StudyCollection : BindingListWrapper<StudyItem>
	{
		internal StudyCollection(BindingList<StudyItem> list)
			: base(list) {}

		public StudyItem GetFirstStudy()
		{
			if (this.Count == 0)
				return null;
			return this[0];
		}
	}
}