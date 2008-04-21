using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace ClearCanvas.Enterprise.Core.Imex
{
    public interface IExportItem
    {
        void Write(XmlWriter writer);
    }

    public interface IImportItem
    {
        XmlReader Read();
    }


    public interface IXmDataImex
    {
        IEnumerable<IExportItem> Export(IReadContext context);
        void Import(IEnumerable<IImportItem> items, IUpdateContext context);
    }
}
