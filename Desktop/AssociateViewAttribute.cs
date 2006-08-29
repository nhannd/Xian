using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=true)]
    public class AssociateViewAttribute : Attribute
    {
        private Type _viewExtensionPointType;

        public AssociateViewAttribute(Type viewExtensionPointType)
        {
            _viewExtensionPointType = viewExtensionPointType;
        }

        public Type ViewExtensionPointType
        {
            get { return _viewExtensionPointType; }
        }
    }
}
