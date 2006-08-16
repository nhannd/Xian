using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Dicom.DataStore
{
    public class Window
    {
        private Window()
        {

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
