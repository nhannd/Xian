using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.View.WinForms
{
	public class CursorWrapper : IDisposable
	{
		private bool _disposeCursor; 
		private Cursor _cursor;
		private IntPtr _hIcon;
		
		public CursorWrapper(Cursor cursor, bool disposeCursor)
		{
			Platform.CheckForNullReference(cursor, "cursor");

			_cursor = cursor;
			_disposeCursor = disposeCursor;
			_hIcon = IntPtr.Zero;
		}

		public CursorWrapper(Cursor cursor, IntPtr hIcon)
		{
			Platform.CheckForNullReference(cursor, "cursor");

			_cursor = cursor;
			_hIcon = hIcon;
			_disposeCursor = true;
		}

		private CursorWrapper()
		{ 
		}

		public Cursor Cursor
		{
			get { return _cursor; }
			set { _cursor = value; }
		}

		/// <summary>
		/// Destroy Icon handle.
		/// </summary>
		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, SetLastError = true)]
		private extern static bool DestroyIcon(IntPtr handle);

		#region IDisposable Members

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_cursor != null && _disposeCursor)
				{
					_cursor.Dispose();
					_cursor = null;
				}

				if (_hIcon != IntPtr.Zero)
				{
					DestroyIcon(_hIcon);
					_hIcon = IntPtr.Zero;
				}
			}
		}

		public void Dispose()
		{
			try
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
			}
		}

		#endregion
	}
	
	public static class CursorFactory
	{
		public static CursorWrapper CreateCursor(CursorToken cursorToken)
		{
			if (cursorToken.IsSystemCursor)
				return new CursorWrapper(GetSystemCursor(cursorToken.ResourceName), false);

			ResourceResolver resolver = new ResourceResolver(cursorToken.ResourceAssembly);

			if (cursorToken.ResourceName.EndsWith(".cur", StringComparison.InvariantCultureIgnoreCase))
				return FromCursorResource(cursorToken.ResourceName, resolver);

			return FromImageResource(cursorToken.ResourceName, resolver);
		}

		private static Cursor GetSystemCursor(string systemCursorName)
		{
			if (String.Compare(systemCursorName, "Cross", true) == 0)
				return Cursors.Cross;

			if (String.Compare(systemCursorName, "Hand", true) == 0)
				return Cursors.Hand;

			if (String.Compare(systemCursorName, "Help", true) == 0)
				return Cursors.Help;

			if (String.Compare(systemCursorName, "HSplit", true) == 0)
				return Cursors.HSplit;

			if (String.Compare(systemCursorName, "IBeam", true) == 0)
				return Cursors.IBeam;

			if (String.Compare(systemCursorName, "No", true) == 0)
				return Cursors.No;

			if (String.Compare(systemCursorName, "NoMove2D", true) == 0)
				return Cursors.NoMove2D;

			if (String.Compare(systemCursorName, "NoMoveHoriz", true) == 0)
				return Cursors.NoMoveHoriz;

			if (String.Compare(systemCursorName, "NoMoveVert", true) == 0)
				return Cursors.NoMoveVert;

			if (String.Compare(systemCursorName, "PanEast", true) == 0)
				return Cursors.PanEast;

			if (String.Compare(systemCursorName, "PanNE", true) == 0)
				return Cursors.PanNE;

			if (String.Compare(systemCursorName, "PanNorth", true) == 0)
				return Cursors.PanNorth;

			if (String.Compare(systemCursorName, "PanNW", true) == 0)
				return Cursors.PanNW;

			if (String.Compare(systemCursorName, "PanSE", true) == 0)
				return Cursors.PanSE;

			if (String.Compare(systemCursorName, "PanSouth", true) == 0)
				return Cursors.PanSouth;

			if (String.Compare(systemCursorName, "PanSW", true) == 0)
				return Cursors.PanSW;

			if (String.Compare(systemCursorName, "PanWest", true) == 0)
				return Cursors.PanWest;

			if (String.Compare(systemCursorName, "SizeAll", true) == 0)
				return Cursors.SizeAll;

			if (String.Compare(systemCursorName, "SizeNESW", true) == 0)
				return Cursors.SizeNESW;

			if (String.Compare(systemCursorName, "SizeNS", true) == 0)
				return Cursors.SizeNS;

			if (String.Compare(systemCursorName, "SizeNWSE", true) == 0)
				return Cursors.SizeNWSE;

			if (String.Compare(systemCursorName, "SizeWE", true) == 0)
				return Cursors.SizeWE;

			if (String.Compare(systemCursorName, "UpArrow", true) == 0)
				return Cursors.UpArrow;

			if (String.Compare(systemCursorName, "VSplit", true) == 0)
				return Cursors.VSplit;

			return Cursors.Arrow;
		}

		private static CursorWrapper FromImageResource(string imageResource, IResourceResolver resolver)
		{
			Bitmap bitmap = new Bitmap(resolver.OpenResource(imageResource));
			IntPtr hIcon = bitmap.GetHicon();

			//we can dispose of the bitmap right away because the icon is a separate resource.
			bitmap.Dispose();

			Cursor cursor = new Cursor(hIcon);
			return new CursorWrapper(cursor, hIcon);
		}

		private static CursorWrapper FromCursorResource(string cursorResource, IResourceResolver resolver)
		{
			return new CursorWrapper(new Cursor(resolver.OpenResource(cursorResource)), true);
		}
	}
}
