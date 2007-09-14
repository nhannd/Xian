using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.ImageServer.Database;

namespace ClearCanvas.ImageServer.Model.Brokers
{
    public interface IGetFilesystemTiers : IProcedureReadBroker<FilesystemTier>
    {
    }
}
