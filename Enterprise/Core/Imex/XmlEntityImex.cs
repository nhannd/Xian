using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Collections;
using System.Runtime.Serialization;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Enterprise.Core.Imex
{
    public abstract class XmlEntityImex<TEntity, TDataContract> : IXmDataImex
        where TEntity : Entity
    {
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

        public XmlEntityImex()
        {

        }



        protected abstract IEnumerable<TEntity> GetItemsForExport(IReadContext context);

        protected abstract TDataContract Export(TEntity entity, IReadContext context);

        protected abstract void Import(TDataContract data, IUpdateContext context);

        #region IXmEntityImex Members

        IEnumerable<IExportItem> IXmDataImex.Export(IReadContext context)
        {
            foreach (TEntity entity in GetItemsForExport(context))
            {
                TDataContract data = Export(entity, context);
                yield return new ExportItem(data);
            }
        }

        void IXmDataImex.Import(IEnumerable<IImportItem> items, IUpdateContext context)
        {
            foreach (IImportItem item in items)
            {
                TDataContract data = Read(item.Read());
                Import(data, context);
            }
        }

        #endregion

        private static void Write(XmlWriter writer, TDataContract data)
        {
            //DataContractSerializer serializer = new DataContractSerializer(typeof(TDataContract));
            //serializer.WriteObject(writer, data);

            JsmlSerializer.Serialize(writer, data, data.GetType().Name, false);
        }

        private static TDataContract Read(XmlReader reader)
        {
            //DataContractSerializer serializer = new DataContractSerializer(typeof(TDataContract));
            //return (TDataContract)serializer.ReadObject(reader);

            return (TDataContract)JsmlSerializer.Deserialize<TDataContract>(reader);
        }
    }
}
