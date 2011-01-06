#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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