using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Dicom
{
    public class Window
    {
        private Window()
        {

        }

		public Window(Window window)
		{
			_width = window.Width;
			_center = window.Center;
		}

        public Window(double width, double center)
        {
            _width = width;
            _center = center;
        }

        public virtual double Width
        {
            get { return _width; }
            set { _width = value; }
        }

        public virtual double Center
        {
            get { return _center; }
            set { _center = value; }
        }

        #region Private Members
        double _width;
        double _center;
        #endregion
    }
}
