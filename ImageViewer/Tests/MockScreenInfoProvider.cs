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

#if	UNIT_TESTS

using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Tests
{
	/// <summary>
	/// A mock implementation of <see cref="IScreenInfoProvider"/> suitable for unit testing contexts.
	/// </summary>
	/// <remarks>
	/// In order to use this mock extension, the class must be explicitly installed as an extension
	/// of the <see cref="ScreenInfoProviderExtensionPoint"/> extension point before the unit test is
	/// executed. This can be accomplised by installing a custom <see cref="IExtensionFactory"/>, such
	/// as <see cref="UnitTestExtensionFactory"/>.
	/// </remarks>
	/// <example lang="CS">
	/// <code><![CDATA[
	/// UnitTestExtensionFactory extensionFactory = new UnitTestExtensionFactory();
	/// extensionFactory.Define(typeof(ScreenInfoProviderExtensionPoint), typeof(MockScreenInfoProvider));
	/// Platform.SetExtensionFactory(extensionFactory);
	/// ]]></code>
	/// </example>
	public sealed class MockScreenInfoProvider : IScreenInfoProvider
	{
		/// <summary>
		/// Gets the virtual screen of the entire desktop (all display devices).
		/// </summary>
		public Rectangle VirtualScreen
		{
			get { return new Rectangle(0, 0, 640, 480); }
		}

		/// <summary>
		/// Gets all the <see cref="Screen"/>s in the desktop.
		/// </summary>
		public Screen[] GetScreens()
		{
			return new[] {new MockScreen()};
		}

		private sealed class MockScreen : Screen
		{
			public override bool Equals(Screen other)
			{
				return other is MockScreen;
			}

			public override int BitsPerPixel
			{
				get { return 24; }
			}

			public override Rectangle Bounds
			{
				get { return new Rectangle(0, 0, 640, 480); }
			}

			public override string DeviceName
			{
				get { return "V'GER"; }
			}

			public override bool IsPrimary
			{
				get { return true; }
			}

			public override Rectangle WorkingArea
			{
				get { return new Rectangle(0, 0, 640, 480); }
			}
		}
	}
}

#endif