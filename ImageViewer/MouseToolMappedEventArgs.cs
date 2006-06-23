using System;
using System.Collections.Generic;
using System.Text;


namespace ClearCanvas.Workstation.Model
{
    /// <summary>
    /// Arguments for the <see cref="MouseToolMap.MouseToolMapped"/> event.
    /// </summary>
    internal class MouseToolMappedEventArgs : EventArgs
    {
        private XMouseButtons _mouseButton;
        private MouseTool _oldTool;
        private MouseTool _newTool;

        internal MouseToolMappedEventArgs(XMouseButtons mouseButton, MouseTool oldTool, MouseTool newTool)
        {
            _mouseButton = mouseButton;
            _oldTool = oldTool;
            _newTool = newTool;
        }

        /// <summary>
        /// The mouse button whose mapping has changed.
        /// </summary>
        public XMouseButtons MouseButton
        {
            get { return _mouseButton; }
        }

        /// <summary>
        /// The tool that was previously mapped to the mouse button.
        /// </summary>
        public MouseTool OldTool
        {
            get { return _oldTool; }
        }

        /// <summary>
        /// The tool that is newly mapped to the mouse button.
        /// </summary>
        public MouseTool NewTool
        {
            get { return _newTool; }
        }
    }
}
