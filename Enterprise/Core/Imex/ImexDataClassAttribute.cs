using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Core.Imex
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=true, Inherited=true)]
    public class ImexDataClassAttribute : Attribute
    {
        private string _dataClass;

        public ImexDataClassAttribute(string dataClass)
        {
            _dataClass = dataClass;
        }

        public string DataClass
        {
            get { return _dataClass; }
        }
    }
}
