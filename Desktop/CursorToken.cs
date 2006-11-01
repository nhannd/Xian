using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
	public class CursorToken
	{
		public enum SystemCursors
		{
			Arrow = 0,
			Cross,
			Hand,
			Help,
			HSplit,
			IBeam,
			No,
			NoMove2D,
			NoMoveHoriz,
			NoMoveVert,
			PanEast,
			PanNE,
			PanNorth,
			PanNW,
			PanSE,
			PanSouth,
			PanSW,
			PanWest,
			SizeAll,
			SizeNESW,
			SizeNS,
			SizeNWSE,
			SizeWE,
			UpArrow,
			VSplit
		}; 
		
		private string _resourceName;
		private Assembly _resourceAssembly;

		public CursorToken(SystemCursors systemCursor)
		{
			_resourceName = systemCursor.ToString();
			_resourceAssembly = null;
		}

		public CursorToken(string resourceName)
		{
			Platform.CheckForNullReference(resourceName, "resourceName"); 
			Platform.CheckForEmptyString(resourceName, "resourceName");

			_resourceName = resourceName;
			_resourceAssembly = System.Reflection.Assembly.GetCallingAssembly();
		}

		public CursorToken(string resourceName, Assembly resourceAssembly)
		{
			Platform.CheckForNullReference(resourceName, "resourceName");
			Platform.CheckForEmptyString(resourceName, "resourceName");
			Platform.CheckForNullReference(resourceAssembly, "resourceAssembly");

			_resourceName = resourceName;
			_resourceAssembly = resourceAssembly;
		}

		private CursorToken()
		{
		}

		public string ResourceName
		{
			get { return _resourceName; }
		}

		public Assembly ResourceAssembly
		{
			get { return _resourceAssembly; }
		}

		public bool IsSystemCursor
		{
			get { return (_resourceAssembly == null); }
		}
	}
}
