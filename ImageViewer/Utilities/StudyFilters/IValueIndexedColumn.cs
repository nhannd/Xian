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

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters
{
	public interface IValueIndexedColumn
	{
		IEnumerable UniqueValues { get; }
	}

	public interface IValueIndexedColumn<T> : IValueIndexedColumn
	{
		new IEnumerable<T> UniqueValues { get; }
	}
}