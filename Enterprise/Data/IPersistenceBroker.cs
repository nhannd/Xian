using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Data
{
    /// <summary>
    /// Base interface for all persistence broker interfaces.  This interface is not implemented directly.
    /// </summary>
    public interface IPersistenceBroker
    {
        /// <summary>
        /// Used by the framework to establish the <see cref="IPersistenceContext"/> in which an instance of
        /// this broker will act.
        /// </summary>
        /// <param name="context"></param>
        void SetContext(IPersistenceContext context);
    }
}
