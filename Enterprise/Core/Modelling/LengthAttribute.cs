using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Core.Modelling
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class LengthAttribute : Attribute
    {
        private int _min;
        private int _max;

        public LengthAttribute(int min, int max)
        {
            _min = min;
            _max = max;
        }

        public LengthAttribute(int max)
            :this(0, max)
        {
        }

        public int Min { get { return _min; } }
        public int Max { get { return _max; } }
    }
}
