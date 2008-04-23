using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Enterprise.Core.Imex
{
    /// <summary>
    /// Abstract base class for classes that import/export entities in XML format.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TDataContract"></typeparam>
    public abstract class XmlEntityImex<TEntity, TDataContract> : IXmlDataImex
        where TEntity : Entity
    {
        #region ExportItem class

        class ExportItem : IExportItem
        {
            private TDataContract _data;

            public ExportItem(TDataContract data)
            {
                _data = data;
            }

            public void Write(XmlWriter writer)
            {
                XmlEntityImex<TEntity, TDataContract>.Write(writer, _data);
            }
        }

        #endregion

        #region Absract Methods

        /// <summary>
        /// Called to obtain the list of entities to export.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected abstract IEnumerable<TEntity> GetItemsForExport(IReadContext context);

        /// <summary>
        /// Called to export the specified entity.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected abstract TDataContract Export(TEntity entity, IReadContext context);

        /// <summary>
        /// Called to import the specified data.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="context"></param>
        protected abstract void Import(TDataContract data, IUpdateContext context);
        
        #endregion

        #region IXmEntityImex Members

        IEnumerable<IExportItem> IXmlDataImex.Export(IReadContext context)
        {
            foreach (TEntity entity in GetItemsForExport(context))
            {
                TDataContract data = Export(entity, context);
                yield return new ExportItem(data);
            }
        }

        void IXmlDataImex.Import(IEnumerable<IImportItem> items, IUpdateContext context)
        {
            foreach (IImportItem item in items)
            {
                TDataContract data = Read(item.Read());
                Import(data, context);
            }
        }

        #endregion

        #region Helpers

        private static void Write(XmlWriter writer, TDataContract data)
        {
            JsmlSerializer.Serialize(writer, data, data.GetType().Name, false);
        }

        private static TDataContract Read(XmlReader reader)
        {
            return (TDataContract)JsmlSerializer.Deserialize<TDataContract>(reader);
        }

        #endregion
    }
}
