using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer
{
	public class MouseWheelToolMap
	{
		private event EventHandler<MouseWheelToolMappedEventArgs> _mouseToolMapped;
		private MouseTool _mouseTool;

		/// <summary>
		/// Internal constructor.
		/// </summary>
		internal MouseWheelToolMap()
		{
		}

		/// <summary>
		/// Gets or sets the mouse tool associated with the mouse wheel.
		/// </summary>
		public MouseTool MouseTool
		{
			get { return _mouseTool; }
			set 
			{
				if (_mouseTool != value)
				{
					MouseTool oldTool = _mouseTool;
					_mouseTool = value;
					EventsHelper.Fire(_mouseToolMapped, this, new MouseWheelToolMappedEventArgs(oldTool, value));
				}
			}
		}

		/// <summary>
		/// Fired when a mouse wheel mapping changes.
		/// </summary>
		public event EventHandler<MouseWheelToolMappedEventArgs> MouseToolMapped
		{
			add { _mouseToolMapped += value; }
			remove { _mouseToolMapped -= value; }
		}
	}
}
