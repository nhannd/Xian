using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.ImageServer.Database
{
    /// <summary>
    /// Used to represent a specific parameter to a stored procedure.
    /// </summary>
    /// <typeparam name="T">The type associated with the parameter.</typeparam>
    public class ProcedureParameter<T> : SearchCriteria
    {
        private T _value;

        public ProcedureParameter(String key, T value)
            : base(key)
        {
            _value = value;
        }

        public T Value
        {
            get { return _value; }
        }
    }
}
