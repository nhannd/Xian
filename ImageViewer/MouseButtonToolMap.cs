using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer
{
    /// <summary>
    /// Maps mouse buttons to mouse tools.
    /// </summary>
    /// <remarks>
    /// The <see cref="MouseTool"/> class works in conjuction with this class to control
    /// the mapping of mouse tools to mouse buttons.
    /// </remarks>
	public class MouseButtonToolMap
	{
        private event EventHandler<MouseButtonToolMappedEventArgs> _mouseToolMapped;
        private Dictionary<XMouseButtons, MouseTool> _toolMap = new Dictionary<XMouseButtons, MouseTool>();

        /// <summary>
        /// Internal constructor.
        /// </summary>
		internal MouseButtonToolMap()
		{
            // put all possible keys in the map, so we don't get any invalid key exceptions
            _toolMap.Add(XMouseButtons.None, null);
            _toolMap.Add(XMouseButtons.Left, null);
			_toolMap.Add(XMouseButtons.Middle, null);
			_toolMap.Add(XMouseButtons.Right, null);
			_toolMap.Add(XMouseButtons.XButton1, null);
			_toolMap.Add(XMouseButtons.XButton2, null);
		}

        /// <summary>
        /// Gets or sets the mouse tool associated with the specified mouse button.
        /// </summary>
        /// <param name="mouseButton">A mouse button.</param>
        /// <returns>The mouse tool associated with the specified mouse button.</returns>
        public MouseTool this[XMouseButtons mouseButton]
        {
            get 
			{
				// Cheap hack for now to prevent a crash when more than one 
				// button is pressed simultaneously.
				if (!_toolMap.ContainsKey(mouseButton))
					return null;

				return _toolMap[mouseButton]; 
			}
            set
            {
				// Cheap hack for now to prevent a crash when more than one 
				// button is pressed simultaneously.
				if (!_toolMap.ContainsKey(mouseButton))
					return;

                if (_toolMap[mouseButton] != value)
                {
                    MouseTool oldTool = _toolMap[mouseButton];
                    _toolMap[mouseButton] = value;
                    EventsHelper.Fire(_mouseToolMapped, this, new MouseButtonToolMappedEventArgs(mouseButton, oldTool, value));
                }
            }
        }

        /// <summary>
        /// Fired when a mouse button mapping changes.
        /// </summary>
        public event EventHandler<MouseButtonToolMappedEventArgs> MouseToolMapped
        {
            add { _mouseToolMapped += value; }
            remove { _mouseToolMapped -= value; }
        }
	}
}

