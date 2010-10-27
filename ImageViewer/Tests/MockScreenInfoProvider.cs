#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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