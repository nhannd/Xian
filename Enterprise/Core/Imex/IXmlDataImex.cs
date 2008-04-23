using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace ClearCanvas.Enterprise.Core.Imex
{
    /// <summary>
    /// Defines an interface an item to be written to the specified <see cref="XmlWriter"/>.
    /// </summary>
    public interface IExportItem
    {
        void Write(XmlWriter writer);
    }

    /// <summary>
    /// Defines an interface to an item to be read from a <see cref="XmlReader"/>.
    /// </summary>
    public interface IImportItem
    {
        XmlReader Read();
    }

    /// <summary>
    /// Defines an interface to a class that is responsible for exporting/importing data in XML format.
    /// </summary>
    public interface IXmlDataImex
    {
        /// <summary>
        /// Export all data as a set of <see cref="IExportItem"/>s.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        IEnumerable<IExportItem> Export(IReadContext context);

        /// <summary>
        /// Import the specified set of <see cref="IImportItem"/>s.
        /// </summary>
        /// <param name="items"></param>
        /// <param name="context"></param>
        void Import(IEnumerable<IImportItem> items, IUpdateContext context);
    }
}
