using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Enterprise.Data
{
    public interface ICodedValueBroker<TCodedValue> : IPersistenceBroker
        where TCodedValue : CodedValue
    {
        /// <summary>
        /// Loads the domain enumeration from a persistent store.
        /// </summary>
        /// <returns></returns>
        CodedValueDictionary<TCodedValue> LoadDictionary();
    }
}
