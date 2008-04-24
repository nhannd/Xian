using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Core.Imex
{
    [ExtensionOf(typeof(XmlDataImexExtensionPoint))]
    [ImexDataClass("Enumeration")]
    public class XmlEnumValueImex : XmlDataImexBase
    {
        [DataContract]
        public class EnumerationData
        {
            [DataMember]
            public string EnumerationClass;

            [DataMember]
            public List<EnumerationMemberData> Members;
        }

        [DataContract]
        public class EnumerationMemberData
        {
            public EnumerationMemberData()
	        {
	        }

            public EnumerationMemberData(EnumValue value)
	        {
                this.Code = value.Code;
                this.Value = value.Value;
                this.Description = value.Description;
                this.DisplayOrder = value.DisplayOrder;

	        }

            [DataMember]
            public string Code;

            [DataMember]
            public string Value;

            [DataMember]
            public string Description;

            [DataMember]
            public float DisplayOrder;
        }

        #region ExportItem class

        class ExportItem : IExportItem
        {
            private readonly EnumerationData _data;

            public ExportItem(EnumerationData data)
            {
                _data = data;
            }

            public void Write(XmlWriter writer)
            {
                XmlDataImexBase.Write(writer, _data);
            }
        }

        #endregion

        protected override IEnumerable<IExportItem> ExportCore()
        {
            using (PersistenceScope scope = new PersistenceScope(PersistenceContextType.Read))
            {
                IReadContext context = (IReadContext) PersistenceScope.Current;
                IMetadataBroker metaBroker = context.GetBroker<IMetadataBroker>();
                IEnumBroker enumBroker = context.GetBroker<IEnumBroker>();
                List<Type> enumClasses = CollectionUtils.Sort(metaBroker.ListEnumValueClasses(),
                    delegate (Type x, Type y) { return x.FullName.CompareTo(y.FullName);});
                foreach (Type enumClass in enumClasses)
                {
                    EnumerationData data = new EnumerationData();
                    data.EnumerationClass = enumClass.FullName;
                    data.Members = CollectionUtils.Map<EnumValue, EnumerationMemberData>(enumBroker.Load(enumClass),
                        delegate(EnumValue v) { return new EnumerationMemberData(v); });

                    yield return new ExportItem(data);
                }

                scope.Complete();
            }
        }

        protected override void ImportCore(IEnumerable<IImportItem> items)
        {
            using (PersistenceScope scope = new PersistenceScope(PersistenceContextType.Update))
            {
                IUpdateContext context = (IUpdateContext) PersistenceScope.Current;
                context.ChangeSetRecorder.OperationName = this.GetType().FullName;

                IMetadataBroker metaBroker = context.GetBroker<IMetadataBroker>();
                IEnumBroker enumBroker = context.GetBroker<IEnumBroker>();
                IList<Type> enumClasses = metaBroker.ListEnumValueClasses();

                foreach (IImportItem item in items)
                {
                    EnumerationData data = (EnumerationData) Read(item.Read(), typeof(EnumerationData));

                    // find the enum class
                    Type enumClass = CollectionUtils.SelectFirst(enumClasses,
                        delegate(Type ec)
                        {
                            return ec.FullName == data.EnumerationClass;
                        });

                    IList<EnumValue> existingValues = enumBroker.Load(enumClass);
                    foreach (EnumerationMemberData md in data.Members)
                    {
                        // check if the value already exists
                        EnumValue value = CollectionUtils.SelectFirst(existingValues,
                            delegate(EnumValue v) { return v.Code == md.Code; });

                        if(value == null)
                        {
                            // value does not exist - add it
                            value = enumBroker.AddValue(enumClass, md.Code, md.Value, md.Description, md.DisplayOrder);
                            existingValues.Add(value);
                        }
                        else
                        {
                            // value exists - update it
                            enumBroker.UpdateValue(enumClass, md.Code, md.Value, md.Description, md.DisplayOrder);
                        }
                    }
                }

                scope.Complete();
            }
        }
    }
}
