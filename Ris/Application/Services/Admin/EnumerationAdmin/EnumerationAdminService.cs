using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Ris.Application.Common.Admin.EnumerationAdmin;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Services.Admin.EnumerationAdmin
{
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    [ServiceImplementsContract(typeof(IEnumerationAdminService))]
    public class EnumerationAdminService : ApplicationServiceBase, IEnumerationAdminService
    {
        private bool IsSoftEnum(Type enumClass)
        {
            return !CollectionUtils.Contains<Type>(enumClass.Assembly.GetTypes(),
                delegate(Type t)
                {
                    return t.IsEnum && AttributeUtils.HasAttribute<EnumValueClassAttribute>(t, false,
                        delegate(EnumValueClassAttribute attr) { return attr.EnumValueClass.Equals(enumClass); });
                });
        }

        #region IEnumerationAdminService Members

        [ReadOperation]
        public ListEnumerationsResponse ListEnumerations(ListEnumerationsRequest request)
        {
            IMetadataBroker broker = PersistenceContext.GetBroker<IMetadataBroker>();
            IList<Type> enumClasses = broker.ListEnumValueClasses();

            List<EnumerationSummary> enumerations = CollectionUtils.Map<Type, EnumerationSummary, List<EnumerationSummary>>(enumClasses,
                delegate(Type enumClass)
                {
                    return new EnumerationSummary(enumClass.AssemblyQualifiedName, enumClass.Name, IsSoftEnum(enumClass));
                });

            return new ListEnumerationsResponse(enumerations);
        }

        [ReadOperation]
        public ListEnumerationValuesResponse ListEnumerationValues(ListEnumerationValuesRequest request)
        {
            IEnumBroker enumBroker = PersistenceContext.GetBroker<IEnumBroker>();
            IList<EnumValue> enumValues = enumBroker.Load(GetEnumClass(request.AssemblyQualifiedClassName));
            return new ListEnumerationValuesResponse(
                CollectionUtils.Map<EnumValue, EnumValueInfo, List<EnumValueInfo>>(enumValues,
                    delegate(EnumValue value) { return EnumUtils.GetEnumValueInfo(value); }));
        }

        [UpdateOperation]
        public AddValueResponse AddValue(AddValueRequest request)
        {
            Type enumClass = GetEnumClass(request.AssemblyQualifiedClassName);

            IEnumBroker broker = PersistenceContext.GetBroker<IEnumBroker>();
            broker.AddValue(enumClass, request.Value.Code, request.Value.Value, request.Value.Description);

            return new AddValueResponse();
        }

        [UpdateOperation]
        public EditValueResponse EditValue(EditValueRequest request)
        {
            Type enumClass = GetEnumClass(request.AssemblyQualifiedClassName);

            IEnumBroker broker = PersistenceContext.GetBroker<IEnumBroker>();
            broker.UpdateValue(enumClass, request.Value.Code, request.Value.Value, request.Value.Description);

            return new EditValueResponse();
        }

        [UpdateOperation]
        public RemoveValueResponse RemoveValue(RemoveValueRequest request)
        {
            Type enumClass = GetEnumClass(request.AssemblyQualifiedClassName);

            IEnumBroker broker = PersistenceContext.GetBroker<IEnumBroker>();
            broker.RemoveValue(enumClass, request.Value.Code);

            return new RemoveValueResponse();
        }

        #endregion

        private Type GetEnumClass(string enumerationName)
        {
            Type enumClass = Type.GetType(enumerationName);
            if (enumClass == null)
                throw new RequestValidationException("Invalid enumeration name.");

            return enumClass;
        }
    }
}
