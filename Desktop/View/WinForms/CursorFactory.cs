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

		public Cursor Cursor
		{
			get { return _cursor; }
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

			if (cursorToken.ResourceName.EndsWith(".cur", StringComparison.InvariantCultureIgnoreCase))
				return FromCursorResource(cursorToken.ResourceName, cursorToken.Resolver);

			return FromImageResource(cursorToken.ResourceName, cursorToken.Resolver);
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
