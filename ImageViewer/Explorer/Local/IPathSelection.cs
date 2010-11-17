#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Explorer.Local
{
	/// <summary>
	/// Represents a selection of file system paths.
	/// </summary>
	public interface IPathSelection : IEnumerable<string>
	{
		/// <summary>
		/// Gets the selected path at the specified index.
		/// </summary>
		string this[int index] { get; }

		/// <summary>
		/// Gets the number of paths in the selection.
		/// </summary>
		int Count { get; }

		/// <summary>
		/// Determines whether or not the specified path is in this selection.
		/// </summary>
		bool Contains(string path);
	}

	/// <summary>
	/// A basic implementation of <see cref="IPathSelection"/>.
	/// </summary>
	public class PathSelection : Selection<string>, IPathSelection
	{
		/// <summary>
		/// Constructs an empty path selection.
		/// </summary>
		public PathSelection() {}

		/// <summary>
		/// Constructs a path selection out of the specified file system paths.
		/// </summary>
		/// <param name="paths">The file system paths in the selection.</param>
		public PathSelection(params string[] paths) : base(paths) {}

		/// <summary>
		/// Constructs a path selection out of the specified file system paths.
		/// </summary>
		/// <param name="paths">The file system paths in the selection.</param>
		public PathSelection(IEnumerable<string> paths) : base(paths) {}

		/// <summary>
		/// Gets the selected path at the specified index.
		/// </summary>
		public string this[int index]
		{
			get { return Items[index]; }
		}
	}
}