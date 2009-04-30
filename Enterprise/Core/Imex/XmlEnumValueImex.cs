#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
            	this.Deactivated = value.Deactivated;
	        }

            [DataMember]
            public string Code;

            [DataMember]
            public string Value;

            [DataMember]
            public string Description;

            [DataMember]
            public float DisplayOrder;

			[DataMember]
			public bool Deactivated;
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
                IReadContext context = (IReadContext) PersistenceScope.CurrentContext;
                IMetadataBroker metaBroker = context.GetBroker<IMetadataBroker>();
                IEnumBroker enumBroker = context.GetBroker<IEnumBroker>();
                List<Type> enumClasses = CollectionUtils.Sort(metaBroker.ListEnumValueClasses(),
                    delegate (Type x, Type y) { return x.FullName.CompareTo(y.FullName);});
                foreach (Type enumClass in enumClasses)
                {
                    EnumerationData data = new EnumerationData();
                    data.EnumerationClass = enumClass.FullName;
                    data.Members = CollectionUtils.Map<EnumValue, EnumerationMemberData>(enumBroker.Load(enumClass, true),
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
                IUpdateContext context = (IUpdateContext) PersistenceScope.CurrentContext;
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

					if(enumClass == null)
					{
						Platform.Log(LogLevel.Error, string.Format("{0} is not a valid enumeration class name.", data.EnumerationClass));
						continue;
					}

					IList<EnumValue> existingValues = enumBroker.Load(enumClass, true);
					foreach (EnumerationMemberData md in data.Members)
					{
						// check if a conflicting value exists
						// (this can happen if there is existing data in the db with the same value but different code)
						EnumValue conflict = CollectionUtils.SelectFirst(existingValues,
							delegate(EnumValue v) { return v.Code != md.Code && v.Value == md.Value; });

						if(conflict != null)
						{
							Platform.Log(LogLevel.Error, string.Format("{0} value {1} conflicts with existing value {2} and will not be imported.",
								data.EnumerationClass, md.Code, conflict.Code));
							continue;
						}

						// check if the value already exists
						EnumValue value = CollectionUtils.SelectFirst(existingValues,
							delegate(EnumValue v) { return v.Code == md.Code; });

						if (value == null)
						{
							// value does not exist - add it
							value = enumBroker.AddValue(enumClass, md.Code, md.Value, md.Description, md.DisplayOrder, md.Deactivated);
							existingValues.Add(value);
						}
						else
						{
							// value exists - update it
							enumBroker.UpdateValue(enumClass, md.Code, md.Value, md.Description, md.DisplayOrder, md.Deactivated);
						}
					}

					context.SynchState();
				}

                scope.Complete();
            }
        }
    }
}
