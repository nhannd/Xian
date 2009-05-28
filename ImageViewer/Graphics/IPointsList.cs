using System;
using System.Collections.Generic;
using System.Drawing;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// Provides data for the <see cref="IPointsList"/> events.
	/// </summary>
	public sealed class IndexEventArgs : EventArgs
	{
		/// <summary>
		/// Gets the index of the point to which the event occurred.
		/// </summary>
		public readonly int Index;

		/// <summary>
		/// Constructs a new object to hold data for the <see cref="IPointsList"/> events.
		/// </summary>
		/// <param name="index">The index of the point to which the event occurred.</param>
		public IndexEventArgs(int index)
		{
			this.Index = index;
		}
	}

	/// <summary>
	/// An observable list of points defining an <see cref="IGraphic"/>.
	/// </summary>
	/// <remarks>
	/// The coordinate space of points in this list varies depending on the <see cref="CoordinateSystem"/> of the owning graphic.
	/// </remarks>
	public interface IPointsList : IList<PointF>
	{
		/// <summary>
		/// Gets a value indicating if the first and last points of the list are coincident.
		/// </summary>
		bool IsClosed { get; }

		/// <summary>
		/// Suspends notification of the <see cref="PointAdded"/>, <see cref="PointChanged"/>, <see cref="PointRemoved"/> and <see cref="PointsCleared"/> events.
		/// </summary>
		void SuspendEvents();

		/// <summary>
		/// Resumes notification of the <see cref="PointAdded"/>, <see cref="PointChanged"/>, <see cref="PointRemoved"/> and <see cref="PointsCleared"/> events.
		/// </summary>
		void ResumeEvents();

		/// <summary>
		/// Occurs when a point is added to the list.
		/// </summary>
		event EventHandler<IndexEventArgs> PointAdded;

		/// <summary>
		/// Occurs when the value of a point in the list has changed.
		/// </summary>
		event EventHandler<IndexEventArgs> PointChanged;

		/// <summary>
		/// Occurs when a point is removed from the list.
		/// </summary>
		event EventHandler<IndexEventArgs> PointRemoved;

		/// <summary>
		/// Occurs when the list is cleared.
		/// </summary>
		event EventHandler PointsCleared;
	}
}