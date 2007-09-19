using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Core.Modelling
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class MaxLengthAttribute : Attribute
    {
        private int _length;

        public MaxLengthAttribute(int length)
        {
            _length = length;
        }
    }
}
