using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Core
{
    /// <summary>
    /// Used by <see cref="IPersistenceBroker.Load"/> to provide fine control over the loading of entities.
    /// </summary>
    [Flags]
    public enum EntityLoadFlags
    {
        /// <summary>
        /// Default value
        /// </summary>
        None = 0x0000,

        /// <summary>
        /// Forces a version check, causing an exception to be thrown if the version does not match
        /// </summary>
        CheckVersion = 0x0001,

        /// <summary>
        /// Asks for a proxy to the entity, rather than loading the full entity.  There is no guarantee
        /// that this flag will be obeyed, because the underlying implementation may not support proxies,
        /// or the entity may not be proxiable.
        /// </summary>
        Proxy = 0x0002,
    }
}
