#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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

using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	/// <summary>
	/// Provides data for control point related events.
	/// </summary>
	public class ControlPointEventArgs : CollectionEventArgs<PointF>
	{
		private int _controlPointIndex;

		/// <summary>
		/// 
		/// </summary>
		public ControlPointEventArgs()
		{

		}

		/// <summary>
		/// Initializes a new instance of <see cref="ControlPointEventArgs"/>.
		/// </summary>
		/// <param name="controlPointIndex"></param>
		/// <param name="controlPointLocation"></param>
		public ControlPointEventArgs(int controlPointIndex, PointF controlPointLocation)
		{
			Platform.CheckNonNegative(controlPointIndex, "controlPointIndex");
			Platform.CheckForNullReference(controlPointLocation, "controlPointLocation");

			_controlPointIndex = controlPointIndex;
			base.Item = controlPointLocation;
		}

		/// <summary>
		/// Gets the index of <see cref="ControlPoint"/> in 
		/// <see cref="ControlPointGroup"/>.
		/// </summary>
		public int ControlPointIndex
		{
			get { return _controlPointIndex; }
		}

		/// <summary>
		/// Gets the location of the control point.
		/// </summary>
		public PointF ControlPointLocation
		{
			get { return base.Item; }
		}
	}
}
