#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Data.Linq;
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Common.Serialization;

namespace ClearCanvas.ImageViewer.StudyManagement.Core.Storage
{
    public class ConfigurationBroker : Broker
    {
        private class Item<T> where T : class
        {
            public T Value { get; private set; }
            public Binary CurrentVersion { get; private set; }

            public Item(T value, Binary currentVersion)
            {
                Value = value;
                CurrentVersion = currentVersion;
            }

            public static volatile Item<T> Cached;
        }

        public ConfigurationBroker(DicomStoreDataContext context)
            : base(context)
        {
        }

        public T GetDataContractValue<T>() where T : class
        {
            var name = typeof (T).FullName;
            var cached = Item<T>.Cached;

            Configuration stored;
            if (cached == null)
            {
                Platform.Log(LogLevel.Debug, "No cached configuration item for contract '{0}'", name);
                stored = (from c in Context.Configurations where c.Name == name select c).FirstOrDefault();
            }
            else
            {
                stored = (from c in Context.Configurations
                          where c.Name == name && c.Version != cached.CurrentVersion
                          select c).FirstOrDefault();

                if (stored != null)
                    Platform.Log(LogLevel.Debug, "Detected new configuration item for contract '{0}'", name);
            }

            if (stored != null)
            {
                var value = JsmlSerializer.Deserialize<T>(stored.Value);
                //We cache configuration items for 2 reasons:
                // 1. they are accessed very frequently, and the serialization/deserialization cost could be high.
                // 2. they are changed very infrequently ... see 1.
                cached = Item<T>.Cached = new Item<T>(value, stored.Version);
            }

            return cached != null ? cached.Value : null;
        }

        public void SetDataContractValue<T>(T value) where T : class
        {
            var name = typeof(T).FullName;
            var serializedValue = JsmlSerializer.Serialize(value, name);
            var item = (from c in Context.Configurations where c.Name == name select c).FirstOrDefault();
            if (item == null)
                Context.Configurations.InsertOnSubmit(new Configuration { Name = name, Value = serializedValue });
            else
                item.Value = serializedValue;

            Item<T>.Cached = null; //may as well.
        }
    }
}
