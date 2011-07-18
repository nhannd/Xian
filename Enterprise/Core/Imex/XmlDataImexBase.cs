#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Enterprise.Core.Imex
{
    /// <summary>
    /// Abstract base implementation of <see cref="IXmlDataImex"/>.
    /// </summary>
    public abstract class XmlDataImexBase : IXmlDataImex
    {
        /// <summary>
        /// Implements export functionality.
        /// </summary>
        /// <returns></returns>
        protected abstract IEnumerable<IExportItem> ExportCore();

        /// <summary>
        /// Implements import functionality.
        /// </summary>
        /// <param name="items"></param>
        protected abstract void ImportCore(IEnumerable<IImportItem> items);

        #region IXmlDataImex Members

        IEnumerable<IExportItem> IXmlDataImex.Export()
        {
            return ExportCore();
        }

        void IXmlDataImex.Import(IEnumerable<IImportItem> items)
        {
            ImportCore(items);
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Writes the specified data to the specified xml writer.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="data"></param>
        protected static void Write(XmlWriter writer, object data)
        {
            JsmlSerializer.Serialize(writer, data, data.GetType().Name);
        }

        /// <summary>
        /// Reads an object of the specified class from the xml reader.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="dataContractClass"></param>
        /// <returns></returns>
        protected static object Read(XmlReader reader, Type dataContractClass)
        {
            return JsmlSerializer.Deserialize(reader, dataContractClass);
        }

         #endregion
    }
}
