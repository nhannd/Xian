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
    public abstract class XmlEntityImex<TEntity, TDataContract> : XmlDataImexBase
        where TEntity : Entity
    {
        #region ExportItem class

        class ExportItem : IExportItem
        {
            private readonly TDataContract _data;

            public ExportItem(TDataContract data)
            {
                _data = data;
            }

            public void Write(XmlWriter writer)
            {
                XmlDataImexBase.Write(writer, _data);
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

        #region Protected overrides

        protected override IEnumerable<IExportItem> ExportCore()
        {
            bool more = true;
            for (int row = 0; more; row += ItemsPerReadTransaction)
            {
                using (PersistenceScope scope = new PersistenceScope(PersistenceContextType.Read))
                {
                    IReadContext context = (IReadContext)PersistenceScope.CurrentContext;
                    IList<TEntity> items = GetItemsForExport(context, row, ItemsPerReadTransaction);
                    foreach (TEntity entity in items)
                    {
                        TDataContract data = Export(entity, context);
                        yield return new ExportItem(data);
                    }

                    // there may be more rows if the last query returned any items
                    more = (items.Count > 0);
                    scope.Complete();
                }
            }
        }

        protected override void ImportCore(IEnumerable<IImportItem> items)
        {
            IEnumerator<IImportItem> enumerator = items.GetEnumerator();
            for(bool more = true; more; )
            {
                using (PersistenceScope scope = new PersistenceScope(PersistenceContextType.Update))
                {
                    IUpdateContext context = (IUpdateContext)PersistenceScope.CurrentContext;
                    context.ChangeSetRecorder.OperationName = this.GetType().FullName;

                    for (int j = 0; j < ItemsPerUpdateTransaction && more; j++)
                    {
                        more = enumerator.MoveNext();
                        if (more)
                        {
                            IImportItem item = enumerator.Current;
                            TDataContract data = (TDataContract)Read(item.Read(), typeof(TDataContract));
                            Import(data, context);
                        }
                    }
                    scope.Complete();
                }
            }
        }

        #endregion

    }
}
