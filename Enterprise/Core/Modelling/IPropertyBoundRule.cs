using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace ClearCanvas.Enterprise.Core.Modelling
{
    /// <summary>
    /// Identifies a rule as being logically bound to one or more properties of an object. In other words,
    /// the rule is strictly a function of the values of the specified properties, and is not a function of
    /// anything else.
    /// </summary>
    public interface IPropertyBoundRule
    {
        /// <summary>
        /// Gets an array of the properties of which this rule is a function.
        /// </summary>
        PropertyInfo[] Properties { get; }
    }
}
