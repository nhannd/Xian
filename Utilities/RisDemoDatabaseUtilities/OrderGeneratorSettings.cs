#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Healthcare;
using System.Collections;

namespace ClearCanvas.Utilities.RisDemoDatabaseUtilities
{
    public class OrderGeneratorSettings : EntityGeneratorSettingsList, IEntityGeneratorSettingsList
    {
        public OrderGeneratorSettings()
        {
            _settings = new List<EntityGeneratorSetting>();
            _settings.Add(new EntityGeneratorSetting("MinOrders", 1));
            _settings.Add(new EntityGeneratorSetting("MaxOrders", 2));
        }

        public override bool SettingsValid()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override IEntityGeneratorSettingsList Copy()
        {
            OrderGeneratorSettings orderSettings = new OrderGeneratorSettings();
            foreach (EntityGeneratorSetting setting in this.Settings)
            {
                orderSettings._settings.Add(new EntityGeneratorSetting(setting.Name, setting.Setting));
            }

            return orderSettings;
        }


        public int MinOrders
        {
            get { return GetIntFromSettingsValue("MinOrders"); }
        }

        public int MaxOrders
        {
            get { return GetIntFromSettingsValue("MaxOrders"); }
        }
    }
}
