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

        private const int ItemsPerReadTransaction = 100;
        private const int ItemsPerUpdateTransaction = 100;


        #region Absract Methods

        /// <summary>
        /// Called to obtain the list of entities to export.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="firstRow"></param>
        /// <param name="maxRows"></param>
        /// <returns></returns>
        protected abstract IList<TEntity> GetItemsForExport(IReadContext context, int firstRow, int maxRows);

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

        IEnumerable<IExportItem> IXmlDataImex.Export()
        {
            bool more = true;
            for (int row = 0; more; row += ItemsPerReadTransaction)
            {
                using (PersistenceScope scope = new PersistenceScope(PersistenceContextType.Read))
                {
                    IReadContext context = (IReadContext)PersistenceScope.Current;
                    IList<TEntity> items = GetItemsForExport(context, row, ItemsPerReadTransaction);
                    foreach (TEntity entity in items)
                    {
                        TDataContract data = Export(entity, context);
                        yield return new ExportItem(data);
                    }

                    // there may be more rows if the last query returned the max number of items
                    more = (items.Count == ItemsPerReadTransaction);
                    scope.Complete();
                }
            }
        }

        void IXmlDataImex.Import(IEnumerable<IImportItem> items)
        {
            IEnumerator<IImportItem> enumerator = items.GetEnumerator();
            for(bool more = true; more; )
            {
                using (PersistenceScope scope = new PersistenceScope(PersistenceContextType.Update))
                {
                    IUpdateContext context = (IUpdateContext)PersistenceScope.Current;
                    context.ChangeSetRecorder.OperationName = this.GetType().FullName;

                    for (int j = 0; j < ItemsPerUpdateTransaction && more; j++)
                    {
                        more = enumerator.MoveNext();
                        if (more)
                        {
                            TDataContract data = Read(enumerator.Current.Read());
                            Import(data, context);
                        }
                    }
                    scope.Complete();
                }
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
