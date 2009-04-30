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
using System.Drawing;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// An interface for providers of screen information.
	/// </summary>
	public interface IScreenInfoProvider
	{
		/// <summary>
		/// Gets the virtual screen of the entire desktop (all display devices).
		/// </summary>
		Rectangle VirtualScreen { get; }

		/// <summary>
		/// Gets all the <see cref="Screen"/>s in the desktop.
		/// </summary>
		Screen[] GetScreens();
	}

	/// <summary>
	/// An extension point for <see cref="IScreenInfoProvider"/>s.
	/// </summary>
	public sealed class ScreenInfoProviderExtensionPoint : ExtensionPoint<IScreenInfoProvider>
	{
	}

	/// <summary>
	/// Abstract class representing a single screen in the desktop.
	/// </summary>
	public abstract class Screen : IEquatable<Screen>
	{
		private static readonly IScreenInfoProvider _screenInfoProvider;

		static Screen()
		{
			try
			{
				_screenInfoProvider = (IScreenInfoProvider)new ScreenInfoProviderExtensionPoint().CreateExtension();
			}
			catch(Exception e)
			{
				Platform.Log(LogLevel.Error, e, "A valid IScreenInfoProvider extension must exist.");
				throw; //crash.
			}
		}

		#region Public Properties

		/// <summary>
		/// Gets the desktop's virtual screen.
		/// </summary>
		public static Rectangle VirtualScreen
		{
			get { return _screenInfoProvider.VirtualScreen; }
		}

		/// <summary>
		/// Gets an array of all the <see cref="Screen"/>s in the desktop.
		/// </summary>
		public static Screen[] AllScreens
		{
			get { return _screenInfoProvider.GetScreens(); }	
		}

		#region IEquatable<Screen> Members

		/// <summary>
		/// Gets whether or not this <see cref="Screen"/> object is equivalent to another.
		/// </summary>
		public abstract bool Equals(Screen other);

		#endregion
		#endregion

		#region Public Properties

		/// <summary>
		/// Gets the number of bits per pixel of the device.
		/// </summary>
		public abstract int BitsPerPixel { get; }

		/// <summary>
		/// Gets the bounds of the screen inside the <see cref="VirtualScreen"/>.
		/// </summary>
		public abstract Rectangle Bounds { get; }

		/// <summary>
		/// Gets the name of the device.
		/// </summary>
		public abstract string DeviceName { get; }

		/// <summary>
		/// Gets whether or not this is the primary screen.
		/// </summary>
		public abstract bool IsPrimary { get; }

		/// <summary>
		/// Gets the area of the <see cref="Screen"/> in which an <see cref="IDesktopWindow"/> can be maximized.
		/// </summary>
		public abstract Rectangle WorkingArea { get; }

		#endregion
	}
}
