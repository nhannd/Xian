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