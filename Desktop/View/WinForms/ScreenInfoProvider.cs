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
using ClearCanvas.Common;
using System.Windows.Forms;
using ClearCanvas.Desktop;
using WinFormsScreen = System.Windows.Forms.Screen;

namespace ClearCanvas.Desktop.View.WinForms
{
	/// <summary>
	/// Extension implementing <see cref="IScreenInfoProvider"/>.
	/// </summary>
	[ExtensionOf(typeof(ScreenInfoProviderExtensionPoint))]
	public class ScreenInfoProvider : IScreenInfoProvider
	{
		#region IScreenInfoProvider Members

		/// <summary>
		/// Gets the virtual screen of the entire desktop (all display devices).
		/// </summary>
		public System.Drawing.Rectangle VirtualScreen
		{
			get { return SystemInformation.VirtualScreen; }
		}

		/// <summary>
		/// Gets all the <see cref="Screen"/>s in the desktop.
		/// </summary>
		/// <returns></returns>
		public Screen[] GetScreens()
		{
			WinFormsScreen[] winformsScreens = WinFormsScreen.AllScreens;
			Screen[] screens = new Screen[winformsScreens.Length];
			for (int i = 0; i < winformsScreens.Length; ++i)
				screens[i] = new WinFormsScreenProxy(winformsScreens[i]);

			return screens;
		}

		#endregion
	}

	/// <summary>
	/// A proxy class for <see cref="System.Windows.Forms.Screen"/> objects.
	/// </summary>
	/// <remarks>This class can be instantiated and used anywhere a <see cref="ClearCanvas.Desktop.Screen"/> is needed.</remarks>
	public class WinFormsScreenProxy : Screen, IEquatable<WinFormsScreenProxy>
	{
		private readonly WinFormsScreen _screen;

		/// <summary>
		/// Constructor.
		/// </summary>
		public WinFormsScreenProxy(WinFormsScreen screen)
		{
			Platform.CheckForNullReference(screen, "screen");
			_screen = screen;
		}

		/// <summary>
		/// Gets the number of bits per pixel of the device.
		/// </summary>
		public override int BitsPerPixel
		{
			get { return _screen.BitsPerPixel; }
		}

		/// <summary>
		/// Gets the bounds of the screen inside the <see cref="Screen.VirtualScreen"/>.
		/// </summary>
		public override System.Drawing.Rectangle Bounds
		{
			get { return _screen.Bounds; }
		}

		/// <summary>
		/// Gets the name of the device.
		/// </summary>
		public override string DeviceName
		{
			get { return _screen.DeviceName; }
		}

		/// <summary>
		/// Gets whether or not this is the primary screen.
		/// </summary>
		public override bool IsPrimary
		{
			get { return _screen.Primary; }
		}

		/// <summary>
		/// Gets the area of the <see cref="Screen"/> in which a <see cref="IDesktopWindow"/> can be maximized.
		/// </summary>
		public override System.Drawing.Rectangle WorkingArea
		{
			get { return _screen.WorkingArea; }
		}

		public override int GetHashCode()
		{
			return _screen.GetHashCode();
		}

		public override string ToString()
		{
			return _screen.ToString();
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;

			if (obj is WinFormsScreen)
				return _screen.Equals(obj);

			return this.Equals(obj as Screen);
		}

		public override bool Equals(Screen other)
		{
			if (other == null)
				return false;

			return this.Equals(other as WinFormsScreenProxy);
		}

		#region IEquatable<WinformsScreenProxy> Members

		public bool Equals(WinFormsScreenProxy other)
		{
			if (other == null)
				return false;

			return _screen.Equals(other._screen);
		}

		#endregion
	}
}
