using System;
using System.Collections.Generic;
using System.Drawing;

namespace ClearCanvas.ImageViewer.Graphics
{
	public sealed class IndexEventArgs : EventArgs
	{
		public readonly int Index;

		public IndexEventArgs(int index)
		{
			this.Index = index;
		}
	}

	/// <summary>
	/// An observable list of points defining a <see cref="Graphic"/>.
	/// </summary>
	public interface IPointsList : IList<PointF>
	{
		bool IsClosed { get; }
		void SuspendEvents();
		void ResumeEvents();
		event EventHandler<IndexEventArgs> PointAdded;
		event EventHandler<IndexEventArgs> PointChanged;
		event EventHandler<IndexEventArgs> PointRemoved;
		event EventHandler PointsCleared;
	}
}