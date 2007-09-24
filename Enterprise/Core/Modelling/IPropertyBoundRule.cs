using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace ClearCanvas.Enterprise.Core.Modelling
{
    public interface IPropertyBoundRule
    {
        PropertyInfo[] Properties { get; }
    }
}
