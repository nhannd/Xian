using System;

namespace ClearCanvas.ImageServer.Common
{
    /// <summary>
    /// Attribute to describe an enum
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class EnumInfoAttribute:Attribute
    {
        public String ShortDescription;
        public String LongDescription;
    }
}