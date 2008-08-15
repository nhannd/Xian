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
