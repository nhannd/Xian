#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

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
