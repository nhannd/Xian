using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Ris.Application.Services.Admin
{
    public interface IDataImporter
    {
        void Import(List<string> rows, IUpdateContext context);
    }

    [ExtensionPoint]
    public class DataImporterExtensionPoint : ExtensionPoint<IDataImporter>
    {
    }

}
