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

using System.Reflection;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// A helper class for storing information about cursors that are to be
	/// shown in the view.
	/// </summary>
	public class CursorToken
	{
		/// <summary>
		/// Common cursors normally provided by the system.
		/// </summary>
		public enum SystemCursors
		{
			/// <summary>
			/// An arrow cursor.
			/// </summary>
			Arrow = 0,
			/// <summary>
			/// A crosshair cursor.
			/// </summary>
			Cross,
			/// <summary>
			/// A 'hand' cursor.
			/// </summary>
			Hand,
			/// <summary>
			/// A Help (?) cursor.
			/// </summary>
			Help,
			/// <summary>
			/// A horizontal splitter (resize) cursor.
			/// </summary>
			HSplit,
			/// <summary>
			/// An I-Beam (or text editing) cursor.
			/// </summary>
			IBeam,
			/// <summary>
			/// A cursor indicating that an operation cannot occur.
			/// </summary>
			No,
			/// <summary>
			/// A cursor indicating that a move (drag/drop) operation cannot occur.
			/// </summary>
			NoMove2D,
			/// <summary>
			/// A cursor indicating that a horizontal move operation cannot occur.
			/// </summary>
			NoMoveHoriz,
			/// <summary>
			/// A cursor indicating that a vertical move operation cannot occur.
			/// </summary>
			NoMoveVert,
			/// <summary>
			/// A cursor with a right-hand arrow.
			/// </summary>
			PanEast,
			/// <summary>
			/// A 'move' cursor with North (up) and East (left) arrows.
			/// </summary>
			PanNE,
			/// <summary>
			/// A 'move' cursor with an up arrow.
			/// </summary>
			PanNorth,
			/// <summary>
			/// A 'move' cursor with North (up) and West (left) arrows.
			/// </summary>
			PanNW,
			/// <summary>
			/// A 'move' cursor with South (down) and East (right) arrows.
			/// </summary>
			PanSE,
			/// <summary>
			/// A 'move' cursor with a South (down) arrow.
			/// </summary>
			PanSouth,
			/// <summary>
			/// A 'move' cursor with South (down) and West (left) arrows.
			/// </summary>
			PanSW,
			/// <summary>
			/// A 'move' cursor with a West (left) arrow.
			/// </summary>
			PanWest,
			/// <summary>
			/// A 'move' cursor with arrows in all directions.
			/// </summary>
			SizeAll,
			/// <summary>
			/// A 'move' cursor with North-east and South-west arrows.
			/// </summary>
			SizeNESW,
			/// <summary>
			/// A 'move' cursor with North-south arrows.
			/// </summary>
			SizeNS,
			/// <summary>
			/// A 'move' cursor with North-west and South-east arrows.
			/// </summary>
			SizeNWSE,
			/// <summary>
			/// A 'move' cursor with West-east arrows.
			/// </summary>
			SizeWE,
			/// <summary>
			/// A cursor with an 'up' arrow.
			/// </summary>
			UpArrow,
			/// <summary>
			/// A vertical splitter (resize) cursor.
			/// </summary>
			VSplit
		}; 
		
		private string _resourceName;
		private ResourceResolver _resolver;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="systemCursor">The system cursor to show in the view.</param>
		public CursorToken(SystemCursors systemCursor)
		{
			_resourceName = systemCursor.ToString();
			_resolver = null;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <remarks>
		/// The resource is resolved using the calling assembly (from <see cref="System.Reflection.Assembly.GetCallingAssembly"/>).
		/// </remarks>
		/// <param name="resourceName">The resource name of the cursor.</param>
		public CursorToken(string resourceName)
		{
			Platform.CheckForEmptyString(resourceName, "resourceName");

			_resourceName = resourceName;
			_resolver = new ResourceResolver(Assembly.GetCallingAssembly());
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="resourceName">The resource name of the cursor.</param>
		/// <param name="resourceAssembly">The assembly where the cursor resource resides.</param>
		public CursorToken(string resourceName, Assembly resourceAssembly)
		{
			Platform.CheckForEmptyString(resourceName, "resourceName");
			Platform.CheckForNullReference(resourceAssembly, "resourceAssembly");

			_resourceName = resourceName;
			_resolver = new ResourceResolver(resourceAssembly);
		}

		/// <summary>
		/// Gets the string resource name of the cursor.
		/// </summary>
		public string ResourceName
		{
			get { return _resourceName; }
		}

		/// <summary>
		/// Gets t<see cref="IResourceResolver"/> for the cursor resource.
		/// </summary>
		public IResourceResolver Resolver
		{
			get { return _resolver; }
		}

		/// <summary>
		/// Gets whether or not the cursor is one of the <see cref="SystemCursors"/>.
		/// </summary>
		public bool IsSystemCursor
		{
			get { return (_resolver == null); }
		}
	}
}
