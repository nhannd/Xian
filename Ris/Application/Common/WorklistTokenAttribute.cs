using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Ris.Application.Common
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class WorklistTokenAttribute : Attribute
    {
        private string _description;

        public WorklistTokenAttribute()
        {
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }
    }
}
