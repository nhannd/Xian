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

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// A collection of <see cref="Frame"/> objects.
	/// </summary>
	public class FrameCollection : IEnumerable<Frame>
	{
		private readonly List<Frame> _frames = new List<Frame>();

		internal FrameCollection()
		{
		}

		/// <summary>
		/// Gets the number of <see cref="Frame"/> objects in the collection.
		/// </summary>
		public int Count
		{
			get { return _frames.Count; }
		}

		/// <summary>
		/// Gets the <see cref="Frame"/> at the specified index.
		/// </summary>
		/// <param name="frameNumber">The frame number. The first frame is frame 1.</param>
		/// <returns></returns>
		public Frame this[int frameNumber]
		{
			get
			{
				Platform.CheckPositive(frameNumber, "frameNumber");
				return _frames[frameNumber-1];
			}
		}

		/// <summary>
		/// Adds a <see cref="Frame"/> to the collection.
		/// </summary>
		/// <param name="frame"></param>
		/// <remarks>
		/// This method should only be used by subclasses of <see cref="ImageSop"/>.
		/// </remarks>
		public void Add(Frame frame)
		{
			_frames.Add(frame);
		}

		#region IEnumerable<Frame> Members

		///<summary>
		///Returns an enumerator that iterates through the collection.
		///</summary>
		///
		///<returns>
		///A <see cref="T:System.Collections.Generic.IEnumerator`1"></see> that can be used to iterate through the collection.
		///</returns>
		///<filterpriority>1</filterpriority>
		public IEnumerator<Frame> GetEnumerator()
		{
			return _frames.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		///<summary>
		///Returns an enumerator that iterates through a collection.
		///</summary>
		///
		///<returns>
		///An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
		///</returns>
		///<filterpriority>2</filterpriority>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return _frames.GetEnumerator();
		}

		#endregion
	}
}
