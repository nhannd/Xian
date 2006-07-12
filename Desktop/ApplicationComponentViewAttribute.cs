using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=false)]
    public class ApplicationComponentViewAttribute : Attribute
    {
        private Type _viewExtensionPointType;

        public ApplicationComponentViewAttribute(Type viewExtensionPointType)
        {
            _viewExtensionPointType = viewExtensionPointType;
        }

        public Type ViewExtensionPointType
        {
            get { return _viewExtensionPointType; }
        }
    }
}
