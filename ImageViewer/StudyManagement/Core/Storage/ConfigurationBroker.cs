#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Linq;
using ClearCanvas.Common.Serialization;

namespace ClearCanvas.ImageViewer.StudyManagement.Core.Storage
{
    public class ConfigurationBroker: Broker
    {
        public ConfigurationBroker(DicomStoreDataContext context)
            : base(context)
        {
        }

        public T GetDataContractValue<T>(string name) where T : class
        {
            var item = (from c in Context.Configurations where c.Name == name select c).FirstOrDefault();
            if (item == null)
                return null;

            return JsmlSerializer.Deserialize<T>(item.Value);
        }

        public void SetDataContractValue<T>(string name, T value) where T : class
        {
            var serializedValue = JsmlSerializer.Serialize(value, name);
            var item = (from c in Context.Configurations where c.Name == name select c).FirstOrDefault();
            if (item == null)
                Context.Configurations.InsertOnSubmit(new Configuration { Name = name, Value = serializedValue });
            else
                item.Value = serializedValue;
        }
    }
}
