using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.ImageServer.Database;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.ImageServer.Model.Brokers
{
    /// <summary>
    /// Broker for accessing <see cref="TypeEnum"/> values.
    /// </summary>
    public interface ITypeEnum : IEnumBroker<TypeEnum>
    {
    }
}
