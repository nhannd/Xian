using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Core
{
    /// <summary>
    /// Defines the interface to a read-context.  A read-context allows the application to perform read-only
    /// operations on a persistent store.
    /// </summary>
    public interface IReadContext : IPersistenceContext
    {
    }
}
