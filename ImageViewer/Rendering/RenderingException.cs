#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
using System.Drawing;

namespace ClearCanvas.ImageViewer.Rendering
{
	/// <summary>
	/// The exception that is thrown when any <see cref="Exception"/> is thrown in the rendering pipeline.
	/// </summary>
	public class RenderingException : Exception
	{
		/// <summary>
		/// Gets the <see cref="DrawMode"/> of the failed rendering operation.
		/// </summary>
		public readonly DrawMode DrawMode = DrawMode.Render;

		/// <summary>
		/// Gets the window ID of the failed rendering operation.
		/// </summary>
		/// <remarks>
		/// On Windows platforms, this would be the window handle, or hWnd.
		/// </remarks>
		public readonly IntPtr WindowId = IntPtr.Zero;

		/// <summary>
		/// Gets the context ID of the failed rendering operation.
		/// </summary>
		/// <remarks>
		/// On Windows platforms, this would be the device context handle, or hDC.
		/// </remarks>
		public readonly IntPtr ContextId = IntPtr.Zero;

		/// <summary>
		/// Gets the client rectangle of the failed rendering operation.
		/// </summary>
		public readonly Rectangle ClientRectangle = Rectangle.Empty;

		/// <summary>
		/// Gets the clipping rectangle of the failed rendering operation.
		/// </summary>
		public readonly Rectangle ClipRectangle = Rectangle.Empty;

		/// <summary>
		/// Initializes a <see cref="RenderingException"/>.
		/// </summary>
		/// <param name="innerException">The actual exception that was thrown in the rendering pipeline.</param>
		/// <param name="drawArgs">The <see cref="DrawArgs"/> of the failed rendering operation.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="innerException"/> is null.</exception>
		public RenderingException(Exception innerException, DrawArgs drawArgs)
			: base("An exception was thrown in the rendering pipeline.", innerException)
		{
			if (innerException == null)
				throw new ArgumentNullException("innerException", "An inner exception must be provided.");

			// record as much information as possible about the rendering operation for debugging purposes
			if (drawArgs != null)
			{
				DrawMode = drawArgs.DrawMode;
				if (drawArgs.RenderingSurface != null)
				{
					WindowId = drawArgs.RenderingSurface.WindowID;
					ContextId = drawArgs.RenderingSurface.ContextID;
					ClientRectangle = drawArgs.RenderingSurface.ClientRectangle;
					ClipRectangle = drawArgs.RenderingSurface.ClipRectangle;
				}
			}
		}

		//TODO (CR Sept 2010): remove or call InnerExceptionMessage.
		/// <summary>
		/// Gets the message of the actual exception that was thrown.
		/// </summary>
		/// <remarks>
		/// This property is the same as accessing the <see cref="Exception.Message"/> property on <see cref="Exception.InnerException"/>.
		/// </remarks>
		public string SpecificMessage
		{
			get { return InnerException.Message; }
		}
	}
}