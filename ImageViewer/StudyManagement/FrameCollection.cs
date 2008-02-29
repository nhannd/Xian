using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// A collection of <see cref="Frame"/> objects.
	/// </summary>
	public class FrameCollection : IEnumerable
	{
		private List<Frame> _frames = new List<Frame>();

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

		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns></returns>
		public IEnumerator GetEnumerator()
		{
			return _frames.GetEnumerator();
		}
	}
}
