using System;
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Common
{
    [Cloneable]
    public sealed class ExtensionData
    {
        private readonly IDictionary<Type, object> _data;

        public ExtensionData()
        {
            _data = new Dictionary<Type, object>();
        }

        private ExtensionData(ExtensionData source, ICloningContext context)
        {
            _data = new Dictionary<Type, object>();
            foreach (var sourceData in source._data)
            {
                var valueClone = CloneBuilder.Clone(sourceData.Value);
                if (valueClone != null)
                    _data[sourceData.Key] = valueClone;
            }
        }

        public object this[Type key]
        {
            get
            {
                object value;
                if (_data.TryGetValue(key, out value))
                    return value;

                return null;
            }
            set
            {
                _data[key] = value;
            }
        }
    }
}
