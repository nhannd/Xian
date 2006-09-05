using System;
using System.Collections.Generic;
using System.Text;


namespace ClearCanvas.ImageViewer
{
    /// <summary>
    /// Arguments for the <see cref="MouseToolMap.MouseToolMapped"/> event.
    /// </summary>
    public class MouseWheelToolMappedEventArgs : EventArgs
    {
        private MouseTool _oldTool;
        private MouseTool _newTool;

        internal MouseWheelToolMappedEventArgs(MouseTool oldTool, MouseTool newTool)
        {
            _oldTool = oldTool;
            _newTool = newTool;
        }

        /// <summary>
        /// The tool that was previously mapped to the mouse wheel.
        /// </summary>
        public MouseTool OldTool
        {
            get { return _oldTool; }
        }

        /// <summary>
        /// The tool that is newly mapped to the mouse wheel.
        /// </summary>
        public MouseTool NewTool
        {
            get { return _newTool; }
        }
    }
}
