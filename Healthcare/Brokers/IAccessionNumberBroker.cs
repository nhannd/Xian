using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise;

namespace ClearCanvas.Healthcare.Brokers
{
    /// <summary>
    /// Custom broker that provides Accession numbers.
    /// </summary>
    public interface IAccessionNumberBroker : IPersistenceBroker
    {
        string GetNextAccessionNumber();
    }
}
