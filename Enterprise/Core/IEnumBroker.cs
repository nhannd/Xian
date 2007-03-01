using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Core
{
    /// <summary>
    /// Defines the interface for a broker that returns a list of <see cref="EnumValue"/> that contain
    /// meta-data about a C# enum.
    /// </summary>
    /// <typeparam name="E">The <see cref="EnumValue"/> class that this broker returns</typeparam>
    public interface IEnumBroker<E, ETable> : IPersistenceBroker
    {
        /// <summary>
        /// Loads the domain enumeration from a persistent store.
        /// </summary>
        /// <returns></returns>
        ETable Load();
    }
}
