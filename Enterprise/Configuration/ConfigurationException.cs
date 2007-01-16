using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Configuration
{
    public class ConfigurationException : Exception
    {
        public ConfigurationException(string message)
            :base(message)
        {
        }
    }
}
