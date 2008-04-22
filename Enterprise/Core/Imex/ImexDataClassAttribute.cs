using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Core.Imex
{
    /// <summary>
    /// Defines the class of data that an extension of <see cref="XmlDataImexExtensionPoint"/> is
    /// responsible for.
    /// </summary>
    /// <remarks>
    /// The data-class is a logical class name and need not refer to an actual entity class.  However,
    /// this is the recommended practice.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=true)]
    public class ImexDataClassAttribute : Attribute
    {
        private readonly string _dataClass;
        private int _itemsPerFile;

        public ImexDataClassAttribute(string dataClass)
        {
            _dataClass = dataClass;
        }

        public string DataClass
        {
            get { return _dataClass; }
        }

        public int ItemsPerFile
        {
            get { return _itemsPerFile; }
            set { _itemsPerFile = value; }
        }
    }
}
