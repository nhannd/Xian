using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Dicom
{
    public class Window : IEquatable<Window>
    {
		#region Private Members

		double _width;
		double _center;

		#endregion

		/// <summary>
		/// NHibernate Constructor.
		/// </summary>
        public Window()
        {

        }

		/// <summary>
		/// Mandatory Constructor.
		/// </summary>
		public Window(double width, double center)
		{
			_width = width;
			_center = center;
		}

		#region NHibernate Persistent Properties

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

		#endregion

		public override bool Equals(object obj)
		{
			if (obj == this)
				return true;

			if (obj is Window)
				return this.Equals((Window) obj);

			return false;
		}

		#region IEquatable<Window> Members

		public bool Equals(Window other)
		{
			return Width == other.Width && Center == other.Center;
		}

		#endregion
	}
}
