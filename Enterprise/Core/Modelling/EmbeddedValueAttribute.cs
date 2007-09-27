using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Core.Modelling
{
    /// <summary>
    /// When applied to a property of a domain class, indicates that that property  models
    /// an embedded value as opposed to an object reference, even though the property-type may be a .NET reference type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class EmbeddedValueAttribute : Attribute
    {
    }
}
