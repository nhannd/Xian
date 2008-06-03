using System;
using System.ComponentModel;

namespace ClearCanvas.ImageServer.Enterprise
{
    [AttributeUsage(AttributeTargets.Field)]
    public class EnumValueDescriptionAttribute : DescriptionAttribute
    {
        private readonly string _longDescription;
        
        public EnumValueDescriptionAttribute(string description, string longDescription)
            :base(description)
        {
            _longDescription = longDescription;
        }

        public string LongDescription
        {
            get
            {
                return _longDescription;
            }
        }
    }

}
