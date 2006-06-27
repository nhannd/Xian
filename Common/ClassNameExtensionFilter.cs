using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common
{
    public class ClassNameExtensionFilter : ExtensionFilter
    {
        private string _name;

        public ClassNameExtensionFilter(string name)
        {
            _name = name;
        }

        public override bool Test(ExtensionInfo extension)
        {
            return extension.ExtensionClass.FullName.Equals(_name);
        }
    }
}
